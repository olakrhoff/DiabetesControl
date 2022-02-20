using System;
using System.Collections.Generic;

using DiabetesContolApp.Models;
using DiabetesContolApp.GlobalLogic;

using Xamarin.Forms;

namespace DiabetesContolApp.Views
{
    public partial class DayProfileDetailPage : ContentPage
    {
        public DayProfileModel DayProfile { get; set; }
        public event EventHandler<DayProfileModel> DayProfileSaved;
        public event EventHandler<DayProfileModel> DayProfileAdded;

        public DayProfileDetailPage(DayProfileModel dayProfile)
        {
            if (dayProfile == null)
                return;

            DayProfile = dayProfile;

            InitializeComponent();

            BindingContext = DayProfile;

            if (dayProfile.DayProfileID == -1) //We are adding a DayProfile
            {
                name.Text = "";
                targetGlucoseValue.Text = "";
                timePickerStartTime.Time = DateTime.Now.TimeOfDay;
            }
            else
            {
                timePickerStartTime.Time = new TimeSpan(DayProfile.StartTime / 100, DayProfile.StartTime % 100, 0);
            }
        }

        async void SaveClicked(System.Object sender, System.EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(name.Text) ||
                String.IsNullOrWhiteSpace(targetGlucoseValue.Text) ||
                !Helper.ConvertToFloat(targetGlucoseValue.Text, out float targetGlucoseValueFloat) ||
                !Helper.ConvertToFloat(carbScalar.Text, out float carbScalarFloat) ||
                    !Helper.ConvertToFloat(glucoseScalar.Text, out float glucoseScalarFloat))
            {
                await DisplayAlert("Error", "All fields must be filled out", "OK");
                return;
            }

            DayProfile.Name = name.Text;
            DayProfile.TargetGlucoseValue = targetGlucoseValueFloat;
            DayProfile.StartTime = (ushort)(timePickerStartTime.Time.Hours * 100 + timePickerStartTime.Time.Minutes);

            //------------------------------

            DayProfile.CarbScalar = carbScalarFloat;
            DayProfile.GlucoseScalar = glucoseScalarFloat;

            //------------------------------

            if (DayProfile.DayProfileID == -1) //The DayProfile is not in the database
                DayProfileAdded?.Invoke(this, DayProfile);
            else
                DayProfileSaved?.Invoke(this, DayProfile);

            await Navigation.PopAsync();
        }
    }
}
