using System;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IDayProfileRepo
    {
        Task<DayProfileModel> GetDayProfileAsync(int dayProfileID);
        Task<bool> DeleteDayProfileAsync(int fakeDayProfileID);
    }
}
