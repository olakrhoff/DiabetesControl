using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Service;
using DiabetesContolApp.Models;

using MathNet.Numerics;
using MathNet.Numerics.Distributions;

using Xamarin.Forms;
using System.Diagnostics;

namespace DiabetesContolApp.GlobalLogic
{
    public static class Algorithm
    {
        private const int MINIMUM_OCCURENCES = 10;
        private const double LOWER_BOUND_FOR_PREDICTION_INTERVAL = -1.0;


        /// <summary>
        /// Handles the logic of the algorithm scheme.
        /// Starts by partitioning the glucose error between the Logs
        /// connnected to the Reminder. Then it checks all elements
        /// in all Logs if there is an adjustment that can be made
        /// to the corresponding scalar, if so, it applies it.
        /// </summary>
        /// <param name="reminderID"></param>
        async public static void RunStatisticsOnReminder(int reminderID)
        {
            ReminderService reminderService = new();
            ReminderModel reminder = await reminderService.GetReminderAsync(reminderID);
            if (reminder == null)
                return;


            //Updates the logs to have all objects in them, since we will need them later
            LogService logService = new();
            for (int i = 0; i < reminder.Logs.Count; ++i)
                reminder.Logs[i] = await logService.GetLogAsync(reminder.Logs[i].LogID);

            bool curruptLog = reminder.Logs.Exists(log => log == null); //Check if any of the logs wasnt found
            if (curruptLog || reminder.Logs.Count == 0)
                return; //The data is corrupt, this should not happen, and can therfore not be processed

            //Do the partitioning from the Reminder to all the Logs
            List<LogModel> logs = await PartitionGlucoseErrorToLogs(reminder);

            //Check all elements in all Logs and update the respective scalar if possible
            UpdateScalarValues(logs);
        }

        /// <summary>
        /// Loops through all the logs and update the scalars for
        /// all the groceries and day profiles in them.
        /// </summary>
        /// <param name="logs"></param>
        private static void UpdateScalarValues(List<LogModel> logs)
        {
            foreach (LogModel log in logs)
            {
                UpdateDayProfileScalars(log.DayProfile.DayProfileID);
                UpdateCorrectionInsulinScalar();
                foreach (NumberOfGroceryModel numberOfGrocery in log.NumberOfGroceries)
                    UpdateGroceryScalar(numberOfGrocery.Grocery.GroceryID);
            }
        }

        /// <summary>
        /// Gets all logs since the last update of the
        /// correction insulin scalar update.
        /// Gets data points from the logs, based on the partitioning
        /// of the error to the correction dose. These data points
        /// are used to statisticly get a safe new scalar.
        /// </summary>
        async private static void UpdateCorrectionInsulinScalar()
        {
            try
            {
                ScalarService scalarService = new();

                ScalarModel correctionScalar = await scalarService.GetNewestScalarOfScalarType(ScalarTypes.CORRECTION_INSULIN);

                LogService logService = new();
                List<LogModel> logsAfterDate = await logService.GetAllLogsAfterDateAsync(correctionScalar.DateTimeCreated);

                logsAfterDate = logsAfterDate.FindAll(log => log.IsLogDataValid()); //Filter out corrupt data.

                List<DataPoint> dataPoints = new();
                if (logsAfterDate.Count >= MINIMUM_OCCURENCES) //Don't waste time here if there isn't enough data
                    foreach (LogModel log in logsAfterDate)
                    {
                        DataPoint dataPoint = GetDataPointForCorrectionInsulin(log);
                        if (dataPoint.IsValid)
                            dataPoints.Add(dataPoint);
                    }

                if (dataPoints.Count >= MINIMUM_OCCURENCES)
                {
                    var globalVariables = Application.Current as App; //Get access to properites in the App

                    double distance = GetGreatestSafeDistanceFromWantedLine(dataPoints);

                    double extraInsulin = distance / globalVariables.InsulinToGlucoseRatio; //Either what we were missing or gave to much

                    //The statistics for correction insulin is adjusted to be per unit of insulin
                    //so all we need is to add the "extra insulin per unit" to 1 to get the new
                    //scalar value. This is equvilent to say the:
                    //(avg. correction insulin + extra insulin) / avg. correction insulin
                    //since we operate with "per unit" values the avg. correction insulin would be equal to 1.
                    double newScalar = 1 + extraInsulin;

                    globalVariables.InsulinToGlucoseRatio /= (float)newScalar; //Update the ratio

                    correctionScalar.ScalarValue = (float)newScalar;

                    await scalarService.UpdateScalarAsync(correctionScalar);
                }
            }
            catch (NullReferenceException nre)
            {
                Debug.WriteLine(nre.StackTrace);
                return;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return;
            }
        }

        /// <summary>
        /// Gets the error per unit of insulin sat for correction.
        /// </summary>
        /// <param name="log"></param>
        /// <returns>
        /// Data point with data, not valid if the
        /// log dosn't have a glucose after meal value
        /// and other necessary data.
        /// </returns>
        private static DataPoint GetDataPointForCorrectionInsulin(LogModel log)
        {
            try
            {
                if (!log.IsLogDataValid())
                    throw new ArgumentException("All log data must be valid for the log to be used in the method.");


                //These next two lines explain the actual code-line under.
                //double glucoseErrorForCorrection = log.GetGlucoseError() * (log.CorrectionDose / log.InsulinEstimate); //Partition the glucose error
                //double glucoseErrorPerUnitOfCorrectionInsulin = glucoseErrorForCorrection / log.CorrectionDose; //Adjust to error per unit of correction insulin 

                double glucoseErrorPerUnitOfCorrectionInsulin = log.GetGlucoseError() / log.InsulinEstimate;

                return new DataPoint(true, log.DateTimeValue, glucoseErrorPerUnitOfCorrectionInsulin);
            }
            catch (ArgumentException ae)
            {
                Debug.WriteLine(ae.StackTrace);
                return new DataPoint(false, DateTime.Now, -1.0f);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return new DataPoint(false, DateTime.Now, -1.0f);
            }
        }

        /// <summary>
        /// Gets all logs with this day profile ID.
        /// Checks if there is statistical support to
        /// change the scalar, if so, it is changed,
        /// otherwise nothing happens. This checks both
        /// the carbs-scalar and the glucose-scalar.
        /// </summary>
        /// <param name="dayProfileID"></param>
        async private static void UpdateDayProfileScalars(int dayProfileID)
        {
            DayProfileService dayProfileService = new();
            DayProfileModel currentDayProfile = await dayProfileService.GetDayProfileAsync(dayProfileID);

            ScalarService scalarService = new();
            //Get datetime for when carb-scalar and glucose-scalar was last updated
            currentDayProfile.SetScalarTypeToCarbScalar();
            ScalarModel carbScalar = await scalarService.GetNewestScalarForScalarObjectAsync(currentDayProfile);
            currentDayProfile.SetScalarTypeToGlucoseScalar();
            ScalarModel glucoseScalar = await scalarService.GetNewestScalarForScalarObjectAsync(currentDayProfile);



            //Get all other log-entries with the same DayProfile
            LogService logService = new();
            List<LogModel> logsWithDayProfileID = await logService.GetAllLogsWithDayProfileIDAsync(dayProfileID);

            //Filter out all invalid logs
            logsWithDayProfileID = logsWithDayProfileID.FindAll(log => log.IsLogDataValid());

            //Create list for carb-scalar
            List<LogModel> logsForCarbScalar = logsWithDayProfileID.FindAll(log => log.DateTimeValue > carbScalar.DateTimeCreated);
            //Create list for glucose-scalar
            List<LogModel> logsForGlucoseScalar = logsWithDayProfileID.FindAll(log => log.DateTimeValue > glucoseScalar.DateTimeCreated);

            //Create data points from logs for carb-scalar
            List<DataPoint> dataPointsForCarbScalar = new();
            if (logsForCarbScalar.Count >= MINIMUM_OCCURENCES) //If there is not enough data, don't waste time with this
                foreach (LogModel log in logsForCarbScalar)
                {
                    DataPoint dataPointForCarbScalar = GetDataPointForDayProfileCarbScalar(log);
                    if (dataPointForCarbScalar.IsValid)
                        dataPointsForCarbScalar.Add(dataPointForCarbScalar);
                }

            //Create data points from logs for glucose-scalar
            List<DataPoint> dataPointsForGlucoseScalar = new();
            if (logsForGlucoseScalar.Count >= MINIMUM_OCCURENCES) //If there is not enough data, don't waste time with this
                foreach (LogModel log in logsForGlucoseScalar)
                {
                    DataPoint dataPointForGlucoseScalar = GetDataPointForDayProfileGlucoseScalar(log);
                    if (dataPointForGlucoseScalar.IsValid)
                        dataPointsForGlucoseScalar.Add(dataPointForGlucoseScalar);
                }


            //Change carb-scalar based on statistics
            if (dataPointsForCarbScalar.Count >= MINIMUM_OCCURENCES) //Must have more than a given number of data points
            {
                var globalVariables = Application.Current as App; //Get access to properites in the App

                double distance = GetGreatestSafeDistanceFromWantedLine(dataPointsForCarbScalar);

                double insulinGivenOnAvgForCarbScalar = logsForCarbScalar.Sum(log => log.GetInsulinFromDayProfileCarbScalar()) / logsForCarbScalar.Count; //Average insulin with this day profiles carb-scalar
                double extraInsulin = distance / globalVariables.InsulinToGlucoseRatio; //How much insulin (either more or less) is needed to close the distance to the line

                double newScalar = (insulinGivenOnAvgForCarbScalar + extraInsulin) / insulinGivenOnAvgForCarbScalar; //The new scalar to obtain the wanted values is calculated

                DayProfileModel dayProfile = await dayProfileService.GetDayProfileAsync(dayProfileID);
                dayProfile.CarbScalar = (float)newScalar;

                await dayProfileService.UpdateDayProfileAsync(dayProfile); //Update the DayProfile

                carbScalar.ScalarValue = (float)newScalar;

                await scalarService.UpdateScalarAsync(carbScalar); //Update the Scalar
            }

            //Change glucose-scalar based on statistics
            if (dataPointsForGlucoseScalar.Count >= MINIMUM_OCCURENCES) //Must have more than a given number of data points
            {
                var globalVariables = Application.Current as App; //Get access to properites in the App

                double distance = GetGreatestSafeDistanceFromWantedLine(dataPointsForGlucoseScalar);

                double insulinGivenOnAvgForGlucoseScalar = logsForGlucoseScalar.Sum(log => log.GetInsulinFromDayProfileCarbScalar()) / logsForGlucoseScalar.Count; //Average insulin with this day profiles glucose-scalar
                double extraInsulin = distance / globalVariables.InsulinToGlucoseRatio; //How much insulin (either more or less) is needed to close the distance to the line

                double newScalar = (insulinGivenOnAvgForGlucoseScalar + extraInsulin) / insulinGivenOnAvgForGlucoseScalar; //The new scalar to obtain the wanted values is calculated

                DayProfileModel dayProfile = await dayProfileService.GetDayProfileAsync(dayProfileID);
                dayProfile.GlucoseScalar = (float)newScalar;

                await dayProfileService.UpdateDayProfileAsync(dayProfile); //Update the DayProfile

                glucoseScalar.ScalarValue = (float)newScalar;

                await scalarService.UpdateScalarAsync(glucoseScalar); //Update the Scalar
            }
        }

        /// <summary>
        /// Takes a regression line from the data points given.
        /// It also finds the (95%) prediction interval for the data.
        /// It then checks what the is the greatest safest distance
        /// it can "move" the line to obtain the wanted line (y = 0)
        /// and still have the lower bound of the prediction interval
        /// be over the LOWER_BOUND_FOR_PREDICTION_INTERVAL
        ///
        /// IMPORTANT: Negative numbers are regarded as smaller
        /// distances. So -5 will be smaller than 1. Negativ numbers indicate
        /// that the line is to low, which is bad, since this makes it much more
        /// likely for the user to get too low glucose levels.
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns>The greatest safest distance.</returns>
        private static double GetGreatestSafeDistanceFromWantedLine(List<DataPoint> dataPoints)
        {
            List<double> xValues = new(), yValues = new();

            dataPoints.ForEach(point =>
            {
                xValues.Add(point.TimeStamp.ToOADate());
                yValues.Add((double)point.GlucoseError);
            });

            Tuple<double, double> alphaAndBetaHat = Fit.Line(xValues.ToArray(), yValues.ToArray());

            Tuple<double, double> predictionIntervallNextPoint = Statistics.PredictionInterval(xValues, yValues, alphaAndBetaHat.Item1, alphaAndBetaHat.Item2, 1 - 0.95);


            //Check the ends of the line to find the smallest distance to the wanted line on the X-axis
            double smallestDifferenceToXAxis = Math.Min(alphaAndBetaHat.Item1, alphaAndBetaHat.Item1 + alphaAndBetaHat.Item2 * xValues[xValues.Count - 1]);

            //Returns the smallest distance either between the regression line and the X-axis or the lower prediction line and the LOWER_BOUND_FOR_PREDICTION_INTERVALL
            return Math.Min(smallestDifferenceToXAxis, predictionIntervallNextPoint.Item2 - LOWER_BOUND_FOR_PREDICTION_INTERVAL);
        }

        /// <summary>
        /// Data returned from UpdateScalar-method()
        /// Updated is true if scalar can be changed.
        /// Scalar holds value for the greates difference between
        /// regression line and the wanted line, only
        /// valid if Updated is true.
        /// </summary>
        struct ScalarUpdatedData
        {
            public bool Updated { get; set; }
            public double Difference { get; set; }

            public ScalarUpdatedData(bool updated, double difference)
            {
                Updated = updated;
                Difference = difference;
            }
        }

        /// <summary>
        /// Get a log, find the glucose error the
        /// log had, and creates a datapoint with the
        /// error and date time value for the contribution the
        /// DayProfile carb-scalar had.
        /// </summary>
        /// <param name="log"></param>
        /// <returns>DataPoint, with error caused by carb-scalar and datetime.</returns>
        private static DataPoint GetDataPointForDayProfileCarbScalar(LogModel log)
        {
            try
            {
                if (log.DayProfile == null || log.DayProfile.DayProfileID < 0)
                    throw new ArgumentException("Log must contain a valid DayProfile.");
                if (!log.IsLogDataValid())
                    throw new ArgumentNullException("Log must have all valid critical data to generate a data point.");


                //Calculate the glucose error partitioned to the carb-scalar
                float glucoseErrorForCarbScalarPartitioned = log.GetGlucoseError() * (log.GetInsulinFromDayProfileCarbScalar() / log.GetInsulinForCarbs());

                return new DataPoint(true, log.DateTimeValue, glucoseErrorForCarbScalarPartitioned);
            }
            catch (ArgumentNullException ane)
            {
                Debug.WriteLine(ane.StackTrace);
            }
            catch (ArgumentException ae)
            {
                Debug.WriteLine(ae.StackTrace);
            }
            return new DataPoint(false, DateTime.Now, -1.0f);
        }

        /// <summary>
        /// Get a log, find the glucose error the
        /// log had, and creates a datapoint with the
        /// error and date time value for the contribution the
        /// DayProfile glucose-scalar had.
        /// </summary>
        /// <param name="log"></param>
        /// <returns>DataPoint, with error caused by glucose-scalar and datetime.</returns>
        private static DataPoint GetDataPointForDayProfileGlucoseScalar(LogModel log)
        {
            try
            {
                if (log.DayProfile == null || log.DayProfile.DayProfileID < 0)
                    throw new ArgumentException("Log must contain a valid DayProfile.");
                if (!log.IsLogDataValid())
                    throw new ArgumentNullException("Log must have all valid critical data to generate a data point.");


                //Calculate the glucose error partitioned to the carb-scalar
                float glucoseErrorForGlucoseScalarPartitioned = log.GetGlucoseError() * (log.GetInsulinFromDayProfileGlucoseScalar() / log.GetInsulinForGlucose());

                return new DataPoint(true, log.DateTimeValue, glucoseErrorForGlucoseScalarPartitioned);
            }
            catch (ArgumentNullException ane)
            {
                Debug.WriteLine(ane.StackTrace);
            }
            catch (ArgumentException ae)
            {
                Debug.WriteLine(ae.StackTrace);
            }
            return new DataPoint(false, DateTime.Now, -1.0f);
        }

        private static void UpdateGroceryScalar(int groceryID)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// This method partition the error in glucose after meal,
        /// based on the logs insulin estimate,
        /// between all logs involved (connected to the reminder).
        /// The logs are updated in their database.
        ///
        /// WARNING: This method need the whole Log object, so the Logs
        /// in the reminder must be filled out.
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns>The list of the updated logs connected to the reminder, is null if error occured</returns>
        async private static Task<List<LogModel>> PartitionGlucoseErrorToLogs(ReminderModel reminder)
        {
            try
            {
                if (!reminder.IsGlucoseAfterMealValid())
                    throw new ArgumentNullException("Glucose after meal value in Reminder must not be null"); //Should not be null, it is needed in this method

                //We calculate the glucose error in the reminder
                float lastLogTargetGlucoseValue = reminder.Logs[reminder.Logs.Count - 1].DayProfile.TargetGlucoseValue;
                float glucoseErrorInReminder = (float)reminder.GlucoseAfterMeal - lastLogTargetGlucoseValue;

                //Find the total amount of insulin estimated by the Logs
                var totalInsulinGiven = reminder.Logs.Sum(log => Math.Abs(log.InsulinEstimate));

                if (totalInsulinGiven == 0)
                    return null; //TODO: This is a safty, to avoid an edge case scenario


                //Partition the total glucose error from the Reminder between the Logs
                foreach (LogModel log in reminder.Logs)
                {
                    var targetGlucoseForLog = log.DayProfile.TargetGlucoseValue; //Get the target glucose value for the Log

                    //We partiton the glucose difference/error based on the percentage of the insulin estimate
                    //from one log to the total insulin given within the reminder.
                    var glucoseErrorForLogPartitioned = glucoseErrorInReminder * (log.InsulinEstimate / totalInsulinGiven);


                    var globalVariables = Application.Current as App; //Get access to properites in the App


                    //We adjust the difference to account for correctness with what was accualy given
                    //This means that if the app estimate was 3.0 units and the user sat 1 unit
                    //We add in what the remining 2 units (3 - 1 = 2) would theoretically add.
                    //So if 1 unit lowers the glucose by 1.5 glucose (mmol/L), then the glucose would,
                    //in theory, if the estimated units had been sat be: 2 * 1.5 = 3 mmol/L lower
                    //If glucose after meal then was 7.8 we register 7.8 - 3 = 4.8 as what the algorithm
                    //currently would get it to. 

                    //E.g.
                    //  4.8                       = 7.8                          - (3                   - 1                  ) * 1.5 | Case: adjusted insulin down
                    //  12.3                      = 7.8                          - (1                   - 4                  ) * 1.5 | Case: adjusted insulin up
                    //  8.3                       = 15.8                         - (5                   - 0                  ) * 1.5 | Case: did not set any insulin (happens sometimes)
                    var glucoseErrorAdjusted = glucoseErrorForLogPartitioned - (log.InsulinEstimate - log.InsulinFromUser) * globalVariables.InsulinToGlucoseRatio;

                    log.GlucoseAfterMeal = glucoseErrorAdjusted;

                    LogService logService = new();
                    await logService.UpdateLogAsync(log); //Update the Log in the datebase to hold the GlucoseAfterMeal value
                }

                return reminder.Logs;
            }
            catch (NullReferenceException nre)
            {
                Debug.WriteLine(nre.StackTrace);
                Debug.WriteLine(nre.Message);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }
    }


    /// <summary>
    /// Used in analysis.
    /// timestamp is x-coord, and glucoseError is y-coord.
    /// </summary>
    public struct DataPoint
    {
        public bool IsValid { get; set; }
        public DateTime TimeStamp { get; set; }
        public double GlucoseError { get; set; }

        public DataPoint(bool isValid, DateTime timeStamp, double glucoseError)
        {
            IsValid = isValid;
            TimeStamp = timeStamp;
            GlucoseError = glucoseError;
        }
    }
}
