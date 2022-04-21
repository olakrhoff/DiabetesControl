using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;


using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;
using DiabetesContolApp.Repository.Interfaces;
using DiabetesContolApp.Service.Interfaces;

namespace DiabetesContolApp.Service
{
    /// <summary>
    /// This is the Service class for Groceries. It is responsible for
    /// Assembeling and disassembling GroceryModel objects and make the
    /// appropriate calls to the respective repositories.
    /// </summary>
    public class GroceryService : IGroceryService
    {
        private readonly IGroceryRepo _groceryRepo;
        private readonly IGroceryLogRepo _groceryLogRepo;
        private readonly ILogRepo _logRepo;
        private readonly IReminderRepo _reminderRepo;
        private readonly IDayProfileRepo _dayProfileRepo;

        public GroceryService(IGroceryRepo groceryRepo, IGroceryLogRepo groceryLogRepo, ILogRepo logRepo, IReminderRepo reminderRepo, IDayProfileRepo dayProfileRepo)
        {
            _groceryRepo = groceryRepo;
            _groceryLogRepo = groceryLogRepo;
            _logRepo = logRepo;
            _reminderRepo = reminderRepo;
            _dayProfileRepo = dayProfileRepo;
        }

        public static GroceryService GetGroceryService()
        {
            return new GroceryService(GroceryRepo.GetGroceryRepo(), new GroceryLogRepo(), new LogRepo(), new ReminderRepo(), DayProfileRepo.GetDayProfileRepo());
        }


        /// <summary>
        /// Gets all GroceryModels.
        /// </summary>
        /// <returns>List of GroceryModels, might be empty.</returns>
        async public Task<List<GroceryModel>> GetAllGroceriesAsync()
        {
            return await _groceryRepo.GetAllGroceriesAsync();
        }

        /// <summary>
        /// Inserts a new groceryModel into the database.
        /// </summary>
        /// <param name="newGrocery"></param>
        /// <returns>Returns true if it was inserted, else false.</returns>
        async public Task<bool> InsertGroceryAsync(GroceryModel newGrocery)
        {
            return await _groceryRepo.InsertGroceryAsync(newGrocery);
        }

        /// <summary>
        /// Updates the grocery in the database.
        /// </summary>
        /// <param name="grocery"></param>
        /// <returns>True if it was updated, else false.</returns>
        async public Task<bool> UpdateGroceryAsync(GroceryModel grocery)
        {
            return await _groceryRepo.UpdateGroceryAsync(grocery);
        }

        /// <summary>
        /// Deletes all logs who have used this grocery.
        /// Then it deletes the grocery. 
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>True if deleted, else false</returns>
        async public Task<bool> DeleteGroceryAsync(int groceryID)
        {
            List<GroceryLogModel> groceryLogsWithGroceryID = await _groceryLogRepo.GetAllGroceryLogsWithGroceryID(groceryID);

            List<int> logIDs = groceryLogsWithGroceryID.Select(log => log.Log.LogID).ToList();

            await _groceryLogRepo.DeleteAllGroceryLogsWithGroceryIDAsync(groceryID); //Deletes all entries in cross table

            LogService logService = new(_logRepo, _groceryLogRepo, _reminderRepo, _dayProfileRepo);
            await logService.DeleteAllLogsAsync(logIDs); //Deletes all logs


            return await _groceryRepo.DeleteGroceryAsync(groceryID);
        }

        /// <summary>
        /// Gets the Grocery with the given ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>GroceryModel with the given ID, or null if not found.</returns>
        async public Task<GroceryModel> GetGroceryAsync(int groceryID)
        {
            return await _groceryRepo.GetGroceryAsync(groceryID);
        }
    }
}
