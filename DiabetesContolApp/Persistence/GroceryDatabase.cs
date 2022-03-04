using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

using SQLite;
using Xamarin.Forms;

namespace DiabetesContolApp.Persistence
{
    public class GroceryDatabase : ModelDatabaseAbstract
    {
        private static GroceryDatabase instance = null;

        public GroceryDatabase()
        {
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

        /*
         * This method deletes a grocery by the object itself.
         * First it deletes all GroceryLog entries that it is connected to,
         * aswell as all the Log entries that have it as a grocery.
         * This is to ensure the integrety of the data.
         * 
         * Paramas: GroceryModel (grocey), the grocery that is to be deleted
         * Return: int, the number of rows deleted
         */
        async internal Task<int> DeleteGroceryAsync(GroceryModel grocery)
        {
            LogDatabase logDatabase = LogDatabase.GetInstance();

            List<GroceryLogModel> groceryLogs = await connection.Table<GroceryLogModel>().ToListAsync();

            foreach (GroceryLogModel groceryLog in groceryLogs)
                if (groceryLog.GroceryID == grocery.GroceryID)
                {
                    await connection.DeleteAsync(groceryLog); //Deletes all the entries in GroceryLog who are connected to the Grocery
                    await logDatabase.DeleteLogAsync(groceryLog.LogID); //Delete the log awell
                }

            return await connection.DeleteAsync(grocery);
        }
    }
}
