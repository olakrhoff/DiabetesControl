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
        /// Then fills out the GroceryModels in them
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>List of NumberOfGroceryModel with GroceryModels</returns>
        async public Task<List<NumberOfGroceryModel>> GetAllWithLogID(int logID)
        {
            List<NumberOfGroceryModel> numberOfGroceries = await groceryLogRepo.GetAllWithLogID(logID);

            //TODO: If grocery is not found the Log might be corrupt, consider deleting the log in this case.
            foreach (NumberOfGroceryModel numberOfGrocery in numberOfGroceries)
                numberOfGrocery.Grocery = await groceryRepo.GetAsync(numberOfGrocery.Grocery.GroceryID);

            return numberOfGroceries;
        }
    }
}
