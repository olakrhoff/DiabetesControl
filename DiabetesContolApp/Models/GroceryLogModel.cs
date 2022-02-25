﻿using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DiabetesContolApp.Models
{
    [Table("GroceryLog")]
    public class GroceryLogModel
    {
        [ForeignKey(typeof(GroceryModel))]
        public int GroceryID { get; set; }
        [ForeignKey(typeof(LogModel))]
        public int LogID { get; set; }
        [NotNull]
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
    }

}