using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;

namespace DiabetesContolApp.Service
{
    /// <summary>
    /// This is the Service class for GroceryLogs. It is responsible for
    /// Assembeling and disassembling GroceryLogModel objects and make the
    /// appropriate calls to the respective repositories.
    /// </summary>
    public class GroceryLogService
    {
        private GroceryLogRepo groceryLogRepo = new();
        private GroceryRepo groceryRepo = new();

        public GroceryLogService()
        {
        }


        /// <summary>
        /// Gets all GroceryLogs with the given log ID
        /// Then converts them to NumberOfGroeryModels and
        /// fills them out in with GroceryModels.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>List of NumberOfGroceryModel with GroceryModels</returns>
        async public Task<List<NumberOfGroceryModel>> GetAllGroceryLogsAsNumberOfGroceryWithLogID(int logID)
        {
            //List<NumberOfGroceryModel> numberOfGroceries = await groceryLogRepo.GetAllGroceryLogsWithLogID(logID);
            List<GroceryLogModel> groceryLogs = await groceryLogRepo.GetAllGroceryLogsWithLogID(logID);

            List<NumberOfGroceryModel> numberOfGroceries = new();

            foreach (GroceryLogModel groceryLog in groceryLogs) //Convert all GroceryLogs to NumberOfGroceries
                numberOfGroceries.Add(new(groceryLog));

            //TODO: If grocery is not found the Log might be corrupt, consider deleting the log in this case.
            foreach (NumberOfGroceryModel numberOfGrocery in numberOfGroceries)
                numberOfGrocery.Grocery = await groceryRepo.GetGroceryAsync(numberOfGrocery.Grocery.GroceryID);

            return numberOfGroceries;
        }
    }
}
