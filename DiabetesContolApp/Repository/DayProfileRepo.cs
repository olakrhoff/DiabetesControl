using System;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;

namespace DiabetesContolApp.Repository
{
    public class DayProfileRepo
    {
        private DayProfileDatabase dayProfileDatabase = DayProfileDatabase.GetInstance();

        public DayProfileRepo()
        {
        }

        /// <summary>
        /// Gets the DAO and converts it to a DayProfileModel.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>Returns null if DAO wasn't found, else the new Model with the given ID.</returns>
        async public Task<DayProfileModel> GetAsync(int dayProfileID)
        {
            DayProfileModelDAO dayProfileDAO = await dayProfileDatabase.GetDayProfileAsync(dayProfileID);

            if (dayProfileDAO == null)
                return null;

            return new(dayProfileDAO);
        }
    }
}
