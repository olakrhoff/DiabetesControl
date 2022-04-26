using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Persistence.Interfaces
{
    public interface IDayProfileDatabase
    {
        /// <summary>
        /// Deletes the DayProfile with the given ID.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>int, number of row deleted.</returns>
        Task<int> DeleteDayProfileAsync(int dayProfileID);

        /// <summary>
        /// Gets all DayProfileDAOs and converts them
        /// to DayProfileModels.
        /// </summary>
        /// <returns>List of DayProfileModels.</returns>
        Task<List<DayProfileModelDAO>> GetAllDayProfilesAsync();

        /// <summary>
        /// Gets the DAO with the given ID, if it exists.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>The DAO with the given ID, or null if no element was found.</returns>
        Task<DayProfileModelDAO> GetDayProfileAsync(int dayProfileID);

        /// <summary>
        /// Inserts a DayProfileDAO into the database.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>int, number of rows added.</returns>
        Task<int> InsertDayProfileAsync(DayProfileModelDAO dayProfile);

        /// <summary>
        /// Takes a DayProfileDAO and updates it in the database.
        /// </summary>
        /// <param name="dayProfile"></param>
        /// <returns>int, number of rows updated.</returns>
        Task<int> UpdateDayProfileAsync(DayProfileModelDAO dayProfile);
    }
}