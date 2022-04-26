using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Persistence.Interfaces
{
    public interface IScalarDatabase
    {
        /// <summary>
        /// Gets all ScalarDAOs from the database.
        /// </summary> 
        /// <returns>List of ScalarDAOs, might be empty.</returns>
        Task<List<ScalarModelDAO>> GetAllScalarsAsync();

        /// <summary>
        /// Get all DAOs with given type
        /// from the database.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>List og ScalarDAOs with given type.</returns>
        Task<List<ScalarModelDAO>> GetAllScalarsOfTypeAsync(int type);

        /// <summary>
        /// Gets all the ScalarDAO with
        /// the given type and ID.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectID"></param>
        /// <returns>List of ScalarDAOs, might be empty</returns>
        Task<List<ScalarModelDAO>> GetAllScalarsOfTypeWithObjectIDAsync(int type, int objectID);

        /// <summary>
        /// Gets the ScalarDAO with the given ID.
        /// </summary>
        /// <param name="scalarID"></param>
        /// <returns>ScalarDAO, might be null</returns>
        Task<ScalarModelDAO> GetScalarAsync(int scalarID);

        /// <summary>
        /// Inserts the new ScalarDAO into the database.
        /// </summary>
        /// <param name="newScalarDAO"></param>
        /// <returns>int, number of rows added. -1 if an error occured.</returns>
        Task<int> InsertScalarAsync(ScalarModelDAO newScalarDAO);

        /// <summary>
        /// Updates the ScalarDAO in the database.
        /// </summary>
        /// <param name="scalarDAO"></param>
        /// <returns>int, number of rows updated. -1 if an error occured.</returns>
        Task<int> UpdateScalarAsync(ScalarModelDAO scalarDAO);
    }
}