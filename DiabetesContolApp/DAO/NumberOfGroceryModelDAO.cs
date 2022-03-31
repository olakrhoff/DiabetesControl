using System;

namespace DiabetesContolApp.DAO
{
    public class NumberOfGroceryModelDAO : IEquatable<NumberOfGroceryModelDAO>, IComparable<NumberOfGroceryModelDAO>
    {
        private uint _numberOfGrocery;
        public GroceryModelDAO Grocery { get; set; }

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
            }
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
