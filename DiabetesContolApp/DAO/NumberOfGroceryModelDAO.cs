using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiabetesContolApp.DAO
{
    public class NumberOfGroceryModelDAO : IEquatable<NumberOfGroceryModelDAO>, IComparable<NumberOfGroceryModelDAO>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private uint _numberOfGrocery;
        public GroceryModelDAO Grocery { get; set; }

        public NumberOfGroceryModelDAO(uint number, GroceryModelDAO grocery)
        {
            NumberOfGrocery = number;
            Grocery = grocery;
        }

        public NumberOfGroceryModelDAO(GroceryModelDAO grocery)
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


        static public List<NumberOfGroceryModelDAO> GetNumberOfGroceries(List<GroceryModelDAO> groceries)
        {
            List<NumberOfGroceryModelDAO> temp = new();

            foreach (GroceryModelDAO grocery in groceries)
                temp.Add(new NumberOfGroceryModelDAO(0, grocery));

            return temp;
        }

        public bool Equals(NumberOfGroceryModelDAO other)
        {
            return this.Grocery.Equals(other.Grocery);
        }

        public int CompareTo(NumberOfGroceryModelDAO other)
        {
            return this.Grocery.CompareTo(other.Grocery);
        }
    }
}
