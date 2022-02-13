using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;
using DiabetesContolApp.GlobalLogic;
using SQLite;

using Xamarin.Forms;

namespace DiabetesContolApp.Views
{
    public partial class CalculatorPage : ContentPage
    {
        private SQLiteAsyncConnection connection;
        public ObservableCollection<Interval> Intervals { get; set; }

        public CalculatorPage()
        {
            InitializeComponent();

            connection = DependencyService.Get<ISQLiteDB>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            //When the DB needs to be rebuilt
            //await connection.DropTableAsync<Interval>();
            await connection.CreateTableAsync<Interval>();
            var intervals = await connection.Table<Interval>().ToListAsync();
            intervals.Sort(); //Sort the elements
            Intervals = new ObservableCollection<Interval>(intervals);
            pickerDayprofiles.ItemsSource = Intervals;
            pickerDayprofiles.SelectedItem = getIntervalByTime();
            if (pickerDayprofiles.SelectedItem == null && Intervals.Count > 0)
            {
                pickerDayprofiles.SelectedItem = Intervals[0];
            }

            base.OnAppearing();
        }

        private Interval getIntervalByTime()
        {
            if (Intervals.Count == 0)
                return null;

            bool valid = false;
            Interval prev = new Interval();
            int timeNow = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
            foreach (Interval interval in Intervals)
            {
                if (interval.TimeStart <= timeNow && interval.TimeStart >= prev.TimeStart)
                {
                    prev = interval;
                    valid = true;
                }
            }

            return valid ? prev : null;
        }

        async void CalculateClicked(System.Object sender, System.EventArgs e)
        {
            if (String.IsNullOrEmpty(glucose.Text) || String.IsNullOrEmpty(carbs.Text))
            {
                insulinEstimate.Text = "";
                await DisplayAlert("Missing data", "Glucose and carbohydrates must be given", "OK");
            }
            else if (Helper.ConvertToFloat(glucose.Text, out float bloodSugarFloat) && Helper.ConvertToFloat(carbs.Text, out float karbsFloat))
            {
                Interval selectedInterval = pickerDayprofiles.SelectedItem as Interval;
                float korrigeringInsulin = (bloodSugarFloat - selectedInterval.TargetBloodSugar) * selectedInterval.BloodSkalar;
                float foodInsulin = (karbsFloat / (Application.Current as App).BaseSensitivity) * selectedInterval.KarbSkalar;
                float totalInsulin = Math.Abs(foodInsulin) >= 0.1 ? korrigeringInsulin + foodInsulin : korrigeringInsulin * (Application.Current as App).SenesitivityWithoutFood;

                if (totalInsulin > 0)
                    insulinEstimate.Text = String.Format("{0:F1}", totalInsulin);
                else
                    insulinEstimate.Text = "0 Units, you should eat food";
                insulinEstimate.IsVisible = true;
            }
            else
            {
                insulinEstimate.Text = "";
                await DisplayAlert("Wrong data", "Glucose and carbohydrates must be numbers", "OK");
            }
        }

        void AddGroceriesClicked(System.Object sender, System.EventArgs e)
        {
            //TODO: Implement
        }

        void LogInsulinClicked(System.Object sender, System.EventArgs e)
        {
            //TODO: Implement
        }
    }
}
