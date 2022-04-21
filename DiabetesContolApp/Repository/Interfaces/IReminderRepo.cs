using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IReminderRepo
    {
        Task<bool> DeleteReminderAsync(int reminderID);
        Task<List<ReminderModel>> GetAllRemindersAsync();
        Task<List<ReminderModel>> GetAllUnhandledRemindersAsync();
        Task<ReminderModel> GetReminderAsync(int reminderID);
        Task<bool> InsertReminderAsync(ReminderModel newReminder);
        Task<bool> UpdateReminderAsync(ReminderModel reminder);
    }
}