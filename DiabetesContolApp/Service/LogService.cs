using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;
using System.Diagnostics;

namespace DiabetesContolApp.Service
{
    /// <summary>
    /// This is the Service class for Logs. It is responsible for
    /// Assembeling and disassembling LogModel objects and make the
    /// appropriate calls to the respective repositories.
    /// </summary>
    public class LogService
    {
        private LogRepo logRepo = new();
        private GroceryLogRepo groceryLogRepo = new();
        private ReminderRepo reminderRepo = new();
        private DayProfileRepo dayProfileRepo = new();
        private GroceryRepo groceryRepo = new();

        public LogService()
        {
        }

        /// <summary>
        /// Tells the logRepo to insert the log. Then tells the
        /// groceryLogRepo to insert all the cross table entries.
        /// </summary>
        /// <param name="newLog"></param>
        /// <returns>true if log was inserted, else false</returns>
        async public Task<bool> InsertLogAsync(LogModel newLog)
        {
            if (newLog.Reminder == null) //Reminder did not overlap, need new Reminder
            {
                newLog.Reminder = new();

                int reminderID = await reminderRepo.InsertReminderAsync(newLog.Reminder);
                if (reminderID == -1)
                    return false; //An error occured while creating the remidner
                newLog.Reminder.ReminderID = reminderID;
            }

            int logID = await logRepo.InsertLogAsync(newLog); //Insert new Log
            if (logID == -1)
                return false;
            newLog.LogID = logID;

            List<GroceryLogModel> groceryLogs = new();

            foreach (NumberOfGroceryModel numberOfGrocery in newLog.NumberOfGroceryModels)
                groceryLogs.Add(new(numberOfGrocery, newLog));

            if (!await groceryLogRepo.InsertAllGroceryLogsAsync(groceryLogs, logID)) //Insert all grocery-log-cross table entries
            {
                if (!await logRepo.DeleteLogAsync(logID))
                    throw new Exception("This state should not be possible");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deletes all the logs with the given IDs.
        /// </summary>
        /// <param name="logIDs"></param>
        /// <returns>False if an error occurs, else true.</returns>
        async public Task<bool> DeleteAllLogsAsync(List<int> logIDs)
        {
            bool result = true;

            foreach (int id in logIDs)
                if (!await DeleteLogAsync(id))
                    result = false;

            return result;
        }

        /// <summary>
        /// Gets all Logs with the same reminder as the one to
        /// be deleted, since these also need to be deleted.
        /// Deletes all groceryLog cross table entries for all logs.
        /// Then deletes the accual logs.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>Returns false if an error occurs, else true</returns>
        async public Task<bool> DeleteLogAsync(int logID)
        {
            try
            {
                LogModel currentLog = await GetLogAsync(logID);
                List<LogModel> logsWithReminderID = await logRepo.GetAllLogsWithReminderIDAsync(currentLog.Reminder.ReminderID);

                foreach (LogModel log in logsWithReminderID)
                {
                    await groceryLogRepo.DeleteAllGroceryLogsWithLogIDAsync(log.LogID);
                    await logRepo.DeleteLogAsync(log.LogID);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deletes all logs who has the DayProfile with the given ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>False if an error occurs, else true.</returns>
        async public Task<bool> DeleteAllWithDayProfileIDAsync(int dayProfileID)
        {
            try
            {
                List<LogModel> logsWithDayProfileID = await logRepo.GetAllLogsWithDayProfileIDAsync(dayProfileID);

                foreach (LogModel log in logsWithDayProfileID)
                    if (!await DeleteLogAsync(log.LogID))
                        throw new Exception("An error occured while deleting log with logID: " + log.LogID);

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets the logs on the date specified. Then adds
        /// the DayProfile, Reminder and Groceries.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>Returns the lsit of logs with Dayprofile, Reminder and Groceries added</returns>
        async public Task<List<LogModel>> GetLogsOnDateAsync(DateTime dateTime)
        {
            List<LogModel> logsOnDate = await logRepo.GetAllLogsOnDateAsync(dateTime);

            for (int i = 0; i < logsOnDate.Count; ++i)
                logsOnDate[i] = await GetLogAsync(logsOnDate[i].LogID); //Gets the LogModels properly

            logsOnDate = logsOnDate.FindAll(log => log != null); //Remove all null values, if any

            return logsOnDate;
        }

        /// <summary>
        /// Gets the LogModel with the given ID, if it exists.
        /// Then it adds the corresponding DayProfile, Reminder and Groceries.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>
        /// The LogModel with DayProfile, Reminder and Groceries added.
        /// If no LogModel with this ID exists it returns null.
        /// </returns>
        async public Task<LogModel> GetLogAsync(int logID)
        {
            LogModel log = await logRepo.GetLogAsync(logID);
            if (log == null)
                return null;
            log.DayProfile = await dayProfileRepo.GetDayProfileAsync(log.DayProfile.DayProfileID);
            log.Reminder = await reminderRepo.GetReminderAsync(log.Reminder.ReminderID);

            List<GroceryLogModel> groceryLogs = await groceryLogRepo.GetAllGroceryLogsWithLogID(log.LogID);

            List<NumberOfGroceryModel> numberOfGroceries = new();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                numberOfGroceries.Add(new(groceryLog));

            log.NumberOfGroceryModels = numberOfGroceries;

            foreach (NumberOfGroceryModel numberOfGrocery in log.NumberOfGroceryModels)
                numberOfGrocery.Grocery = await groceryRepo.GetGroceryAsync(numberOfGrocery.Grocery.GroceryID); //Gets the Groceries in the NumberOfGroceries in the log

            return log;
        }
        /// <summary>
        /// Tells the logRepo to update the log. Then the
        /// groceryLogRepo to update all the cross table entries.
        /// </summary>
        /// <param name="log"></param>
        /// <returns>Returns true if it was updated, else false</returns>
        async public Task<bool> UpdateLogAsync(LogModel log)
        {
            if (!await logRepo.UpdateLogAsync(log))
                return false; //No log was updated, stop

            bool deleted = await groceryLogRepo.DeleteAllGroceryLogsWithLogIDAsync(log.LogID); //Delete all entries with this log

            List<GroceryLogModel> groceryLogs = new();

            foreach (NumberOfGroceryModel numberOfGrocery in log.NumberOfGroceryModels)
                groceryLogs.Add(new(numberOfGrocery, log));

            bool added = await groceryLogRepo.InsertAllGroceryLogsAsync(groceryLogs, log.LogID); //Add all the new ones

            if (deleted && added)
                return true; //If no problems, return true
            return false; //If problem, return false
        }

        /// <summary>
        /// Get the newest log added.
        /// </summary>
        /// <returns>Returns LogModel for the newest log, return null if there are no LogModels.</returns>
        async public Task<LogModel> GetNewestLogAsync()
        {
            LogModel newestLog = await logRepo.GetNewestLogAsync();

            return await GetLogAsync(newestLog.LogID); //Gets the LogModel properly
        }
    }
}
