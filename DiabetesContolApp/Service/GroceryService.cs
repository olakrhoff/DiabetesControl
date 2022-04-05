using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;


using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;

namespace DiabetesContolApp.Service
{
    /// <summary>
    /// This is the Service class for Groceries. It is responsible for
    /// Assembeling and disassembling GroceryModel objects and make the
    /// appropriate calls to the respective repositories.
    /// </summary>
    public class GroceryService
    {
        private GroceryRepo groceryRepo = new();
        private GroceryLogRepo groceryLogRepo = new();

        public GroceryService()
        {
        }

        /// <summary>
        /// Gets all GroceryModels.
        /// </summary>
        /// <returns>List of GroceryModels.</returns>
        async public Task<List<GroceryModel>> GetGroceriesAsync()
        {
            return await groceryRepo.GetAllGroceriesAsync();
        }

        /// <summary>
        /// Inserts a new groceryModel into the database.
        /// </summary>
        /// <param name="newGrocery"></param>
        /// <returns>Returns true if it was inserted, else false.</returns>
        async public Task<bool> InsertGroceryAsync(GroceryModel newGrocery)
        {
            return await groceryRepo.InsertGroceryAsync(newGrocery);
        }

        /// <summary>
        /// Updates the grocery in the database.
        /// </summary>
        /// <param name="grocery"></param>
        /// <returns>True if it was updated, else false.</returns>
        async public Task<bool> UpdateGroceryAsync(GroceryModel grocery)
        {
            return await groceryRepo.UpdateGroceryAsync(grocery);
        }

        /// <summary>
        /// Deletes all logs who have used this grocery.
        /// Then it deletes the grocery. 
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>True if deleted, else false</returns>
        async public Task<bool> DeleteGroceryAsync(int groceryID)
        {
            List<GroceryLogModel> groceryLogsWithGroceryID = await groceryLogRepo.GetAllGroceryLogsWithGroceryID(groceryID);

            List<int> logIDs = groceryLogsWithGroceryID.Select(log => log.Log.LogID).ToList();

            //TODO: Should have some safety code here to handle these deletions failing.
            await groceryLogRepo.DeleteAllGroceryLogsWithGroceryIDAsync(groceryID); //Deletes all entries in cross table

            LogService logService = new();
            await logService.DeleteAllLogsAsync(logIDs); //Deletes all logs


            return await groceryRepo.DeleteGroceryAsync(groceryID);
        }

        /// <summary>
        /// Gets the Grocery with the given ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>GroceryModel with the given ID, or null if not found.</returns>
        async public Task<GroceryModel> GetGroceryAsync(int groceryID)
        {
            return await groceryRepo.GetGroceryAsync(groceryID);
        }
    }
}
