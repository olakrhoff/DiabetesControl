using System.Globalization;
using DiabetesContolApp.Models;

using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DiabetesContolApp.DAO
{
    [Table("GroceryLog")]
    public class GroceryLogModelDAO : IModelDAO
    {
        [PrimaryKey, AutoIncrement]
        public int GroceryLogID { get; set; }
        [ForeignKey(typeof(GroceryModelDAO))]
        public int GroceryID { get; set; }
        [ForeignKey(typeof(LogModelDAO))]
        public int LogID { get; set; }
        [NotNull]
        public uint NumberOfGrocery { get; set; }
        public float InsulinForGrocery { get; set; }

        public GroceryLogModelDAO()
        {
            GroceryLogID = -1;
        }

        public GroceryLogModelDAO(GroceryLogModel groceryLog)
        {
            GroceryLogID = groceryLog.GroceryLogID;
            GroceryID = groceryLog.Grocery.GroceryID;
            LogID = groceryLog.Log.LogID;
            NumberOfGrocery = groceryLog.NumberOfGrocery;
            InsulinForGrocery = groceryLog.InsulinForGrocery;
        }

        public string ToStringCSV()
        {
            return GroceryLogID + "," +
                GroceryID + "," +
                LogID + "," +
                NumberOfGrocery + "," +
                InsulinForGrocery.ToString("0.00", CultureInfo.InvariantCulture) + "\n";
        }
    }

}
