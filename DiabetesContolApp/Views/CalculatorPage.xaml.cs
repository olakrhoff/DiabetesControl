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
        public ObservableCollection<DayProfileModel> DayProfiles { get; set; }

        public CalculatorPage()
        {
            InitializeComponent();

            connection = DependencyService.Get<ISQLiteDB>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            await connection.CreateTableAsync<DayProfileModel>();
            var dayProfiles = await connection.Table<DayProfileModel>().ToListAsync();
            dayProfiles.Sort(); //Sort the elements
            DayProfiles = new ObservableCollection<DayProfileModel>(dayProfiles);
            pickerDayprofiles.ItemsSource = DayProfiles;
            pickerDayprofiles.SelectedItem = GetDayProfileByTime();
            if (pickerDayprofiles.SelectedItem == null && DayProfiles.Count > 0)
                pickerDayprofiles.SelectedItem = DayProfiles[0];

            base.OnAppearing();
        }

        private DayProfileModel GetDayProfileByTime()
        {
            
            if (DayProfiles.Count == 0)
                return null;

            bool valid = false;
            DayProfileModel prev = new DayProfileModel();
            int timeNow = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
            foreach (DayProfileModel dayProfile in DayProfiles)
            {
                if (dayProfile.StartTime <= timeNow && dayProfile.StartTime >= prev.StartTime)
                {
                    prev = dayProfile;
                    valid = true;
                }
                else
                    break; //Since the list is sorted we can exit here
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

        async void AddGroceriesClicked(System.Object sender, System.EventArgs e)
        {
            var page = new GrocerySelectionListPage();

            await Navigation.PushAsync(page);
        }

        void LogInsulinClicked(System.Object sender, System.EventArgs e)
        {
            //TODO: Implement
        }
    }
}
