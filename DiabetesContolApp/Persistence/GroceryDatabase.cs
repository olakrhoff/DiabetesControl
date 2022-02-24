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

        async internal void InsertGroceryAsync(GroceryModel newGrocery)
        {
            await connection.InsertAsync(newGrocery);
        }

        async internal Task<GroceryModel> GetGroceryAsync(int groceryID)
        {
            return await connection.GetAsync<GroceryModel>(groceryID);
        }
    }
}
