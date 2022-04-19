using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IDayProfileRepo
    {
        Task<DayProfileModel> GetDayProfileAsync(int dayProfileID);
        Task<bool> DeleteDayProfileAsync(int fakeDayProfileID);
        Task<List<DayProfileModel>> GetAllDayProfilesAsync();
        Task<bool> InsertDayProfileAsync(DayProfileModel newDayProfile);
        Task<bool> UpdateDayProfileAsync(DayProfileModel dayProfile);
    }
}
