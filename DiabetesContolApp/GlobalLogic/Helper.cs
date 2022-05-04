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
using System.Threading.Tasks;

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

        async public static Task<List<ShareFile>> DatabaseToString()
        {
            List<ShareFile> shareFiles = new();

            //Write groceries to file
            string data = "groceryData.csv";
            string groceryFilePath = Path.Combine(FileSystem.CacheDirectory, data);
            await WriteDatabaseToCSVFile(groceryFilePath, GroceryDatabase.GetInstance());
            shareFiles.Add(new ShareFile(groceryFilePath));

            //Write day profiles to file
            data = "dayProfileData.csv";
            string dayProfileFilePath = Path.Combine(FileSystem.CacheDirectory, data);
            await WriteDatabaseToCSVFile(dayProfileFilePath, DayProfileDatabase.GetInstance());
            shareFiles.Add(new ShareFile(dayProfileFilePath));

            //Write reminders to file
            data = "reminderData.csv";
            string reminderPath = Path.Combine(FileSystem.CacheDirectory, data);
            await WriteDatabaseToCSVFile(reminderPath, ReminderDatabase.GetInstance());
            shareFiles.Add(new ShareFile(reminderPath));

            //Write logs to file
            data = "logData.csv";
            string logPath = Path.Combine(FileSystem.CacheDirectory, data);
            await WriteDatabaseToCSVFile(logPath, LogDatabase.GetInstance());
            shareFiles.Add(new ShareFile(logPath));

            //Write logGrocery cross table to file
            data = "groceryLogData.csv";
            string groceryLogPath = Path.Combine(FileSystem.CacheDirectory, data);
            await WriteDatabaseToCSVFile(groceryLogPath, GroceryLogDatabase.GetInstance());
            shareFiles.Add(new ShareFile(groceryLogPath));


            //Now we want to write the data needed for the analysis
            //We start with the groceries
            shareFiles.AddRange(await WriteGroceriesToCSVFile());
            //We add the dayProfile-data
            shareFiles.AddRange(await WriteDayProfilesToCSVFile());
            //We add the correction insulin-data
            shareFiles.Add(await WriteCorrectionInsulinToCSVFile());

            return shareFiles;
        }

        /// <summary>
        /// Gets all Logs with Grocery-models and writes them to
        /// ShareFiles.
        /// </summary>
        /// <returns>List of ShareFiles.</returns>
        async private static Task<List<ShareFile>> WriteGroceriesToCSVFile()
        {
            const int MINIMUM_NEEDED_DATA = 15;

            List<ShareFile> shareFiles = new();

            List<GroceryModel> groceries = await GroceryService.GetGroceryService().GetAllGroceriesAsync();

            foreach (GroceryModel grocery in groceries)
            {
                string filename = "grocery_" + grocery.Name + "_" + grocery.BrandName + ".csv";
                if (filename.Contains("/"))
                    filename = filename.Replace("/", "-");
                string groceryFilepath = Path.Combine(FileSystem.CacheDirectory, filename);
                //Logic
                List<LogModel> logsWithGrocery = (await LogService.GetLogService().GetAllLogsAsync()).Where(log =>
                {
                    foreach (var nog in log.NumberOfGroceries)
                        if (nog.Grocery.GroceryID == grocery.GroceryID)
                            return true;
                    return false;
                }).ToList();

                if (logsWithGrocery.Count < MINIMUM_NEEDED_DATA)
                    continue;

                //Get all reminders connected to the Logs
                List<ReminderModel> reminders = new();
                foreach (var log in logsWithGrocery)
                    if (reminders.Find(r => r.ReminderID == log.Reminder.ReminderID) == null)
                    {
                        reminders.Add(await ReminderService.GetReminderService().GetReminderAsync(log.Reminder.ReminderID));
                        reminders.Last().Logs = logsWithGrocery.FindAll(log => log.Reminder.ReminderID == reminders.Last().ReminderID);
                        reminders.Last().Logs.Sort();
                    }

                if (reminders.Count < MINIMUM_NEEDED_DATA)
                    continue;

                string output = "TimeStamp;Scalar;GlucoseError\n";


                foreach (ReminderModel reminder in reminders)
                {
                    if (!reminder.IsGlucoseAfterMealValid())
                        continue;
                    output += reminder.DateTimeValue.ToString("yyyy/MM/dd HH/mm") + ";"
                        + (await ScalarService.GetScalarService().GetFirstScalarBeforeDateForTypeWithObjectIDAsync(grocery.GetScalarType(), grocery.GetIDForScalarObject(), reminder.DateTimeValue)).ScalarValue + ";"
                        + ((float)reminder.GlucoseAfterMeal - reminder.Logs.Last().DayProfile.TargetGlucoseValue) + "\n";
                }

                //Logic
                File.WriteAllText(groceryFilepath, output);
                shareFiles.Add(new ShareFile(groceryFilepath));
            }

            return shareFiles;
        }

        /// <summary>
        /// Gets all Logs with DayProfiles-models and writes them to
        /// ShareFiles.
        /// </summary>
        /// <returns>List of ShareFiles.</returns>
        async private static Task<List<ShareFile>> WriteDayProfilesToCSVFile()
        {
            const int MINIMUM_NEEDED_DATA = 15;

            List<ShareFile> shareFiles = new();

            List<DayProfileModel> dayProfiles = await DayProfileService.GetDayProfileService().GetAllDayProfilesAsync();

            foreach (DayProfileModel dayProfile in dayProfiles)
            {
                string filename = "dayProfile_" + dayProfile.Name + ".csv";
                if (filename.Contains("/"))
                    filename = filename.Replace("/", "-");
                string dayProfilePath = Path.Combine(FileSystem.CacheDirectory, filename);
                //Logic
                List<LogModel> logsWithDayProfile = await LogService.GetLogService().GetAllLogsWithDayProfileIDAsync(dayProfile.DayProfileID);

                if (logsWithDayProfile.Count < MINIMUM_NEEDED_DATA)
                    continue; //There is not enough data to be used in statistical analysis

                //Get all reminders connected to the Logs
                List<ReminderModel> reminders = new();
                foreach (var log in logsWithDayProfile)
                    if (reminders.Find(r => r.ReminderID == log.Reminder.ReminderID) == null)
                    {
                        reminders.Add(await ReminderService.GetReminderService().GetReminderAsync(log.Reminder.ReminderID));
                        reminders.Last().Logs = logsWithDayProfile.FindAll(log => log.Reminder.ReminderID == reminders.Last().ReminderID);
                        reminders.Last().Logs.Sort();
                    }

                if (reminders.Count < MINIMUM_NEEDED_DATA)
                    continue; //Again not enough data

                string output = "TimeStamp;CarbScalar;GlucoseScalar;GlucoseError\n";


                foreach (ReminderModel reminder in reminders)
                {
                    if (!reminder.IsGlucoseAfterMealValid())
                        continue;
                    dayProfile.SetScalarTypeToCarbScalar();
                    output += reminder.DateTimeValue.ToString("yyyy/MM/dd HH/mm") + ";"
                        + (await ScalarService.GetScalarService().GetFirstScalarBeforeDateForTypeWithObjectIDAsync(dayProfile.GetScalarType(), dayProfile.GetIDForScalarObject(), reminder.DateTimeValue)).ScalarValue + ";";
                    dayProfile.SetScalarTypeToGlucoseScalar();
                    output += (await ScalarService.GetScalarService().GetFirstScalarBeforeDateForTypeWithObjectIDAsync(dayProfile.GetScalarType(), dayProfile.GetIDForScalarObject(), reminder.DateTimeValue)).ScalarValue + ";"
                    + ((float)reminder.GlucoseAfterMeal - reminder.Logs.Last().DayProfile.TargetGlucoseValue) + "\n";
                }

                //Logic
                File.WriteAllText(dayProfilePath, output);
                shareFiles.Add(new ShareFile(dayProfilePath));
            }

            return shareFiles;
        }

        /// <summary>
        /// Gets all Logs with CorrectionInsulin and writes them to
        /// ShareFiles.
        /// </summary>
        /// <returns>List of ShareFiles.</returns>
        async private static Task<ShareFile> WriteCorrectionInsulinToCSVFile()
        {
            const int MINIMUM_NEEDED_DATA = 15;

            ShareFile shareFile;

            List<LogModel> logsWithCorrectionInsulin = (await LogService.GetLogService().GetAllLogsAsync()).Where(log => log.CorrectionInsulin != 0 && log.IsLogDataValid()).ToList();

            if (logsWithCorrectionInsulin.Count < MINIMUM_NEEDED_DATA)
                return null; //There is not enough data to be used in statistical analysis

            //Get all reminders connected to the Logs
            List<ReminderModel> reminders = new();
            foreach (var log in logsWithCorrectionInsulin)
                if (reminders.Find(r => r.ReminderID == log.Reminder.ReminderID) == null)
                {
                    reminders.Add(await ReminderService.GetReminderService().GetReminderAsync(log.Reminder.ReminderID));
                    reminders.Last().Logs = logsWithCorrectionInsulin.FindAll(log => log.Reminder.ReminderID == reminders.Last().ReminderID);
                    reminders.Last().Logs.Sort();
                }

            if (reminders.Count < MINIMUM_NEEDED_DATA)
                return null; //Again not enough data

            string output = "TimeStamp;Scalar;GlucoseInsulinSensitivity;GlucoseError\n";


            string filename = "correctionInsulin.csv";
            string dayProfilePath = Path.Combine(FileSystem.CacheDirectory, filename);
            //Logic

            List<string> timestamps = new();
            List<float> scalars = new();
            List<float> glucoseInsulinSensitivity = new();
            List<float> glucoseError = new();

            //foreach (ReminderModel reminder in reminders)
            for (int i = reminders.Count - 1; i >= 0; i--)
            {
                if (!reminders[i].IsGlucoseAfterMealValid())
                    continue;
                ReminderModel reminder = reminders[i];
                timestamps.Add(reminder.DateTimeValue.ToString("yyyy/MM/dd HH/mm"));
                scalars.Add((await ScalarService.GetScalarService().GetFirstScalarBeforeDateForTypeWithObjectIDAsync(ScalarTypes.CORRECTION_INSULIN, -1, reminder.DateTimeValue)).ScalarValue);
                /*if (glucoseInsulinSensitivity.Count == 0)
                    glucoseInsulinSensitivity.Add((Application.Current as App).InsulinToGlucoseRatio);
                else if (scalars.Last() != scalars[scalars.Count - 2])
                    glucoseInsulinSensitivity.Add(glucoseInsulinSensitivity.Last() * scalars.Last());
                else
                    glucoseInsulinSensitivity.Add(glucoseInsulinSensitivity.Last());*/
                glucoseError.Add((float)reminder.GlucoseAfterMeal - reminder.Logs.Last().DayProfile.TargetGlucoseValue);
            }
            timestamps.Reverse();
            scalars.Reverse();
            glucoseError.Reverse();

            double test = 2.2d; //This was the inital value that was stared with
            var values = scalars.Distinct();
            foreach (var s in values)
            {
                test /= s;
                foreach (var val in scalars)
                    if (val == s)
                        glucoseInsulinSensitivity.Add((float)test);
            }
            for (int i = 0; i < timestamps.Count; ++i)
            {
                output += timestamps[i] + ";"
                    + scalars[i] + ";"
                    + glucoseInsulinSensitivity[i] + ";"
                    + glucoseError[i] + "\n";
            }
            //Logic
            File.WriteAllText(dayProfilePath, output);
            return new ShareFile(dayProfilePath);

        }

        /// <summary>
        /// Writes the given database table to the spesified
        /// filepath.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="databaseConnection"></param>
        /// <returns></returns>
        async private static Task WriteDatabaseToCSVFile(string filepath, ModelDatabaseAbstract databaseConnection)
        {
            string output = "";

            List<DAO.IModelDAO> models = await databaseConnection.GetAllAsync();
            output += databaseConnection.HeaderForCSVFile();
            models.ForEach(model => output += model.ToStringCSV());

            File.WriteAllText(filepath, output);
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

        /// <summary>
        /// This method takes a double value and rounds it of to
        /// the "digits"-number of significant digits
        /// </summary>
        /// <example>
        /// value = 0.001234
        /// We want the 2 (digits = 2) most significant digits
        ///
        /// Step by step
        /// 0.001234 => 1.234000
        /// 1.234000 => 1.230000
        /// 1.230000 => 0.001230
        ///
        /// 
        /// </example>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        public static void FirstNSignificantDigits(this ref double value, uint digits)
        {
            if (value == 0d)
                return; //There is nothing her to change.

            //We then scale the number to be on scientific format
            double scaleFactor = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(value))) + 1);

            //Then we round to the correct number of significant figures, then scale the value back
            value = scaleFactor * Math.Round(value / scaleFactor, (int)digits);
        }
    }
}
