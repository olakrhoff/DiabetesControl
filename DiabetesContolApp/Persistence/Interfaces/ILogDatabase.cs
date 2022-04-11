using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Persistence.Interfaces
{
    public interface ILogDatabase
    {
        public Task<int> InsertLogAsync(LogModelDAO newLog);
        public Task<List<LogModelDAO>> GetLogsWithDayProfileIDAsync(int dayProfileID);
        public Task<List<LogModelDAO>> GetLogsWithReminderIDAsync(int reminderID);
        public Task<LogModelDAO> GetLogAsync(int logID);
        public Task<List<LogModelDAO>> GetAllLogsAsync();
        public Task<int> UpdateLogAsync(LogModelDAO log);
        public Task<int> DeleteLogAsync(int logID);
    }
}
