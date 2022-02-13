using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DiabetesContolApp.Views;

namespace DiabetesContolApp
{
    public partial class App : Application
    {
        //Keys
        private const string BaseSensitivityKey = "BaseSensitivity";
        private const string SenesitivityWithoutFoodKey = "SenesitivityWithoutFood";

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
        }

        public int BaseSensitivity
        {
            get
            {
                if (Properties.ContainsKey(BaseSensitivityKey))
                {
                    return (int)Properties[BaseSensitivityKey];
                }
                return 0; //Returns -1 if there is no prpoertiy sat
            }

            set
            {
                Properties[BaseSensitivityKey] = value;
            }
        }

        public float SenesitivityWithoutFood
        {
            get
            {
                if (Properties.ContainsKey(SenesitivityWithoutFoodKey))
                {
                    return (float)Properties[SenesitivityWithoutFoodKey];
                }
                return 0f;
            }

            set
            {
                Properties[SenesitivityWithoutFoodKey] = value;
            }
        }

    }
}
