using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IReminderRepo
    {
        Task<bool> UpdateReminderAsync(ReminderModel reminder);
        Task<bool> DeleteReminderAsync(int reminderID);
        Task<ReminderModel> GetReminderAsync(int reminderID);
        Task<bool> InsertReminderAsync(ReminderModel newReminder);
        Task<List<ReminderModel>> GetAllUnhandledRemindersAsync();
        Task<List<ReminderModel>> GetAllRemindersAsync();
    }
}
