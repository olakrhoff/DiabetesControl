using System;
using System.Collections.Generic;

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

        public GroceryLogModelDAO(int groceryID, int logID, uint numberOfGrocery, float insulinForGrocery)
        {
            GroceryLogID = -1;
            GroceryID = groceryID;
            LogID = logID;
            NumberOfGrocery = numberOfGrocery;
            InsulinForGrocery = insulinForGrocery;
        }
        /*
        static public List<GroceryLogModelDAO> GetGroceryLogs(List<NumberOfGroceryModelDAO> numberOfGroceries, int logID)
        {
            List<GroceryLogModelDAO> groceryLogs = new();

            if (numberOfGroceries != null)
                foreach (NumberOfGroceryModelDAO numberOfGrocery in numberOfGroceries)
                    groceryLogs.Add(new(numberOfGrocery.Grocery.GroceryID, logID, numberOfGrocery.NumberOfGrocery));

            return groceryLogs;
        }*/
        /*
        static public List<NumberOfGroceryModelDAO> GetNumberOfGroceries(List<GroceryLogModelDAO> groceryLogs)
        {
            List<NumberOfGroceryModelDAO> numberOfGroceries = new();

            foreach (GroceryLogModelDAO groceryLog in groceryLogs)
            {
                var tempNumberOfGrocery = new NumberOfGroceryModelDAO(groceryLog.NumberOfGrocery, new GroceryModelDAO());
                tempNumberOfGrocery.Grocery.GroceryID = groceryLog.GroceryID;
                numberOfGroceries.Add(tempNumberOfGrocery);
            }

            return numberOfGroceries;
        }*/

        public string ToStringCSV()
        {
            return GroceryLogID + "," +
                GroceryID + "," +
                LogID + "," +
                NumberOfGrocery + "\n";
        }
    }

}
