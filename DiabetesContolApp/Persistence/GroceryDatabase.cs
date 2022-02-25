using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

using SQLite;
using Xamarin.Forms;

namespace DiabetesContolApp.Persistence
{
    public class GroceryDatabase
    {
        private readonly SQLiteAsyncConnection connection;

        private static GroceryDatabase instance = null;

        public GroceryDatabase()
        {
            connection = DependencyService.Get<ISQLiteDB>().GetConnection();

            connection.CreateTableAsync<LogModel>().Wait();
            connection.CreateTableAsync<GroceryModel>().Wait();
            connection.CreateTableAsync<DayProfileModel>().Wait();
            connection.CreateTableAsync<GroceryLogModel>().Wait();
        }

        public static GroceryDatabase GetInstance()
        {
            return instance == null ? new GroceryDatabase() : instance;
        }

        async internal Task<int> InsertGroceryAsync(GroceryModel newGrocery)
        {
            return await connection.InsertAsync(newGrocery);
        }

        async internal Task<GroceryModel> GetGroceryAsync(int groceryID)
        {
            return await connection.GetAsync<GroceryModel>(groceryID);
        }

        async internal Task<List<GroceryModel>> GetGroceriesAsync()
        {
            return await connection.Table<GroceryModel>().ToListAsync();
        }

        async internal Task<int> UpdateGroceryAsync(GroceryModel grocery)
        {
            return await connection.UpdateAsync(grocery);
        }

        async internal Task<int> DeleteGroceryAsync(GroceryModel grocery)
        {

            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().ToListAsync();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                if (groceryLog.GroceryID == grocery.GroceryID)
                    await connection.DeleteAsync(groceryLog); //Deletes all the entries in GroceryLog who are connected to the Grocery

            return await connection.DeleteAsync(grocery);
        }
    }
}
