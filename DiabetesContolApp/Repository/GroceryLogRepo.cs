using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;

namespace DiabetesContolApp.Repository
{
    public class GroceryLogRepo
    {
        private GroceryLogDatabase groceryLogDatabase = GroceryLogDatabase.GetInstance();

        public GroceryLogRepo()
        {
        }

        /// <summary>
        /// Creates GroceryLogModelDAO objects of all the GroceryLogModels.
        /// Then inserts all the DAOs into the database.
        /// </summary>
        /// <param name="groceryLogs"></param>
        /// <returns>Return false if an error occured, else true</returns>
        async public Task<bool> InsertAllGroceryLogsAsync(List<GroceryLogModel> groceryLogs, int logID)
        {
            if (groceryLogs.Count == 0)
                return true; //Empty list

            List<GroceryLogModelDAO> groceryLogDAOs = new();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                groceryLogDAOs.Add(new(groceryLog));

            int rowsAdded = await groceryLogDatabase.InsertAllGroceryLogsAsync(groceryLogDAOs);

            if (rowsAdded == groceryLogs.Count)
                return true; //All elements was added
            else if (rowsAdded == 0)
                return false;

            throw new Exception("This state should never happen");
            //return false;
        }

        /// <summary>
        /// Deletes all elements with the given logID.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>Return false if an error occurs, else true.</returns>
        async public Task<bool> DeleteAllGroceryLogsWithLogIDAsync(int logID)
        {
            if (await groceryLogDatabase.DeleteAllGroceryLogsWithLogIDAsync(logID) >= 0)
                return true;
            return false;
        }

        /// <summary>
        /// Deletes all elements with the given GroceryID
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>Returns false if an error occurs, else true.</returns>
        async public Task<bool> DeleteAllGroceryLogsWithGroceryIDAsync(int groceryID)
        {
            if (await groceryLogDatabase.DeleteAllGroceryLogsWithGroceryIDAsync(groceryID) >= 0)
                return true;
            return false;
        }

        /// <summary>
        /// Gets all GroceryLogDAOs with a given grocery ID.
        /// Then converts them into GroceryLogModel objects.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>List of GroceryLogModels.</returns>
        async public Task<List<GroceryLogModel>> GetAllGroceryLogsWithGroceryID(int groceryID)
        {
            List<GroceryLogModelDAO> groceryLogsDAO = await groceryLogDatabase.GetAllGroceryLogsWithGroceryID(groceryID);

            List<GroceryLogModel> groceryLogs = new();

            foreach (GroceryLogModelDAO groceryLogDAO in groceryLogsDAO)
                groceryLogs.Add(new(groceryLogDAO));

            return groceryLogs;
        }

        /// <summary>
        /// Gets all GroceryLogModelDAO with the given log ID.
        /// Then converts them to GroceryLogModels.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>Return list of GroceryLogModel, might be empty.</returns>
        async public Task<List<GroceryLogModel>> GetAllGroceryLogsWithLogID(int logID)
        {
            List<GroceryLogModelDAO> groceryLogDAOs = await groceryLogDatabase.GetAllGroceryLogsWithLogID(logID);

            List<GroceryLogModel> groceryLogs = new();

            foreach (GroceryLogModelDAO groceryLogDAO in groceryLogDAOs)
                groceryLogs.Add(new GroceryLogModel(groceryLogDAO));

            return groceryLogs;
        }
    }
}
