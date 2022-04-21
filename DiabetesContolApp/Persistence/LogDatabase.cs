using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence.Interfaces;


using SQLite;
using Xamarin.Forms;

namespace DiabetesContolApp.Persistence
{
    public sealed class LogDatabase : ModelDatabaseAbstract, ILogDatabase
    {
        private static readonly LogDatabase _instance = null;

        public LogDatabase(SQLiteAsyncConnection connection = null) : base(connection)
        {
        }

        public static LogDatabase GetInstance()
        {
            return _instance == null ? new LogDatabase() : _instance;
        }


        /// <summary>
        /// This method inserts a log into the database.
        /// </summary>
        /// <param name="newLogEntry">LogModelDAO, the log to insert</param>
        /// <returns>The number of rows added, -1 if an error occured.</returns>
        async public Task<int> InsertLogAsync(LogModelDAO newLog)
        {
            try
            {
                return await _connection.InsertAsync(newLog);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return -1;
            }
        }

        /// <summary>
        /// Gets all logs with a given DayProfile ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>List of LogModelDAOs with given DayProfile ID, might be empty.</returns>
        async public Task<List<LogModelDAO>> GetAllLogsWithDayProfileIDAsync(int dayProfileID)
        {
            try
            {
                return await _connection.Table<LogModelDAO>().Where(logDAO => logDAO.DayProfileID == dayProfileID).ToListAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return new();
            }
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
        async public Task<List<LogModelDAO>> GetAllLogsWithReminderIDAsync(int reminderID)
        {
            try
            {
                List<LogModelDAO> logDAOs = await _connection.Table<LogModelDAO>().Where(log => log.ReminderID == reminderID).ToListAsync();
                return logDAOs;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return new();
            }
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
                LogModelDAO log = await _connection.GetAsync<LogModelDAO>(logID);
                return log;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine(ioe.StackTrace);
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
        /// Gets all LogDAOs from the database.
        /// </summary>
        /// <returns>List of LogDAOs, might be empty.</returns>
        async public Task<List<LogModelDAO>> GetAllLogsAsync()
        {
            try
            {
                return await _connection.Table<LogModelDAO>().ToListAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return new();
            }
        }

        /// <summary>
        /// Updates the given log by it's ID
        /// </summary>
        /// <param name="log"></param>
        /// <returns>int, the number of rows updated, -1 if an error occured.</returns>
        async public Task<int> UpdateLogAsync(LogModelDAO log)
        {
            try
            {
                return await _connection.UpdateAsync(log);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return -1;
            }
        }

        /// <summary>
        /// This method deletes a LogDAO based on it's ID.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>int, the number of rows deleted, -1 if an error occured.</returns>
        async public Task<int> DeleteLogAsync(int logID)
        {
            try
            {
                return await _connection.DeleteAsync<LogModelDAO>(logID);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return -1;
            }
        }

        public override string HeaderForCSVFile()
        {
            return "LogID,DayProfileID,ReminderID,DateTimeValue,GlucoseAtMeal,GlucoseAfterMeal,InsulinEstimate,InsulinFromUser,CorrectionInsulin\n";
        }

        public override async Task<List<IModelDAO>> GetAllAsync()
        {
            return new(await _connection.Table<LogModelDAO>().ToListAsync());
        }
    }
}
