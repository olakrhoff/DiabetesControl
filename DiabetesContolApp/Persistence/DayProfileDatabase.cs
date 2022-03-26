using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.DAO;

using SQLite;

using Xamarin.Forms;
using System.Diagnostics;

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

        /// <summary>
        /// Gets the DAO with the given ID, if it exists.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>The DAO with the given ID, or null if no element was found.</returns>
        async public Task<DayProfileModelDAO> GetDayProfileAsync(int dayProfileID)
        {
            try
            {
                DayProfileModelDAO dayProfileDAO = await connection.GetAsync<DayProfileModelDAO>(dayProfileID);
                return dayProfileDAO;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        async public Task<List<DayProfileModelDAO>> GetDayProfilesAsync()
        {
            return await connection.Table<DayProfileModelDAO>().ToListAsync();
        }

        async public Task<int> InsertDayProfileAsync(DayProfileModelDAO dayProfile)
        {
            return await connection.InsertAsync(dayProfile);
        }

        async public Task<int> UpdateDayProfileAsync(DayProfileModelDAO dayProfile)
        {
            return await connection.UpdateAsync(dayProfile);
        }

        async public Task<int> DeleteDayProfileAsync(DayProfileModelDAO dayProfile)
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
