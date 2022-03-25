using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiabetesContolApp.Models
{
    public class LogModel : /*INotifyPropertyChanged,*/ IComparable<LogModel>, IEquatable<LogModel>, IModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int LogID { get; set; }
        public DayProfileModel DayProfile { get; set; }
        public ReminderModel Reminder { get; set; }
        public long DateTimeLong { get; set; }
        public float GlucoseAtMeal { get; set; }
        public float? GlucoseAfterMeal { get; set; }
        public float CorrectionInsulin { get; set; }
        public List<NumberOfGroceryModel> NumberOfGroceryModels { get; set; }


        public LogModel()
        {
            LogID = -1;
            DayProfile = new();
            Reminder = new();
        }

        public LogModel(int dayProfileID, DateTime dateTime, float insulinEstimate, float insulinFromUser, float glucoseAtMeal, List<NumberOfGroceryModel> numberOfGroceries, float? glucoseAfterMeal = null)
        {
            LogID = -1;
            Reminder = new();
            DayProfile = new(dayProfileID);
            DateTimeValue = dateTime;
            InsulinEstimate = insulinEstimate;
            InsulinFromUser = insulinFromUser;
            GlucoseAtMeal = glucoseAtMeal;
            GlucoseAfterMeal = glucoseAfterMeal;
            NumberOfGroceryModels = numberOfGroceries != null ? numberOfGroceries : new();
        }


        /*
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/

        public int CompareTo(LogModel other)
        {
            return this.DateTimeValue.CompareTo(other.DateTimeValue);
        }

        public bool Equals(LogModel other)
        {
            return this.DateTimeValue.Equals(other.DateTimeValue);
        }


        public DateTime DateTimeValue
        {
            get
            {
                return DateTime.FromBinary(DateTimeLong);
            }

            set
            {
                if (value.ToBinary() != this.DateTimeLong)
                {
                    this.DateTimeLong = value.ToBinary();
                    //OnPropertyChanged();
                }
                //If it is equal to the previous value there is no need to update it
            }
        }


        public string TimeString
        {
            get
            {
                return DateTimeValue.ToString("HH:mm");
            }
        }

        private float _insulinEstimate = -1.0f;

        public float InsulinEstimate
        {
            get
            {
                return this._insulinEstimate;
            }

            set
            {
                if (value >= 0.0f && value != this._insulinEstimate)
                {
                    this._insulinEstimate = value;
                    //OnPropertyChanged();
                }
                //If value is not greater than 0 or is the same, we don't wnat to set it
            }
        }

        private float _insulinFromUser = -1.0f;


        public float InsulinFromUser
        {
            get
            {
                return this._insulinFromUser;
            }

            set
            {
                if (value >= 0.0f && value != this._insulinFromUser)
                {
                    this._insulinFromUser = value;
                    //OnPropertyChanged();
                }
                //If value is not greater than 0 or is the same, we don't want to set it
            }
        }

        public string ToStringCSV()
        {
            return LogID + "," +
                DayProfile.DayProfileID + "," +
                Reminder.ReminderID + "," +
                DateTimeValue.ToString("yyyy/MM/dd HH:mm") + "," +
                GlucoseAtMeal + "," +
                GlucoseAfterMeal + "\n";
        }

    }
}