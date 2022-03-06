using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace DiabetesContolApp.Views
{
    public partial class SettingsAndStatisticsPage : ContentPage
    {
        public SettingsAndStatisticsPage()
        {
            InitializeComponent();

            BindingContext = Application.Current as App;
        }

        protected override void OnDisappearing()
        {
            BindingContext = Application.Current as App;
            //Force the saving of the properties
            Application.Current.SavePropertiesAsync();

            base.OnDisappearing();
        }

        /// <summary>
        /// This method resets TimeUsed to zero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>void</returns>
        void ResetTimeClicked(System.Object sender, System.EventArgs e)
        {
            (Application.Current as App).TimeUsed = 0;
        }
    }
}
