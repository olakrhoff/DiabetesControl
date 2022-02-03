using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;
using SQLite;
using Xamarin.Forms;

namespace DiabetesContolApp.Views
{
    public partial class Calculator : ContentPage
    {
        private SQLiteAsyncConnection connection;
        public ObservableCollection<Interval> Intervals { get; set; }

        public Calculator()
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
            picker.ItemsSource = Intervals;
            picker.SelectedItem = getIntervalByTime();
            if (picker.SelectedItem == null && Intervals.Count > 0)
            {
                picker.SelectedItem = Intervals[0];
            }

            base.OnAppearing();
        }

        private Interval getIntervalByTime()
        {
            //TODO: handle empty list
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
            if (String.IsNullOrEmpty(bloodsugar.Text) || String.IsNullOrEmpty(karbs.Text))
            {
                labelOutput.Text = "";
                await DisplayAlert("Mangler data", "Blodsukker og karbohydrater må settes", "OK");
            }
            else if (Double.TryParse(bloodsugar.Text, out double bloodsugarNumber) && Double.TryParse(karbs.Text, out double karbsNumber))
            {
                Interval selectedInterval = picker.SelectedItem as Interval;
                double korrigering = (bloodsugarNumber - selectedInterval.TargetBloodSugar) * selectedInterval.BloodSkalar;
                double food = (karbsNumber / (Application.Current as App).BaseSensitivity) * selectedInterval.KarbSkalar;
                double newValue = food != 0d ? korrigering + food : korrigering * (Application.Current as App).SenesitivityWithoutFood;
                if (newValue > 0)
                    labelOutput.Text = String.Format("{0:F1} Enheter", newValue);
                else
                    labelOutput.Text = "0 Enheter, du burde spise";
                labelOutput.IsVisible = true;
                String t = (Application.Current as App).BaseSensitivity + " " + food + " " + newValue;
                //await DisplayAlert("SE HER", t, "Ferdig");
            }
            else
            {
                labelOutput.Text = "";
                await DisplayAlert("Feil data", "Blodsukker og karbohydrater må settes til tall", "OK");
            }
        }
    }
}
