using System;
using System.Collections.Generic;
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
    }
}
