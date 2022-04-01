using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using DiabetesContolApp.DAO;
using System.Diagnostics;

namespace DiabetesContolApp.Persistence
{
    public class ScalarDatabase : ModelDatabaseAbstract
    {
        private static ScalarDatabase instance = null;

        public ScalarDatabase()
        {
        }

        public static ScalarDatabase GetInstance()
        {
            return instance == null ? new ScalarDatabase() : instance;
        }

        /// <summary>
        /// Gets all the ScalarDAO with
        /// the given type and ID.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectID"></param>
        /// <returns>List of ScalarDAOs, might be empty</returns>
        async public Task<List<ScalarModelDAO>> GetAllScalarsOfTypeWithObjectID(int type, int objectID)
        {
            try
            {
                List<ScalarModelDAO> scalarDAOs = await connection.Table<ScalarModelDAO>().Where(scalarDAO => scalarDAO.TypeOfScalar == type && scalarDAO.IDOfObject == objectID).ToListAsync();
                return scalarDAOs;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return new();
            }
        }

        public override Task<List<IModelDAO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public override string HeaderForCSVFile()
        {
            throw new NotImplementedException();
        }


    }
}
