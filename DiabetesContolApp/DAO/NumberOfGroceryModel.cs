using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiabetesContolApp.DAO
{
    public class NumberOfGroceryModel : IEquatable<NumberOfGroceryModel>, IComparable<NumberOfGroceryModel>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private uint _numberOfGrocery;
        public GroceryModel Grocery { get; set; }

        public NumberOfGroceryModel(uint number, GroceryModel grocery)
        {
            NumberOfGrocery = number;
            Grocery = grocery;
        }

        public NumberOfGroceryModel(GroceryModel grocery)
        {
            NumberOfGrocery = 0;
            Grocery = grocery;
        }

        public uint NumberOfGrocery
        {
            get
            {
                return this._numberOfGrocery;
            }

            set
            {
                this._numberOfGrocery = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        static public List<NumberOfGroceryModel> GetNumberOfGroceries(List<GroceryModel> groceries)
        {
            List<NumberOfGroceryModel> temp = new();

            foreach (GroceryModel grocery in groceries)
                temp.Add(new NumberOfGroceryModel(0, grocery));

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
