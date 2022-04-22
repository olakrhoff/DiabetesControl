using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Service.Interfaces
{
    public interface IGroceryService
    {

        /// <summary>
        /// Deletes all logs who have used this grocery.
        /// Then it deletes the grocery. 
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>True if deleted, else false</returns>
        Task<bool> DeleteGroceryAsync(int groceryID);

        /// <summary>
        /// Gets all GroceryModels.
        /// </summary>
        /// <returns>List of GroceryModels, might be empty.</returns>
        Task<List<GroceryModel>> GetAllGroceriesAsync();

        /// <summary>
        /// Gets the Grocery with the given ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>GroceryModel with the given ID, or null if not found.</returns>
        Task<GroceryModel> GetGroceryAsync(int groceryID);

        /// <summary>
        /// Inserts a new groceryModel into the database.
        /// </summary>
        /// <param name="newGrocery"></param>
        /// <returns>Returns true if it was inserted, else false.</returns>
        Task<bool> InsertGroceryAsync(GroceryModel newGrocery);

        /// <summary>
        /// Updates the grocery in the database.
        /// </summary>
        /// <param name="grocery"></param>
        /// <returns>True if it was updated, else false.</returns>
        Task<bool> UpdateGroceryAsync(GroceryModel grocery);
    }
}