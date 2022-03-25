using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DiabetesContolApp.DAO
{
    [Table("Grocery")]
    public class GroceryModel : INotifyPropertyChanged, IEquatable<GroceryModel>, IComparable<GroceryModel>, IModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int GroceryID { get; set; }

        private string _name;
        private string _brandName;

        [NotNull]
        public float CarbsPer100Grams { get; set; }
        [NotNull]
        public string NameOfPortion { get; set; }
        [NotNull]
        public float GramsPerPortion { get; set; }

        private float _carbScalar;

        public GroceryModel()
        {
            GroceryID = -1; //This will indicate that the Grocery is not yet added to the database
            this._carbScalar = 1.0f; //This is the default of the scalar, when it is one it has no effect on the calculations
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(GroceryModel other)
        {
            return this.GroceryID.Equals(other.GroceryID);
        }

        public int CompareTo(GroceryModel other)
        {
            return this.Name.CompareTo(other.Name);
        }

        [NotNull, MaxLength(255)]
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                if (this._name == value)
                    return;
                try
                {
                    if (value.Length > 255)
                        throw new ArgumentOutOfRangeException("The name attribute of a grocery can not be longer than 255 chars");
                }
                catch (ArgumentOutOfRangeException aoore)
                {
                    //If an error occurs, we simply do not set the value
                    return;
                }

                this._name = value;
                OnPropertyChanged();
            }
        }

        [MaxLength(255)]
        public string BrandName
        {
            get
            {
                return this._brandName;
            }

            set
            {
                if (this._brandName == value)
                    return;
                try
                {
                    if (value.Length > 255)
                        throw new ArgumentOutOfRangeException("The name attribute of a grocery can not be longer than 255 chars");
                }
                catch (ArgumentOutOfRangeException aoore)
                {
                    //If an error occurs, we simply do not set the value
                    return;
                }

                this._brandName = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public float CarbScalar
        {
            get
            {
                return this._carbScalar;
            }

            set
            {
                if (this._carbScalar == value)
                    return;
                try
                {
                    if (value <= 0.0f)
                        throw new ArgumentOutOfRangeException("The attribute CarbsScalar must be a positive float greater than zero");
                }
                catch (ArgumentOutOfRangeException aoore)
                {
                    //If an error occurs, we simply do not set the value
                    return;
                }

                this._carbScalar = value;
                OnPropertyChanged();
            }
        }

        public string ToStringCSV()
        {
            return GroceryID + ", " + Name + ", " + BrandName + ", " + CarbsPer100Grams + ", " + NameOfPortion + ", " + GramsPerPortion + ", " + CarbScalar + "\n";
        }
    }
}
