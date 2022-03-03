using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Models;
using DiabetesContolApp.GlobalLogic;

using SQLite;
using Xamarin.Forms;

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

            if (newLogEntry.GlucoseAfterMeal == null)
            {
                var logs = await GetLogsAsync(DateTime.Now); //Get logs for today
                if (logs == null || logs.Count == 0) //In the case of it just turning midnight
                    logs.AddRange(await GetLogsAsync(DateTime.Now.AddDays(-1))); //Get logs from yesterday

                logs.Sort(); //Get them in chronological order

                LogModel newestLog = logs[logs.Count - 1];

                if (newestLog.DateTimeValue.AddHours(ReminderModel.TIME_TO_WAIT) > newLogEntry.DateTimeValue) //If the meals are overlapping
                {
                    newLogEntry.ReminderID = newestLog.ReminderID;
                    ReminderModel reminder = await reminderDatabase.GetReminderAsync(newLogEntry.ReminderID);
                    reminder.UpdateDateTime();
                    await reminderDatabase.UpdateReminderAsync(reminder);
                }
                else
                {
                    //Need to create a new reminder
                    await reminderDatabase.InsertReminderAsync(new ReminderModel());
                    var reminders = await reminderDatabase.GetRemindersAsync();
                    ReminderModel newestReminder = reminders.Max();
                    newLogEntry.ReminderID = newestReminder.ReminderID;
                }
            }

            var rowsAdded = await connection.InsertAsync(newLogEntry);

            await connection.InsertAllAsync(GroceryLogModel.GetGroceryLogs(newLogEntry.NumberOfGroceryModels, newLogEntry.LogID));

            await UpdateAverageTDD();


            return rowsAdded;
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

        async internal Task<LogModel> GetLogAsync(int logID)
        {
            LogModel log = await connection.GetAsync<LogModel>(logID);

            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().Where(e => e.LogID == logID).ToListAsync();

            log.NumberOfGroceryModels = GroceryLogModel.GetNumberOfGroceries(groceryLogs);

            GroceryDatabase groceryDatabase = GroceryDatabase.GetInstance();

            foreach (NumberOfGroceryModel numberOfGrocery in log.NumberOfGroceryModels)
                numberOfGrocery.Grocery = await groceryDatabase.GetGroceryAsync(numberOfGrocery.Grocery.GroceryID);


            return log;
        }

        async internal Task<int> UpdateLogAsync(LogModel log)
        {
            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().Where(e => e.LogID == log.LogID).ToListAsync();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                await connection.DeleteAsync(groceryLog); //Delete the prevoius GroceryLog entris for the log

            //Insert the updated grocery list for the log
            await connection.InsertAllAsync(GroceryLogModel.GetGroceryLogs(log.NumberOfGroceryModels, log.LogID));


            return await connection.UpdateAsync(log);
        }

        async internal Task<List<LogModel>> GetLogsAsync(DateTime dateTime)
        {
            var logs = await connection.Table<LogModel>().ToListAsync();

            List<LogModel> temp = new();

            foreach (LogModel log in logs)
                if (log.DateTimeValue.Date.Equals(dateTime.Date))
                    temp.Add(log);

            logs = temp;

            for (int i = 0; i < logs.Count; ++i)
                logs[i] = await GetLogAsync(logs[i].LogID);

            return logs;
        }

        /*
         * This method deletes a log, it calls the delete
         * by logID method.
         * 
         * Params: LogModel (log), the log to be deleted
         * Return: int, number of rows deleted
         */
        async internal Task<int> DeleteLogAsync(LogModel log)
        {
            return await DeleteLogAsync(log.LogID);

            /*
            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().ToListAsync();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                if (groceryLog.LogID == log.LogID)
                    await connection.DeleteAsync(groceryLog); //Deletes all the entries in GroceryLog who are connected to the Grocery

            return await connection.DeleteAsync(log);*/
        }

        /*
         * This method deletes a log based on its ID,
         * it also deletes all the groceryLog entries it is
         * connected to first
         * 
         * Parmas: int (logID), the ID of the log to be deleted
         * Return: int, number of rows deleted
         */
        async internal Task<int> DeleteLogAsync(int logID)
        {
            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().ToListAsync();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                if (groceryLog.LogID == logID)
                    await connection.DeleteAsync(groceryLog); //Deletes all the entries in GroceryLog who are connected to the Grocery

            return await connection.DeleteAsync<LogModel>(logID);
        }
    }
}
