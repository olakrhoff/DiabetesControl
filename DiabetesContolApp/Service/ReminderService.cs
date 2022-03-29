using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Checks all reminders if their timer is done
        /// and are ready to be handled. If they haven't
        /// already been handled, then they are handled.
        /// </summary>
        async public void HandleReminders()
        {
            List<ReminderModel> unhandledReminders = await reminderRepo.GetAllUnhandledRemindersAsync();

            foreach (ReminderModel reminder in unhandledReminders)
                if (await reminder.Handle())
                    await reminderRepo.UpdateAsync(reminder);
        }
    }
}
