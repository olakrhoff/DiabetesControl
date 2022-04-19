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

        public DayProfileService()
        {
        }

        /// <summary>
        /// Gets all DAyProfileModels.
        /// </summary>
        /// <returns>List of DayProfiles.</returns>
        async public Task<List<DayProfileModel>> GetAllDayProfilesAsync()
        {
            List<DayProfileModel> dayProfiles = await dayProfileRepo.GetAllDayProfilesAsync();

            for (int i = 0; i < dayProfiles.Count; ++i)
                dayProfiles[i] = await GetDayProfileAsync(dayProfiles[i].DayProfileID);

            dayProfiles = dayProfiles.FindAll(dayProfile => dayProfile != null);

            return dayProfiles;
        }

        /// <summary>
        /// Inserts a new DayProfileModel into the database.
        /// </summary>
        /// <param name="newDayProfile"></param>
        /// <returns>ID of new DayProfile, -1 if an error occured.</returns>
        async public Task<int> InsertDayProfileAsync(DayProfileModel newDayProfile)
        {
            if (!await dayProfileRepo.InsertDayProfileAsync(newDayProfile))
                return -1;

            DayProfileModel newestDayProfile = await dayProfileRepo.GetNewestDayProfileAsync();

            if (newestDayProfile == null)
                return -1;

            return newestDayProfile.DayProfileID;
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
            LogService logService = LogService.GetLogService();
            //Delete all log using this day profile
            await logService.DeleteAllWithDayProfileIDAsync(dayProfileID);

            return await dayProfileRepo.DeleteDayProfileAsync(dayProfileID);
        }

        /// <summary>
        /// Gets the DayProfileModel with the given ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>DayProfileModel with given ID or null if not found.</returns>
        async public Task<DayProfileModel> GetDayProfileAsync(int dayProfileID)
        {
            DayProfileModel dayProfile = await dayProfileRepo.GetDayProfileAsync(dayProfileID);

            if (dayProfile.TargetGlucoseValue <= 0.0f) //This indicates an illegal value, might have been used as a temp value
            {
                await DeleteDayProfileAsync(dayProfile.DayProfileID);
                return null;
            }

            return dayProfile;
        }
    }
}
