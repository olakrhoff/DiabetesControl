using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

using SQLiteNetExtensions.Attributes;

namespace DiabetesContolApp.Models
{
    [Table("Reminder")]
    public class ReminderModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //The number of hours the reminder is sat to wait
        const int TIME_TO_WAIT = 3;

        [PrimaryKey, AutoIncrement]
        public int ReminderID { get; set; }
        [NotNull]
        public long DateTimeLong { get; set; } //When to remind the user
        public float? GlucoseAfterMeal { get; set; }
        [OneToMany]
        public List<LogModel> Logs { get; set; }

        public ReminderModel()
        {
            ReminderID = -1;
            DateTimeLong = DateTime.Now.AddHours(TIME_TO_WAIT).ToBinary();
        }




        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                    OnPropertyChanged();
                }
                //If it is equal to the previous value there is no need to update it
            }
        }
    }
}