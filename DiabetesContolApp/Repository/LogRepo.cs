using System;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;
using System.Diagnostics;
using System.Collections.Generic;

namespace DiabetesContolApp.Repository
{
    public class LogRepo
    {
        private LogDatabase logDatabase = LogDatabase.GetInstance();

        public LogRepo()
        {
        }

        /// <summary>
        /// Converts LogModel to DAO object. Inserts the DAO
        /// into the database.
        /// </summary>
        /// <param name="newLog"></param>
        /// <returns>Returns the LogID of the new log, if an error occurs -1 is returned</returns>
        async public Task<int> InsertAsync(LogModel newLog)
        {
            LogModelDAO newLogDAO = new(newLog);


            if (await logDatabase.InsertLogAsync(newLogDAO) > 0)
            {
                LogModelDAO log = await logDatabase.GetNewestLogAsync();
                return log.LogID;
            }
            return -1; //Return invalid ID
        }

        /// <summary>
        /// Deletes the log with the given ID from the database.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>Returns true if it was deleted, if an error occurs then false</returns>
        async public Task<bool> DeleteAsync(int logID)
        {
            try
            {
                await logDatabase.DeleteLogAsync(logID);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Gets all logDAOs on given date. Then converts all to
        /// Model objects.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>The list of LogModels on the given date</returns>
        async public Task<List<LogModel>> GetAllOnDateAsync(DateTime dateTime)
        {
            List<LogModelDAO> logDAOs = await logDatabase.GetLogsOnDateAsync(dateTime);

            List<LogModel> logsOnDate = new();

            foreach (LogModelDAO logDAO in logDAOs)
                logsOnDate.Add(await GetAsync(logDAO.LogID));

            return logsOnDate;
        }

        /// <summary>
        /// Gets all LogModelDAOs with a given
        /// DayProfile ID, converts them into LogModels.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>List of LogModels who has the given DayProfile ID.</returns>
        async public Task<List<LogModel>> GetAllWithDayProfileID(int dayProfileID)
        {
            List<LogModelDAO> logsDAOWithDayProfileID = await logDatabase.GetLogsWithDayProfile(dayProfileID);

            List<LogModel> logs = new();

            foreach (LogModelDAO logDAO in logsDAOWithDayProfileID)
                logs.Add(new(logDAO));

            return logs;
        }

        /// <summary>
        /// Deletes all Logs with matching the IDs in the list.
        /// </summary>
        /// <param name="logIDs"></param>
        /// <returns>True if no problem, else false.</returns>
        async public Task<bool> DeleteAllAsync(List<int> logIDs)
        {
            try
            {
                foreach (int logID in logIDs)
                    await DeleteAsync(logID);
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
        async public Task<LogModel> GetAsync(int logID)
        {
            LogModelDAO logDAO = await logDatabase.GetLogAsync(logID);
            if (logDAO == null)
                return null;

            return new(logDAO);
        }


        /// <summary>
        /// Updates the log with the matching ID.
        /// </summary>
        /// <param name="log"></param>
        /// <returns>True if it was updated, else false.</returns>
        async public Task<bool> UpdateAsync(LogModel log)
        {
            LogModelDAO logDAO = new(log);

            if (await logDatabase.UpdateLogAsync(logDAO) > 0)
                return true;
            return false; //No log with this ID
        }

        /// <summary>
        /// Gets the newest Log.
        /// </summary>
        /// <returns>Return LogModel, unless there are no Logs, then it returns null.</returns>
        async public Task<LogModel> GetNewestAsync()
        {
            LogModelDAO logDAO = await logDatabase.GetNewestLogAsync();

            if (logDAO == null)
                return null;

            return new(logDAO);
        }
    }
}
