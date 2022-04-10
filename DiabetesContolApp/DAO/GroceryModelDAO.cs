using System;
using System.Diagnostics;
using System.Globalization;

using DiabetesContolApp.Models;

using SQLite;

namespace DiabetesContolApp.DAO
{
    [Table("Grocery")]
    public class GroceryModelDAO : IEquatable<GroceryModelDAO>, IComparable<GroceryModelDAO>, IModelDAO
    {
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

        public GroceryModelDAO()
        {
            GroceryID = -1;
        }

        public GroceryModelDAO(GroceryModel grocery)
        {
            GroceryID = grocery.GroceryID;
            CarbsPer100Grams = grocery.CarbsPer100Grams;
            NameOfPortion = grocery.NameOfPortion;
            GramsPerPortion = grocery.GramsPerPortion;
            Name = grocery.Name;
            BrandName = grocery.BrandName;
            CarbScalar = grocery.CarbScalar;
        }

        public bool Equals(GroceryModelDAO other)
        {
            return this.GroceryID.Equals(other.GroceryID);
        }

        public int CompareTo(GroceryModelDAO other)
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
                    Debug.WriteLine(aoore.StackTrace);
                    Debug.WriteLine(aoore.Message);
                    //If an error occurs, we simply do not set the value
                    return;
                }

                this._name = value;
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
                    Debug.WriteLine(aoore.StackTrace);
                    Debug.WriteLine(aoore.Message);
                    //If an error occurs, we simply do not set the value
                    return;
                }

                this._brandName = value;
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
                    Debug.WriteLine(aoore.StackTrace);
                    Debug.WriteLine(aoore.Message);
                    //If an error occurs, we simply do not set the value
                    return;
                }

                this._carbScalar = value;
            }
        }

        public string ToStringCSV()
        {
            return GroceryID + "," +
                Name + "," +
                BrandName + "," +
                CarbsPer100Grams.ToString("0.00", CultureInfo.InvariantCulture) + "," +
                NameOfPortion + "," +
                GramsPerPortion.ToString("0.00", CultureInfo.InvariantCulture) + "," +
                CarbScalar.ToString("0.00", CultureInfo.InvariantCulture) + "\n";
        }


    }
}
