using System;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IReminderRepo
    {
        Task<bool> UpdateReminderAsync(ReminderModel reminder);
        Task<bool> DeleteReminderAsync(int reminderID);
        Task<ReminderModel> GetReminderAsync(int reminderID);
    }
}
