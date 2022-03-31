using System;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;
using System.Collections.Generic;

namespace DiabetesContolApp.Repository
{
    public class ReminderRepo
    {
        private ReminderDatabase reminderDatabase = ReminderDatabase.GetInstance();

        public ReminderRepo()
        {
        }

        /// <summary>
        /// Gets the reminderDAO with the corresponding reminderID.
        /// Converts it to a ReminderModel.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>
        /// If the remidner exists it returns the ReminderModel,
        /// if it doesn't it returns null.
        /// </returns>
        async public Task<ReminderModel> GetReminderAsync(int reminderID)
        {
            ReminderModelDAO reminderDAO = await reminderDatabase.GetReminderAsync(reminderID);

            if (reminderDAO == null)
                return null;

            return new(reminderDAO);
        }

        /// <summary>
        /// Gets the reminder with the highest DateTimeValue.
        /// </summary>
        /// <returns>ReminderModel with highest ID, might be null if no reminders exist.</returns>
        async public Task<ReminderModel> GetNewestReminder()
        {
            List<ReminderModelDAO> reminderDAOs = await reminderDatabase.GetAllRemindersAsync();

            if (reminderDAOs.Count == 0)
                return null;

            reminderDAOs.Sort();

            return await GetReminderAsync(reminderDAOs[reminderDAOs.Count - 1].ReminderID);
        }

        /// <summary>
        /// Converts the RemidnerModel to a DAO and updates
        /// it in the database.
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns>True if updated, else false.</returns>
        async public Task<bool> UpdateReminderAsync(ReminderModel reminder)
        {
            return await reminderDatabase.UpdateReminderAsync(new(reminder)) > 0;
        }

        /// <summary>
        /// Converts ReminderModel to DAO and deletes it
        /// in the database.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>False if error occurs, else true</returns>
        async public Task<bool> DeleteReminderAsync(int reminderID)
        {
            return await reminderDatabase.DeleteReminderAsync(reminderID) >= 0;
        }

        /// <summary>
        /// Gets all ReminderDAOs and converts them
        /// into ReminderModels.
        /// </summary>
        /// <returns>List of ReminderModels, might be empty.</returns>
        async public Task<List<ReminderModel>> GetAllRemindersAsync()
        {
            List<ReminderModelDAO> reminderDAOs = await reminderDatabase.GetAllRemindersAsync();

            List<ReminderModel> reminders = new();

            foreach (ReminderModelDAO reminderDAO in reminderDAOs)
                reminders.Add(new(reminderDAO));

            return reminders;
        }

        /// <summary>
        /// Gets all RemidnerDAOs which are unhandled,
        /// converts them into ReminderModels.
        /// </summary>
        /// <returns>List of ReminderModels which are unhandled, might be empty</returns>
        async public Task<List<ReminderModel>> GetAllUnhandledRemindersAsync()
        {
            List<ReminderModelDAO> unhandledReminderDAOs = await reminderDatabase.GetAllUnhandledRemindersAsync();

            List<ReminderModel> unhandledReminders = new();

            foreach (ReminderModelDAO reminderDAO in unhandledReminderDAOs)
                unhandledReminders.Add(new(reminderDAO));

            return unhandledReminders;
        }

        /// <summary>
        /// Converts ReminderModel to DAO and inserts it into database.
        /// </summary>
        /// <param name="newReminder"></param>
        /// <returns>bool, true if inserted, else false</returns>
        async public Task<bool> InsertReminderAsync(ReminderModel newReminder)
        {
            return await reminderDatabase.InsertReminderAsync(new(newReminder)) > 0;
        }
    }
}
