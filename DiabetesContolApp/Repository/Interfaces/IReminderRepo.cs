using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IReminderRepo
    {
        /// <summary>
        /// Converts ReminderModel to DAO and deletes it
        /// in the database.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>False if error occurs, else true</returns>
        Task<bool> DeleteReminderAsync(int reminderID);

        /// <summary>
        /// Gets all ReminderDAOs and converts them
        /// into ReminderModels.
        /// </summary>
        /// <returns>List of ReminderModels, might be empty.</returns>
        Task<List<ReminderModel>> GetAllRemindersAsync();

        /// <summary>
        /// Gets all RemidnerDAOs which are unhandled,
        /// converts them into ReminderModels.
        /// </summary>
        /// <returns>List of ReminderModels which are unhandled, might be empty</returns>
        Task<List<ReminderModel>> GetAllUnhandledRemindersAsync();

        /// <summary>
        /// Gets the reminderDAO with the corresponding reminderID.
        /// Converts it to a ReminderModel.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>
        /// If the remidner exists it returns the ReminderModel,
        /// if it doesn't it returns null.
        /// </returns>
        Task<ReminderModel> GetReminderAsync(int reminderID);

        /// <summary>
        /// Converts ReminderModel to DAO and inserts it into database.
        /// </summary>
        /// <param name="newReminder"></param>
        /// <returns>bool, true if inserted, else false</returns>
        Task<bool> InsertReminderAsync(ReminderModel newReminder);

        /// <summary>
        /// Converts the RemidnerModel to a DAO and updates
        /// it in the database.
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns>True if updated, else false.</returns>
        Task<bool> UpdateReminderAsync(ReminderModel reminder);
    }
}