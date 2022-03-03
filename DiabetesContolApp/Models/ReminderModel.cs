using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SQLite;

using DiabetesContolApp.GlobalLogic;

using SQLiteNetExtensions.Attributes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        public bool IsHandled { get; set; }

        public ReminderModel()
        {
            ReminderID = -1;
            //DateTimeLong = DateTime.Now.AddHours(TIME_TO_WAIT).ToBinary();
            DateTimeLong = DateTime.Now.AddSeconds(TIME_TO_WAIT).ToBinary();
            IsHandled = false;
        }

        async public Task<bool> Handle()
        {
            if (DateTimeValue > DateTime.Now)
                return false; //The reminder is not ready to be handled


            //TODO: Ask for glucose after meal

            string userInput = await Application.Current.MainPage.DisplayPromptAsync("Glucose after meal", $"What was your glucose at {DateTimeValue.ToString("t")}", keyboard: Keyboard.Numeric);

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