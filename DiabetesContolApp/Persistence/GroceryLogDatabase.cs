using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence.Interfaces;

namespace DiabetesContolApp.Persistence
{
    public class GroceryLogDatabase : ModelDatabaseAbstract, IGroceryLogDatabase
    {
        private static GroceryLogDatabase _instance = null;

        public GroceryLogDatabase()
        {
        }

        public static GroceryLogDatabase GetInstance()
        {
            return _instance == null ? new GroceryLogDatabase() : _instance;
        }

        /// <summary>
        /// Insert all elements in the list into the database. If there is a problem
        /// it undo the adding and no elements will be added.
        /// </summary>
        /// <param name="groceryLogs"></param>
        /// <returns>int, the number of rows inserted, if an error occurs, then zero is added</returns>
        async public Task<int> InsertAllGroceryLogsAsync(List<GroceryLogModelDAO> groceryLogs)
        {
            int rowsAdded = await _connection.InsertAllAsync(groceryLogs);

            if (rowsAdded == groceryLogs.Count)
                return rowsAdded;

            //One or more elements was not added

            List<GroceryLogModelDAO> allGroceryLogs = await _connection.Table<GroceryLogModelDAO>().ToListAsync();

            List<GroceryLogModelDAO> addedNow = allGroceryLogs.GetRange(allGroceryLogs.Count - rowsAdded - 1, rowsAdded);

            foreach (GroceryLogModelDAO groceryLog in addedNow)
                await _connection.DeleteAsync(groceryLog);

            return 0;
        }

        /// <summary>
        /// Deletes all rows with the given logID.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>int, the number of rows deleted, -1 if an error occurs.</returns>
        async public Task<int> DeleteAllGroceryLogsWithLogIDAsync(int logID)
        {
            //TODO: This should be moved to the Service layer
            try
            {
                List<GroceryLogModelDAO> groceryLogsWithLogID = await _connection.Table<GroceryLogModelDAO>().Where(groceryLog => groceryLog.LogID == logID).ToListAsync();

                int rowsDeleted = 0;

                foreach (GroceryLogModelDAO groceryLog in groceryLogsWithLogID)
                    rowsDeleted += await _connection.DeleteAsync(groceryLog);

                return rowsDeleted;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return -1; //Indicating error
            }
        }

        /// <summary>
        /// Deletes all rows with a given Grocery ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>int, the number of rows deleted, -1 if an error occurs.</returns>
        async public Task<int> DeleteAllGroceryLogsWithGroceryIDAsync(int groceryID)
        {
            //TODO: This should be in the Service layer
            try
            {
                List<GroceryLogModelDAO> groceryLogsWithLogID = await _connection.Table<GroceryLogModelDAO>().Where(groceryLog => groceryLog.GroceryID == groceryID).ToListAsync();

                int rowsDeleted = 0;

                foreach (GroceryLogModelDAO groceryLog in groceryLogsWithLogID)
                    rowsDeleted += await _connection.DeleteAsync(groceryLog);

                return rowsDeleted;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return -1; //Indicating error
            }
        }

        /// <summary>
        /// Gets all DAOs with the given Grocery ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>List of GroceryLogModelDAOs with the given Grocery ID.</returns>
        async public Task<List<GroceryLogModelDAO>> GetAllGroceryLogsWithGroceryID(int groceryID)
        {
            List<GroceryLogModelDAO> groceryLogDAOsWithGroceryID = await _connection.Table<GroceryLogModelDAO>().Where(groceryLogDAO => groceryLogDAO.GroceryID == groceryID).ToListAsync();
            if (groceryLogDAOsWithGroceryID == null)
                return new List<GroceryLogModelDAO>();
            return groceryLogDAOsWithGroceryID;
        }

        /// <summary>
        /// Gets all DAOs with the given log ID.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>If no elements was found return empty list, else list of elements found</returns>
        async public Task<List<GroceryLogModelDAO>> GetAllGroceryLogsWithLogID(int logID)
        {
            List<GroceryLogModelDAO> groceryLogDAOsWithLogID = await _connection.Table<GroceryLogModelDAO>().Where(groceryLogDAO => groceryLogDAO.LogID == logID).ToListAsync();
            if (groceryLogDAOsWithLogID == null)
                return new List<GroceryLogModelDAO>();
            return groceryLogDAOsWithLogID;
        }

        public override string HeaderForCSVFile()
        {
            return "GroceryLogID,GroceryID,LogID,NumberOfGrocery,InsulinForGrocery\n";
        }

        async public override Task<List<IModelDAO>> GetAllAsync()
        {
            return new(await _connection.Table<GroceryLogModelDAO>().ToListAsync());
        }
    }
}
