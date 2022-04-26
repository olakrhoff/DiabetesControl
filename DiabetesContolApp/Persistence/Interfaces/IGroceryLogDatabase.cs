using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Persistence.Interfaces
{
    public interface IGroceryLogDatabase
    {
        /// <summary>
        /// Deletes all rows with a given Grocery ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>int, the number of rows deleted, -1 if an error occurs.</returns>
        Task<int> DeleteAllGroceryLogsWithGroceryIDAsync(int groceryID);

        /// <summary>
        /// Deletes all rows with the given logID.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>int, the number of rows deleted, -1 if an error occurs.</returns>
        Task<int> DeleteAllGroceryLogsWithLogIDAsync(int logID);

        /// <summary>
        /// Gets all DAOs with the given Grocery ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>List of GroceryLogModelDAOs with the given Grocery ID.</returns>
        Task<List<GroceryLogModelDAO>> GetAllGroceryLogsWithGroceryID(int groceryID);

        /// <summary>
        /// Gets all DAOs with the given log ID.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>If no elements was found return empty list, else list of elements found</returns>
        Task<List<GroceryLogModelDAO>> GetAllGroceryLogsWithLogID(int logID);

        /// <summary>
        /// Insert all elements in the list into the database. If there is a problem
        /// it undo the adding and no elements will be added.
        /// </summary>
        /// <param name="groceryLogs"></param>
        /// <returns>int, the number of rows inserted, if an error occurs, then zero is added</returns>
        Task<int> InsertAllGroceryLogsAsync(List<GroceryLogModelDAO> groceryLogs);
    }
}