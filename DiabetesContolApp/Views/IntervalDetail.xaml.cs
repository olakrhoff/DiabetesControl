using System;
using System.Collections.Generic;
using DiabetesContolApp.Models;
using DiabetesContolApp.GlobalLogic;
using Xamarin.Forms;

namespace DiabetesContolApp.Views
{
    public partial class IntervalDetail : ContentPage
    {
        public Interval Interval { get; set; }
        private bool _new = false;
        public event EventHandler<Interval> IntervalAdded;
        public event EventHandler<Interval> IntervalSaved;

        public IntervalDetail(Interval interval)
        {
            Interval = interval;
            if (interval.ID == -1)
            {
                this._new = true;
            }
            InitializeComponent();

            BindingContext = Interval;
            if (this._new)
            {
                //If there is a new interval we just set the timepicker to whatever the time is at the given moment
                timePicker.Time = DateTime.Now.TimeOfDay;
                targetBloodSugar.Text = "";
                bloodSkalar.Text = "";
                karbSkalar.Text = "";
            }
            else
            {
                //Sets the time picker to the time spesified in the interval
                timePicker.Time = new TimeSpan(Interval.TimeStart / 100, Interval.TimeStart % 100, 0);
            }
        }

        async void SaveClicked(System.Object sender, System.EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(targetBloodSugar.Text) ||
                String.IsNullOrWhiteSpace(bloodSkalar.Text) ||
                String.IsNullOrWhiteSpace(karbSkalar.Text) ||
                !Helper.ConvertToFloat(targetBloodSugar.Text, out float targetBloodSugarFloat) ||
                !Helper.ConvertToFloat(bloodSkalar.Text, out float bloodSkalarFloat) ||
                !Helper.ConvertToFloat(karbSkalar.Text, out float karbSkalarFloat))
            {
                await DisplayAlert("Error", "All fields must be filled", "OK");
                return;
            }

            Interval.TargetBloodSugar = targetBloodSugarFloat;
            Interval.BloodSkalar = bloodSkalarFloat;
            Interval.KarbSkalar = karbSkalarFloat;

            if (String.IsNullOrWhiteSpace(Interval.Name) || Interval.BloodSkalar < 0 || Interval.KarbSkalar < 0 || Interval.TargetBloodSugar < 4)
            {
                await DisplayAlert("Error", "All fields must be filled", "OK");
                return;
            }

            Interval.TimeStart = timePicker.Time.Hours * 100 + timePicker.Time.Minutes;
            if (this._new)
            {
                IntervalAdded?.Invoke(this, Interval);
            }
            else
            {
                IntervalSaved?.Invoke(this, Interval);
            }
            await Navigation.PopAsync();
        }
    }
}
