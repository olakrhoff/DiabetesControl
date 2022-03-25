﻿using System;
using System.Collections.Generic;

using SQLiteNetExtensions.Attributes;

namespace DiabetesContolApp.Models
{

    public class GroceryLogModel : IModel
    {

        public int GroceryLogID { get; set; }

        public int GroceryID { get; set; }

        public int LogID { get; set; }

        public uint NumberOfGrocery { get; set; }

        public GroceryLogModel()
        {
        }

        public GroceryLogModel(int groceryID, int logID, uint numberOfGrocery)
        {
            GroceryID = groceryID;
            LogID = logID;
            NumberOfGrocery = numberOfGrocery;
        }

        static public List<GroceryLogModel> GetGroceryLogs(List<NumberOfGroceryModel> numberOfGroceries, int logID)
        {
            List<GroceryLogModel> groceryLogs = new();

            if (numberOfGroceries != null)
                foreach (NumberOfGroceryModel numberOfGrocery in numberOfGroceries)
                    groceryLogs.Add(new(numberOfGrocery.Grocery.GroceryID, logID, numberOfGrocery.NumberOfGrocery));

            return groceryLogs;
        }

        static public List<NumberOfGroceryModel> GetNumberOfGroceries(List<GroceryLogModel> groceryLogs)
        {
            List<NumberOfGroceryModel> numberOfGroceries = new();

            foreach (GroceryLogModel groceryLog in groceryLogs)
            {
                var tempNumberOfGrocery = new NumberOfGroceryModel(groceryLog.NumberOfGrocery, new GroceryModel());
                tempNumberOfGrocery.Grocery.GroceryID = groceryLog.GroceryID;
                numberOfGroceries.Add(tempNumberOfGrocery);
            }

            return numberOfGroceries;
        }

        public string ToStringCSV()
        {
            return GroceryLogID + ", " +
                GroceryID + ", " +
                LogID + ", " +
                NumberOfGrocery + "\n";
        }
    }

}
