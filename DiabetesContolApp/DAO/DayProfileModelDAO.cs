using System;
using System.Diagnostics;
using System.Globalization;

using DiabetesContolApp.Models;

using SQLite;

namespace DiabetesContolApp.DAO
{
    [Table("DayProfile")]
    public class DayProfileModelDAO : IComparable<DayProfileModelDAO>, IEquatable<DayProfileModelDAO>, IModelDAO
    {
        [PrimaryKey, AutoIncrement]
        public int DayProfileID { get; set; }

        private string _name = "";
        private long _startTime;
        private float _carbScalar;
        private float _glucoseScalar;

        [NotNull]
        public float TargetGlucoseValue { get; set; }

        public DayProfileModelDAO()
        {
            DayProfileID = -1;
        }

        public DayProfileModelDAO(DayProfileModel dayProfile)
        {
            DayProfileID = dayProfile.DayProfileID;
            Name = dayProfile.Name;
            StartTime = dayProfile.StartTime;
            CarbScalar = dayProfile.CarbScalar;
            GlucoseScalar = dayProfile.GlucoseScalar;
            TargetGlucoseValue = dayProfile.TargetGlucoseValue;
        }

        public int CompareTo(DayProfileModelDAO other)
        {
            if (other == null)
                return 1;
            return this.StartTime.CompareTo(other.StartTime);
        }

        public bool Equals(DayProfileModelDAO other)
        {
            if (other == null)
                return false;
            return this.DayProfileID.Equals(other.DayProfileID);
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

        [NotNull]
        public DateTime StartTime
        {
            get
            {
                return DateTime.FromBinary(this._startTime);
            }

            set
            {
                if (value.ToBinary() != this._startTime)
                {
                    this._startTime = value.ToBinary();
                }
                //If it is equal to the previous value there is no need to update it
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

        [NotNull]
        public float GlucoseScalar
        {
            get
            {
                return this._glucoseScalar;
            }

            set
            {
                if (this._glucoseScalar == value)
                    return;
                try
                {
                    if (value <= 0.0f)
                        throw new ArgumentOutOfRangeException("The attribute GlucoseScalar must be a positive float greater than zero");
                }
                catch (ArgumentOutOfRangeException aoore)
                {
                    Debug.WriteLine(aoore.StackTrace);
                    Debug.WriteLine(aoore.Message);
                    //If an error occurs, we simply do not set the value
                    return;
                }

                this._glucoseScalar = value;
            }
        }

        public string ToStringCSV()
        {
            return DayProfileID + "," +
                Name + "," +
                StartTime.ToString("HH:mm") + "," +
                CarbScalar.ToString("0.00", CultureInfo.InvariantCulture) + "," +
                GlucoseScalar.ToString("0.00", CultureInfo.InvariantCulture) + "," +
                TargetGlucoseValue.ToString("0.00", CultureInfo.InvariantCulture) + "\n";
        }
    }
}
