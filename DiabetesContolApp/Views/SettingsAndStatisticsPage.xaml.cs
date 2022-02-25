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

        protected override void OnAppearing()
        {
            BindingContext = Application.Current as App;

            base.OnAppearing();
        }
    }
}
