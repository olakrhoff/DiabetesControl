using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;
using DiabetesContolApp.Repository.Interfaces;

namespace DiabetesContolApp.Service
{
    /// <summary>
    /// This is the Service class for GroceryLogs. It is responsible for
    /// Assembeling and disassembling GroceryLogModel objects and make the
    /// appropriate calls to the respective repositories.
    /// </summary>
    public class GroceryLogService
    {
        private readonly IGroceryLogRepo _groceryLogRepo;
        private readonly IGroceryRepo _groceryRepo;

        public GroceryLogService(IGroceryLogRepo groceryLogRepo, IGroceryRepo groceryRepo)
        {
            _groceryLogRepo = groceryLogRepo;
            _groceryRepo = groceryRepo;
        }

        public static GroceryLogService GetGroceryLogService()
        {
            return new GroceryLogService(new GroceryLogRepo(), new GroceryRepo());
        }


        /// <summary>
        /// Gets all GroceryLogs with the given log ID
        /// Then converts them to NumberOfGroeryModels and
        /// fills them out in with GroceryModels.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>List of NumberOfGroceryModel with GroceryModels, might be empty.</returns>
        async public Task<List<NumberOfGroceryModel>> GetAllGroceryLogsAsNumberOfGroceryWithLogID(int logID)
        {
            //List<NumberOfGroceryModel> numberOfGroceries = await groceryLogRepo.GetAllGroceryLogsWithLogID(logID);
            List<GroceryLogModel> groceryLogs = await _groceryLogRepo.GetAllGroceryLogsWithLogID(logID);

            List<NumberOfGroceryModel> numberOfGroceries = new();

            foreach (GroceryLogModel groceryLog in groceryLogs) //Convert all GroceryLogs to NumberOfGroceries
                numberOfGroceries.Add(new(groceryLog));

            //TODO: If grocery is not found the Log might be corrupt, consider deleting the log in this case.
            foreach (NumberOfGroceryModel numberOfGrocery in numberOfGroceries)
                numberOfGrocery.Grocery = await _groceryRepo.GetGroceryAsync(numberOfGrocery.Grocery.GroceryID);

            return numberOfGroceries;
        }
    }
}
