using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Models;
using DiabetesContolApp.DAO;
using DiabetesContolApp.Persistence;
using DiabetesContolApp.Persistence.Interfaces;
using DiabetesContolApp.Repository.Interfaces;

namespace DiabetesContolApp.Repository
{
    public class ScalarRepo : IScalarRepo
    {
        private readonly IScalarDatabase _scalarDatabase;

        public ScalarRepo(IScalarDatabase scalarDatabase)
        {
            _scalarDatabase = scalarDatabase;
        }

        public static ScalarRepo GetScalarRepo()
        {
            return new ScalarRepo(ScalarDatabase.GetInstance());
        }

        /// <summary>
        /// Gets all the ScalarDAO of the given type,
        /// and object ID, then converts them to ScalarModels.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectID"></param>
        /// <returns>List of ScalarModels with the given type and objectID, if might be empty.</returns>
        async public Task<List<ScalarModel>> GetAllScalarsOfTypeWithObjectID(ScalarTypes type, int objectID)
        {
            List<ScalarModelDAO> scalarDAOs = await _scalarDatabase.GetAllScalarsOfTypeWithObjectIDAsync((int)type, objectID);

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

            return await _scalarDatabase.UpdateScalarAsync(scalarDAO) > 0;
        }

        /// <summary>
        /// Gets all ScalarDAOs with given type and converts
        /// them into ScalarModels.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>List of ScalarModels, might be empty.</returns>
        async public Task<List<ScalarModel>> GetAllScalarsOfTypeAsync(ScalarTypes type)
        {
            List<ScalarModelDAO> scalarDAOs = await _scalarDatabase.GetAllScalarsOfTypeAsync((int)type);

            List<ScalarModel> scalarsOfType = new();

            foreach (ScalarModelDAO scalarDAO in scalarDAOs)
                scalarsOfType.Add(new(scalarDAO));

            scalarsOfType = scalarsOfType.FindAll(scalar => scalar != null); //Filter out corrupt data

            return scalarsOfType;
        }

        /// <summary>
        /// Converts Scalar to DAO and inserts new Scalar into the database.
        /// </summary>
        /// <param name="newScalar"></param>
        /// <returns>True if inserted, else false</returns>
        async public Task<bool> InsertScalarAsync(ScalarModel newScalar)
        {
            ScalarModelDAO newScalarDAO = new(newScalar);

            return await _scalarDatabase.InsertScalarAsync(newScalarDAO) > 0;
        }

        /// <summary>
        /// Gets the ScalarDAO with the given ID,
        /// then converts it into a ScalarModel.
        /// </summary>
        /// <param name="scalarID"></param>
        /// <returns>ScalarModel with given ID, might be null.</returns>
        async public Task<ScalarModel> GetScalarAsync(int scalarID)
        {
            ScalarModelDAO scalarDAO = await _scalarDatabase.GetScalarAsync(scalarID);

            if (scalarDAO == null)
                return null;

            ScalarModel scalar = new(scalarDAO);

            return scalar;
        }

        /// <summary>
        /// Gets all scalarDAOs, then converts them
        /// to ScalarModels.
        /// </summary>
        /// <returns>List of ScalarModels, might be empty.</returns>
        async public Task<List<ScalarModel>> GetAllScalarsAsync()
        {
            List<ScalarModelDAO> scalarDAOs = await _scalarDatabase.GetAllScalarsAsync();

            List<ScalarModel> scalars = new();

            foreach (ScalarModelDAO scalarDAO in scalarDAOs)
                scalars.Add(new(scalarDAO));

            return scalars;
        }
    }
}
