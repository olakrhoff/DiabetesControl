using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;

namespace DiabetesContolApp.Service
{
    /// <summary>
    /// This is the Service class for Reminders. It is responsible for
    /// Assembeling and disassembling RemdinerModel objects and make the
    /// appropriate calls to the respective repositories.
    /// </summary>
    public class ReminderService
    {
        private ReminderRepo reminderRepo = new();
        private LogRepo logRepo = new();

        public ReminderService()
        {
        }

        /// <summary>
        /// Inserts the new Reminder into the database.
        /// </summary>
        /// <param name="newReminder"></param>
        /// <returns>
        /// int, the ID of the newly added
        /// Reminder, -1 if an error occured.
        /// </returns>
        async public Task<int> InsertReminderAsync(ReminderModel newReminder)
        {
            if (!await reminderRepo.InsertReminderAsync(newReminder))
                return -1;

            ReminderModel newestReminder = await reminderRepo.GetNewestReminder();

            return newestReminder.ReminderID;
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

            reminder.Logs = await logRepo.GetAllLogsWithReminderIDAsync(reminder.ReminderID);

            return reminder;
        }

        /// <summary>
        /// Checks all reminders if their timer is done
        /// and are ready to be handled. If they haven't
        /// already been handled, then they are handled.
        /// </summary>
        async public void HandleRemindersAsync()
        {
            List<ReminderModel> reminders = await GetAllRemindersAsync();

            //Reminders without logs are invalid, therfore delete them.
            reminders.ForEach(async r =>
            {
                if (r.Logs.Count == 0)
                    await DeleteReminderAsync(r.ReminderID);
            });

            foreach (ReminderModel reminder in reminders)
                await reminder.Handle();

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
                await logRepo.UpdateLogAsync(log);

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

            unhandledReminders = unhandledReminders.FindAll(reminder => reminder != null); //Filter out missing data

            return unhandledReminders;
        }

        /// <summary>
        /// Gets all ReminderModels then adds all LogModels to them.
        /// </summary>
        /// <returns>List of ReminderModels, might be empty.</returns>
        async public Task<List<ReminderModel>> GetAllRemindersAsync()
        {
            List<ReminderModel> reminders = await reminderRepo.GetAllRemindersAsync();

            for (int i = 0; i < reminders.Count; ++i)
                reminders[i] = await GetReminderAsync(reminders[i].ReminderID); //Get reminder with Logs

            return reminders;
        }

        /// <summary>
        /// Deletes the Remdiner with the given ID.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>False if error occurs, else true</returns>
        async public Task<bool> DeleteReminderAsync(int reminderID)
        {
            return await reminderRepo.DeleteReminderAsync(reminderID);
        }
    }
}
