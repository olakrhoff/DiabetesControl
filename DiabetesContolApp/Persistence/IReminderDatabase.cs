using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Persistence.Interfaces
{
    public interface IReminderDatabase
    {
        /// <summary>
        /// This method deletes the reminder itself based on it's ID.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>
        /// Task&lt;int&gt;
        ///
        /// Task for async.
        ///
        /// int for the number of rows delete, in the reminder table.
        /// If an error occurs it returns -1.
        /// </returns>
        Task<int> DeleteReminderAsync(int reminderID);

        /// <summary>
        /// Gets all ReminderDAOs.
        /// </summary>
        /// <returns>List of all ReminderDAOs, might be empty.</returns>
        Task<List<ReminderModelDAO>> GetAllRemindersAsync();

        /// <summary>
        /// Gets all remidners which are unhandled.
        /// </summary>
        /// <returns>List of unhandled Reminders, might be empty</returns>
        Task<List<ReminderModelDAO>> GetAllUnhandledRemindersAsync();

        /// <summary>
        /// Gets the newest Reminder.
        /// </summary>
        /// <returns>ReminderDAO for the newest reminder, null if no reminders exist.</returns>
        Task<ReminderModelDAO> GetNewestReminderAsync();

        /// <summary>
        /// This method gets the corresponding Reminder from the database
        /// based on its ID.
        /// </summary>
        /// <param name="reminderID">The ID of the reminder</param>
        /// <returns>
        /// Task&lt;ReminderModel&gt;
        /// Task for async, ReminderModel,
        /// the corresponding reminder. If not found it returns null
        /// </returns>
        Task<ReminderModelDAO> GetReminderAsync(int reminderID);

        /// <summary>
        /// Inserts a new Reminder into the database.
        /// </summary>
        /// <param name="reminderModel"></param>
        /// <returns>The number of rows added.</returns>
        Task<int> InsertReminderAsync(ReminderModelDAO reminderModel);

        /// <summary>
        /// Updates the reminderDAO in the database
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns>int, number of rows updated, -1 if an error occured.</returns>
        Task<int> UpdateReminderAsync(ReminderModelDAO reminder);
    }
}