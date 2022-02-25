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

        public LogDetailPage(LogModel log = null)
        {
            Log = log == null ? new() : log;

            InitializeComponent();

            BindingContext = Log;
            if (log == null) //A new log entry
            {
                NumberOfGrocerySummary = new();
                glucoseAtMeal.Text = insulinFromUser.Text = "";
                timePickerTimeOfMeal.Time = DateTime.Now.TimeOfDay;
            }
            else
            {
                timePickerTimeOfMeal.Time = log.DateTimeValue.TimeOfDay;
                NumberOfGrocerySummary = new(log.NumberOfGroceryModels);
            }

            groceriesAddedList.ItemsSource = NumberOfGrocerySummary;
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
            //TODO: Validate entries

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

            //TODO: Add groceries

            if (Log.LogID == -1)
                LogAdded?.Invoke(this, Log);
            else
                LogSaved?.Invoke(this, Log);

            await Navigation.PopAsync();
        }
    }
}
