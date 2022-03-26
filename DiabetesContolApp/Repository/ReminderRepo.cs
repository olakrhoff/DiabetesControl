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
    }
}
