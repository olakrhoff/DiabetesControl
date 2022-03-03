using System;
using System.Collections.Generic;
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

        async public Task<int> InsertReminderAsync(ReminderModel reminderModel)
        {
            return await connection.InsertAsync(reminderModel);
        }

        /*
         * This method loops through all reminders,
         * and call the Handle()-method on the ones who
         * are yet to be handled. If they were changed in 
         * the handle()-method, they are updated in the database
         * 
         * Parmas: None
         * 
         * Return: void
         */
        async public void HandleReminders()
        {
            List<ReminderModel> unhandledReminders = await GetRemindersAsync();

            unhandledReminders.ForEach(async e =>
            {
                if (!e.IsHandled && await e.Handle())
                    await UpdateReminderAsync(e);
            });
        }

        /*
         * This method updates the reminder in the database with the 
         * corresponding ReminderID.
         * 
         * Parmas: ReminderModel (reminder), the reminder object who holds
         * the new values.
         * 
         * Return: Task<int>, task for async, int: the number of rows updated
         */
        async public Task<int> UpdateReminderAsync(ReminderModel reminder)
        {
            return await connection.UpdateAsync(reminder);
        }

        /*
         * This method gets all the reminder in the database.
         * 
         * Parmas: None
         * 
         * Return: Task<List<ReminderModel>>, Task for async, List<ReminderModel>
         * is the list from the database.
         */
        async public Task<List<ReminderModel>> GetRemindersAsync()
        {
            return await connection.Table<ReminderModel>()?.ToListAsync();
        }
    }
}