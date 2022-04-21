using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Persistence.Interfaces
{
    public interface IDayProfileDatabase
    {
        Task<int> DeleteDayProfileAsync(int dayProfileID);
        Task<List<IModelDAO>> GetAllAsync();

        /// <summary>
        /// Gets all DayProfileDAOs and converts them
        /// to DayProfileModels.
        /// </summary>
        /// <returns>List of DayProfileModels.</returns>
        Task<List<DayProfileModelDAO>> GetAllDayProfilesAsync();
        Task<DayProfileModelDAO> GetDayProfileAsync(int dayProfileID);
        string HeaderForCSVFile();
        Task<int> InsertDayProfileAsync(DayProfileModelDAO dayProfile);
        Task<int> UpdateDayProfileAsync(DayProfileModelDAO dayProfile);
    }
}