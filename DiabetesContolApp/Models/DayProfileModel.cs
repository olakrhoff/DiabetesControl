using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

using DiabetesContolApp.DAO;


namespace DiabetesContolApp.Models
{
    public class DayProfileModel : IComparable<DayProfileModel>, IEquatable<DayProfileModel>, IModel//, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private string _name;
        private long _startTime;
        private float _carbScalar;
        private float _glucoseScalar;

        public int DayProfileID { get; set; }
        public float TargetGlucoseValue { get; set; }

        public DayProfileModel()
        {
            DayProfileID = -1;
        }

        public DayProfileModel(int dayProfileID)
        {
            DayProfileID = dayProfileID;
        }

        public DayProfileModel(DayProfileModelDAO dayProfileDAO)
        {
            DayProfileID = dayProfileDAO.DayProfileID;
            Name = dayProfileDAO.Name;
            StartTime = dayProfileDAO.StartTime;
            CarbScalar = dayProfileDAO.CarbScalar;
            GlucoseScalar = dayProfileDAO.GlucoseScalar;
            TargetGlucoseValue = dayProfileDAO.TargetGlucoseValue;
        }

        /*
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/

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
                //OnPropertyChanged();
            }
        }

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
                    //OnPropertyChanged();
                }
                //If it is equal to the previous value there is no need to update it
            }
        }

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
                //OnPropertyChanged();
            }
        }

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
                //OnPropertyChanged();
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
