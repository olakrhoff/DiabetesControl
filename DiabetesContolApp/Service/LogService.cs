using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;

namespace DiabetesContolApp.Service
{
    public class LogService
    {
        private LogRepo logRepo;
        private GroceryLogRepo groceryLogRepo;

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
            int logID = await logRepo.InsertAsync(newLog);
            if (logID == -1)
                return false;


            if (!await groceryLogRepo.InsertAllAsync(newLog.NumberOfGroceryModels, logID))
            {
                if (!await logRepo.DeleteAsync(logID))
                    throw new Exception("This state should not be possible");
                return false;
            }

            return true;
        }

        async public Task<bool> DeleteLogAsync(int logID)
        {
            throw new NotImplementedException();
        }

        async public Task<List<LogModel>> GetLogsAsync(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        async public Task<bool> UpdateLogAsync(LogModel log)
        {
            throw new NotImplementedException();
        }

        async public Task<LogModel> GetNewestLogAsync()
        {
            throw new NotImplementedException();
        }
    }
}
