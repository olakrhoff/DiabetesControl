﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Models;

using SQLite;
using Xamarin.Forms;

namespace DiabetesContolApp.Persistence
{
    public sealed class LogDatabase
    {
        private readonly SQLiteAsyncConnection connection;

        private static LogDatabase instance = null;

        public LogDatabase()
        {
            connection = DependencyService.Get<ISQLiteDB>().GetConnection();

            if (false)
            {
                connection.DropTableAsync<LogModel>().Wait();
                connection.DropTableAsync<GroceryModel>().Wait();
                connection.DropTableAsync<DayProfileModel>().Wait();
                connection.DropTableAsync<GroceryLogModel>().Wait();
            }
            connection.CreateTableAsync<LogModel>().Wait();
            connection.CreateTableAsync<GroceryModel>().Wait();
            connection.CreateTableAsync<DayProfileModel>().Wait();
            connection.CreateTableAsync<GroceryLogModel>().Wait();
        }

        public static LogDatabase GetInstance()
        {
            return instance == null ? new LogDatabase() : instance;
        }

        async internal Task<int> InsertLogAsync(LogModel newLogEntry)
        {
            var id = await connection.InsertAsync(newLogEntry);

            await connection.InsertAllAsync(GroceryLogModel.GetGroceryLogs(newLogEntry.NumberOfGroceryModels, newLogEntry.LogID));

            return id;
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
            //TODO: Remove GroceryLog entreis if they are no longer present, and add the new ones, if any



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

        async internal Task<int> DeleteLogAsync(LogModel log)
        {
            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().ToListAsync();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                if (groceryLog.LogID == log.LogID)
                    await connection.DeleteAsync(groceryLog); //Deletes all the entries in GroceryLog who are connected to the Grocery

            return await connection.DeleteAsync(log);
        }
    }
}
