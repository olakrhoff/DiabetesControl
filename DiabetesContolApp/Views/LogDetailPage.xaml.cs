using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;
using DiabetesContolApp.GlobalLogic;

using Xamarin.Forms;

namespace DiabetesContolApp.Views
{
    public partial class LogDetailPage : ContentPage
    {
        public LogModel Log { get; set; }
        public event EventHandler<LogModel> LogSaved;
        public event EventHandler<LogModel> LogAdded;

        private ObservableCollection<NumberOfGroceryModel> NumberOfGrocerySummary;
        public ObservableCollection<DayProfileModel> DayProfiles { get; set; }

        DayProfileDatabase dayProfileDatabase = DayProfileDatabase.GetInstance();

        public LogDetailPage(LogModel log)
        {
            Log = log == null ? new() : log;

            InitializeComponent();

            BindingContext = Log;

            if (log.LogID == -1) //A new log entry
            {
                NumberOfGrocerySummary = new();
                glucoseAtMeal.Text = insulinFromUser.Text = "";
                timePickerTimeOfMeal.Time = DateTime.Now.TimeOfDay;
                datePickerDateOfMeal.Date = Log.DateTimeValue.Date;
            }
            else
            {
                timePickerTimeOfMeal.Time = log.DateTimeValue.TimeOfDay;
                datePickerDateOfMeal.Date = log.DateTimeValue.Date;
                NumberOfGrocerySummary = new(log.NumberOfGroceryModels);
            }

            groceriesAddedList.ItemsSource = NumberOfGrocerySummary;
        }

        async protected override void OnAppearing()
        {
            var dayProfiles = await dayProfileDatabase.GetDayProfilesAsync();
            dayProfiles.Sort(); //Sort the elements

            DayProfiles = new ObservableCollection<DayProfileModel>(dayProfiles);
            dayProfilePicker.ItemsSource = DayProfiles;

            int index = -1;
            if (Log.DayProfileID != -1)
                for (int i = 0; i < DayProfiles.Count; ++i)
                    if (DayProfiles[i].DayProfileID == Log.DayProfileID)
                    {
                        index = i;
                        break;
                    }
            if (index != -1)
                dayProfilePicker.SelectedItem = DayProfiles[index];
            else if (DayProfiles.Count > 0)
                dayProfilePicker.SelectedItem = DayProfiles[0];

            base.OnAppearing();
        }

        async void EditGroceriesClicked(System.Object sender, System.EventArgs e)
        {
            GrocerySelectionListPage page = NumberOfGrocerySummary == null ? new() : new(NumberOfGrocerySummary.ToList());

            page.NumberOfGroceryListSaved += (source, args) =>
            {
                NumberOfGrocerySummary = new(args);
                groceriesAddedList.ItemsSource = NumberOfGrocerySummary;
            };

            page.NumberOfGroceryDeleted += (source, args) =>
            {
                if (NumberOfGrocerySummary != null)
                    NumberOfGrocerySummary.Remove(args);
            };

            await Navigation.PushAsync(page);
        }

        async void SaveClicked(System.Object sender, System.EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(glucoseAtMeal.Text) ||
                String.IsNullOrWhiteSpace(insulinFromUser.Text) ||
                !Helper.ConvertToFloat(glucoseAtMeal.Text, out float glucoseAtMealFloat) ||
                !Helper.ConvertToFloat(insulinFromUser.Text, out float insulinFromUserFloat))
            {
                await DisplayAlert("Error", "All fields must be filled out", "OK");
                return;
            }

            if (!Helper.ConvertToFloat(glucoseAfterMeal.Text, out float glucoseAfterMealFloat))
                Log.GlucoseAfterMeal = null;
            else
                Log.GlucoseAfterMeal = glucoseAfterMealFloat;

            Log.GlucoseAtMeal = glucoseAtMealFloat;
            Log.DateTimeValue = new DateTime(datePickerDateOfMeal.Date.Year, datePickerDateOfMeal.Date.Month, datePickerDateOfMeal.Date.Day, timePickerTimeOfMeal.Time.Hours, timePickerTimeOfMeal.Time.Minutes, 0);
            Log.InsulinFromUser = insulinFromUserFloat;
            Log.DayProfileID = (dayProfilePicker.SelectedItem as DayProfileModel).DayProfileID;

            Log.NumberOfGroceryModels = NumberOfGrocerySummary.ToList();

            //Calcualte the estimated insulin
            float insulinEsitmate = Helper.CalculateInsulin(Log.GlucoseAtMeal, Log.NumberOfGroceryModels, await dayProfileDatabase.GetDayProfileAsync(Log.DayProfileID));

            Log.InsulinEstimate = insulinEsitmate;

            if (Log.LogID == -1)
                LogAdded?.Invoke(this, Log);
            else
                LogSaved?.Invoke(this, Log);

            await Navigation.PopAsync();
        }
    }
}
