using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;

namespace DiabetesContolApp.Service
{
    /// <summary>
    /// This is the Service class for DayProfiles. It is responsible for
    /// Assembeling and disassembling DayProfileModel objects and make the
    /// appropriate calls to the respective repositories.
    /// </summary>
    public class DayProfileService
    {
        private DayProfileRepo dayProfileRepo = new();

        private LogService logService = new();

        public DayProfileService()
        {
        }

        /// <summary>
        /// Gets all DAyProfileModels.
        /// </summary>
        /// <returns>List of DayProfiles.</returns>
        async public Task<List<DayProfileModel>> GetAllDayProfilesAsync()
        {
            return await dayProfileRepo.GetAllDayProfilesAsync();
        }

        /// <summary>
        /// Inserts a new DayProfileModel into the database.
        /// </summary>
        /// <param name="newDayProfile"></param>
        /// <returns>True if inserted, else false</returns>
        async public Task<bool> InsertDayProfileAsync(DayProfileModel newDayProfile)
        {
            return await dayProfileRepo.InsertDayProfileAsync(newDayProfile);
        }

        /// <summary>
        /// Updates the DayProfileModel in the database.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>True if updated, else false.</returns>
        async public Task<bool> UpdateDayProfileAsync(DayProfileModel dayProfile)
        {
            return await dayProfileRepo.UpdateDayProfileAsync(dayProfile);
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

            return await dayProfileRepo.DeleteDayProfileAsync(dayProfileID);
        }

        /// <summary>
        /// Gets the DayProfileModel with the given ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>DayProfileModel with giveen ID or null if not found.</returns>
        async public Task<DayProfileModel> GetDayProfileAsync(int dayProfileID)
        {
            return await dayProfileRepo.GetDayProfileAsync(dayProfileID);
        }
    }
}
