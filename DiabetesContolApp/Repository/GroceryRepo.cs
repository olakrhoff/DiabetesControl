using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;
using DiabetesContolApp.Persistence.Interfaces;
using DiabetesContolApp.Repository.Interfaces;

namespace DiabetesContolApp.Repository
{
    public class GroceryRepo : IGroceryRepo
    {
        private readonly IGroceryDatabase _groceryDatabase;

        public GroceryRepo(IGroceryDatabase groceryDatabase)
        {
            _groceryDatabase = groceryDatabase;
        }

        public static GroceryRepo GetGroceryRepo()
        {
            return new GroceryRepo(GroceryDatabase.GetInstance());
        }

        /// <summary>
        /// Gets all the groceryDAOs and converts them into GroceryModels
        /// </summary>
        /// <returns>List of GroceryModels, might be empty.</returns>
        async public Task<List<GroceryModel>> GetAllGroceriesAsync()
        {
            List<GroceryModelDAO> groceriesDAO = await _groceryDatabase.GetAllGroceriesAsync();

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
        async public Task<GroceryModel> GetGroceryAsync(int groceryID)
        {
            GroceryModelDAO groceryDAO = await _groceryDatabase.GetGroceryAsync(groceryID);
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
        async public Task<bool> UpdateGroceryAsync(GroceryModel grocery)
        {
            return await _groceryDatabase.UpdateGroceryAsync(new(grocery)) > 0;
        }

        /// <summary>
        /// Converts GroceryModel to DAO and deletes the
        /// object in the database.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>False if an error occured, else true.</returns>
        async public Task<bool> DeleteGroceryAsync(int groceryID)
        {
            return await _groceryDatabase.DeleteGroceryAsync(groceryID) >= 0;
        }

        /// <summary>
        /// Converts GroceryModel to DAO object and inserts it
        /// into the database.
        /// </summary>
        /// <param name="newGrocery"></param>
        /// <returns>True if it was inserted, else false</returns>
        async public Task<bool> InsertGroceryAsync(GroceryModel newGrocery)
        {
            GroceryModelDAO groceryDAO = new(newGrocery);

            if (await _groceryDatabase.InsertGroceryAsync(groceryDAO) > 0)
                return true;
            return false;
        }
    }
}
