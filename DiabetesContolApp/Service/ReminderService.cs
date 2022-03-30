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
        private LogRepo logRepo = new();

        public ReminderService()
        {
        }

        /// <summary>
        /// Gets the ReminderModel with the given ID.
        /// Then it adds the Logs to it.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>ReminderModel with given ID or null if not found.</returns>
        async public Task<ReminderModel> GetReminderAsync(int reminderID)
        {
            ReminderModel reminder = await reminderRepo.GetReminderAsync(reminderID);
            if (reminder == null)
                return null;

            reminder.Logs = await logRepo.GetAllWithReminderIDAsync(reminder.ReminderID);

            return reminder;
        }

        /// <summary>
        /// Checks all reminders if their timer is done
        /// and are ready to be handled. If they haven't
        /// already been handled, then they are handled.
        /// </summary>
        async public void HandleRemindersAsync()
        {
            List<ReminderModel> unhandledReminders = await GetAllUnhandledRemindersAsync();

            foreach (ReminderModel reminder in unhandledReminders)
                if (await reminder.Handle())
                    await UpdateReminderAsync(reminder);
        }

        /// <summary>
        /// Updates the logs attached to the Remdiner
        /// then the remidner itself.
        ///
        /// The logs NumberOfGrocery list will not be updated since
        /// the reminder doesn't change this value, therefore the
        /// call goes to the repo and not thte service.
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns>False if an error occurs, else true.</returns>
        async private Task<bool> UpdateReminderAsync(ReminderModel reminder)
        {
            foreach (LogModel log in reminder.Logs)
                await logRepo.UpdateAsync(log);

            return await reminderRepo.UpdateReminderAsync(reminder);
        }

        /// <summary>
        /// Gets all unhandled ReminderModels, then gets
        /// them with LogModels.
        /// </summary>
        /// <returns>List of ReminderModels with Logs and are unhandled.</returns>
        async private Task<List<ReminderModel>> GetAllUnhandledRemindersAsync()
        {
            List<ReminderModel> unhandledReminders = await reminderRepo.GetAllUnhandledRemindersAsync();

            for (int i = 0; i < unhandledReminders.Count; ++i)
                unhandledReminders[i] = await GetReminderAsync(unhandledReminders[i].ReminderID); //Get reminder with Logs

            return unhandledReminders;
        }
    }
}
