using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Models;

using SQLite;

using Xamarin.Forms;

namespace DiabetesContolApp.Persistence
{
    public class DayProfileDatabase
    {
        private readonly SQLiteAsyncConnection connection;

        private static DayProfileDatabase instance = null;

        public DayProfileDatabase()
        {
            connection = DependencyService.Get<ISQLiteDB>().GetConnection();

            connection.CreateTableAsync<LogModel>().Wait();
            connection.CreateTableAsync<GroceryModel>().Wait();
            connection.CreateTableAsync<DayProfileModel>().Wait();
            connection.CreateTableAsync<GroceryLogModel>().Wait();
        }

        public static DayProfileDatabase GetInstance()
        {
            return instance == null ? new DayProfileDatabase() : instance;
        }

        async internal Task<List<DayProfileModel>> GetDayProfilesAsync()
        {
            return await connection.Table<DayProfileModel>().ToListAsync();
        }

        async internal Task<int> InsertDayProfileAsync(DayProfileModel dayProfile)
        {
            return await connection.InsertAsync(dayProfile);
        }

        async internal Task<int> UpdateDayProfileAsync(DayProfileModel dayProfile)
        {
            return await connection.UpdateAsync(dayProfile);
        }

        async internal Task<int> DeleteDayProfileAsync(DayProfileModel dayProfile)
        {
            List<LogModel> logs = await connection.Table<LogModel>().ToListAsync();

            LogDatabase logDatabase = LogDatabase.GetInstance();

            foreach (LogModel log in logs)
                if (log.DayProfileID == dayProfile.DayProfileID)
                    await logDatabase.DeleteLogAsync(log);

            return await connection.DeleteAsync(dayProfile);
        }
    }
}
