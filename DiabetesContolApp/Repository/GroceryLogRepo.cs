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
        /// Creates GroceryLogModelDAO objects of all the NumberOfGroceryModels.
        /// Then inserts all the DAOs into the database.
        /// </summary>
        /// <param name="numberOfGroceries"></param>
        /// <returns>Return false if an error occured, else true</returns>
        async public Task<bool> InsertAllAsync(List<NumberOfGroceryModel> numberOfGroceries, int logID)
        {
            if (numberOfGroceries.Count == 0)
                return true; //Empty list

            List<GroceryLogModelDAO> groceryLogs = new();

            foreach (NumberOfGroceryModel numberOfGrocery in numberOfGroceries)
                groceryLogs.Add(new(numberOfGrocery.Grocery.GroceryID, logID, numberOfGrocery.NumberOfGrocery, numberOfGrocery.InsulinForGroceries));

            int rowsAdded = await groceryLogDatabase.InsertAllAsync(groceryLogs);

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
        /// <returns>Return false if an error occurs, else true</returns>
        async public Task<bool> DeleteAllAsync(int logID)
        {
            if (await groceryLogDatabase.DeleteAllWithLogID(logID) >= 0)
                return true;
            return false;
        }

        /// <summary>
        /// Gets all GroceryLogModelDAO with the given log ID.
        /// Then converts them to NumberOfGroceryModels.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>Return list of NumberOfGroceryModel, might be empty.</returns>
        async public Task<List<NumberOfGroceryModel>> GetAllWithLogID(int logID)
        {
            List<GroceryLogModelDAO> groceryLogDAOs = await groceryLogDatabase.GetAllWithLogID(logID);

            List<GroceryLogModel> groceryLogs = new();
            List<NumberOfGroceryModel> numberOfGroceries = new();

            foreach (GroceryLogModelDAO groceryLogDAO in groceryLogDAOs)
            {
                groceryLogs.Add(new(groceryLogDAO));
                numberOfGroceries.Add(new(groceryLogs[groceryLogs.Count - 1]));
            }

            return numberOfGroceries;
        }
    }
}
