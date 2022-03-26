using System;
using System.Collections.Generic;

using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Models
{

    public class GroceryLogModel : IModel
    {

        public int GroceryLogID { get; set; }
        public GroceryModel Grocery { get; set; }
        public LogModel Log { get; set; }
        public uint NumberOfGrocery { get; set; }
        public float InsulinForGrocery { get; set; }



        public GroceryLogModel()
        {
            GroceryLogID = -1;
        }

        public GroceryLogModel(GroceryLogModelDAO groceryLogDAO)
        {
            GroceryLogID = groceryLogDAO.GroceryLogID;
            Grocery = new(groceryLogDAO.GroceryID);
            Log = new(groceryLogDAO.LogID);
            NumberOfGrocery = groceryLogDAO.NumberOfGrocery;
            InsulinForGrocery = groceryLogDAO.InsulinForGrocery;
        }


        public GroceryLogModel(GroceryModel grocery, LogModel log, uint numberOfGrocery, float insulinForGrocery)
        {
            GroceryLogID = -1;
            Grocery = grocery;
            Log = log;
            NumberOfGrocery = numberOfGrocery;
            InsulinForGrocery = insulinForGrocery;
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
                Grocery.GroceryID + "," +
                Log.LogID + "," +
                NumberOfGrocery + "\n";
        }
    }

}
