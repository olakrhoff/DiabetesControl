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
        /// <returns>The number of rows added, -1 if an error occured.</returns>
        async public Task<int> InsertLogAsync(LogModelDAO newLog)
        {
            try
            {
                return await connection.InsertAsync(newLog);
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
        async public Task<List<LogModelDAO>> GetLogsWithDayProfileIDAsync(int dayProfileID)
        {
            try
            {
                return await connection.Table<LogModelDAO>().Where(logDAO => logDAO.DayProfileID == dayProfileID).ToListAsync();
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
                return await connection.Table<LogModelDAO>().ToListAsync();
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
                return await connection.UpdateAsync(log);
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
                return await connection.DeleteAsync<LogModelDAO>(logID);
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
            return new(await connection.Table<LogModelDAO>().ToListAsync());
        }
    }
}
