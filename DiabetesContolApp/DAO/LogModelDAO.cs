using System;
using System.Globalization;

using DiabetesContolApp.Models;

using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DiabetesContolApp.DAO
{
    [Table("Log")]
    public class LogModelDAO : IComparable<LogModelDAO>, IEquatable<LogModelDAO>, IModelDAO
    {
        private float _insulinEstimate = -1.0f;
        private float _insulinFromUser = -1.0f;

        [PrimaryKey, AutoIncrement]
        public int LogID { get; set; }
        [ForeignKey(typeof(DayProfileModelDAO))]
        public int DayProfileID { get; set; }
        [ForeignKey(typeof(ReminderModelDAO))]
        public int ReminderID { get; set; }
        [NotNull]
        public long DateTimeLong { get; set; }
        [NotNull]
        public float GlucoseAtMeal { get; set; }
        public float? GlucoseAfterMeal { get; set; }
        public float CorrectionInsulin { get; set; }

        public LogModelDAO()
        {
            LogID = -1;
        }

        public LogModelDAO(LogModel newLog)
        {
            LogID = newLog.LogID;
            DayProfileID = newLog.DayProfile.DayProfileID;
            ReminderID = newLog.Reminder.ReminderID;
            DateTimeValue = newLog.DateTimeValue;
            GlucoseAtMeal = newLog.GlucoseAtMeal;
            GlucoseAfterMeal = newLog.GlucoseAfterMeal;
            InsulinEstimate = newLog.InsulinEstimate;
            InsulinFromUser = newLog.InsulinFromUser;
            CorrectionInsulin = newLog.CorrectionInsulin;
        }

        public int CompareTo(LogModelDAO other)
        {
            return this.DateTimeValue.CompareTo(other.DateTimeValue);
        }

        public bool Equals(LogModelDAO other)
        {
            return this.DateTimeValue.Equals(other.DateTimeValue);
        }

        [Ignore]
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

        [Ignore]
        public string TimeString
        {
            get
            {
                return DateTimeValue.ToString("HH:mm");
            }
        }

        [NotNull]
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

        [NotNull]
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

        public string ToStringCSV()
        {
            return LogID + "," +
                DayProfileID + "," +
                ReminderID + "," +
                DateTimeValue.ToString("yyyy/MM/dd HH:mm") + "," +
                GlucoseAtMeal.ToString("0.00", CultureInfo.InvariantCulture) + "," +
                GlucoseAfterMeal?.ToString("0.00", CultureInfo.InvariantCulture) + "," +
                InsulinEstimate.ToString("0.00", CultureInfo.InvariantCulture) + "," +
                InsulinFromUser.ToString("0.00", CultureInfo.InvariantCulture) + "," +
                CorrectionInsulin.ToString("0.00", CultureInfo.InvariantCulture) + "\n";
        }
    }
}
