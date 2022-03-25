using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.DAO;

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
        async internal Task<DayProfileModelDAO> GetDayProfileAsync(int dayProfileID)
        {
            return await connection.GetAsync<DayProfileModelDAO>(dayProfileID);
        }

        async internal Task<List<DayProfileModelDAO>> GetDayProfilesAsync()
        {
            return await connection.Table<DayProfileModelDAO>().ToListAsync();
        }

        async internal Task<int> InsertDayProfileAsync(DayProfileModelDAO dayProfile)
        {
            return await connection.InsertAsync(dayProfile);
        }

        async internal Task<int> UpdateDayProfileAsync(DayProfileModelDAO dayProfile)
        {
            return await connection.UpdateAsync(dayProfile);
        }

        async internal Task<int> DeleteDayProfileAsync(DayProfileModelDAO dayProfile)
        {
            List<LogModelDAO> logs = await connection.Table<LogModelDAO>().ToListAsync();

            LogDatabase logDatabase = LogDatabase.GetInstance();

            foreach (LogModelDAO log in logs)
                if (log.DayProfileID == dayProfile.DayProfileID)
                    await logDatabase.DeleteLogAsync(log.LogID);

            return await connection.DeleteAsync(dayProfile);
        }

        public override string HeaderForCSVFile()
        {
            return "DayProfileID, Name, StartTime, CarbScalar, GlucoseScalar, TargetGlucoseValue\n";
        }

        async public override Task<List<IModelDAO>> GetAllAsync()
        {
            return new(await connection.Table<DayProfileModelDAO>().ToListAsync());
        }
    }
}
