using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IScalarRepo
    {
        /// <summary>
        /// Gets all scalarDAOs, then converts them
        /// to ScalarModels.
        /// </summary>
        /// <returns>List of ScalarModels, might be empty.</returns>
        Task<List<ScalarModel>> GetAllScalarsAsync();

        /// <summary>
        /// Gets all ScalarDAOs with given type and converts
        /// them into ScalarModels.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>List of ScalarModels, might be empty.</returns>
        Task<List<ScalarModel>> GetAllScalarsOfTypeAsync(ScalarTypes type);

        /// <summary>
        /// Gets all the ScalarDAO of the given type,
        /// and object ID, then converts them to ScalarModels.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectID"></param>
        /// <returns>List of ScalarModels with the given type and objectID, if might be empty.</returns>
        Task<List<ScalarModel>> GetAllScalarsOfTypeWithObjectID(ScalarTypes type, int objectID);

        /// <summary>
        /// Gets the ScalarDAO with the given ID,
        /// then converts it into a ScalarModel.
        /// </summary>
        /// <param name="scalarID"></param>
        /// <returns>ScalarModel with given ID, might be null.</returns>
        Task<ScalarModel> GetScalarAsync(int scalarID);

        /// <summary>
        /// Converts Scalar to DAO and inserts new Scalar into the database.
        /// </summary>
        /// <param name="newScalar"></param>
        /// <returns>True if inserted, else false</returns>
        Task<bool> InsertScalarAsync(ScalarModel newScalar);

        /// <summary>
        /// Converts the Scalar into a DAO, and
        /// updates the DAO in the database.
        /// </summary>
        /// <param name="carbScalar"></param>
        /// <returns>True if updated, else false.</returns>
        Task<bool> UpdateScalarAsync(ScalarModel carbScalar);
    }
}