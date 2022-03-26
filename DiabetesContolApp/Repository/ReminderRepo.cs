using System;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;

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
        async public Task<ReminderModel> GetAsync(int reminderID)
        {
            ReminderModelDAO reminderDAO = await reminderDatabase.GetReminderAsync(reminderID);

            if (reminderDAO == null)
                return null;

            return new(reminderDAO);
        }

        /// <summary>
        /// Converts ReminderModel to DAO and inserts it into database.
        /// </summary>
        /// <param name="newReminder"></param>
        /// <returns>
        /// Return the ID of the newly added Reminder.
        /// If error returns -1.
        /// </returns>
        async public Task<int> InsertAsync(ReminderModel newReminder)
        {
            if (await reminderDatabase.InsertReminderAsync(new(newReminder)) == 0)
                return -1;

            ReminderModelDAO newestRemidnerDAO = await reminderDatabase.GetNewestReminderAsync();
            if (newestRemidnerDAO == null)
                return -1;
            return newestRemidnerDAO.ReminderID;
        }
    }
}
