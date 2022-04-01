using System;
using System.Collections.Generic;

using DiabetesContolApp.Views;
using DiabetesContolApp.Service;
using DiabetesContolApp.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiabetesContolApp
{
    public partial class App : Application
    {
        //Keys
        private const string InsulinToCarbohydratesRatioKey = "InsulinToCarbohydratesRatio";
        private const string InsulinToGlucoseRatioKey = "InsulinToGlucoseRatio";
        private const string InsulinOnlyCorrectionScalarKey = "InsulinOnlyCorrectionScalar";
        private const string TimeUsedKey = "TimeUsed";


        private DateTime StartTime = DateTime.Now;

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainTabbedPage());
        }

        protected override void OnStart()
        {
            StartTime = DateTime.Now;
            CheckReminders();
        }

        protected override void OnSleep()
        {
            SavePropertiesAsync();

            ulong timeInApp = (ulong)(DateTime.Now - StartTime).TotalSeconds;

            TimeUsed += timeInApp;
        }

        protected override void OnResume()
        {
            StartTime = DateTime.Now;
            CheckReminders();
        }

        /*
         * This variable gives the number of carbs per unit of insulin
         * 
         * If it is not stored in properties, it returns -1.0f
         */
        public float InsulinToCarbohydratesRatio
        {
            get
            {
                if (Properties.ContainsKey(InsulinToCarbohydratesRatioKey))
                    return (float)Properties[InsulinToCarbohydratesRatioKey];
                return -1.0f; //Returns -1 if there is no propertiy sat
            }

            set
            {
                if (!Properties.ContainsKey(InsulinToCarbohydratesRatioKey))
                    Properties.Add(InsulinToCarbohydratesRatioKey, -1.0f);
                Properties[InsulinToCarbohydratesRatioKey] = value;
            }
        }

        /// <summary>
        /// This variable gives the number of glucose per unit of insulin.
        ///
        /// Example: If a user sat 1 unit of insulin the glucose would go
        /// down "this value" number of values. If the value is 2 then
        /// if the user sat 3 unit of insulin their glucose would go down
        /// 2 * 3 = 6 mmol/L. This gives it the form of X mmol/L per units of inulin (mmol/L/U).
        /// 
        /// If it is not stored in properties, it returns -1.0f
        /// </summary>
        public float InsulinToGlucoseRatio
        {
            get
            {
                if (Properties.ContainsKey(InsulinToGlucoseRatioKey))
                    return (float)Properties[InsulinToGlucoseRatioKey];
                return -1.0f; //Returns -1 if there is no propertiy sat
            }

            set
            {
                if (!Properties.ContainsKey(InsulinToGlucoseRatioKey))
                    Properties.Add(InsulinToGlucoseRatioKey, -1.0f);
                Properties[InsulinToGlucoseRatioKey] = value;
            }
        }

        /*
         * This variable gives the sensitiviy shift in correction insulin
         * if only correction insulin is sat
         * 
         * If it is not stored in properties, it returns -1.0f
         */
        public float InsulinOnlyCorrectionScalar
        {
            get
            {
                if (Properties.ContainsKey(InsulinOnlyCorrectionScalarKey))
                    return (float)Properties[InsulinOnlyCorrectionScalarKey];
                return -1.0f; //Returns -1 if there is no propertiy sat
            }

            set
            {
                if (!Properties.ContainsKey(InsulinOnlyCorrectionScalarKey))
                    Properties.Add(InsulinOnlyCorrectionScalarKey, -1.0f);
                Properties[InsulinOnlyCorrectionScalarKey] = value;
            }
        }

        /*
         * This variables gives the time the user has been inside
         * the app, given in seconds.
         */
        public ulong TimeUsed
        {
            get
            {
                if (Properties.ContainsKey(TimeUsedKey))
                    return (ulong)Properties[TimeUsedKey];
                return 0L;
            }

            set
            {
                if (!Properties.ContainsKey(TimeUsedKey))
                    Properties.Add(TimeUsedKey, 0L);
                Properties[TimeUsedKey] = value;
            }
        }

        /// <summary>
        /// This variables gives the time the user has been inside
        /// the app, given in minutes, based on the TimeUsed variable.
        /// </summary>
        public ulong TimeUsedInMinutes
        {
            get
            {
                return TimeUsed / 60;
            }
        }

        /// <summary>
        /// Checks if there are any reminders to handle,
        /// if so, they are handled.
        /// </summary>
        private void CheckReminders()
        {
            ReminderService reminderService = new();
            reminderService.HandleRemindersAsync();
        }
    }
}
