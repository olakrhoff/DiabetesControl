using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;
using System.Diagnostics;

namespace DiabetesContolApp.Service
{
    public class LogService
    {
        private LogRepo logRepo = new();
        private GroceryLogRepo groceryLogRepo = new();
        private ReminderRepo reminderRepo = new();
        private DayProfileRepo dayProfileRepo = new();

        private GroceryLogService groceryLogService = new();

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
            if (newLog.Reminder.ReminderID == -1) //Remidner did not overlap, need new Reminder
            {
                int reminderID = await reminderRepo.InsertAsync(new());
                if (reminderID == -1)
                    return false; //An error occured while creating the remidner

                newLog.Reminder.ReminderID = reminderID;
            }

            int logID = await logRepo.InsertAsync(newLog); //Insert new Log
            if (logID == -1)
                return false;


            if (!await groceryLogRepo.InsertAllAsync(newLog.NumberOfGroceryModels, logID)) //Insert all grocery-log-cross table entries
            {
                if (!await logRepo.DeleteAsync(logID))
                    throw new Exception("This state should not be possible");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deletes all groceryLog cross table entries.
        /// Then deletes the accual log.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>Returns false if an error occurs, else true</returns>
        async public Task<bool> DeleteLogAsync(int logID)
        {
            if (!await groceryLogRepo.DeleteAllWithLogIDAsync(logID))
                throw new Exception("This state should not happen");

            return await logRepo.DeleteAsync(logID);
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
                List<LogModel> logsWithDayProfileID = await logRepo.GetAllWithDayProfileID(dayProfileID);

                foreach (LogModel log in logsWithDayProfileID)
                    await DeleteLogAsync(log.LogID);

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
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
            List<LogModel> logsOnDate = await logRepo.GetAllOnDateAsync(dateTime);

            for (int i = 0; i < logsOnDate.Count; ++i)
                logsOnDate[i] = await GetLogAsync(logsOnDate[i].LogID);

            logsOnDate = logsOnDate.FindAll(log => log != null); //Remove all null values, if any

            return logsOnDate;
        }

        /// <summary>
        /// Gets the LogModel with the given ID, if it exists.
        /// Then it adds the corresponding DayProfile, Remidner and Groceries.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>
        /// The LogModel with DayProfile, Remidner and Groceries added.
        /// If no LogModel with this ID exists it returns null.
        /// </returns>
        async public Task<LogModel> GetLogAsync(int logID)
        {
            LogModel log = await logRepo.GetAsync(logID);
            if (log == null)
                return null;
            log.DayProfile = await dayProfileRepo.GetAsync(log.DayProfile.DayProfileID);
            log.Reminder = await reminderRepo.GetAsync(log.Reminder.ReminderID);
            //log.NumberOfGroceryModels = await groceryLogRepo.GetAllWithLogID(log.LogID);
            log.NumberOfGroceryModels = await groceryLogService.GetAllWithLogID(log.LogID);

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
            if (!await logRepo.UpdateAsync(log))
                return false; //No log was updated, stop

            bool deleted = await groceryLogRepo.DeleteAllWithLogIDAsync(log.LogID); //Delete all entries with this log
            bool added = await groceryLogRepo.InsertAllAsync(log.NumberOfGroceryModels, log.LogID); //Add all the new ones

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
            return await logRepo.GetNewestAsync();
        }
    }
}
