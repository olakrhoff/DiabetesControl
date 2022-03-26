using System;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;
using System.Collections.Generic;

namespace DiabetesContolApp.Repository
{
    public class DayProfileRepo
    {
        private DayProfileDatabase dayProfileDatabase = DayProfileDatabase.GetInstance();

        public DayProfileRepo()
        {
        }

        /// <summary>
        /// Gets all DayProfileDAOs and converts them
        /// to DayProfileModels.
        /// </summary>
        /// <returns>List of DayProfileModels.</returns>
        async public Task<List<DayProfileModel>> GetAllAsync()
        {
            List<DayProfileModelDAO> dayProfilesDAO = await dayProfileDatabase.GetDayProfilesAsync();

            List<DayProfileModel> dayProfiles = new();

            foreach (DayProfileModelDAO dayProfileDAO in dayProfilesDAO)
                dayProfiles.Add(new(dayProfileDAO));

            return dayProfiles;
        }

        /// <summary>
        /// Converts a DayProfileModel to a DAO, then
        /// updates into the database.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>True if inserted, else false.</returns>
        async public Task<bool> UpdateAsync(DayProfileModel dayProfile)
        {
            return await dayProfileDatabase.UpdateDayProfileAsync(new(dayProfile)) > 0;
        }

        /// <summary>
        /// Deletes the DAO object in the database with the
        /// same ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>True if deleted, else false.</returns>
        async public Task<bool> DeleteAsync(int dayProfileID)
        {
            return await dayProfileDatabase.DeleteDayProfileAsync(dayProfileID) > 0;
        }

        /// <summary>
        /// Converts DayProfile to DAO and inserts into database.
        /// </summary>
        /// <param name="newDayProfile"></param>
        /// <returns>True if inserted, else false.</returns>
        async public Task<bool> InsertAsync(DayProfileModel newDayProfile)
        {
            DayProfileModelDAO dayProfileDAO = new(newDayProfile);

            return await dayProfileDatabase.InsertDayProfileAsync(dayProfileDAO) > 0;
        }

        /// <summary>
        /// Gets the DAO and converts it to a DayProfileModel.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>Returns null if DAO wasn't found, else the new Model with the given ID.</returns>
        async public Task<DayProfileModel> GetAsync(int dayProfileID)
        {
            DayProfileModelDAO dayProfileDAO = await dayProfileDatabase.GetDayProfileAsync(dayProfileID);

            if (dayProfileDAO == null)
                return null;

            return new(dayProfileDAO);
        }
    }
}
