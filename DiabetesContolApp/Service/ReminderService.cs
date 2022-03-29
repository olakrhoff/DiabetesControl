using System;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;

namespace DiabetesContolApp.Service
{
    public class ReminderService
    {
        private ReminderRepo reminderRepo = new();

        public ReminderService()
        {
        }

        /// <summary>
        /// Gets the ReminderModel with the given ID.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>ReminderModel with given ID or null if not found.</returns>
        async public Task<ReminderModel> GetReminderAsync(int reminderID)
        {
            return await reminderRepo.GetAsync(reminderID);
        }
    }
}
