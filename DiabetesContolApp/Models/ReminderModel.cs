using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SQLite;

using DiabetesContolApp.GlobalLogic;
using DiabetesContolApp.Persistence;

using SQLiteNetExtensions.Attributes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiabetesContolApp.Models
{
    [Table("Reminder")]
    public class ReminderModel : INotifyPropertyChanged, IComparable<ReminderModel>, IEquatable<ReminderModel>, IModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //The number of hours the reminder is sat to wait
        public static int TIME_TO_WAIT = 3;

        [PrimaryKey, AutoIncrement]
        public int ReminderID { get; set; }
        [NotNull]
        public long DateTimeLong { get; set; } //When to remind the user
        public float? GlucoseAfterMeal { get; set; }
        [OneToMany]
        public List<LogModel> Logs { get; set; }

        public bool IsHandled { get; set; }

        public ReminderModel()
        {
            ReminderID = -1;
            DateTimeLong = DateTime.Now.AddHours(TIME_TO_WAIT).ToBinary();
            IsHandled = false;
            Logs = new();
        }

        /// <summary>
        /// Method checks if the reminder is to be handled, this means
        /// that the "timer" has gone. If so it collects the glucose value
        /// at the spesified time from the user. Then it runs the statistics
        /// for all involved groceries and day profiles.
        /// </summary>
        /// <returns>
        /// true if reminder was handled, else false
        /// </returns>
        async public Task<bool> Handle()
        {
            if (DateTimeValue > DateTime.Now)
                return false; //The reminder is not ready to be handled

            if (!await Application.Current.MainPage.DisplayAlert("Glucose after meal", "Want to enter gluocse now?", "OK", "Later"))
                return false;
            string userInput = await Application.Current.MainPage.DisplayPromptAsync("Glucose after meal", $"What was your glucose at {DateTimeValue.ToString("H:mm")}", "Confirm", "Currupt", keyboard: Keyboard.Numeric);

            //if userInput is null, currupt was clicked
            if (userInput == null)
            {
                GlucoseAfterMeal = -1.0f; //Indicates invalid data

                (await LogDatabase.GetInstance().GetLogsWithReminderAsync(ReminderID)).ForEach(async log =>
                {
                    log.GlucoseAfterMeal = -1.0f; //Set it invald.
                    await LogDatabase.GetInstance().UpdateLogAsync(log);
                });

                IsHandled = true;

                return true;
            }

            if (Helper.ConvertToFloat(userInput, out float glucoseAfterMeal))
            {
                GlucoseAfterMeal = glucoseAfterMeal;
                IsHandled = true;
            }


            //TODO: Call statistical algorithm on involved logs.


            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Updates the time the reminder is finnished to
        /// TIME_TO_WAIT number of hours from current time.
        /// </summary>
        public void UpdateDateTime()
        {
            DateTimeValue = DateTime.Now.AddHours(TIME_TO_WAIT);
        }

        public bool ReadyToHandle()
        {
            return DateTime.Now > DateTimeValue;
        }

        public int CompareTo(ReminderModel other)
        {
            return DateTimeValue.CompareTo(other.DateTimeValue);
        }

        public bool Equals(ReminderModel other)
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
                    OnPropertyChanged();
                }
                //If it is equal to the previous value there is no need to update it
            }
        }

        public string ToStringCSV()
        {
            return ReminderID + ", " +
                DateTimeValue.ToString("yyyy/MM/dd HH:mm") + ", " +
                GlucoseAfterMeal + ", " +
                IsHandled + "\n";
        }
    }
}