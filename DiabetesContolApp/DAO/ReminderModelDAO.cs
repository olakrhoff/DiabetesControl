using System;
using System.Globalization;

using DiabetesContolApp.Models;

using SQLite;

namespace DiabetesContolApp.DAO
{
    [Table("Reminder")]
    public class ReminderModelDAO : IComparable<ReminderModelDAO>, IEquatable<ReminderModelDAO>, IModelDAO
    {
        //The number of hours the reminder is sat to wait
        public static int TIME_TO_WAIT = 3;

        [PrimaryKey, AutoIncrement]
        public int ReminderID { get; set; }
        [NotNull]
        public long DateTimeLong { get; set; } //When to remind the user
        public float? GlucoseAfterMeal { get; set; }
        public bool IsHandled { get; set; }

        public ReminderModelDAO()
        {
            ReminderID = -1;
        }

        public ReminderModelDAO(ReminderModel reminder)
        {
            ReminderID = reminder.ReminderID;
            DateTimeLong = reminder.DateTimeLong;
            DateTimeValue = reminder.DateTimeValue;
            GlucoseAfterMeal = reminder.GlucoseAfterMeal;
            IsHandled = reminder.IsHandled;
        }

        public int CompareTo(ReminderModelDAO other)
        {
            return DateTimeValue.CompareTo(other.DateTimeValue);
        }

        public bool Equals(ReminderModelDAO other)
        {
            return DateTimeValue.Equals(other.DateTimeValue);
        }

        [Ignore]
        public DateTime DateTimeValue
        {
            get
            {
                return DateTime.FromBinary(this.DateTimeLong);
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

        public string ToStringCSV()
        {
            return ReminderID + "," +
                DateTimeValue.ToString("yyyy/MM/dd HH:mm") + "," +
                GlucoseAfterMeal?.ToString("0.00", CultureInfo.InvariantCulture) + "," +
                IsHandled + "\n";
        }
    }
}