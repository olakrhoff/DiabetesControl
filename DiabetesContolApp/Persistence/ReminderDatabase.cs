using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.DAO;

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

        /// <summary>
        /// Inserts a new Reminder into the database.
        /// </summary>
        /// <param name="reminderModel"></param>
        /// <returns>The number of rows added.</returns>
        async public Task<int> InsertReminderAsync(ReminderModelDAO reminderModel)
        {
            return await _connection.InsertAsync(reminderModel);
        }

        /// <summary>
        /// Gets all remidners which are unhandled.
        /// </summary>
        /// <returns>List of unhandled Reminders, might be empty</returns>
        async public Task<List<ReminderModelDAO>> GetAllUnhandledRemindersAsync()
        {
            try
            {
                List<ReminderModelDAO> unhandledRemidners = await _connection.Table<ReminderModelDAO>().Where(reminder => !reminder.IsHandled).ToListAsync();
                return unhandledRemidners;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return new();
            }
        }

        /// <summary>
        /// Gets all ReminderDAOs.
        /// </summary>
        /// <returns>List of all ReminderDAOs, might be empty.</returns>
        async public Task<List<ReminderModelDAO>> GetAllRemindersAsync()
        {
            List<ReminderModelDAO> reminderDAOs = await _connection.Table<ReminderModelDAO>().ToListAsync();
            if (reminderDAOs == null)
                return new();
            return reminderDAOs;
        }

        /*
        /// <summary>
        /// This method loops through all reminders,
        /// and call the Handle()-method on the ones who
        /// are yet to be handled.If they were changed in
        /// the handle()-method, they are updated in the database
        /// </summary>
        /// <returns>void</returns>
        async public void HandleReminders()
        {
            List<ReminderModelDAO> reminders = await GetRemindersAsync();

            reminders.ForEach(async e =>
            {
                if (!e.IsHandled && await e.Handle())
                    await UpdateReminderAsync(e);
            });
        }*/

        /// <summary>
        /// Gets the newest Reminder.
        /// </summary>
        /// <returns>ReminderDAO for the newest reminder, null if no reminders exist.</returns>
        async public Task<ReminderModelDAO> GetNewestReminderAsync()
        {
            List<ReminderModelDAO> remindersDAO = await _connection.Table<ReminderModelDAO>().ToListAsync();

            if (remindersDAO.Count == 0)
                return null;

            return remindersDAO[remindersDAO.Count - 1];
        }

        /// <summary>
        /// Updates the reminderDAO in the database
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns>int, number of rows updated, -1 if an error occured.</returns>
        async public Task<int> UpdateReminderAsync(ReminderModelDAO reminder)
        {
            try
            {
                return await _connection.UpdateAsync(reminder);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.Message);
                return -1;
            }
        }

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
        async public Task<ReminderModelDAO> GetReminderAsync(int reminderID)
        {
            try
            {
                ReminderModelDAO reminder = await _connection.GetAsync<ReminderModelDAO>(reminderID);
                return reminder;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine(ioe.Message);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        /*
        async public Task<List<ReminderModelDAO>> GetRemindersAsync()
        {
            return await connection.Table<ReminderModelDAO>()?.ToListAsync();
        }*/


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
        async public Task<int> DeleteReminderAsync(int reminderID)
        {
            try
            {
                return await _connection.DeleteAsync<ReminderModelDAO>(reminderID);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return -1;
            }
        }

        public override string HeaderForCSVFile()
        {
            return "ReminderID, DateTimeValue, GlucoseAfterMeal, IsHandled\n";
        }

        async public override Task<List<IModelDAO>> GetAllAsync()
        {
            return new(await _connection.Table<ReminderModelDAO>().ToListAsync());
        }
    }
}