using System;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;
using System.Collections.Generic;

namespace DiabetesContolApp.Repository
{
    public class GroceryLogRepo
    {
        private GroceryLogDatabase groceryLogDatabase = GroceryLogDatabase.GetInstance();

        public GroceryLogRepo()
        {
        }

        /// <summary>
        /// Creates DAO objects of all the NumberOfGroceryModels.
        /// Then inserts all the DAOs into the database.
        /// </summary>
        /// <param name="numberOfGroceries"></param>
        /// <returns>Return false if an error occured, else true</returns>
        async public Task<bool> InsertAllAsync(List<NumberOfGroceryModel> numberOfGroceries, int logID)
        {
            List<GroceryLogModelDAO> groceryLogs = new();

            foreach (NumberOfGroceryModel numberOfGrocery in numberOfGroceries)
                groceryLogs.Add(new(numberOfGrocery.Grocery.GroceryID, logID, numberOfGrocery.NumberOfGrocery, numberOfGrocery.InsulinForGroceries));
        }
    }
}
