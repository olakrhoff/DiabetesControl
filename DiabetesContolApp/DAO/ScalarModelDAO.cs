using System;

using DiabetesContolApp.Models;

using SQLite;

namespace DiabetesContolApp.DAO
{
    /// <summary>
    /// This class is used to represent an entry in the database
    /// for scalars to keep track of the scalar history.
    /// </summary>
    [Table("Scalar")]
    public class ScalarModelDAO
    {
        [PrimaryKey, AutoIncrement]
        public int ScalarID { get; set; }
        [NotNull]
        public int TypeOfScalar { get; set; }
        [NotNull]
        public int ScalarObjectID { get; set; }
        [NotNull]
        public float ScalarValue { get; set; }
        [NotNull]
        public DateTime DateTimeCreated { get; set; }

        public ScalarModelDAO()
        {
            ScalarID = -1;
            TypeOfScalar = -1;
            ScalarObjectID = -1;
            ScalarValue = -1.0f;
            DateTimeCreated = DateTime.Now;
        }

        public ScalarModelDAO(ScalarModel scalar)
        {
            ScalarID = scalar.ScalarID;
            TypeOfScalar = (int)scalar.TypeOfScalar;
            ScalarObjectID = scalar.ScalarObjectID;
            ScalarValue = scalar.ScalarValue;
            DateTimeCreated = scalar.DateTimeCreated;
        }

        public ScalarModelDAO(int scalarID, int typeOfScalar, int iDOfType, float scalarValue, DateTime dateTimeCreated)
        {
            ScalarID = scalarID;
            TypeOfScalar = typeOfScalar;
            ScalarObjectID = (int)iDOfType;
            ScalarValue = scalarValue;
            DateTimeCreated = dateTimeCreated;
        }
    }
}
