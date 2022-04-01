using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.DAO;
using DiabetesContolApp.GlobalLogic;

using SQLite;
using Xamarin.Forms;
using System.Diagnostics;
using DiabetesContolApp.Models;

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


        /// <summary>
        /// This method inserts a log into the database.
        /// </summary>
        /// <param name="newLogEntry">LogModelDAO, the log to insert</param>
        /// <returns>The number of rows added.</returns>
        async public Task<int> InsertLogAsync(LogModelDAO newLog)
        {

            return await connection.InsertAsync(newLog);
        }

        /// <summary>
        /// Gets all logs with a given DayProfile ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>List of LogModelDAOs with given DayProfile ID.</returns>
        async public Task<List<LogModelDAO>> GetLogsWithDayProfileIDAsync(int dayProfileID)
        {
            return await connection.Table<LogModelDAO>().Where(logDAO => logDAO.DayProfileID == dayProfileID).ToListAsync();
        }


        /// <summary>
        /// Gets all logs that occur after a given time
        /// </summary>
        /// <param name="lastUpdate"></param>
        /// <returns>List of logs after given time, if none were found, list is empty.</returns>
        async public Task<List<LogModel>> GetAllLogsAfterAsync(DateTime lastUpdate)
        {
            List<LogModel> logs = await connection.Table<LogModel>().Where(log => log.DateTimeValue.CompareTo(lastUpdate) > 0).ToListAsync();

            if (logs == null)
                return new();
            return logs;
        }

        /// <summary>
        /// Gets all LogDAOs connected to a reminder by the
        /// ReminderID.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>
        /// List of LogModelDAOs connected to the remidnerID, if no one, then
        /// an empty list.
        /// </returns>
        async public Task<List<LogModelDAO>> GetLogsWithReminderIDAsync(int reminderID)
        {
            try
            {
                List<LogModelDAO> logDAOs = await connection.Table<LogModelDAO>().Where(log => log.ReminderID == reminderID).ToListAsync();
                return logDAOs;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return new();
            }
        }

        /// <summary>
        /// This method get the newest Log based on the time it
        /// has in its variable DateTime.
        /// </summary>
        /// <returns>
        /// Task&lt;LogModelDAO&gt;
        /// 
        /// Task is for async
        /// LogModelDAO is the newest model,
        /// if no Log is found, it returns null.
        /// </returns>
        async public Task<LogModelDAO> GetNewestLogAsync()
        {
            var logs = await connection.Table<LogModelDAO>().ToListAsync();

            if (logs.Count == 0)
                return null;

            logs.Sort();

            return logs[logs.Count - 1];
        }

        /// <summary>
        /// Gets the log with the corresponding ID,
        /// if it doesn't exist it returns null
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>
        /// The LogModelDAO if it exists, else null
        /// </returns>
        async public Task<LogModelDAO> GetLogAsync(int logID)
        {
            try
            {
                LogModelDAO log = await connection.GetAsync<LogModelDAO>(logID);
                return log;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine(ioe.Message);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Updates the given log by it's ID
        /// </summary>
        /// <param name="log"></param>
        /// <returns>int, the number of rows updated</returns>
        async public Task<int> UpdateLogAsync(LogModelDAO log)
        {
            return await connection.UpdateAsync(log);
        }

        /// <summary>
        /// Gets all logs, filter on the given date, then returns whats left.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>Return all log on given date</returns>
        async public Task<List<LogModelDAO>> GetLogsOnDateAsync(DateTime dateTime)
        {
            var logs = await connection.Table<LogModelDAO>().ToListAsync();

            List<LogModelDAO> temp = new();

            foreach (LogModelDAO log in logs)
                if (log.DateTimeValue.Date.Equals(dateTime.Date))
                    temp.Add(log);

            return temp;
        }

        /// <summary>
        /// This method deletes a log based on it's ID.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>int, the number of rows deleted</returns>
        async public Task<int> DeleteLogAsync(int logID)
        {
            return await connection.DeleteAsync<LogModelDAO>(logID);
        }

        public override string HeaderForCSVFile()
        {
            return "LogID,DayProfileID,ReminderID,DateTimeValue,GlucoseAtMeal,GlucoseAfterMeal,InsulinEstimate,InsulinFromUser,CorrectionInsulin\n";
        }

        public override async Task<List<IModelDAO>> GetAllAsync()
        {
            return new(await connection.Table<LogModelDAO>().ToListAsync());
        }
    }
}
