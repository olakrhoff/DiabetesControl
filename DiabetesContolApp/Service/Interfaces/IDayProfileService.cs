using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Service.Interfaces
{
    public interface IDayProfileService
    {
        /// <summary>
        /// Deletes all Logs who use the DayProfile.
        /// Then deletes the accual DayProfile.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>False if an error, else true</returns>
        Task<bool> DeleteDayProfileAsync(int dayProfileID);

        /// <summary>
        /// Gets all DAyProfileModels.
        /// </summary>
        /// <returns>List of DayProfiles.</returns>
        Task<List<DayProfileModel>> GetAllDayProfilesAsync();

        /// <summary>
        /// Gets the DayProfileModel with the given ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>DayProfileModel with given ID or null if not found.</returns>
        Task<DayProfileModel> GetDayProfileAsync(int dayProfileID);

        /// <summary>
        /// Gets the DayProfile with the highest ID.
        /// </summary>
        /// <returns>DayProfileModel with the highest ID, null if no DayProfiles exists</returns>
        Task<DayProfileModel> GetNewestDayProfileAsync();

        /// <summary>
        /// Inserts a new DayProfileModel into the database.
        /// </summary>
        /// <param name="newDayProfile"></param>
        /// <returns>ID of new DayProfile, -1 if an error occured.</returns>
        Task<int> InsertDayProfileAsync(DayProfileModel newDayProfile);

        /// <summary>
        /// Updates the DayProfileModel in the database.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>True if updated, else false.</returns>
        Task<bool> UpdateDayProfileAsync(DayProfileModel dayProfile);
    }
}