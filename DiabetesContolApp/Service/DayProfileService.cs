using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;

namespace DiabetesContolApp.Service
{
    public class DayProfileService
    {
        private DayProfileRepo dayProfileRepo;
        private LogService logService;

        public DayProfileService()
        {
        }

        /// <summary>
        /// Gets all DAyProfileModels.
        /// </summary>
        /// <returns>List of DayProfiles.</returns>
        async public Task<List<DayProfileModel>> GetDayProfilesAsync()
        {
            return await dayProfileRepo.GetAllAsync();
        }

        /// <summary>
        /// Inserts a new DayProfileModel into the database.
        /// </summary>
        /// <param name="newDayProfile"></param>
        /// <returns>True if inserted, else false</returns>
        async public Task<bool> InsertDayProfileAsync(DayProfileModel newDayProfile)
        {
            return await dayProfileRepo.InsertAsync(newDayProfile);
        }

        /// <summary>
        /// Updates the DayProfileModel in the database.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>True if updated, else false.</returns>
        async public Task<bool> UpdateDayProfileAsync(DayProfileModel dayProfile)
        {
            return await dayProfileRepo.UpdateAsync(dayProfile);
        }

        /// <summary>
        /// Deletes all Logs who use the DayProfile.
        /// Then deletes the accual DayProfile.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>False if an error, else true</returns>
        async public Task<bool> DeleteDayProfileAsync(int dayProfileID)
        {
            //Delete all log using this day profile
            await logService.DeleteAllWithDayProfileIDAsync(dayProfileID);

            return await dayProfileRepo.DeleteAsync(dayProfileID);
        }

        /// <summary>
        /// Gets the DayProfileModel with the given ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>DayProfileModel with giveen ID or null if not found.</returns>
        async public Task<DayProfileModel> GetDayProfileAsync(int dayProfileID)
        {
            return await dayProfileRepo.GetAsync(dayProfileID);
        }
    }
}
