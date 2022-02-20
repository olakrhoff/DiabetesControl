using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DiabetesContolApp.Views;

namespace DiabetesContolApp
{
    public partial class App : Application
    {
        //Keys
        private const string InsulinToCarbohydratesRatioKey = "InsulinToCarbohydratesRatio";
        private const string InsulinToGlucoseRatioKey = "InsulinToGlucoseRatio";
        private const string InsulinOnlyCorrectionScalarKey = "InsulinOnlyCorrectionScalar";

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainTabbedPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            //TODO: Update day profile picker
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

        /*
         * This variable gives the number of glucose per unit of insulin
         * 
         * If it is not stored in properties, it returns -1.0f
         */
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

    }
}
