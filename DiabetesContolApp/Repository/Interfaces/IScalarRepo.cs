using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IScalarRepo
    {
        Task<List<ScalarModel>> GetAllScalarsAsync();
        Task<List<ScalarModel>> GetAllScalarsOfType(ScalarTypes type);
        Task<List<ScalarModel>> GetAllScalarsOfTypeWithObjectID(ScalarTypes type, int objectID);
        Task<ScalarModel> GetScalarAsync(int scalarID);
        Task<bool> InsertScalarAsync(ScalarModel newScalar);
        Task<bool> UpdateScalarAsync(ScalarModel carbScalar);
    }
}