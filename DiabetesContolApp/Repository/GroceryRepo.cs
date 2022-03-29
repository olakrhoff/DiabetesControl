using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;

namespace DiabetesContolApp.Repository
{
    public class GroceryRepo
    {
        private GroceryDatabase groceryDatabase = GroceryDatabase.GetInstance();

        public GroceryRepo()
        {
        }

        /// <summary>
        /// Gets all the groceryDAOs and converts them into GroceryModels
        /// </summary>
        /// <returns>List of GroceryModels.</returns>
        async public Task<List<GroceryModel>> GetAllAsync()
        {
            List<GroceryModelDAO> groceriesDAO = await groceryDatabase.GetGroceriesAsync();

            List<GroceryModel> groceries = new();

            foreach (GroceryModelDAO groceryDAO in groceriesDAO)
                groceries.Add(new(groceryDAO));

            return groceries;
        }

        /// <summary>
        /// Gets the GroceryDAO an converts it to a GroceryModel,
        /// with the given grocery ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>The GroceryModel with the given ID, null if not found</returns>
        async public Task<GroceryModel> GetAsync(int groceryID)
        {
            GroceryModelDAO groceryDAO = await groceryDatabase.GetGroceryAsync(groceryID);
            if (groceryDAO == null)
                return null;

            return new(groceryDAO);
        }

        /// <summary>
        /// Converts the GroceryModel to a DAO object. Then updates
        /// it in the database.
        /// </summary>
        /// <param name="grocery"></param>
        /// <returns>True if updated, else false.</returns>
        async public Task<bool> UpdateAsync(GroceryModel grocery)
        {
            return await groceryDatabase.UpdateGroceryAsync(new(grocery)) > 0;
        }

        /// <summary>
        /// Converts GroceryModel to DAO and deletes the
        /// object in the database.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>True if delete, else false</returns>
        async public Task<bool> DeleteAsync(int groceryID)
        {
            return await groceryDatabase.DeleteGroceryAsync(groceryID) > 0;
        }

        /// <summary>
        /// Converts GroceryModel to DAO object and inserts it
        /// into the database.
        /// </summary>
        /// <param name="newGrocery"></param>
        /// <returns>True if it was inserted, else false</returns>
        async public Task<bool> InsertAsync(GroceryModel newGrocery)
        {
            GroceryModelDAO groceryDAO = new(newGrocery);

            if (await groceryDatabase.InsertGroceryAsync(groceryDAO) > 0)
                return true;
            return false;
        }
    }
}
