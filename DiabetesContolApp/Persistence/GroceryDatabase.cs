using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence.Interfaces;


namespace DiabetesContolApp.Persistence
{
    public class GroceryDatabase : ModelDatabaseAbstract, IGroceryDatabase
    {
        private static GroceryDatabase instance = null;

        public GroceryDatabase()
        {
        }

        public static GroceryDatabase GetInstance()
        {
            return instance == null ? new GroceryDatabase() : instance;
        }

        /// <summary>
        /// Inserts DAO into database.
        /// </summary>
        /// <param name="newGrocery"></param>
        /// <returns>int, number of rows added.</returns>
        async public Task<int> InsertGroceryAsync(GroceryModelDAO newGrocery)
        {
            return await _connection.InsertAsync(newGrocery);
        }

        /// <summary>
        /// Get the groceryDAO with the given ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>Retursns the GroceryDAO, if not found then null.</returns>
        async public Task<GroceryModelDAO> GetGroceryAsync(int groceryID)
        {
            try
            {
                GroceryModelDAO groceryDAO = await _connection.GetAsync<GroceryModelDAO>(groceryID);
                return groceryDAO;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Get all the GroceryDAO entries.
        /// </summary>
        /// <returns>Return a List of GroceryDAO objects.</returns>
        async public Task<List<GroceryModelDAO>> GetAllGroceriesAsync()
        {
            List<GroceryModelDAO> groceriesDAO = await _connection.Table<GroceryModelDAO>().ToListAsync();

            if (groceriesDAO == null)
                return new();

            return groceriesDAO;
        }

        /// <summary>
        /// Updates the given groceryDAO.
        /// </summary>
        /// <param name="grocery"></param>
        /// <returns>Returns the number of rows updated.</returns>
        async public Task<int> UpdateGroceryAsync(GroceryModelDAO grocery)
        {
            return await _connection.UpdateAsync(grocery);
        }

        /// <summary>
        /// Deletes the groceryDAO in the database
        /// with the provided ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>int, number of rows delete, -1 if an error occured.</returns>
        async public Task<int> DeleteGroceryAsync(int groceryID)
        {
            try
            {
                return await _connection.DeleteAsync<GroceryModelDAO>(groceryID);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.Message);
                return -1;
            }
        }

        public override string HeaderForCSVFile()
        {
            return "GroceryID, Name, BrandName, CarbsPer100Grams, NameOfPortion, GramsPerPortion, CarbScalar\n";
        }

        async public override Task<List<IModelDAO>> GetAllAsync()
        {
            return new(await _connection.Table<GroceryModelDAO>().ToListAsync());
        }
    }
}
