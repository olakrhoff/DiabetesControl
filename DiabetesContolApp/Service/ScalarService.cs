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
        /// Gets the newest scalar of the given type,
        /// and object ID.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectID"></param>
        /// <returns>Newest ScalarModel with the given type and objectID, if not exists then null.</returns>
        async public Task<ScalarModel> GetNewestScalarForScalarObject(IScalarObject scalarObject)
        {
            List<ScalarModel> scalars = await scalarRepo.GetAllScalarsOfTypeWithObjectID(scalarObject.GetScalarType(), scalarObject.GetIDForScalarObject());

            if (scalars.Count == 0)
                return null;

            return scalars.Max();
        }
    }
}
