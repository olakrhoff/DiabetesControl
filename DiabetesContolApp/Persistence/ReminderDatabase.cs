using System;
using System.Threading.Tasks;
using DiabetesContolApp.Models;
using SQLite;
using Xamarin.Forms;

namespace DiabetesContolApp.Persistence
{
    public class ReminderDatabase : ModelDatabaseAbstract
    {

        private static ReminderDatabase instance = null;

        public ReminderDatabase()
        {
        }

        public static ReminderDatabase GetInstance()
        {
            return instance == null ? new ReminderDatabase() : instance;
        }

        async internal Task<int> InsertReminderAsync(ReminderModel reminderModel)
        {
            return await connection.InsertAsync(reminderModel);
        }
    }
}