using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiabetesContolApp.Models
{
    public class NumberOfGroceryModel : IEquatable<NumberOfGroceryModel>, IComparable<NumberOfGroceryModel>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public uint NumberOfGrocery { get; set; }
        public GroceryModel Grocery { get; set; }
        public float InsulinForGroceries { get; set; }

        public NumberOfGroceryModel(uint number, GroceryModel grocery, float insulinForGroceries)
        {
            NumberOfGrocery = number;
            Grocery = grocery;
            InsulinForGroceries = insulinForGroceries;
        }

        public NumberOfGroceryModel(GroceryLogModel groceryLog)
        {
            NumberOfGrocery = groceryLog.NumberOfGrocery;
            Grocery = groceryLog.Grocery;
            InsulinForGroceries = groceryLog.InsulinForGrocery;
        }

        /// <summary>
        /// This is used to create a dummy object for the
        /// listing of groceries in the calculator
        /// </summary>
        /// <param name="grocery"></param>
        public NumberOfGroceryModel(GroceryModel grocery)
        {
            NumberOfGrocery = 0;
            Grocery = grocery;
            InsulinForGroceries = 0.0f;
        }


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Turns a list of GroceryModels into a
        /// list of NumberOfGroceryModel
        /// </summary>
        /// <param name="groceries"></param>
        /// <returns>The list of NumberOfGroceryModels</returns>
        static public List<NumberOfGroceryModel> GetNumberOfGroceries(List<GroceryModel> groceries)
        {
            List<NumberOfGroceryModel> temp = new();

            foreach (GroceryModel grocery in groceries)
                temp.Add(new NumberOfGroceryModel(grocery));

            return temp;
        }

        public bool Equals(NumberOfGroceryModel other)
        {
            return this.Grocery.Equals(other.Grocery);
        }

        public int CompareTo(NumberOfGroceryModel other)
        {
            return this.Grocery.CompareTo(other.Grocery);
        }
    }
}
