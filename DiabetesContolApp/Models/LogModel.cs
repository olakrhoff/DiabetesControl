using System;
using System.Collections.Generic;
using System.ComponentModel;

using DiabetesContolApp.DAO;

namespace DiabetesContolApp.Models
{
    public class LogModel : IComparable<LogModel>, IEquatable<LogModel>
    {
        public int LogID { get; set; }
        public DayProfileModel DayProfile { get; set; }
        public ReminderModel Reminder { get; set; }
        public long DateTimeLong { get; set; }
        public float GlucoseAtMeal { get; set; }
        public float? GlucoseAfterMeal { get; set; }
        public float CorrectionInsulin { get; set; }
        public List<NumberOfGroceryModel> NumberOfGroceries { get; set; }


        public LogModel()
        {
            LogID = -1;
            DayProfile = new();
            Reminder = new();
            NumberOfGroceries = new();
            DateTimeValue = DateTime.Now;
        }

        public LogModel(int logID)
        {
            LogID = logID;
            DayProfile = new();
            Reminder = new();
            NumberOfGroceries = new();
            DateTimeValue = DateTime.Now;
        }

        public LogModel(LogModelDAO logDAO)
        {
            LogID = logDAO.LogID;
            Reminder = new(logDAO.ReminderID);
            DayProfile = new(logDAO.DayProfileID);
            DateTimeValue = logDAO.DateTimeValue;
            InsulinEstimate = logDAO.InsulinEstimate;
            InsulinFromUser = logDAO.InsulinFromUser;
            GlucoseAtMeal = logDAO.GlucoseAtMeal;
            GlucoseAfterMeal = logDAO.GlucoseAfterMeal;
            CorrectionInsulin = logDAO.CorrectionInsulin;
            NumberOfGroceries = new();
        }

        public LogModel(DayProfileModel dayProfile, ReminderModel reminder, DateTime dateTime, float insulinEstimate, float insulinFromUser, float glucoseAtMeal, List<NumberOfGroceryModel> numberOfGroceries, float? glucoseAfterMeal = null)
        {
            LogID = -1;
            Reminder = reminder;
            DayProfile = dayProfile;
            DateTimeValue = dateTime;
            InsulinEstimate = insulinEstimate;
            InsulinFromUser = insulinFromUser;
            GlucoseAtMeal = glucoseAtMeal;
            GlucoseAfterMeal = glucoseAfterMeal;
            NumberOfGroceries = numberOfGroceries ?? new();
        }

        public LogModel(int logID, DayProfileModel dayProfile, ReminderModel reminder, DateTime dateTime, float insulinEstimate, float insulinFromUser, float glucoseAtMeal, float correctionInsulin, List<NumberOfGroceryModel> numberOfGroceries, float? glucoseAfterMeal = null)
        {
            LogID = logID;
            Reminder = reminder;
            DayProfile = dayProfile;
            DateTimeValue = dateTime;
            InsulinEstimate = insulinEstimate;
            InsulinFromUser = insulinFromUser;
            GlucoseAtMeal = glucoseAtMeal;
            GlucoseAfterMeal = glucoseAfterMeal;
            CorrectionInsulin = correctionInsulin;
            NumberOfGroceries = numberOfGroceries ?? new();
        }

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
                }
                //If value is not greater than 0 or is the same, we don't want to set it
            }
        }

        /// <summary>
        /// Check to see if the Logs data is valid and can be used.
        /// </summary>
        /// <returns>True if all critical data is valid, else false.</returns>
        public bool IsLogDataValid()
        {
            return GlucoseAfterMeal != null &&
                DayProfile != null &&
                Reminder != null &&
                NumberOfGroceries != null;
        }

        /// <summary>
        /// Gets the insulin for the carbs only. This
        /// including the scalar of the DayProfile.
        /// </summary>
        /// <returns>float, insulin for the carbs</returns>
        public float GetInsulinForCarbs()
        {
            return InsulinEstimate - CorrectionInsulin;
        }

        /// <summary>
        /// Gets the insulin generated by the
        /// DayProfile carb-scalar.
        /// </summary>
        /// <returns>float, insulin from carbs-scalar in DayProfile.</returns>
        public float GetInsulinFromDayProfileCarbScalar()
        {
            float insulinForCarbs = GetInsulinForCarbs();
            return insulinForCarbs - insulinForCarbs / DayProfile.CarbScalar;
        }

        /// <summary>
        /// Gets the glucose error in the log.
        /// </summary>
        /// <returns>float, the glucose error.</returns>
        public float GetGlucoseError()
        {
            return (float)GlucoseAfterMeal - DayProfile.TargetGlucoseValue;
        }

        /// <summary>
        /// Gets the insulin for the glucose only. This
        /// including the scalar of the DayProfile.
        /// </summary>
        /// <returns>float, insulin for the glucose</returns>
        public float GetInsulinForGlucose()
        {
            return CorrectionInsulin;
        }

        /// <summary>
        /// Gets the insulin generated by the
        /// DayProfile glucose-scalar.
        /// </summary>
        /// <returns>float, insulin from glucose-scalar in DayProfile.</returns>
        public float GetInsulinFromDayProfileGlucoseScalar()
        {
            float insulinForGlucose = GetInsulinForGlucose();
            return insulinForGlucose - insulinForGlucose / DayProfile.GlucoseScalar;
        }

        /// <summary>
        /// Gets the total insulin the grocery, with
        /// the given ID, was at fault for.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>float, the amount of insulin.</returns>
        public float GetInsulinFromGroceryWithID(int groceryID)
        {
            foreach (NumberOfGroceryModel numberOfGrocery in NumberOfGroceries)
                if (numberOfGrocery.Grocery.GroceryID == groceryID)
                    return numberOfGrocery.InsulinForGroceries;
            throw new ArgumentException("No grocery with given ID (" + groceryID + ") was found in the log");
        }

        /// <summary>
        /// Gets the insulin per portion of a given grocery,
        /// with the given ID.
        /// </summary>
        /// <param name="groceryID"></param>
        /// <returns>float, the per portion amount for the given Grocery.</returns>
        public float GetInsulinPerPortionFromGroceryWithID(int groceryID)
        {
            foreach (NumberOfGroceryModel numberOfGrocery in NumberOfGroceries)
                if (numberOfGrocery.Grocery.GroceryID == groceryID)
                    return numberOfGrocery.InsulinForGroceries / numberOfGrocery.NumberOfGrocery;
            throw new ArgumentException("No grocery with given ID (" + groceryID + ") was found in the log");
        }
    }
}