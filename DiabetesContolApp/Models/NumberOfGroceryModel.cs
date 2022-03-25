using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DiabetesContolApp.Models
{
    public class NumberOfGroceryModel : IEquatable<NumberOfGroceryModel>, IComparable<NumberOfGroceryModel>//, INotifyPropertyChanged
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

        public NumberOfGroceryModel(GroceryModel grocery)
        {
            NumberOfGrocery = 0;
            Grocery = grocery;
            InsulinForGroceries = 0.0f;
            throw new NotImplementedException("This constructor should probalbly not exist");
        }

        /*
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/


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
