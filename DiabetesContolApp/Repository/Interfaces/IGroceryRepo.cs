using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IGroceryRepo
    {
        Task<GroceryModel> GetGroceryAsync(int groceryID);
        Task<List<GroceryModel>> GetAllGroceriesAsync();
        Task<bool> InsertGroceryAsync(GroceryModel newGrocery);
        Task<bool> UpdateGroceryAsync(GroceryModel grocery);
        Task<bool> DeleteGroceryAsync(int groceryID);
    }
}
