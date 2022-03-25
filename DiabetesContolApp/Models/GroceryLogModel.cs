using System;
using System.Collections.Generic;

using SQLiteNetExtensions.Attributes;

namespace DiabetesContolApp.Models
{

    public class GroceryLogModel : IModel
    {

        public int GroceryLogID { get; set; }
        public NumberOfGroceryModel NumberOfGrocery { get; set; }
        public LogModel Log { get; set; }


        public GroceryLogModel()
        {
        }

        public GroceryLogModel(NumberOfGroceryModel numberOfGrocery, LogModel log)
        {
            GroceryLogID = -1;
            NumberOfGrocery = numberOfGrocery;
            Log = log;
        }

        static public List<GroceryLogModel> GetGroceryLogs(List<NumberOfGroceryModel> numberOfGroceries, int logID)
        {
            List<GroceryLogModel> groceryLogs = new();

            if (numberOfGroceries != null)
                foreach (NumberOfGroceryModel numberOfGrocery in numberOfGroceries)
                    //groceryLogs.Add(new(numberOfGrocery.Grocery.GroceryID, logID, numberOfGrocery.NumberOfGrocery));
                    throw new NotImplementedException("This seems like a bad way of doing things");

            return groceryLogs;
        }

        static public List<NumberOfGroceryModel> GetNumberOfGroceries(List<GroceryLogModel> groceryLogs)
        {
            List<NumberOfGroceryModel> numberOfGroceries = new();

            foreach (GroceryLogModel groceryLog in groceryLogs)
            {
                throw new NotImplementedException("This seems like a bad way of doing things");
                //var tempNumberOfGrocery = new NumberOfGroceryModel(groceryLog.NumberOfGrocery, new GroceryModel());
                //tempNumberOfGrocery.Grocery.GroceryID = groceryLog.GroceryID;
                //numberOfGroceries.Add(tempNumberOfGrocery);
            }

            return numberOfGroceries;
        }

        public string ToStringCSV()
        {
            return GroceryLogID + "," +
                NumberOfGrocery.Grocery.GroceryID + "," +
                Log.LogID + "," +
                NumberOfGrocery.NumberOfGrocery + "\n";
        }
    }

}
