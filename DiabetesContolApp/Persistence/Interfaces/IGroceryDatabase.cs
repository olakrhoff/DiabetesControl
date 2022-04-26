using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Persistence.Interfaces
{
    public interface IGroceryDatabase
    {
        /// <summary>
        /// Deletes the groceryDAO in the database
        /// with the provided ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>int, number of rows delete, -1 if an error occured.</returns>
        Task<int> DeleteGroceryAsync(int groceryID);

        /// <summary>
        /// Get all the GroceryDAO entries.
        /// </summary>
        /// <returns>Return a List of GroceryDAO objects.</returns>
        Task<List<GroceryModelDAO>> GetAllGroceriesAsync();

        /// <summary>
        /// Get the groceryDAO with the given ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>Retursns the GroceryDAO, if not found then null.</returns>
        Task<GroceryModelDAO> GetGroceryAsync(int groceryID);

        /// <summary>
        /// Inserts DAO into database.
        /// </summary>
        /// <param name="newGrocery"></param>
        /// <returns>int, number of rows added.</returns>
        Task<int> InsertGroceryAsync(GroceryModelDAO newGrocery);

        /// <summary>
        /// Updates the given groceryDAO.
        /// </summary>
        /// <param name="grocery"></param>
        /// <returns>Returns the number of rows updated.</returns>
        Task<int> UpdateGroceryAsync(GroceryModelDAO grocery);
    }
}