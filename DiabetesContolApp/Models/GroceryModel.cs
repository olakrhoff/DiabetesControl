using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DiabetesContolApp.Models
{
    [Table("Grocery")]
    public class GroceryModel : INotifyPropertyChanged, IEquatable<GroceryModel>, IComparable<GroceryModel>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int GroceryID { get; set; }

        private string _name;

        [NotNull]
        public float CarbsPer100Grams { get; set; }
        [NotNull]
        public string NameOfPortion { get; set; }
        [NotNull]
        public float GramsPerPortion { get; set; }

        private float _carbScalar;

        [ManyToMany(typeof(GroceryLogModel))]
        public List<LogModel> Logs { get; set; }

        public GroceryModel()
        {
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
                    //TODO: Do some logging maybe
                    return;
                }

                this._name = value;
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
                    //TODO: Do some logging maybe
                    return;
                }

                this._carbScalar = value;
                OnPropertyChanged();
            }
        }
    }
}
