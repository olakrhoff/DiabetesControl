using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;
using DiabetesContolApp.Repository.Interfaces;
using DiabetesContolApp.Persistence.Interfaces;

namespace DiabetesContolApp.Repository
{
    public class LogRepo : ILogRepo
    {
        private readonly ILogDatabase _logDatabase;

        public LogRepo()
        {
        }

        public LogRepo(ILogDatabase logDatabase = null)
        {
            if (logDatabase == null)
                _logDatabase = LogDatabase.GetInstance();
            else
                _logDatabase = logDatabase;
        }

        /// <summary>
        /// Converts LogModel to DAO object. Inserts the DAO
        /// into the database.
        /// </summary>
        /// <param name="newLog"></param>
        /// <returns>Returns true if inserted else false.</returns>
        async public Task<bool> InsertLogAsync(LogModel newLog)
        {
            LogModelDAO newLogDAO = new(newLog);

            return await _logDatabase.InsertLogAsync(newLogDAO) > 0;
        }

        /// <summary>
        /// Deletes the log with the given ID from the database.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>Returns true if it was deleted, if an error occurs then false</returns>
        async public Task<bool> DeleteLogAsync(int logID)
        {
            try
            {
                await _logDatabase.DeleteLogAsync(logID);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Gets all LogModelDAOs with the given reminderID.
        /// Then converts them into LogModels.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>List of LogModels with the given reminder ID, might be empty.</returns>
        async public Task<List<LogModel>> GetAllLogsWithReminderIDAsync(int reminderID)
        {
            List<LogModelDAO> logDAOs = await _logDatabase.GetLogsWithReminderIDAsync(reminderID);

            List<LogModel> logsWithRemiderID = new();

            foreach (LogModelDAO logDAO in logDAOs)
                logsWithRemiderID.Add(new(logDAO));

            return logsWithRemiderID;
        }

        /// <summary>
        /// Gets all LogModelDAOs with a given
        /// DayProfile ID, converts them into LogModels.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>List of LogModels who has the given DayProfile ID.</returns>
        async public Task<List<LogModel>> GetAllLogsWithDayProfileIDAsync(int dayProfileID)
        {
            List<LogModelDAO> logsDAOWithDayProfileID = await _logDatabase.GetLogsWithDayProfileIDAsync(dayProfileID);

            List<LogModel> logs = new();

            foreach (LogModelDAO logDAO in logsDAOWithDayProfileID)
                logs.Add(new(logDAO));

            return logs;
        }

        /// <summary>
        /// Gets all LogModelDAOs then converts them into LogModels.
        /// </summary>
        /// <returns>List of LogModels, might be empty.</returns>
        async public Task<List<LogModel>> GetAllLogsAsync()
        {
            List<LogModelDAO> logDAOs = await _logDatabase.GetAllLogsAsync();

            List<LogModel> logs = new();

            foreach (LogModelDAO logDAO in logDAOs)
                logs.Add(new(logDAO));

            return logs;
        }

        /// <summary>
        /// Deletes all Logs with matching the IDs in the list.
        /// </summary>
        /// <param name="logIDs"></param>
        /// <returns>True if no problem, else false.</returns>
        async public Task<bool> DeleteAllLogsAsync(List<int> logIDs)
        {
            try
            {
                foreach (int logID in logIDs)
                    await DeleteLogAsync(logID);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Gets the logDAO with the given logID, if it exists,
        /// then convert it to a logModel.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>
        /// Return the LogModel with the given logID or
        /// null if no log with the given ID was found.
        /// </returns>
        async public Task<LogModel> GetLogAsync(int logID)
        {
            LogModelDAO logDAO = await _logDatabase.GetLogAsync(logID);
            if (logDAO == null)
                return null;

            return new(logDAO);
        }


        /// <summary>
        /// Updates the log with the matching ID.
        /// </summary>
        /// <param name="log"></param>
        /// <returns>True if it was updated, else false.</returns>
        async public Task<bool> UpdateLogAsync(LogModel log)
        {
            LogModelDAO logDAO = new(log);

            if (await _logDatabase.UpdateLogAsync(logDAO) > 0)
                return true;
            return false; //No log with this ID
        }
    }
}
