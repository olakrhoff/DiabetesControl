using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using DiabetesContolApp.GlobalLogic;
using DiabetesContolApp.DAO;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiabetesContolApp.Models
{
    public class ReminderModel : IComparable<ReminderModel>, IEquatable<ReminderModel>, IModel
    {
        //The number of hours the reminder is sat to wait
        public static int TIME_TO_WAIT = 3;

        public int ReminderID { get; set; }
        public long DateTimeLong { get; set; } //When to remind the user
        public float? GlucoseAfterMeal { get; set; }
        public List<LogModel> Logs { get; set; }
        public bool IsHandled { get; set; }

        public ReminderModel()
        {
            ReminderID = -1;
            DateTimeLong = DateTime.Now.AddHours(TIME_TO_WAIT).ToBinary();
            IsHandled = false;
            Logs = new();
        }

        public ReminderModel(int reminderID)
        {
            ReminderID = reminderID;
            DateTimeLong = DateTime.Now.AddHours(TIME_TO_WAIT).ToBinary();
            IsHandled = false;
            Logs = new();
        }

        public ReminderModel(ReminderModelDAO reminderDAO)
        {
            ReminderID = reminderDAO.ReminderID;
            DateTimeValue = reminderDAO.DateTimeValue;
            GlucoseAfterMeal = reminderDAO.GlucoseAfterMeal;
            Logs = new();
            IsHandled = reminderDAO.IsHandled;
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
            if (!ReadyToHandle())
                return false; //The reminder is not ready to be handled

            if (GlucoseAfterMeal == null) //If we are missing the glucose after meal value, we need to get it from the user
            {
                if (!await Application.Current.MainPage.DisplayAlert("Glucose after meal", "Want to enter gluocse now?", "OK", "Later"))
                    return false;
                string userInput = await Application.Current.MainPage.DisplayPromptAsync("Glucose after meal", $"What was your glucose at {DateTimeValue.ToString("H:mm")}", "Confirm", "Currupt", keyboard: Keyboard.Numeric);

                //if userInput is null, currupt was clicked
                if (userInput == null)
                {
                    GlucoseAfterMeal = -1.0f; //Indicates invalid data
                    Logs.ForEach(log => log.GlucoseAfterMeal = null); //Set all data to be invalid.

                    IsHandled = true;
                    return true;
                }

                if (Helper.ConvertToFloat(userInput, out float glucoseAfterMeal))
                    GlucoseAfterMeal = glucoseAfterMeal;
            }
            else if (GlucoseAfterMeal == -1.0f) //Make sure all Logs connected are properly adjusted
            {
                Logs.ForEach(log => log.GlucoseAfterMeal = null); //Set all data to be invalid.
                IsHandled = true;
                return true;
            }

            IsHandled = true;
            await Algorithm.RunStatisticsOnReminder(this);
            return true;
        }

        /// <summary>
        /// Updates the time the reminder is finished to
        /// TIME_TO_WAIT number of hours from current time or
        /// if a time is supplied, TIME_TO_WAIT after the given time.
        /// </summary>
        /// <param name="dateTime"></param>
        public void UpdateDateTime(DateTime? dateTime = null)
        {
            if (dateTime == null)
                DateTimeValue = DateTime.Now.AddHours(TIME_TO_WAIT);
            else
                DateTimeValue = ((DateTime)dateTime).AddHours(TIME_TO_WAIT);
        }

        /// <summary>
        /// Checks if the reminder is ready to be handled
        /// </summary>
        /// <returns>Returns true if the reminder doesn't overlap with the current time within TIME_TO_WAIT hours, else false</returns>
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

        /// <summary>
		/// Used to see if the glucose after meal
		/// value has been sat valid or not, this
        /// value also has to be greater than 0.
		/// </summary>
		/// <returns>True if value is not null and greater than 0</returns>
        public bool IsGlucoseAfterMealValid()
        {
            if (GlucoseAfterMeal == null)
                return false;
            return GlucoseAfterMeal > 0;
        }

        public string ToStringCSV()
        {
            return ReminderID + "," +
                DateTimeValue.ToString("yyyy/MM/dd HH:mm") + "," +
                GlucoseAfterMeal + "," +
                IsHandled + "\n";
        }
    }
}