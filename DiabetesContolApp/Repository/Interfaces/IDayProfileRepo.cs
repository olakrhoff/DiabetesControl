using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IDayProfileRepo
    {
        /// <summary>
        /// Gets all DayProfileDAOs and converts them
        /// to DayProfileModels.
        /// </summary>
        /// <returns>List of DayProfileModels.</returns>
        public Task<List<DayProfileModel>> GetAllDayProfilesAsync();

        /// <summary>
        /// Converts a DayProfileModel to a DAO, then
        /// updates into the database.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>True if inserted, else false.</returns>
        public Task<bool> UpdateDayProfileAsync(DayProfileModel dayProfile);

        /// <summary>
        /// Deletes the DAO object in the database with the
        /// same ID.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>True if deleted, else false.</returns>
        public Task<bool> DeleteDayProfileAsync(int dayProfileID);

        /// <summary>
        /// Converts DayProfile to DAO and inserts into database.
        /// </summary>
        /// <param name="newDayProfile"></param>
        /// <returns>bool, true if inserted, else false</returns>
        public Task<bool> InsertDayProfileAsync(DayProfileModel newDayProfile);

        /// <summary>
        /// Gets the DAO and converts it to a DayProfileModel.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>Returns null if DAO wasn't found, else the new Model with the given ID.</returns>
        public Task<DayProfileModel> GetDayProfileAsync(int dayProfileID);
    }
}
