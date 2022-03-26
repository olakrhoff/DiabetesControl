using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Persistence
{
    public class GroceryLogDatabase : ModelDatabaseAbstract
    {
        private static GroceryLogDatabase instance = null;

        public GroceryLogDatabase()
        {
        }

        public static GroceryLogDatabase GetInstance()
        {
            return instance == null ? new GroceryLogDatabase() : instance;
        }

        public override string HeaderForCSVFile()
        {
            return "GroceryLogID, GroceryID, LogID, NumberOfGrocery\n";
        }

        async public override Task<List<IModelDAO>> GetAllAsync()
        {
            return new(await connection.Table<GroceryLogModelDAO>().ToListAsync());
        }

        /// <summary>
        /// Insert all elements in the list into the database. If there is a problem
        /// it undo the adding and no elements will be added.
        /// </summary>
        /// <param name="groceryLogs"></param>
        /// <returns>int, the number of rows inserted, if an error occurs, then zero is added</returns>
        async public Task<int> InsertAllAsync(List<GroceryLogModelDAO> groceryLogs)
        {
            int rowsAdded = await connection.InsertAllAsync(groceryLogs);

            if (rowsAdded == groceryLogs.Count)
                return rowsAdded;

            //One or more elements was not added

            List<GroceryLogModelDAO> allGroceryLogs = await connection.Table<GroceryLogModelDAO>().ToListAsync();

            List<GroceryLogModelDAO> addedNow = allGroceryLogs.GetRange(allGroceryLogs.Count - rowsAdded - 1, rowsAdded);

            foreach (GroceryLogModelDAO groceryLog in addedNow)
                await connection.DeleteAsync(groceryLog);

            return 0;
        }

        /// <summary>
        /// Deletes all rows with the given logID.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>int, the number of rows deleted, -1 if an error occurs</returns>
        async public Task<int> DeleteAllWithLogID(int logID)
        {
            try
            {
                List<GroceryLogModelDAO> groceryLogsWithLogID = await connection.Table<GroceryLogModelDAO>().Where(groceryLog => groceryLog.LogID == logID).ToListAsync();

                int rowsDeleted = 0;

                foreach (GroceryLogModelDAO groceryLog in groceryLogsWithLogID)
                    rowsDeleted += await connection.DeleteAsync(groceryLog);

                return rowsDeleted;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return -1; //Indicating error
            }
        }

        /// <summary>
        /// Gets all DAOs with the given log ID.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>If no elements was found return empty list, else list of elements found</returns>
        async public Task<List<GroceryLogModelDAO>> GetAllWithLogID(int logID)
        {
            List<GroceryLogModelDAO> groceryLogDAOsWithLogID = await connection.Table<GroceryLogModelDAO>().Where(groceryLogDAO => groceryLogDAO.LogID == logID).ToListAsync();
            if (groceryLogDAOsWithLogID == null)
                return new List<GroceryLogModelDAO>();
            return groceryLogDAOsWithLogID;
        }
    }
}
