using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;

namespace DiabetesContolApp.Service
{
    public class ScalarService
    {
        private ScalarRepo scalarRepo = new();


        /// <summary>
        /// Gets the newest scalar of the given an IScalarObject.
        /// </summary>
        /// <param name="scalarObject"></param>
        /// <returns>Newest ScalarModel with the given type and objectID, if not exists then null.</returns>
        async public Task<ScalarModel> GetNewestScalarForScalarObjectAsync(IScalarObject scalarObject)
        {
            List<ScalarModel> scalars = await scalarRepo.GetAllScalarsOfTypeWithObjectID(scalarObject.GetScalarType(), scalarObject.GetIDForScalarObject());

            if (scalars.Count == 0)
                return null;

            return scalars.Max();
        }

        /// <summary>
        /// Updates the given scalar.
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns>True if updated, else false</returns>
        async public Task<bool> UpdateScalarAsync(ScalarModel scalar)
        {
            return await scalarRepo.UpdateScalarAsync(scalar);
        }

        /// <summary>
        /// Get the newest Scalar of the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Newest ScalarModel, null if not found.</returns>
        async public Task<ScalarModel> GetNewestScalarOfScalarType(ScalarTypes type)
        {
            List<ScalarModel> scalarsWithType = await scalarRepo.GetAllScalarsOfType(type);
            if (scalarsWithType.Count == 0)
                return null;

            return scalarsWithType.Max();
        }
    }
}
