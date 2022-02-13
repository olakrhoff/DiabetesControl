using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace DiabetesContolApp.Models
{
    public class GroceryModel
    {
        private int _groceryID { get; set; }
        public string Name { get; set; }
        public float CarbsPer100Grams { get; set; }
        public string NameOfPortion { get; set; }
        public ushort GramsPerPortion { get; set; }
        private float _carbScalar { get; set; }

        public GroceryModel()
        {
            //This indicates that the object doesn't exist in the database
            this._groceryID = -1;
        }

        public int GetID()
        {
            return this._groceryID;
        }


    }
}
