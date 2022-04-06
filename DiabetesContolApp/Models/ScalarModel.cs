using System;

using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Models
{
    public class ScalarModel : IModel, IComparable<ScalarModel>
    {
        public int ScalarID { get; set; }
        public ScalarTypes TypeOfScalar { get; set; }
        public int ScalarObjectID { get; set; }
        public float ScalarValue { get; set; }
        public DateTime DateTimeCreated { get; set; }

        public ScalarModel(int scalarID, ScalarTypes scalarType, int scalarObjectID, float scalarValue, DateTime dateTimeCreated)
        {
            ScalarID = scalarID;
            TypeOfScalar = scalarType;
            ScalarObjectID = scalarObjectID;
            ScalarValue = scalarValue;
            DateTimeCreated = dateTimeCreated;
        }

        public ScalarModel(ScalarModelDAO scalarDAO)
        {
            ScalarID = scalarDAO.ScalarID;
            TypeOfScalar = (ScalarTypes)scalarDAO.TypeOfScalar;
            ScalarObjectID = scalarDAO.ScalarObjectID;
            ScalarValue = scalarDAO.ScalarValue;
            DateTimeCreated = scalarDAO.DateTimeCreated;
        }

        public int CompareTo(ScalarModel other)
        {
            return this.DateTimeCreated.CompareTo(other.DateTimeCreated);
        }

        public string ToStringCSV()
        {
            throw new NotImplementedException();
        }
    }

    public enum ScalarTypes
    {
        GROCERY = 0,
        DAY_PROFILE_CARB = 1,
        DAY_PROFILE_GLUCOSE = 2,
        CORRECTION_INSULIN = 3
    }
}
