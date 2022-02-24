using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DiabetesContolApp.Models
{
    [Table("Log")]
    public class LogModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        [PrimaryKey, AutoIncrement]
        public int LogID { get; set; }
        [ForeignKey(typeof(DayProfileModel))]
        public int DayProfileID { get; set; }
        [NotNull]
        private long _dateTime { get; set; }
        [NotNull]
        private float _insulinEstimate { get; set; }
        [NotNull]
        private float _insulinFromUser { get; set; }
        [NotNull]
        public float GlucoseAtMeal { get; set; }
        
        public float? GlucoseAfterMeal { get; set; }
        [ManyToMany(typeof(GroceryLogModel))]
        public List<GroceryModel> GroceryModels { get; set; }
        [Ignore]
        public List<NumberOfGroceryModel> NumberOfGroceryModels { get; set; }


        public LogModel()
        {

        }

        public LogModel(int dayProfileID, DateTime dateTime, float insulinEstimate, float insulinFromUser, float glucoseAtMeal, List<NumberOfGroceryModel> numberOfGroceries, float? glucoseAfterMeal = null)
        {
            DayProfileID = dayProfileID;
            DateTime = dateTime;
            InsulinEstimate = insulinEstimate;
            InsulinFromUser = insulinFromUser;
            GlucoseAtMeal = glucoseAtMeal;    
            GlucoseAfterMeal = glucoseAfterMeal;
            NumberOfGroceryModels = numberOfGroceries != null ? numberOfGroceries : new();
        }



        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Ignore]
        public DateTime DateTime
        {
            get
            {
                return DateTime.FromBinary(_dateTime);
            }

            set
            {
                if (value.ToBinary() != this._dateTime)
                {
                    this._dateTime = value.ToBinary();
                    OnPropertyChanged();
                }
                //If it is equal to the previous value there is no need to update it
            }
        }

        [Ignore]
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
                    OnPropertyChanged();
                }
                //If value is not greater than 0 or is the same, we don't wnat to set it
            }
        }

        [Ignore]
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
                    OnPropertyChanged();
                }
                //If value is not greater than 0 or is the same, we don't want to set it
            }
        }
    }
}
