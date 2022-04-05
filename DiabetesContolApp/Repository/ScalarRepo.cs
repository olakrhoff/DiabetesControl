using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;

namespace DiabetesContolApp.Repository
{
    public class ScalarRepo
    {
        ScalarDatabase scalarDatabase = ScalarDatabase.GetInstance();

        /// <summary>
        /// Gets all the ScalarDAO of the given type,
        /// and object ID, then converts them to ScalarModels.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectID"></param>
        /// <returns>List of ScalarModels with the given type and objectID, if might be empty.</returns>
        async public Task<List<ScalarModel>> GetAllScalarsOfTypeWithObjectID(ScalarTypes type, int objectID)
        {
            List<ScalarModelDAO> scalarDAOs = await scalarDatabase.GetAllScalarsOfTypeWithObjectID((int)type, objectID);

            List<ScalarModel> scalars = new();

            foreach (ScalarModelDAO scalarDAO in scalarDAOs)
                scalars.Add(new(scalarDAO));

            scalars = scalars.FindAll(scalar => scalar != null);

            return scalars;
        }

        /// <summary>
        /// Converts the Scalar into a DAO, and
        /// updates the DAO in the database.
        /// </summary>
        /// <param name="carbScalar"></param>
        /// <returns>True if updated, else false.</returns>
        async public Task<bool> UpdateScalarAsync(ScalarModel carbScalar)
        {
            ScalarModelDAO scalarDAO = new(carbScalar);

            return await scalarDatabase.UpdateScalarAsync(scalarDAO) > 0;
        }

        /// <summary>
        /// Gets all ScalarDAOs with given type and converts
        /// them into ScalarModels.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>List of ScalarModels, might be empty.</returns>
        async public Task<List<ScalarModel>> GetAllScalarsOfType(ScalarTypes type)
        {
            List<ScalarModelDAO> scalarDAOs = await scalarDatabase.GetAllScalarsOfType((int)type);

            List<ScalarModel> scalarsOfType = new();

            foreach (ScalarModelDAO scalarDAO in scalarDAOs)
                scalarsOfType.Add(new(scalarDAO));

            scalarsOfType = scalarsOfType.FindAll(scalar => scalar != null); //Filter out corrupt data

            return scalarsOfType;
        }
    }
}
