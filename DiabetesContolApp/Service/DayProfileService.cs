using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;
using DiabetesContolApp.Repository.Interfaces;

namespace DiabetesContolApp.Service
{
    /// <summary>
    /// This is the Service class for DayProfiles. It is responsible for
    /// Assembeling and disassembling DayProfileModel objects and make the
    /// appropriate calls to the respective repositories.
    /// </summary>
    public class DayProfileService
    {
        private readonly IDayProfileRepo _dayProfileRepo;
        private readonly ILogRepo _logRepo;
        private readonly IGroceryLogRepo _groceryLogRepo;
        private readonly IReminderRepo _reminderRepo;

        public DayProfileService(IDayProfileRepo dayProfileRepo, ILogRepo logRepo, IGroceryLogRepo groceryLogRepo, IReminderRepo reminderRepo)
        {
            _dayProfileRepo = dayProfileRepo;
            _logRepo = logRepo;
            _groceryLogRepo = groceryLogRepo;
            _reminderRepo = reminderRepo;
        }

        public static DayProfileService GetDayProfileService()
        {
            return new DayProfileService(new DayProfileRepo(), new LogRepo(), new GroceryLogRepo(), new ReminderRepo());
        }

        /// <summary>
        /// Gets all DAyProfileModels.
        /// </summary>
        /// <returns>List of DayProfiles.</returns>
        async public Task<List<DayProfileModel>> GetAllDayProfilesAsync()
        {
            List<DayProfileModel> dayProfiles = await _dayProfileRepo.GetAllDayProfilesAsync();

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
            if (!await _dayProfileRepo.InsertDayProfileAsync(newDayProfile))
                return -1;

            DayProfileModel newestDayProfile = await GetNewestDayProfileAsync();

            if (newestDayProfile == null)
                return -1;

            return newestDayProfile.DayProfileID;
        }

        /// <summary>
        /// Gets the DayProfile with the highest ID.
        /// </summary>
        /// <returns>DayProfileModel with the highest ID, null if no DayProfiles exists</returns>
        async public Task<DayProfileModel> GetNewestDayProfileAsync()
        {
            List<DayProfileModel> dayProfiles = await GetAllDayProfilesAsync();

            if (dayProfiles.Count == 0)
                return null;

            int newestDayProfileID = dayProfiles.Max(dayprofile => dayprofile.DayProfileID);

            return await GetDayProfileAsync(newestDayProfileID);
        }

        /// <summary>
        /// Updates the DayProfileModel in the database.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>True if updated, else false.</returns>
        async public Task<bool> UpdateDayProfileAsync(DayProfileModel dayProfile)
        {
            return await _dayProfileRepo.UpdateDayProfileAsync(dayProfile);
        }

        /// <summary>
        /// Deletes all Logs who use the DayProfile.
        /// Then deletes the accual DayProfile.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>False if an error, else true</returns>
        async public Task<bool> DeleteDayProfileAsync(int dayProfileID)
        {
            LogService logService = new(_logRepo, _groceryLogRepo, _reminderRepo, _dayProfileRepo);
            //Delete all log using this day profile
            await logService.DeleteAllWithDayProfileIDAsync(dayProfileID);

            return await _dayProfileRepo.DeleteDayProfileAsync(dayProfileID);
        }

        /// <summary>
        /// Gets the DayProfileModel with the given ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>DayProfileModel with given ID or null if not found.</returns>
        async public Task<DayProfileModel> GetDayProfileAsync(int dayProfileID)
        {
            DayProfileModel dayProfile = await _dayProfileRepo.GetDayProfileAsync(dayProfileID);

            if (dayProfile == null)
                return null;

            if (dayProfile.TargetGlucoseValue < 0.0f) //This indicates an illegal value, might have been used as a temp value
            {
                await DeleteDayProfileAsync(dayProfile.DayProfileID);
                return null;
            }

            return dayProfile;
        }
    }
}
