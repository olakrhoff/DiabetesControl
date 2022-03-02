using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Models;

using SQLite;

using Xamarin.Forms;

namespace DiabetesContolApp.Persistence
{
    public class DayProfileDatabase : ModelDatabaseAbstract
    {

        private static DayProfileDatabase instance = null;

        public DayProfileDatabase()
        {
        }

        public static DayProfileDatabase GetInstance()
        {
            return instance == null ? new DayProfileDatabase() : instance;
        }

        /*
         * This method gets a DayProfileModel based on its ID.
         * 
         * Parmas: int, the ID of the DayProfileModel
         * 
         * Return: int, the primaryKey (ID) of the DayProfile.
         */
        async internal Task<DayProfileModel> GetDayProfileAsync(int dayProfileID)
        {
            return await connection.GetAsync<DayProfileModel>(dayProfileID);
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
