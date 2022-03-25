using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Models;

namespace DiabetesContolApp.Service
{
    public class GroceryService
    {
        public GroceryService()
        {
        }

        async public Task<List<GroceryModel>> GetGroceriesAsync()
        {
            throw new NotImplementedException();
        }

        async public Task<bool> InsertGroceryAsync(GroceryModel newGrocery)
        {
            throw new NotImplementedException();
        }

        async public Task<bool> UpdateGroceryAsync(GroceryModel grocery)
        {
            throw new NotImplementedException();
        }

        async public Task<bool> DeleteGroceryAsync(GroceryModel grocery)
        {
            throw new NotImplementedException();
        }
    }
}
