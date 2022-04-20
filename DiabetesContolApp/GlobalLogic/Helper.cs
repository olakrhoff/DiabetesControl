using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Service;

using Xamarin.Forms;
using Xamarin.Essentials;
using System.Diagnostics;

namespace DiabetesContolApp.GlobalLogic
{
    static public class Helper
    {
        /*
         * This method converts a string to a float.
         * It ignores if it uses a ',' or '.' as a seperator,
         * but it can only have one of them in the hole string.
         * 
         * Returns false if the format of the number is wrong. Set the float_val to 0.0f.
         * Returns true if format is correct, then convert the string to a float and store it in float_val.
         * 
         */
        static public bool ConvertToFloat(string stringValue, out float floatValue)
        {
            floatValue = 0.0f; //This is just a temp value, if the convertion is successful the value will be changed
            if (stringValue == null)
                return false;
            char[] seperators = { ',', '.' };

            ushort counter = (ushort)stringValue.Count(count => (count == seperators[0] || count == seperators[1]));

            if (counter > 1)
                return false;

            //If there isn't a seperator we can treat it as an integer
            if (counter != 0)
            {
                //The string has only one comma or dot in it
                int choosenSeperator;
                if (stringValue.Contains(seperators[0]))
                    choosenSeperator = 0;
                else
                    choosenSeperator = 1;

                //Check that the number seperator is in a valid place
                var seperatorIndex = stringValue.IndexOf(seperators[choosenSeperator]);
                if (seperatorIndex == 0 || seperatorIndex == stringValue.Length - 1)
                    return false; //Not a valid placment of the seperator

                //If the seperator doesn't match the current culture, then swap it with the other correct one
                if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator != seperators[choosenSeperator].ToString())
                    stringValue = stringValue.Replace(seperators[choosenSeperator], System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]);
            }


            //Try to convert the string to float
            try
            {
                floatValue = Convert.ToSingle(stringValue);
            }
            catch (FormatException fe)
            {
                Debug.WriteLine(fe.StackTrace);
                Debug.WriteLine(fe.Message);
                //Format was not valid
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.Message);
                //This should not happen, but for safety it is still here
                return false;
            }

            return true;
        }

        public static List<ShareFile> DatabaseToString()
        {
            //Write groceries to file
            string data = "groceryData.csv";
            string groceryFilePath = Path.Combine(FileSystem.CacheDirectory, data);
            WriteDatabaseToCSVFile(groceryFilePath, GroceryDatabase.GetInstance());

            //Write day profiles to file
            data = "dayProfileData.csv";
            string dayProfileFilePath = Path.Combine(FileSystem.CacheDirectory, data);
            WriteDatabaseToCSVFile(dayProfileFilePath, DayProfileDatabase.GetInstance());

            //Write reminders to file
            data = "reminderData.csv";
            string reminderPath = Path.Combine(FileSystem.CacheDirectory, data);
            WriteDatabaseToCSVFile(reminderPath, ReminderDatabase.GetInstance());

            //Write logs to file
            data = "logData.csv";
            string logPath = Path.Combine(FileSystem.CacheDirectory, data);
            WriteDatabaseToCSVFile(logPath, LogDatabase.GetInstance());

            //Write logGrocery cross table to file
            data = "groceryLogData.csv";
            string groceryLogPath = Path.Combine(FileSystem.CacheDirectory, data);
            WriteDatabaseToCSVFile(groceryLogPath, GroceryLogDatabase.GetInstance());


            //TODO: TEMP
            data = "breadData.csv";
            string breadDataPath = Path.Combine(FileSystem.CacheDirectory, data);
            //WriteBreadToCSVFile(breadDataPath);

            return new List<ShareFile> { new ShareFile(groceryFilePath),
                new ShareFile(dayProfileFilePath),
                new ShareFile(reminderPath),
                new ShareFile(logPath),
                new ShareFile(groceryLogPath),
                new ShareFile(breadDataPath)
            };
        }

        async private static void WriteBreadToCSVFile(string filePath)
        {
            ReminderService reminderService = ReminderService.GetReminderService();
            LogService logService = LogService.GetLogService();

            List<LogModel> logs = new();

            List<ReminderModel> reminders = await reminderService.GetAllRemindersAsync();

            reminders = reminders.Where(remidner => remidner.IsHandled && remidner.GlucoseAfterMeal > 0).ToList();

            foreach (ReminderModel reminder in reminders)
            {
                List<LogModel> logsWithReminderID = await logService.GetAllLogsWithReminderIDAsync(reminder.ReminderID);
                double totalInsulin = logsWithReminderID.Sum(log => log.InsulinEstimate);
                double glucoseError = (float)reminder.GlucoseAfterMeal - logsWithReminderID[logsWithReminderID.Count - 1].DayProfile.TargetGlucoseValue;

                foreach (LogModel log in logsWithReminderID)
                {
                    float targetGlucose = log.DayProfile.TargetGlucoseValue;
                    log.GlucoseAfterMeal = (float)(targetGlucose + glucoseError * (log.InsulinEstimate / totalInsulin));
                }
                logs.AddRange(logsWithReminderID);
            }

            string output = "TimeStamp,GlucoseErrorForBreadPerBread\n";

            //Get all log with bread
            logs = logs.Where(log =>
            {
                foreach (NumberOfGroceryModel g in log.NumberOfGroceries)
                    if (g.Grocery.Name == "Brød")
                        return true;
                return false;
            }).ToList();

            //Find fault for bread per log
            foreach (LogModel log in logs)
            {
                if (log.GlucoseAfterMeal == null || log.GlucoseAfterMeal == -1.0f)
                    continue; //Not ready or corrupt data.

                DayProfileModel dayProfile = log.DayProfile;
                float targetGlucose = dayProfile.TargetGlucoseValue;
                float gluoseError = (float)log.GlucoseAfterMeal - targetGlucose;

                NumberOfGroceryModel numberOfGrocery = log.NumberOfGroceries.Find(log => log.Grocery.Name == "Brød");
                float carbsPerBread = numberOfGrocery.Grocery.CarbsPer100Grams * numberOfGrocery.Grocery.GramsPerPortion * numberOfGrocery.Grocery.CarbScalar;

                Application app = Application.Current as App;

                float breadInsulin = numberOfGrocery.InsulinForGroceries;

                float gluoseErrorForBreadPerBread = gluoseError * (breadInsulin / log.InsulinEstimate) / numberOfGrocery.NumberOfGrocery;

                output += log.DateTimeValue.ToString("yyyy/MM/dd HH:mm") + "," + gluoseErrorForBreadPerBread.ToString("0.00", CultureInfo.InvariantCulture) + "\n";
            }
            //Write timestamp and fault to file

            File.WriteAllText(filePath, output);
        }

        async private static void WriteDatabaseToCSVFile(string filePath, ModelDatabaseAbstract databaseConnection)
        {
            string output = "";

            List<DAO.IModelDAO> models = await databaseConnection.GetAllAsync();
            //TODO: THIS SHOULD USE THE SERVICE
            output += databaseConnection.HeaderForCSVFile();
            models.ForEach(model => output += model.ToStringCSV());

            File.WriteAllText(filePath, output);
        }

        /*
         * ObservableCollection does not support the Sort-method.
         * This is a quick fix for that, to make other code more readable
         * 
         * The object T MUST implemnet the IComparable interface.
         * 
         * Parmas: ObservableCollection<T> (observableCollection): The ObservableCollection to be sorted
         * 
         * Return: ObservableCollection<T>, returns the collection sorted.
         */
        static public ObservableCollection<T> SortObservableCollection<T>(ObservableCollection<T> observableCollection)
        {
            var list = observableCollection.ToList();
            list.Sort();

            return new(list);
        }

        //public static float CalculateInsulin(float glucose, List<NumberOfGroceryModel> numberOfGroceryList, DayProfileModel dayProfile)
        /// <summary>
        /// This method calculates the amout of insulin the user needs
        /// based on what the glucose, food and time of day
        /// </summary>
        /// <param name="log"></param>
        /// <returns>void</returns>
        public static void CalculateInsulin(ref LogModel log)
        {
            App globalVariables = Application.Current as App;

            //float insulinForFood = GetCarbsFromFood(log.NumberOfGroceryModels) * log.DayProfile.CarbScalar / globalVariables.InsulinToCarbohydratesRatio;
            float insulinForFood = GetInsulinForGroceries(log.NumberOfGroceries, log.DayProfile.CarbScalar, globalVariables.InsulinToCarbohydratesRatio).Sum(numberOfGrocery => numberOfGrocery.InsulinForGroceries);

            float insulinForCorrection = (log.GlucoseAtMeal - log.DayProfile.TargetGlucoseValue) * log.DayProfile.GlucoseScalar / globalVariables.InsulinToGlucoseRatio;

            //If it is a pure correction dose, no food (carbs)
            if (insulinForFood == 0)
                insulinForCorrection *= globalVariables.InsulinOnlyCorrectionScalar;

            log.CorrectionInsulin = insulinForCorrection;
            log.InsulinEstimate = insulinForFood + insulinForCorrection; //Total insulin
        }

        /// <summary>
        /// Goes through all NumberOfGroceries and adds the amount
        /// of insulin based on the DayProfile carbs scalar and
        /// insulin to carbs ratio given.
        /// </summary>
        /// <param name="numberOfGroceries"></param>
        /// <param name="dayProfileCarbScalar"></param>
        /// <param name="insulinToCarbohydratesRatio"></param>
        /// <returns>List of NumberOfGroceryModels, with InsulinForGroceries filled out.</returns>
        private static List<NumberOfGroceryModel> GetInsulinForGroceries(List<NumberOfGroceryModel> numberOfGroceries, float dayProfileCarbScalar, float insulinToCarbohydratesRatio)
        {
            numberOfGroceries.ForEach(numberOfGrocery =>
            {
                float carbsForGroceries = numberOfGrocery.NumberOfGrocery * numberOfGrocery.Grocery.GramsPerPortion * (numberOfGrocery.Grocery.CarbsPer100Grams / 100) * numberOfGrocery.Grocery.CarbScalar;
                numberOfGrocery.InsulinForGroceries = carbsForGroceries * dayProfileCarbScalar / insulinToCarbohydratesRatio;
            });

            return numberOfGroceries;
        }

        /// <summary>
        /// IMPORTANT: insulin here is always rapid insulin
        /// 
        /// This method uses the 500-rule to calculate
        /// the sensitivity for carbohydrates.This gives the
        /// sensitivity as grams of carbs per unit of insulin.
        /// 
        /// E.g. a TDD = 50 gives a sensitivity og 500/50 = 10
        /// this means that if one eat 10 grams og carbs, one unit of
        /// rapid insulin is needed to counteract it.
        /// The insulin is then calculated later by taking the total
        /// grams of carbs divided by the sensitivty.
        ///
        /// </summary>
        /// <param name="averageTDD">
        /// float (averageTDD), this is the average Total Daily Dose
        /// of rapid insulin in the last seven(valid) days.
        /// </param>
        /// <returns>float, the sensitivity in grams of carbs per unit of insulin.</returns>
        public static float Calculate500Rule(float averageTDD)
        {
            return 500 / averageTDD;
        }

        /// <summary>
        /// IMPORTANT: insulin here is always rapid insulin
        ///
        /// This method uses the 100-rule to calculate
        /// the sensitivity for glucose correction.This gives the
        /// sensitivity as glucose (mmol/L) changed per unit of insulin.
        ///
        /// E.g. a TDD = 50 gives a sensitivity og 100/50 = 2
        /// this means that if one sat one unit of insulin the glucose
        /// would go down by two(e.g. 7,8 mmol/L => 5,8 mmol/L).
        /// The insulin is then calculated later by taking the total
        /// wanted change in glucose divided by the sensitivty.
        /// 
        /// </summary>
        /// <param name="averageTDD">
        /// float (averageTDD), this is the average Total Daily Dose
        /// of rapid insulin in the last seven(valid) days.
        /// </param>
        /// <returns>float, the sensitivity in glucose (mmol/L) changed per unit of insulin.</returns>
        public static float Calculate100Rule(float averageTDD)
        {
            return 100 / averageTDD;
        }
    }
}
