﻿using System;
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
            return await connection.InsertAsync(reminderModel);
        }

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
        }

        /// <summary>
        /// Gets the newest Reminder.
        /// </summary>
        /// <returns>ReminderDAO for the newest reminder, null if no reminders exist.</returns>
        async public Task<ReminderModelDAO> GetNewestReminderAsync()
        {
            List<ReminderModelDAO> remindersDAO = await connection.Table<ReminderModelDAO>().ToListAsync();

            if (remindersDAO.Count == 0)
                return null;

            return remindersDAO[remindersDAO.Count - 1];
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
        async public Task<int> UpdateReminderAsync(ReminderModelDAO reminder)
        {
            return await connection.UpdateAsync(reminder);
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
                ReminderModelDAO reminder = await connection.GetAsync<ReminderModelDAO>(reminderID);
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

        /// <summary>
        /// This method gets the logs that are connected to the
        /// reminder with the spesific ID.
        /// </summary>
        /// <param name="reminderID">
        /// The ID the reminder has.
        /// </param>
        /// <returns>
        /// Task&lt;List&lt;LogModel&gt;&gt;
        ///
        /// Task is for async.
        ///
        /// List&lt;LogModel&gt; is the list of Logs that are
        /// connected to the reminder of the specified ID.
        /// </returns>
        async private Task<List<LogModelDAO>> GetLogsForReminderAsync(int reminderID)
        {
            LogDatabase logDatabase = LogDatabase.GetInstance();

            return (await logDatabase.GetLogsAsync()).Where(log => log.ReminderID == reminderID).ToList();
        }

        /*
         * This method gets all the reminder in the database.
         * 
         * Parmas: None
         * 
         * Return: Task<List<ReminderModel>>, Task for async, List<ReminderModel>
         * is the list from the database.
         */
        async public Task<List<ReminderModelDAO>> GetRemindersAsync()
        {
            return await connection.Table<ReminderModelDAO>()?.ToListAsync();
        }


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
        /// </returns>
        async public Task<int> DeleteReminderAsync(int reminderID)
        {
            return await connection.DeleteAsync<ReminderModelDAO>(reminderID);
        }

        public override string HeaderForCSVFile()
        {
            return "ReminderID, DateTimeValue, GlucoseAfterMeal, IsHandled\n";
        }

        async public override Task<List<IModelDAO>> GetAllAsync()
        {
            return new(await connection.Table<ReminderModelDAO>().ToListAsync());
        }
    }
}