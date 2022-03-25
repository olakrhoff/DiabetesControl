using System;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;

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
        /// <returns>Returns true if it was deleted, else false</returns>
        async public Task<bool> DeleteAsync(int logID)
        {
            return await logDatabase.DeleteLogAsync(logID) > 0;
        }
    }
}
