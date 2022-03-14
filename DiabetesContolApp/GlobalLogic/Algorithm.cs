using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Persistence;
using DiabetesContolApp.Models;

using MathNet.Numerics;
using MathNet.Numerics.Distributions;

using Xamarin.Forms;

namespace DiabetesContolApp.GlobalLogic
{
    public static class Algorithm
    {
        private const int MINIMUM_OCCURENCES = 10;

        async public static void RunStatisticsOnReminder(int reminderID)
        {
            ReminderModel reminder = await ReminderDatabase.GetInstance().GetReminderAsync(reminderID);
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
                //float glucoseError = (float)log.GlucoseAfterMeal - await GetTargetGlucoseForLog(log);

                foreach (NumberOfGroceryModel numberOfGrocery in log.NumberOfGroceryModels)
                    UpdateGroceryScalar(numberOfGrocery.Grocery.GroceryID);
                UpdateDayProfileScalar(log.DayProfileID);
            }
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
            List<LogModel> logsWithDayProfileID = (await LogDatabase.GetInstance().GetLogsAsync()).Where(log => log.DayProfileID == dayProfileID).ToList();


            //Create datapoints from logs
            List<DataPoint> dataPoints = new();
            foreach (LogModel log in logsWithDayProfileID)
                dataPoints.Add(await GetDataPointForDayProfile(log)); //Add data points to the list


            //Change scalar based on statistics

            if (dataPoints.Count >= MINIMUM_OCCURENCES) //Must have more than a given number of entries
            {
                ScalarUpdatedData scalarData = UpdateScalar(dataPoints);

                if (scalarData.Updated)
                {
                    //TODO: Database call to scalar database
                }
            }
        }

        private static ScalarUpdatedData UpdateScalar(List<DataPoint> dataPoints)
        {
            List<double> xValues = new(), yValues = new();

            dataPoints.ForEach(point =>
            {
                xValues.Add(point.TimeStamp.ToOADate());
                yValues.Add((double)point.GlucoseError);
            });

            Tuple<double, double> alphaAndBetaHat = Fit.Line(xValues.ToArray(), yValues.ToArray());

            Tuple<double, double> predictionIntervallNextPoint = Statistics.PredictionInterval(xValues, yValues, alphaAndBetaHat.Item1, alphaAndBetaHat.Item2, 1 - 0.95);

            //TODO: finish

            return new();
        }

        /// <summary>
        /// Data returned from UpdateScalar-method()
        /// Updated is true if scalar can be changed.
        /// Scalar holds value for new scalar, only
        /// valid if Updated is true.
        /// </summary>
        struct ScalarUpdatedData
        {
            public bool Updated { get; set; }
            public float Scalar { get; set; }
        }

        /// <summary>
        /// Get a log, find the glucose error the
        /// log had, and creates a datapoint with the
        /// error and date time value
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        async private static Task<DataPoint> GetDataPointForDayProfile(LogModel log)
        {
            return new DataPoint(log.DateTimeValue, (float)log.GlucoseAfterMeal - await GetTargetGlucoseForLog(log));
        }

        private static void UpdateGroceryScalar(int groceryID)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// This method partition the error in glucose after meal,
        /// based on their insulin estimate,
        /// between all logs involved (connected to the reminder).
        /// The Logs are updated in their database.
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns>A List of the Logs in question</returns>
        async private static Task<List<LogModel>> PartitionGlucoseAfterMeal(ReminderModel reminder)
        {
            var totalInsulinGiven = reminder.Logs.Sum(log => log.InsulinEstimate); //Total insulin from all logs estimate
            var lastTargetGlucoseValue = await GetTargetGlucoseForLog(reminder.Logs[reminder.Logs.Count - 1]);
            var glucoseDifference = reminder.GlucoseAfterMeal - lastTargetGlucoseValue; //Total error in glucose

            foreach (LogModel log in reminder.Logs)
            {
                var targetGlucoseForLog = await GetTargetGlucoseForLog(log); //Get the target glucose value

                //We partiton the glucose difference based on the percentage of the insulin estimate
                //from one log to the total insulin given within the reminder.
                var glucoseDifferencePartitioned = glucoseDifference * (log.InsulinEstimate / totalInsulinGiven);


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
                var glucoseDifferenceAdjusted = glucoseDifferencePartitioned - (log.InsulinEstimate - log.InsulinFromUser) * globalVariables.InsulinToGlucoseRatio;

                log.GlucoseAfterMeal = glucoseDifferenceAdjusted;
                await LogDatabase.GetInstance().UpdateLogAsync(log);
            }

            return reminder.Logs;
        }

        /// <summary>
        /// Helper method to get the target glucose
        /// value of a day profile connected to a log
        /// </summary>
        /// <param name="log"></param>
        /// <returns>
        /// float, the target glucose from the day profile
        /// with the cooresponding ID from the log.
        /// </returns>
        async private static Task<float> GetTargetGlucoseForLog(LogModel log)
        {
            return (await DayProfileDatabase.GetInstance().GetDayProfileAsync(log.DayProfileID)).TargetGlucoseValue; //Target glucose of the log
        }
    }


    /// <summary>
    /// Used in analysis.
    /// timestamp is x-coord, and glucoseError is y-coord.
    /// </summary>
    public struct DataPoint
    {
        public DateTime TimeStamp { get; set; }
        public float GlucoseError { get; set; }

        public DataPoint(DateTime timeStamp, float glucoseError)
        {
            TimeStamp = timeStamp;
            GlucoseError = glucoseError;
        }
    }
}
