using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
namespace DiabetesContolApp.Models
{
    [Table("Interval")]
    public class Interval : IEquatable<Interval>, IComparable<Interval>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        private int _timeStart = -1; //Using the time of day as a key, e.g. 16:00 -> 1600 and 13:23 -> 1323, so tyhe range is from 0 (00:00) to 2400 (24:00)
        private string _name;

        [NotNull]
        public float BloodSkalar { get; set; }
        [NotNull]
        public float KarbSkalar { get; set; }
        [NotNull]
        public float TargetBloodSugar { get; set; }

        public Interval()
        {
        }

        [NotNull]
        public int TimeStart
        {
            get
            {
                return this._timeStart;
            }

            set
            {
                if (this._timeStart == value)
                {
                    return;
                }
                else if (value > 2400 || value < 0)
                {
                    throw new ArgumentOutOfRangeException("The value of the time (_timeStart) in Interval must be between 0 and 2400");
                }
                this._timeStart = value;
                OnPropertyChanged();
            }
        }

        [MaxLength(255)]
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                if (this._name == value)
                {
                    return;
                }
                this._name = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(Interval other)
        {
            if (other == null)
            {
                return false;
            }
            return this.ID.Equals(other.ID);
        }

        public int CompareTo(Interval other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                return this.TimeStart.CompareTo(other.TimeStart);
            }
        }
    }
}
