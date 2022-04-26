using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Service.Interfaces
{
    public interface IReminderService
    {
        /// <summary>
        /// Deletes the Remdiner with the given ID.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>False if error occurs, else true</returns>
        Task<bool> DeleteReminderAsync(int reminderID);

        /// <summary>
        /// Gets all unhandled ReminderModels, then gets
        /// them with LogModels.
        /// </summary>
        /// <returns>List of ReminderModels with Logs and are unhandled.</returns>
        Task<List<ReminderModel>> GetAllRemindersAsync();

        /// <summary>
        /// Gets the reminder with the highest DateTimeValue.
        /// </summary>
        /// <returns>ReminderModel with highest ID, might be null if no reminders exist.</returns>
        Task<ReminderModel> GetNewestReminderAsync();

        /// <summary>
        /// Gets the ReminderModel with the given ID.
        /// Then it adds the Logs to it.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>ReminderModel with given ID or null if not found.</returns>
        Task<ReminderModel> GetReminderAsync(int reminderID);

        /// <summary>
        /// Checks all reminders if their timer is done
        /// and are ready to be handled. If they haven't
        /// already been handled, then they are handled.
        /// </summary>
        Task HandleRemindersAsync();

        /// <summary>
        /// Inserts the new Reminder into the database.
        /// </summary>
        /// <param name="newReminder"></param>
        /// <returns>
        /// int, the ID of the newly added
        /// Reminder, -1 if an error occured.
        /// </returns>
        Task<int> InsertReminderAsync(ReminderModel newReminder);

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
        Task<bool> UpdateReminderAsync(ReminderModel reminder);
    }
}