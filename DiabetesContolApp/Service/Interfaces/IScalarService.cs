using System;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Service.Interfaces
{
    public interface IScalarService
    {
        /// <summary>
        /// Gets the newest scalar of the type and objectID.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectID"></param>
        /// <returns>Newest ScalarModel with given type and objectID, might be null</returns>
        Task<ScalarModel> GetNewestScalarForTypeWithObjectIDAsync(ScalarTypes type, int objectID, DateTime oldestOfObject);

        /// <summary>
        /// Gets the Scalar with the given ID.
        /// </summary>
        /// <param name="scalarID"></param>
        /// <returns>ScalarModel with given ID, might be null.</returns>
        Task<ScalarModel> GetScalarAsync(int scalarID);

        /// <summary>
        /// Inserts the new Scalar.
        /// </summary>
        /// <param name="newScalar"></param>
        /// <returns>int, the ID of the new Scalar, -1 if an error occured.</returns>
        Task<int> InsertScalarAsync(ScalarModel newScalar);

        /// <summary>
        /// Updates the given scalar.
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns>True if updated, else false</returns>
        Task<bool> UpdateScalarAsync(ScalarModel scalar);
    }
}