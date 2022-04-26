using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.Models;
using DiabetesContolApp.Repository;
using DiabetesContolApp.Repository.Interfaces;
using DiabetesContolApp.Service.Interfaces;

namespace DiabetesContolApp.Service
{
    public class ScalarService : IScalarService
    {
        private readonly IScalarRepo _scalarRepo;

        public ScalarService(IScalarRepo scalarRepo)
        {
            _scalarRepo = scalarRepo;
        }

        public static ScalarService GetScalarService()
        {
            return new(ScalarRepo.GetScalarRepo());
        }

        /// <summary>
        /// Gets the newest scalar of the type and objectID.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectID"></param>
        /// <returns>Newest ScalarModel with given type and objectID, might be null</returns>
        async public Task<ScalarModel> GetNewestScalarForTypeWithObjectIDAsync(ScalarTypes type, int objectID, DateTime oldestOfObject)
        {
            List<ScalarModel> scalarsWithType = await _scalarRepo.GetAllScalarsOfTypeAsync(type);

            if (type != ScalarTypes.CORRECTION_INSULIN && objectID >= 0) //Edge case, correction insulin does not have an object represenation in the database, hens no objectID
                scalarsWithType = scalarsWithType.FindAll(scalar => scalar.ScalarObjectID == objectID); //Filter out only the ones with the correct objectID

            if (scalarsWithType.Count > 0)
            {
                ScalarModel currentMax = scalarsWithType[0];
                foreach (ScalarModel scalar in scalarsWithType)
                    if (scalar.DateTimeCreated.CompareTo(currentMax.DateTimeCreated) >= 0)
                        currentMax = scalar;
                return currentMax;
            }
            //If there wasn't a Scalar with these spesifications
            //then we need to create one
            ScalarModel newScalar = new(-1, type, objectID, 1.0f, oldestOfObject.AddDays(-1));
            int idOfnewScalar = await InsertScalarAsync(newScalar);

            return await GetScalarAsync(idOfnewScalar);
        }

        /// <summary>
        /// Inserts the new Scalar.
        /// </summary>
        /// <param name="newScalar"></param>
        /// <returns>int, the ID of the new Scalar, -1 if an error occured.</returns>
        async public Task<int> InsertScalarAsync(ScalarModel newScalar)
        {
            if (!await _scalarRepo.InsertScalarAsync(newScalar))
                return -1;
            ScalarModel newlyInsertedScalar = await GetNewestScalarAsync();

            if (newlyInsertedScalar == null)
                return -1;
            return newlyInsertedScalar.ScalarID;
        }

        /// <summary>
        /// Gets the newest Scalar by
        /// Scalar ID.
        /// </summary>
        /// <returns>ScalarModel with highest ID, might be null.</returns>
        async private Task<ScalarModel> GetNewestScalarAsync()
        {
            List<ScalarModel> scalars = await _scalarRepo.GetAllScalarsAsync();

            if (scalars.Count == 0)
                return null;

            return await GetScalarAsync(scalars.Max(scalar => scalar.ScalarID));
        }

        /// <summary>
        /// Gets the Scalar with the given ID.
        /// </summary>
        /// <param name="scalarID"></param>
        /// <returns>ScalarModel with given ID, might be null.</returns>
        async public Task<ScalarModel> GetScalarAsync(int scalarID)
        {
            return await _scalarRepo.GetScalarAsync(scalarID);
        }

        /// <summary>
        /// Updates the given scalar.
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns>True if updated, else false</returns>
        async public Task<bool> UpdateScalarAsync(ScalarModel scalar)
        {
            return await _scalarRepo.UpdateScalarAsync(scalar);
        }
    }
}
