using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Models;
using DiabetesContolApp.GlobalLogic;

using SQLite;
using Xamarin.Forms;
using System.Diagnostics;

namespace DiabetesContolApp.Persistence
{
    public sealed class LogDatabase : ModelDatabaseAbstract
    {
        private static LogDatabase instance = null;

        public LogDatabase()
        {
        }

        public static LogDatabase GetInstance()
        {
            return instance == null ? new LogDatabase() : instance;
        }


        /*
         * This method inserts a log into the database, after it
         * connects it to a reminder. If the log overlaps with
         * another log, they will share a reminder.
         * 
         * Then it addes all the groceryLog values into the bridge table
         * 
         *
         * Lastly it updates the average TDD (total daily dose (of rapid insulin))
         * 
         * Paramas: LogModel, the log to insert
         * 
         * Return: the number of rows added.
         */
        async internal Task<int> InsertLogAsync(LogModel newLogEntry)
        {
            ReminderDatabase reminderDatabase = ReminderDatabase.GetInstance();

            //If an overlap was registered in the calculator
            if (newLogEntry.ReminderID != -1)
            {
                ReminderModel reminder = await reminderDatabase.GetReminderAsync(newLogEntry.ReminderID);

                try
                {
                    reminder.UpdateDateTime();
                    await reminderDatabase.UpdateReminderAsync(reminder);
                }
                catch (NullReferenceException nre)
                {
                    Debug.WriteLine(nre.Message);
                    newLogEntry.ReminderID = -1; //If there was no remidner with this ID, default the value
                }
            }
            else if (newLogEntry.GlucoseAfterMeal == null)
            {
                LogModel newestLog = await GetNewestLogAsync();
                if (newestLog != null)
                {
                    if (newestLog.DateTimeValue.AddHours(ReminderModel.TIME_TO_WAIT) > newLogEntry.DateTimeValue) //If the meals are overlapping
                    {
                        newLogEntry.ReminderID = newestLog.ReminderID;
                        ReminderModel reminder = await reminderDatabase.GetReminderAsync(newLogEntry.ReminderID);
                        reminder.UpdateDateTime();
                        await reminderDatabase.UpdateReminderAsync(reminder);
                    }
                }
                if (newLogEntry.ReminderID == -1) //If the log still hasn't gotten a reminder connected to it, it need a new one
                {
                    //Need to create a new reminder
                    await reminderDatabase.InsertReminderAsync(new ReminderModel());
                    var reminders = await reminderDatabase.GetRemindersAsync();
                    ReminderModel newestReminder = reminders.Max(); //Gets the reminder ID
                    newLogEntry.ReminderID = newestReminder.ReminderID;
                }
            }

            var rowsAdded = await connection.InsertAsync(newLogEntry);

            await connection.InsertAllAsync(GroceryLogModel.GetGroceryLogs(newLogEntry.NumberOfGroceryModels, newLogEntry.LogID));

            await UpdateAverageTDD();


            return rowsAdded;
        }

        /// <summary>
        /// This method get the newest Log based on the time it
        /// has in its variable DateTime.
        /// </summary>
        /// <returns>
        /// Task&lt;LogModel&gt;
        /// 
        /// Task is for async
        /// LogModel is the newest model,
        /// if no Log is found, it returns null.
        /// </returns>
        async public Task<LogModel> GetNewestLogAsync()
        {
            var logs = await connection.Table<LogModel>().ToListAsync();

            if (logs.Count == 0)
                return null;

            logs.Sort();

            return logs[logs.Count - 1];
        }


        /*
         * This method updates the average TDD (total daily dose (of rapid insulin))
         * The average is of at least three days and at most seven. It will at max
         * look 14 days (two weeks) back in time, older values than these are regarded
         * as too old for use in the TDD. We start looking at yesterday and backward,
         * we do not want to use todays logs, since these are not finished and will
         * pull the TDD down.
         * 
         * TODO: When TimeProfiles, like "weekday" and "weekend" is added, this
         * method should get the average of the current TimeProfile, to be more 
         * accurate in the representation.
         * 
         * Params: None
         * 
         * Return: Task<bool>, Task for async, true if updated, else false
         */
        async private Task<bool> UpdateAverageTDD()
        {
            List<LogModel> logs = new();
            int daysWithLogs = 0, maxIterations = 14;
            for (int i = 1; i <= maxIterations; ++i) //We start looking at yesterday
            {
                var temp = await this.GetLogsAsync(DateTime.Now.AddDays(-i));
                if (temp != null && temp.Count > 0)
                {
                    daysWithLogs++;
                    logs.AddRange(temp);
                }
                if (daysWithLogs >= 7)
                    break; //We have the seven most recent days
            }

            if (daysWithLogs < 3)
                return false; //There were not enough data to updated the TDD

            float newAverageTDD = logs.Sum(log => log.InsulinFromUser) / daysWithLogs;

            //After the average TDD is calcualted we need to update
            //the propertis for carb and glucose sensitivity
            //with the 500- and 100-rule
            App globalVariables = Application.Current as App;

            globalVariables.InsulinToCarbohydratesRatio = Helper.Calculate500Rule(newAverageTDD);
            globalVariables.InsulinToGlucoseRatio = Helper.Calculate100Rule(newAverageTDD);

            await globalVariables.SavePropertiesAsync();

            return true;
        }

        async public Task<LogModel> GetLogAsync(int logID)
        {
            LogModel log = await connection.GetAsync<LogModel>(logID);

            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().Where(e => e.LogID == logID).ToListAsync();

            log.NumberOfGroceryModels = GroceryLogModel.GetNumberOfGroceries(groceryLogs);

            GroceryDatabase groceryDatabase = GroceryDatabase.GetInstance();

            foreach (NumberOfGroceryModel numberOfGrocery in log.NumberOfGroceryModels)
                numberOfGrocery.Grocery = await groceryDatabase.GetGroceryAsync(numberOfGrocery.Grocery.GroceryID);


            return log;
        }

        async public Task<int> UpdateLogAsync(LogModel log)
        {
            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().Where(e => e.LogID == log.LogID).ToListAsync();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                await connection.DeleteAsync(groceryLog); //Delete the prevoius GroceryLog entris for the log

            //Insert the updated grocery list for the log
            await connection.InsertAllAsync(GroceryLogModel.GetGroceryLogs(log.NumberOfGroceryModels, log.LogID));


            return await connection.UpdateAsync(log);
        }

        async public Task<List<LogModel>> GetLogsAsync(DateTime? dateTime = null)
        {
            var logs = await connection.Table<LogModel>().ToListAsync();

            if (dateTime == null)
                return logs;

            //This is safe since we will not get past the
            //if-statment above if dateTime is null
            DateTime dateTimeNotNull = (DateTime)dateTime;

            List<LogModel> temp = new();

            foreach (LogModel log in logs)
                if (log.DateTimeValue.Date.Equals(dateTimeNotNull.Date))
                    temp.Add(log);

            logs = temp;

            for (int i = 0; i < logs.Count; ++i)
                logs[i] = await GetLogAsync(logs[i].LogID);

            return logs;
        }

        /*
         * This method deletes a log based on its ID,
         * it also deletes all the groceryLog entries it is
         * connected to first
         * 
         * Parmas: int (logID), the ID of the log to be deleted
         * Return: int, number of rows deleted
         */
        async public Task<int> DeleteLogAsync(int logID)
        {
            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().ToListAsync();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                if (groceryLog.LogID == logID)
                    await connection.DeleteAsync(groceryLog); //Deletes all the entries in GroceryLog who are connected to the Grocery

            return await connection.DeleteAsync<LogModel>(logID);
        }
    }
}
