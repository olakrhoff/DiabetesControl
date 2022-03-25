using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Persistence
{
    public class AverageTDDDatabase : ModelDatabaseAbstract
    {
        private static AverageTDDDatabase instance = null;

        public AverageTDDDatabase()
        {
        }

        public static AverageTDDDatabase GetInstance()
        {
            return instance == null ? new AverageTDDDatabase() : instance;
        }

        async public override Task<List<IModel>> GetAllAsync()
        {
            return new(await connection.Table<AverageTDDModel>().ToListAsync());
        }

        public override string HeaderForCSVFile()
        {
            return "AverageTDDID, DateTimeValue, AverageTDDValue\n";
        }

        /// <summary>
        /// Adds a new Average TDD value to the table
        /// </summary>
        /// <param name="averageTDD"></param>
        /// <returns>int, the number of rows added</returns>
        async public Task<int> InsertAverageTDD(AverageTDDModel averageTDD)
        {
            return await connection.InsertAsync(averageTDD);
        }
    }
}
