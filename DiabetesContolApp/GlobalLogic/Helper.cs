﻿using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;

using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.IO;
using System.Globalization;

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
        static public bool ConvertToFloat(string string_val, out float float_val)
        {
            float_val = 0.0f; //This is just a temp value, if the convertion is successful the value will be changed
            if (string_val == null)
                return false;
            char[] seperators = { ',', '.' };

            ushort counter = (ushort)string_val.Count(count => (count == seperators[0] || count == seperators[1]));

            if (counter > 1)
                return false;

            //If there isn't a seperator we can treat it as an integer
            if (counter != 0)
            {
                //The string has only one comma or dot in it
                int choosenSeperator;
                if (string_val.Contains(seperators[0]))
                    choosenSeperator = 0;
                else
                    choosenSeperator = 1;

                //Check that the number seperator is in a valid place
                var seperatorIndex = string_val.IndexOf(seperators[choosenSeperator]);
                if (seperatorIndex == 0 || seperatorIndex == string_val.Length - 1)
                    return false; //Not a valid placment of the seperator

                //If the seperator doesn't match the current culture, then swap it with the other correct one
                if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator != seperators[choosenSeperator].ToString())
                    string_val = string_val.Replace(seperators[choosenSeperator], System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]);
            }


            //Try to convert the string to float
            try
            {
                float_val = Convert.ToSingle(string_val);
            }
            catch (FormatException fe)
            {
                //Format was not valid
                return false;
            }
            catch (Exception e)
            {
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
            List<LogModel> logs = new();

            List<ReminderModel> reminders = await ReminderDatabase.GetInstance().GetRemindersAsync();

            reminders = reminders.Where(remidner => remidner.IsHandled && remidner.GlucoseAfterMeal > 0).ToList();

            foreach (ReminderModel reminder in reminders)
            {
                List<LogModel> logsWithReminderID = await LogDatabase.GetInstance().GetLogsWithReminderAsync(reminder.ReminderID);
                double totalInsulin = logsWithReminderID.Sum(log => log.InsulinEstimate);
                double glucoseError = (float)reminder.GlucoseAfterMeal - (await DayProfileDatabase.GetInstance().GetDayProfileAsync(logsWithReminderID[logsWithReminderID.Count - 1].DayProfileID)).TargetGlucoseValue;

                foreach (LogModel log in logsWithReminderID)
                {
                    float targetGlucose = (await DayProfileDatabase.GetInstance().GetDayProfileAsync(log.DayProfileID)).TargetGlucoseValue;
                    log.GlucoseAfterMeal = (float)(targetGlucose + glucoseError * (log.InsulinEstimate / totalInsulin));
                }
                logs.AddRange(logsWithReminderID);
            }

            string output = "TimeStamp,GlucoseErrorForBreadPerBread\n";

            //Get all log with bread
            logs = logs.Where(log =>
            {
                foreach (NumberOfGroceryModel g in log.NumberOfGroceryModels)
                    if (g.Grocery.Name == "Brød")
                        return true;
                return false;
            }).ToList();

            //Find fault for bread per log
            foreach (LogModel log in logs)
            {
                if (log.GlucoseAfterMeal == null || log.GlucoseAfterMeal == -1.0f)
                    continue; //Not ready or corrupt data.

                DayProfileModel dayProfile = await DayProfileDatabase.GetInstance().GetDayProfileAsync(log.DayProfileID);
                float targetGlucose = dayProfile.TargetGlucoseValue;
                float gluoseError = (float)log.GlucoseAfterMeal - targetGlucose;

                NumberOfGroceryModel numberOfGrocery = log.NumberOfGroceryModels.Find(log => log.Grocery.Name == "Brød");
                float carbsPerBread = numberOfGrocery.Grocery.CarbsPer100Grams * numberOfGrocery.Grocery.GramsPerPortion * numberOfGrocery.Grocery.CarbScalar;

                Application app = Application.Current as App;
                List<NumberOfGroceryModel> breadList = new();
                breadList.Add(numberOfGrocery);
                float breadInsulin = CalculateInsulin(dayProfile.TargetGlucoseValue, breadList, dayProfile);

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

        /*
         * This method calculates the amout of insulin the user needs
         * based on what the glucose, food and time of day
         * 
         * Params: float (glucose): the glucose of the user,
         *         List<NumberOfGroceryModel> (numberOfGroceryList): The list of what groceries are to be eaten
         *         and the respective number of portions.
         *         DayprofileModel (dayProfile): The dayprofile holds the info of scalars based on the time of day
         *         both for glucose and carbs.
         *         
         * Return: float, the total amount of insulin to be given by the user.
         */
        public static float CalculateInsulin(float glucose, List<NumberOfGroceryModel> numberOfGroceryList, DayProfileModel dayProfile)
        {
            App globalVariables = Application.Current as App;

            float insulinForFood = GetCarbsFromFood(numberOfGroceryList) * dayProfile.CarbScalar / globalVariables.InsulinToCarbohydratesRatio;

            float insulinForCorrection = (glucose - dayProfile.TargetGlucoseValue) * dayProfile.GlucoseScalar / globalVariables.InsulinToGlucoseRatio;

            //If it is a pure correction dose, no food (carbs)
            if (insulinForFood == 0)
                insulinForCorrection *= globalVariables.InsulinOnlyCorrectionScalar;

            return insulinForFood + insulinForCorrection; //Total insulin
        }

        /*
         * This method gets all the total amout of carbs (times the respective scalars)
         * and returns it.
         * 
         * Parmas: List<NumberOfGroceryModel>, the list of Groceries and the respective
         * number of portions.
         * 
         * Return: float, the total number of carbs in the list, with scaling
         */
        private static float GetCarbsFromFood(List<NumberOfGroceryModel> numberOfGroceryList)
        {
            float totalCarbs = 0.0f;

            if (numberOfGroceryList != null)
                foreach (NumberOfGroceryModel numberOfGrocery in numberOfGroceryList)
                    totalCarbs += numberOfGrocery.NumberOfGrocery * numberOfGrocery.Grocery.GramsPerPortion * (numberOfGrocery.Grocery.CarbsPer100Grams / 100) * numberOfGrocery.Grocery.CarbScalar;

            return totalCarbs;
        }


        /*
         * IMPORTANT: insulin here is always rapid insulin
         * 
         * This method uses the 500-rule to calculate
         * the sensitivity for carbohydrates. This gives the
         * sensitivity as grams of carbs per unit of insulin.
         * 
         * E.g. a TDD = 50 gives a sensitivity og 500/50 = 10
         * this means that if one eat 10 grams og carbs, one unit of
         * rapid insulin is needed to counteract it.
         * The insulin is then calculated later by taking the total
         * grams of carbs divided by the sensitivty.
         * 
         * Parmas: float (averageTDD), this is the average Total Daily Dose
         * of rapid insulin in the last seven (valid) days.
         * 
         * Return: float, the sensitivioty in grams of carbs per unit of insulin.
         */
        public static float Calculate500Rule(float averageTDD)
        {
            return 500 / averageTDD;
        }


        /*
         * IMPORTANT: insulin here is always rapid insulin
         * 
         * This method uses the 100-rule to calculate
         * the sensitivity for glucose correction. This gives the
         * sensitivity as glucose (mmol/L) changed per unit of insulin.
         * 
         * E.g. a TDD = 50 gives a sensitivity og 100/50 = 2
         * this means that if one sat one unit of insulin the glucose
         * would go down by two (e.g. 7,8 mmol/L => 5,8 mmol/L).
         * The insulin is then calculated later by taking the total
         * wanted change in glucose divided by the sensitivty. 
         * 
         * Parmas: float (averageTDD), this is the average Total Daily Dose
         * of rapid insulin in the last seven (valid) days.
         * 
         * Return: float, the sensitivioty in glucose (mmol/L) changed per unit of insulin.
         */
        public static float Calculate100Rule(float averageTDD)
        {
            return 100 / averageTDD;
        }
    }
}
