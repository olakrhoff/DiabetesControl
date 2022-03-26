using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;


using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;

namespace DiabetesContolApp.Service
{
    public class GroceryService
    {
        private GroceryRepo groceryRepo;
        private LogRepo logRepo;
        private GroceryLogRepo groceryLogRepo;

        public GroceryService()
        {
        }

        /// <summary>
        /// Gets all GroceryModels.
        /// </summary>
        /// <returns>List of GroceryModels.</returns>
        async public Task<List<GroceryModel>> GetGroceriesAsync()
        {
            return await groceryRepo.GetAllAsync();
        }

        /// <summary>
        /// Inserts a new groceryModel into the database.
        /// </summary>
        /// <param name="newGrocery"></param>
        /// <returns>Returns true if it was inserted, else false.</returns>
        async public Task<bool> InsertGroceryAsync(GroceryModel newGrocery)
        {
            return await groceryRepo.InsertAsync(newGrocery);
        }

        /// <summary>
        /// Updates the grocery in the database.
        /// </summary>
        /// <param name="grocery"></param>
        /// <returns>True if it was updated, else false.</returns>
        async public Task<bool> UpdateGroceryAsync(GroceryModel grocery)
        {
            return await groceryRepo.UpdateAsync(grocery);
        }

        /// <summary>
        /// Deletes all logs who have used this grocery.
        /// Then it deletes the grocery. 
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>True if deleted, else false</returns>
        async public Task<bool> DeleteGroceryAsync(int groceryID)
        {
            List<GroceryLogModel> groceryLogsWithGroceryID = await groceryLogRepo.GetAllWithGroceryID(groceryID);

            List<int> logIDs = groceryLogsWithGroceryID.Select(log => log.Log.LogID).ToList();

            await groceryLogRepo.DeleteAllWithGroceryIDAsync(groceryID); //Deletes all entries in cross table

            await logRepo.DeleteAllAsync(logIDs); //Deletes all logs


            return await groceryRepo.DeleteAsync(groceryID);
        }
    }
}
