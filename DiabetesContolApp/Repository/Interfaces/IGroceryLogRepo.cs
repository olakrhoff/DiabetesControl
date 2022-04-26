using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IGroceryLogRepo
    {
        /// <summary>
        /// Deletes all elements with the given GroceryID
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>Returns false if an error occurs, else true.</returns>
        Task<bool> DeleteAllGroceryLogsWithGroceryIDAsync(int groceryID);

        /// <summary>
        /// Deletes all elements with the given logID.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>Return false if an error occurs, else true.</returns>
        Task<bool> DeleteAllGroceryLogsWithLogIDAsync(int logID);

        /// <summary>
        /// Gets all GroceryLogDAOs with a given grocery ID.
        /// Then converts them into GroceryLogModel objects.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>List of GroceryLogModels.</returns>
        Task<List<GroceryLogModel>> GetAllGroceryLogsWithGroceryID(int groceryID);

        /// <summary>
        /// Gets all GroceryLogModelDAO with the given log ID.
        /// Then converts them to GroceryLogModels.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>Return list of GroceryLogModel, might be empty.</returns>
        Task<List<GroceryLogModel>> GetAllGroceryLogsWithLogID(int logID);

        /// <summary>
        /// Creates GroceryLogModelDAO objects of all the GroceryLogModels.
        /// Then inserts all the DAOs into the database.
        /// </summary>
        /// <param name="groceryLogs"></param>
        /// <returns>Return false if an error occured, else true</returns>
        Task<bool> InsertAllGroceryLogsAsync(List<GroceryLogModel> groceryLogs);
    }
}