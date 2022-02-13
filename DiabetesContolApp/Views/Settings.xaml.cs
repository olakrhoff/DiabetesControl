using System;
using System.Collections.Generic;
using DiabetesContolApp.GlobalLogic;

using Xamarin.Forms;

namespace DiabetesContolApp.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            var app = Application.Current as App;
            if (app.BaseSensitivity > 0)
                BaseSensitivity.Text = app.BaseSensitivity.ToString();
            if (app.SenesitivityWithoutFood > 0)
                SensitivitySkalarNoFood.Text = app.SenesitivityWithoutFood.ToString();
        }

        void OnChange(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var app = Application.Current as App;

            int NewBaseSensetivity = -1;
            int.TryParse(BaseSensitivity.Text, out NewBaseSensetivity);

            if (NewBaseSensetivity > 0)
            {
                app.BaseSensitivity = NewBaseSensetivity;
                BaseSensitivity.LabelColor = Color.Green;
            }
            else
                BaseSensitivity.LabelColor = Color.Red;

            if (Helper.ConvertToFloat(SensitivitySkalarNoFood.Text, out float sensitivitySkalarNoFoodFloat) && sensitivitySkalarNoFoodFloat > 0f)
            {
                app.SenesitivityWithoutFood = sensitivitySkalarNoFoodFloat;
                SensitivitySkalarNoFood.LabelColor = Color.Green;
            }
            else
                SensitivitySkalarNoFood.LabelColor = Color.Red;

            app.SavePropertiesAsync();
        }

        protected override void OnDisappearing()
        {
            OnChange(null, null);
            base.OnDisappearing();
        }
    }
}
