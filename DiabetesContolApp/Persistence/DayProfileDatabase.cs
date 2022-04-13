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
                return await _connection.GetAsync<DayProfileModelDAO>(dayProfileID);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }


        /// <summary>
        /// Gets all DayProfilesDAOs.
        /// </summary>
        /// <returns>Returns a list of DayProfileDAOs.</returns>
        async public Task<List<DayProfileModelDAO>> GetAllDayProfilesAsync()
        {
            return await _connection.Table<DayProfileModelDAO>().ToListAsync();
        }

        /// <summary>
        /// Inserts a DayProfileDAO into the database.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>int, number of rows added.</returns>
        async public Task<int> InsertDayProfileAsync(DayProfileModelDAO dayProfile)
        {
            return await _connection.InsertAsync(dayProfile);
        }

        /// <summary>
        /// Takes a DayProfileDAO and updates it in the database.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>int, number of rows updated.</returns>
        async public Task<int> UpdateDayProfileAsync(DayProfileModelDAO dayProfile)
        {
            return await _connection.UpdateAsync(dayProfile);
        }

        /// <summary>
        /// Deletes the DayProfile with the given ID.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>int, number of row deleted.</returns>
        async public Task<int> DeleteDayProfileAsync(int dayProfileID)
        {
            return await _connection.DeleteAsync<DayProfileModelDAO>(dayProfileID);
        }

        public override string HeaderForCSVFile()
        {
            return "DayProfileID, Name, StartTime, CarbScalar, GlucoseScalar, TargetGlucoseValue\n";
        }

        async public override Task<List<IModelDAO>> GetAllAsync()
        {
            return new(await _connection.Table<DayProfileModelDAO>().ToListAsync());
        }
    }
}
