using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Service;
using DiabetesContolApp.Models;

using MathNet.Numerics;
using MathNet.Numerics.Distributions;

using Xamarin.Forms;

namespace DiabetesContolApp.GlobalLogic
{
    public static class Algorithm
    {
        private const int MINIMUM_OCCURENCES = 10;
        private const double LOWER_BOUND_FOR_PREDICTION_INTERVALL = -1.0;

        async public static void RunStatisticsOnReminder(int reminderID)
        {
            //ReminderModel reminder = await ReminderDatabase.GetInstance().GetReminderAsync(reminderID);
            ReminderService reminderService = new();
            ReminderModel reminder = await reminderService.GetReminderAsync(reminderID);
            if (reminder == null)
                return;


            List<LogModel> logs = await PartitionGlucoseAfterMeal(reminder);

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
                UpdateDayProfileScalar(log.DayProfile.DayProfileID);
                UpdateCorrectionInsulinScalar();
                foreach (NumberOfGroceryModel numberOfGrocery in log.NumberOfGroceryModels)
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
            DateTime lastUpdate = DateTime.Now; //TODO: Update this to get when the last scalar update for the correction dose was made
            LogService logService = new();
            List<LogModel> logs = await logService.GetAllLogsAfterDateAsync(lastUpdate);

            List<DataPoint> dataPoints = new();
            foreach (LogModel log in logs)
            {
                DataPoint dataPoint = await GetDataPointForCorrectionInsulin(log);
                if (dataPoint.IsValid)
                    dataPoints.Add(dataPoint);
            }

            if (dataPoints.Count >= MINIMUM_OCCURENCES)
            {
                var globalVariables = Application.Current as App; //Get access to properites in the App

                double distance = GetGreatestSafeDistanceFromRegressionLine(dataPoints);

                double extraInsulin = distance / globalVariables.InsulinToGlucoseRatio; //Either what we were missing or gave to much

                double newScalar = 1 + extraInsulin; //The statistics for correction insulin is adjusted to be per unit of insulin so all we need is this

                //TODO: Update the scalar database
            }
        }

        /// <summary>
        /// Gets the error per unit of insulin sat.
        /// </summary>
        /// <param name="log"></param>
        /// <returns>Data point with data, not valid if the log dosn't have a glucose after meal value</returns>
        async private static Task<DataPoint> GetDataPointForCorrectionInsulin(LogModel log)
        {
            if (log.GlucoseAfterMeal == null)
                return new DataPoint(false, DateTime.Now, -1.0f);

            //double totalGlucoseError = (float)log.GlucoseAfterMeal - await GetTargetGlucoseForLog(log);

            //TODO: Continue
            //double totalGlucoseError = log.GetTotalGlucoseError();

            //double glucoseErrorForCorrection = totalGlucoseError * (log.CorrectionDose / log.InsulinEstimate);
            //double glucoseErrorPerUnitOfCorrectionInsulin = glucoseErrorForCorrection / log.CorrectionDose;

            //This is per unit of insulin so it is easier to compare bigger and smaller doses.
            //double glucoseErrorPerUnitOfCorrectionInsulin = totalGlucoseError / log.InsulinEstimate;

            //return new DataPoint(true, log.DateTimeValue, glucoseErrorPerUnitOfCorrectionInsulin);
            return new(); //TODO: Temp
        }

        /// <summary>
        /// Gets all logs with this day profile ID.
        /// Checks if there is statistical support to
        /// change the scalar, if so, it is changed,
        /// otherwise nothing happens.
        /// </summary>
        /// <param name="dayProfileID"></param>
        async private static void UpdateDayProfileScalar(int dayProfileID)
        {
            LogService logService = new();
            //List<LogModel> logsWithDayProfileID = (await logService.GetAllLogsAsync()).Where(log => log.DayProfileID == dayProfileID).ToList();
            List<LogModel> logsWithDayProfileID = await logService.GetAllLogsWithDayProfileIDAsync(dayProfileID);

            //TODO: Only get logs after the newest scalar update for this day profile

            //Create data points from logs
            List<DataPoint> dataPoints = new();
            foreach (LogModel log in logsWithDayProfileID)
            {
                DataPoint dataPoint = GetDataPointForDayProfile(log);
                if (dataPoint.IsValid)
                    dataPoints.Add(dataPoint); //Add data points to the list
            }


            //Change scalar based on statistics

            if (dataPoints.Count >= MINIMUM_OCCURENCES) //Must have more than a given number of entries
            {
                var globalVariables = Application.Current as App; //Get access to properites in the App

                double distance = GetGreatestSafeDistanceFromRegressionLine(dataPoints);

                double totalInsulinGivenAvg = logsWithDayProfileID.Sum(log => log.InsulinFromUser) / logsWithDayProfileID.Count; //Average insulin with this day profile
                double extraInsulin = distance / globalVariables.InsulinToGlucoseRatio; //Either what we were missing or gave to much

                double newScalar = (totalInsulinGivenAvg + extraInsulin) / totalInsulinGivenAvg;

                DayProfileService dayProfileService = new();
                //TODO: Update the scalar database
                DayProfileModel dayProfile = await dayProfileService.GetDayProfileAsync(dayProfileID);
                dayProfile.CarbScalar = (float)newScalar;
                dayProfile.GlucoseScalar = (float)newScalar;

                await dayProfileService.UpdateDayProfileAsync(dayProfile);
            }
        }

        private static double GetGreatestSafeDistanceFromRegressionLine(List<DataPoint> dataPoints)
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
            return Math.Min(smallestDifferenceToXAxis, predictionIntervallNextPoint.Item2 - LOWER_BOUND_FOR_PREDICTION_INTERVALL);
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
        /// error and date time value
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private static DataPoint GetDataPointForDayProfile(LogModel log)
        {
            if (log.DayProfile == null || log.DayProfile.DayProfileID < 0)
                throw new ArgumentException("Log must contain a valid DayProfile.");
            if (log.GlucoseAfterMeal == null)
                throw new ArgumentNullException("Log must have a valid GlucoseAfterMeal value to generate a data point.");

            return new DataPoint(true, log.DateTimeValue, (float)log.GlucoseAfterMeal - log.DayProfile.TargetGlucoseValue);
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
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns>The list of logs connected to the reminder</returns>
        async private static Task<List<LogModel>> PartitionGlucoseAfterMeal(ReminderModel reminder)
        {
            var totalInsulinGiven = reminder.Logs.Sum(log => Math.Abs(log.InsulinEstimate)); //Total insulin from all logs estimate
            if (totalInsulinGiven == 0)
                totalInsulinGiven = 1; //This is to avoid edge case of division by zero

            LogService logService = new();
            for (int i = 0; i < reminder.Logs.Count; ++i)
                reminder.Logs[i] = await logService.GetLogAsync(reminder.Logs[i].LogID);

            float lastTargetGlucoseValue = reminder.Logs[reminder.Logs.Count - 1].DayProfile.TargetGlucoseValue;
            float glucoseError = (float)reminder.GlucoseAfterMeal - lastTargetGlucoseValue; //Total error in glucose

            foreach (LogModel log in reminder.Logs) //TODO: Check if there is any log in this variable
            {
                var targetGlucoseForLog = log.DayProfile.TargetGlucoseValue; //Get the target glucose value

                //We partiton the glucose difference based on the percentage of the insulin estimate
                //from one log to the total insulin given within the reminder.
                var glucoseDifferencePartitioned = glucoseError * (log.InsulinEstimate / totalInsulinGiven);


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
                var glucoseErrorAdjusted = glucoseDifferencePartitioned - (log.InsulinEstimate - log.InsulinFromUser) * globalVariables.InsulinToGlucoseRatio;

                log.GlucoseAfterMeal = glucoseErrorAdjusted;

                await logService.UpdateLogAsync(log);
            }

            return reminder.Logs;
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
