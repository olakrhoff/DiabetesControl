using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Models;

using SQLite;
using Xamarin.Forms;

namespace DiabetesContolApp.Persistence
{
    public sealed class LogDatabase : ModelDatabaseAbstract
    {
        private static LogDatabase instance = null;

        public LogDatabase()
        {
        }

        public static LogDatabase GetInstance()
        {
            return instance == null ? new LogDatabase() : instance;
        }


        /*
         * This method inserts a log into the database, after it
         * connects it to a reminder.
         * 
         * Then it addes all the groceryLog values into the bridge table
         * 
         * Paramas: LogModel, the log to insert
         * 
         * Raturn: the number of rows added.
         */
        async internal Task<int> InsertLogAsync(LogModel newLogEntry)
        {
            var rowsAdded = await connection.InsertAsync(newLogEntry);


            await connection.InsertAllAsync(GroceryLogModel.GetGroceryLogs(newLogEntry.NumberOfGroceryModels, newLogEntry.LogID));

            return rowsAdded;
        }

        async internal Task<LogModel> GetLogAsync(int logID)
        {
            LogModel log = await connection.GetAsync<LogModel>(logID);

            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().Where(e => e.LogID == logID).ToListAsync();

            log.NumberOfGroceryModels = GroceryLogModel.GetNumberOfGroceries(groceryLogs);

            GroceryDatabase groceryDatabase = GroceryDatabase.GetInstance();

            foreach (NumberOfGroceryModel numberOfGrocery in log.NumberOfGroceryModels)
                numberOfGrocery.Grocery = await groceryDatabase.GetGroceryAsync(numberOfGrocery.Grocery.GroceryID);


            return log;
        }

        async internal Task<int> UpdateLogAsync(LogModel log)
        {
            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().Where(e => e.LogID == log.LogID).ToListAsync();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                await connection.DeleteAsync(groceryLog); //Delete the prevoius GroceryLog entris for the log

            //Insert the updated grocery list for the log
            await connection.InsertAllAsync(GroceryLogModel.GetGroceryLogs(log.NumberOfGroceryModels, log.LogID));


            return await connection.UpdateAsync(log);
        }

        async internal Task<List<LogModel>> GetLogsAsync(DateTime dateTime)
        {
            var logs = await connection.Table<LogModel>().ToListAsync();

            List<LogModel> temp = new();

            foreach (LogModel log in logs)
                if (log.DateTimeValue.Date.Equals(dateTime.Date))
                    temp.Add(log);

            logs = temp;

            for (int i = 0; i < logs.Count; ++i)
                logs[i] = await GetLogAsync(logs[i].LogID);

            return logs;
        }

        /*
         * This method deletes a log based on the object itself,
         * it also deletes all the groceryLog entries it is
         * connected to first
         * 
         * Params: LogModel (log), the log to be deleted
         * Return: int, number of rows deleted
         */
        async internal Task<int> DeleteLogAsync(LogModel log)
        {
            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().ToListAsync();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                if (groceryLog.LogID == log.LogID)
                    await connection.DeleteAsync(groceryLog); //Deletes all the entries in GroceryLog who are connected to the Grocery

            return await connection.DeleteAsync(log);
        }

        /*
         * This method deletes a log based on its ID,
         * it also deletes all the groceryLog entries it is
         * connected to first
         * 
         * Parmas: int (logID), the ID of the log to be deleted
         * Return: int, number of rows deleted
         */
        async internal Task<int> DeleteLogAsync(int logID)
        {
            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().ToListAsync();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                if (groceryLog.LogID == logID)
                    await connection.DeleteAsync(groceryLog); //Deletes all the entries in GroceryLog who are connected to the Grocery

            return await connection.DeleteAsync<LogModel>(logID);
        }
    }
}
