using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface ILogRepo
    {
        Task<bool> DeleteAllLogsAsync(List<int> logIDs);
        Task<bool> DeleteLogAsync(int logID);
        Task<List<LogModel>> GetAllLogsAsync();
        Task<List<LogModel>> GetAllLogsWithDayProfileIDAsync(int dayProfileID);
        Task<List<LogModel>> GetAllLogsWithReminderIDAsync(int reminderID);
        Task<LogModel> GetLogAsync(int logID);
        Task<bool> InsertLogAsync(LogModel newLog);
        Task<bool> UpdateLogAsync(LogModel log);
    }
}