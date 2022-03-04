using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace DiabetesContolApp.Models
{
    [Table("DayProfile")]
    public class DayProfileModel : IComparable<DayProfileModel>, IEquatable<DayProfileModel>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int DayProfileID { get; set; }

        private string _name;
        private ushort _startTime;
        private float _carbScalar;
        private float _glucoseScalar;

        [NotNull]
        public float TargetGlucoseValue { get; set; }

        public DayProfileModel()
        {
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(DayProfileModel other)
        {
            if (other == null)
                return 1;
            return this.StartTime.CompareTo(other.StartTime);
        }

        public bool Equals(DayProfileModel other)
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
                    //If an error occurs, we simply do not set the value
                    return;
                }

                this._name = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public ushort StartTime
        {
            get
            {
                return this._startTime;
            }

            set
            {
                if (this._startTime == value)
                    return;
                try
                {
                    if (value > 2400 || value < 0)
                        throw new ArgumentOutOfRangeException("The attribute StartTime must be a positive integer greater than zero and less then 2400");
                }
                catch (ArgumentOutOfRangeException aoore)
                {
                    //If an error occurs, we simply do not set the value
                    return;
                }

                this._startTime = value;
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
                    //If an error occurs, we simply do not set the value
                    return;
                }

                this._glucoseScalar = value;
                OnPropertyChanged();
            }
        }
    }
}
