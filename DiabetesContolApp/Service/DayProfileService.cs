using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.Models;

namespace DiabetesContolApp.Service
{
    public class DayProfileService
    {
        public DayProfileService()
        {
        }

        async public Task<List<DayProfileModel>> GetDayProfilesAsync()
        {
            throw new NotImplementedException();
        }

        async public Task<bool> InsertDayProfileAsync(DayProfileModel newDayProfile)
        {
            throw new NotImplementedException();
        }

        async public Task<bool> UpdateDayProfileAsync(DayProfileModel dayProfile)
        {
            throw new NotImplementedException();
        }

        async public Task<bool> DeleteDayProfileAsync(DayProfileModel dayProfile)
        {
            throw new NotImplementedException();
        }

        async public Task<DayProfileModel> GetDayProfileAsync(int dayProfileID)
        {
            throw new NotImplementedException();
        }
    }
}
