using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Persistence;
using DiabetesContolApp.Models;

using MathNet.Numerics;
using MathNet.Numerics.Distributions;


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
        async private static void UpdateScalarValues(List<LogModel> logs)
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
        /// between all logs involved (connected to the reminder).
        /// The Logs are updated in their database.
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns>A List of the Logs in question</returns>
        async private static Task<List<LogModel>> PartitionGlucoseAfterMeal(ReminderModel reminder)
        {
            var totalInsulinGiven = reminder.Logs.Sum(log => log.InsulinFromUser); //Total insulin from all logs
            var lastTargetGlucoseValue = await GetTargetGlucoseForLog(reminder.Logs[reminder.Logs.Count - 1]);
            var glucoseDifference = reminder.GlucoseAfterMeal - lastTargetGlucoseValue; //Total error in glucose

            foreach (LogModel log in reminder.Logs)
            {
                var targetGlucoseForLog = await GetTargetGlucoseForLog(log);
                log.GlucoseAfterMeal = targetGlucoseForLog + (glucoseDifference * (log.InsulinFromUser / totalInsulinGiven));
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
