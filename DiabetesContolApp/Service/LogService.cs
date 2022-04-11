using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;
using DiabetesContolApp.GlobalLogic;
using DiabetesContolApp.Service.Interfaces;
using DiabetesContolApp.Repository.Interfaces;

namespace DiabetesContolApp.Service
{
    /// <summary>
    /// This is the Service class for Logs. It is responsible for
    /// Assembeling and disassembling LogModel objects and make the
    /// appropriate calls to the respective repositories.
    /// </summary>
    public class LogService : ILogService
    {
        private readonly ILogRepo logRepo;
        private GroceryLogRepo groceryLogRepo;
        private ReminderRepo reminderRepo;
        private DayProfileRepo dayProfileRepo;

        public LogService(ILogRepo logRepo = null)
        {
            if (logRepo == null)
                this.logRepo = new LogRepo();
            else
                this.logRepo = logRepo;
        }

        /// <summary>
        /// Checks if the log has a Reminder, if not it adds a new one.
        /// Then it checks if the Reminder and DayProfile attached
        /// exisits, if so it continues. The Reminder will be updated
        /// to match the new Log. The new Log is inserted into
        /// the database, then all GroceryLog entries in the cross table
        /// are inserted into the database.
        /// </summary>
        /// <param name="newLog"></param>
        /// <returns>true if log was inserted, else false</returns>
        async public Task<bool> InsertLogAsync(LogModel newLog)
        {
            /*
            if (newLog.Reminder == null) //Reminder did not overlap, need new Reminder
            {
                newLog.Reminder = new();

                ReminderService reminderService = new();
                int reminderID = await reminderService.InsertReminderAsync(newLog.Reminder); //Call to service to get ID
                if (reminderID == -1)
                    return false; //An error occured while creating the remidner
                newLog.Reminder.ReminderID = reminderID;
            }
            else
            {
                
                ReminderModel reminder = await reminderRepo.GetReminderAsync(newLog.Reminder.ReminderID);
                if (reminder == null)
                    return false;

                DayProfileModel dayProfile = await dayProfileRepo.GetDayProfileAsync(newLog.DayProfile.DayProfileID);
                if (dayProfile == null)
                    return false;
            }
            */
            bool logInserted = await logRepo.InsertLogAsync(newLog); //Insert new Log
            if (!logInserted)
                return false;
            /*
            //Get the ID of the new log
            LogModel newestLog = await GetNewestLogAsync();

            //We must update the reminder so that it goes of after the last log
            newestLog.Reminder.UpdateDateTime(newestLog.DateTimeValue);
            await reminderRepo.UpdateReminderAsync(newestLog.Reminder);

            newLog.LogID = newestLog.LogID; //We update the LogID to be correct

            List<GroceryLogModel> groceryLogs = new();

            foreach (NumberOfGroceryModel numberOfGrocery in newLog.NumberOfGroceries)
                groceryLogs.Add(new(numberOfGrocery, newLog));

            if (!await groceryLogRepo.InsertAllGroceryLogsAsync(groceryLogs, newLog.LogID)) //Insert all grocery-log-cross table entries
            {
                if (!await logRepo.DeleteLogAsync(newLog.LogID))
                    throw new Exception("This state should not be possible");
                return false;
            }
            */
            return true;
        }

        /// <summary>
        /// Gets all log after and including a given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>List of LogModels after a given date, might be empty.</returns>
        async public Task<List<LogModel>> GetAllLogsAfterDateAsync(DateTime date)
        {
            List<LogModel> logsAfterDate = (await logRepo.GetAllLogsAsync()).Where(log => log.DateTimeValue.Date.CompareTo(date.Date) >= 0).ToList();

            for (int i = 0; i < logsAfterDate.Count; ++i)
                logsAfterDate[i] = await GetLogAsync(logsAfterDate[i].LogID);

            logsAfterDate = logsAfterDate.FindAll(log => log != null); //Filter out bad "gets"

            return logsAfterDate;
        }

        /// <summary>
        /// Gets all logs on a given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>List of LogModels on given date, might be empty.</returns>
        async public Task<List<LogModel>> GetAllLogsOnDateAsync(DateTime date)
        {
            List<LogModel> logsOnDate = (await logRepo.GetAllLogsAsync()).Where(log => log.DateTimeValue.Date.Equals(date.Date)).ToList();

            for (int i = 0; i < logsOnDate.Count; ++i)
                logsOnDate[i] = await GetLogAsync(logsOnDate[i].LogID);

            logsOnDate = logsOnDate.FindAll(log => log != null); //Filter out bad "gets"

            return logsOnDate;
        }

        /// <summary>
        /// Gets all logs.
        /// </summary>
        /// <returns>List of LogModels, might be empty.</returns>
        async public Task<List<LogModel>> GetAllLogsAsync()
        {
            List<LogModel> logs = await logRepo.GetAllLogsAsync();

            for (int i = 0; i < logs.Count; ++i)
                logs[i] = await GetLogAsync(logs[i].LogID);

            logs = logs.FindAll(log => log != null); //Filter out bad "gets"

            return logs;
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
                await reminderRepo.DeleteReminderAsync(currentLog.Reminder.ReminderID);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.Message);
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
        /// Gets all LogModels with the given Reminder ID.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>List of LogModels with the given Reminder ID, might be empty</returns>
        async public Task<List<LogModel>> GetAllLogsWithReminderIDAsync(int reminderID)
        {
            List<LogModel> logsWithReminderID = await logRepo.GetAllLogsWithReminderIDAsync(reminderID);

            for (int i = 0; i < logsWithReminderID.Count; ++i)
                logsWithReminderID[i] = await GetLogAsync(logsWithReminderID[i].LogID); //Gets the LogModels properly

            logsWithReminderID = logsWithReminderID.FindAll(log => log != null); //Remove all null values, if any

            return logsWithReminderID;
        }

        /// <summary>
        /// Gets all LogModels with the given DayProfile ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>List of LogModels with given DayProfile ID, might be empty.</returns>
        async public Task<List<LogModel>> GetAllLogsWithDayProfileIDAsync(int dayProfileID)
        {
            List<LogModel> logsWithDayProfileID = await logRepo.GetAllLogsWithDayProfileIDAsync(dayProfileID);

            for (int i = 0; i < logsWithDayProfileID.Count; ++i)
                logsWithDayProfileID[i] = await GetLogAsync(logsWithDayProfileID[i].LogID); //Gets the LogModels properly

            logsWithDayProfileID = logsWithDayProfileID.FindAll(log => log != null); //Remove all null values, if any

            return logsWithDayProfileID;
        }

        /// <summary>
        /// Gets the LogModel with the given ID, if it exists.
        /// Then it adds the corresponding DayProfile, Reminder and Groceries.
        ///
        /// If the Log is corrupt, missing critical data, it will be deleted.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>
        /// The LogModel with DayProfile, Reminder and Groceries added.
        /// If no LogModel with this ID exists, or if it was corrupt, it returns null.
        /// </returns>
        async public Task<LogModel> GetLogAsync(int logID)
        {
            LogModel log = await logRepo.GetLogAsync(logID);
            if (log == null)
                return null;
            log.DayProfile = await dayProfileRepo.GetDayProfileAsync(log.DayProfile.DayProfileID);
            log.Reminder = await reminderRepo.GetReminderAsync(log.Reminder.ReminderID);

            if (log.DayProfile == null || log.Reminder == null) //If Dayprofile or Reminder doesn't exist Log is corrupt
            {
                //To avoid recursion on the delete call, since delete calls this method,
                //we remove the error for now, by adding a dayprofile and a reminder,
                //then after the delete call is finsihed, we delete these again.

                DayProfileService dayProfileService = new();
                int fakeDayProfileID = await dayProfileService.InsertDayProfileAsync(new());

                ReminderService reminderService = new();
                int fakeReminderID = await reminderService.InsertReminderAsync(new());

                log.DayProfile = await dayProfileRepo.GetDayProfileAsync(fakeDayProfileID);
                log.Reminder = await reminderRepo.GetReminderAsync(fakeReminderID);

                await logRepo.UpdateLogAsync(log);

                await DeleteLogAsync(log.LogID); //Deletes it, because it is corrupt

                await dayProfileRepo.DeleteDayProfileAsync(fakeDayProfileID); //We know this hasn't been connected to anything else, therefore a repo-delete is safe

                //This call is made in the DeleteLogAsync-call as well, but for safty it is here as well
                await reminderRepo.DeleteReminderAsync(fakeReminderID); //We know this hasn't been connected to anything else, therefore a repo-delete is safe

                return null;
            }

            GroceryLogService groceryLogService = new();
            //Get all NumberOfGrocery with Grocery objects attached
            log.NumberOfGroceries = await groceryLogService.GetAllGroceryLogsAsNumberOfGroceryWithLogID(log.LogID);


            //TODO: ---------- TEMP ---------- 

            if ((log.DayProfile.TargetGlucoseValue != log.GlucoseAtMeal && log.CorrectionInsulin == 0) ||
                log.NumberOfGroceries != null && log.NumberOfGroceries.Count != 0 && log.NumberOfGroceries[0].InsulinForGroceries == 0)
            {
                //Update old Logs
                Helper.CalculateInsulin(ref log);
                log.Reminder.IsHandled = log.Reminder.IsHandled;

                await UpdateLogAsync(log);
            }

            //TODO: ---------- TEMP ---------- 

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

            foreach (NumberOfGroceryModel numberOfGrocery in log.NumberOfGroceries)
                groceryLogs.Add(new(numberOfGrocery, log));

            bool added = await groceryLogRepo.InsertAllGroceryLogsAsync(groceryLogs, log.LogID); //Add all the new ones

            if (deleted && added)
                return true; //If no problems, return true
            return false; //If problem, return false
        }

        /// <summary>
        /// Gets the newest LogModel.
        /// Tries to find logs in one day at a time for the past 10 days.
        /// This is for performance. If no Logs were found it gets all
        /// logs, and get the newest from there.
        /// </summary>
        /// <returns>Returns LogModel for the newest log, return null if there are no LogModels.</returns>
        async public Task<LogModel> GetNewestLogAsync()
        {
            List<LogModel> logsOnDate = new();
            //Get all logs from today, if there are any
            //the newest log must be there, if empty get yesterday
            //then go back one day at a time till you find a log
            for (int dayOffset = 0; dayOffset > -10 && logsOnDate.Count == 0; --dayOffset)
                logsOnDate = await GetAllLogsOnDateAsync(DateTime.Now.AddDays(dayOffset));
            if (logsOnDate.Count == 0)
                logsOnDate = await logRepo.GetAllLogsAsync(); //Get all logs if no logs were found on the first 10 tries

            if (logsOnDate.Count == 0) //There are no logs
                return null;

            LogModel newestLog = logsOnDate.Max(); //Get newest element

            return await GetLogAsync(newestLog.LogID); //Gets the LogModel properly
        }
    }
}
