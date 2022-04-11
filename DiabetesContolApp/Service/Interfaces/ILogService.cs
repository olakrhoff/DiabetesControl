using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.Models;

namespace DiabetesContolApp.Service.Interfaces
{
    public interface ILogService
    {
        public Task<bool> InsertLogAsync(LogModel newLog);
        public Task<List<LogModel>> GetAllLogsAfterDateAsync(DateTime date);
        public Task<List<LogModel>> GetAllLogsOnDateAsync(DateTime date);
        public Task<List<LogModel>> GetAllLogsAsync();
        public Task<bool> DeleteAllLogsAsync(List<int> logIDs);
        public Task<bool> DeleteLogAsync(int logID);
        public Task<bool> DeleteAllWithDayProfileIDAsync(int dayProfileID);
        public Task<List<LogModel>> GetAllLogsWithReminderIDAsync(int reminderID);
        public Task<List<LogModel>> GetAllLogsWithDayProfileIDAsync(int dayProfileID);
        public Task<LogModel> GetLogAsync(int logID);
        public Task<bool> UpdateLogAsync(LogModel log);
        public Task<LogModel> GetNewestLogAsync();
    }
}
