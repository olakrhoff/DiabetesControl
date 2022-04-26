using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.Models;

namespace DiabetesContolApp.Service.Interfaces
{
    public interface ILogService
    {
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
        public Task<bool> InsertLogAsync(LogModel newLog);

        /// <summary>
        /// Gets all log after and including a given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>List of LogModels after a given date, might be empty.</returns>
        public Task<List<LogModel>> GetAllLogsAfterDateAsync(DateTime date);

        /// <summary>
        /// Gets all logs on a given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>List of LogModels on given date, might be empty.</returns>
        public Task<List<LogModel>> GetAllLogsOnDateAsync(DateTime date);

        /// <summary>
        /// Gets all logs.
        /// </summary>
        /// <returns>List of LogModels, might be empty.</returns>
        public Task<List<LogModel>> GetAllLogsAsync();

        /// <summary>
        /// Deletes all the logs with the given IDs.
        /// </summary>
        /// <param name="logIDs"></param>
        /// <returns>False if an error occurs, else true.</returns>
        public Task<bool> DeleteAllLogsAsync(List<int> logIDs);

        /// <summary>
        /// Gets all Logs with the same reminder as the one to
        /// be deleted, since these also need to be deleted.
        /// Deletes all groceryLog cross table entries for all logs.
        /// Then deletes the accual logs.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>Returns false if an error occurs, else true</returns>
        public Task<bool> DeleteLogAsync(int logID);

        /// <summary>
        /// Deletes all logs who has the DayProfile with the given ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>False if an error occurs, else true.</returns>
        public Task<bool> DeleteAllWithDayProfileIDAsync(int dayProfileID);

        /// <summary>
        /// Gets all LogModels with the given Reminder ID.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>List of LogModels with the given Reminder ID, might be empty</returns>
        public Task<List<LogModel>> GetAllLogsWithReminderIDAsync(int reminderID);

        /// <summary>
        /// Gets all LogModels with the given DayProfile ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>List of LogModels with given DayProfile ID, might be empty.</returns>
        public Task<List<LogModel>> GetAllLogsWithDayProfileIDAsync(int dayProfileID);

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
        public Task<LogModel> GetLogAsync(int logID);

        /// <summary>
        /// Tells the logRepo to update the log. Then the
        /// groceryLogRepo to update all the cross table entries.
        /// </summary>
        /// <param name="log"></param>
        /// <returns>Returns true if it was updated, else false</returns>
        public Task<bool> UpdateLogAsync(LogModel log);

        /// <summary>
        /// Gets the newest LogModel.
        /// Tries to find logs in one day at a time for the past 10 days.
        /// This is for performance. If no Logs were found it gets all
        /// logs, and get the newest from there.
        /// </summary>
        /// <returns>Returns LogModel for the newest log, return null if there are no LogModels.</returns>
        public Task<LogModel> GetNewestLogAsync();
    }
}
