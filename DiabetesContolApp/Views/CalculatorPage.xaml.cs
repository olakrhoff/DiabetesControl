using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;
using DiabetesContolApp.GlobalLogic;
using SQLite;

using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace DiabetesContolApp.Views
{

    public partial class CalculatorPage : ContentPage
    {
        
        public ObservableCollection<DayProfileModel> DayProfiles { get; set; }
        public ObservableCollection<NumberOfGroceryModel> NumberOfGroceriesSummary { get; set; }
        private float? _insulinEstimate;

        private DayProfileDatabase dayProfileDatabase = DayProfileDatabase.GetInstance();
        private LogDatabase logDatabase = LogDatabase.GetInstance();

        public CalculatorPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var dayProfiles = await dayProfileDatabase.GetDayProfilesAsync();
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
            DayProfileModel prev = new();
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

        async void AddGroceriesClicked(System.Object sender, System.EventArgs e)
        {
            GrocerySelectionListPage page = NumberOfGroceriesSummary == null ? new() : new(NumberOfGroceriesSummary.ToList());

            page.NumberOfGroceryListSaved += (source, args) =>
            {
                NumberOfGroceriesSummary = new(args);
                groceriesAddedList.ItemsSource = NumberOfGroceriesSummary;
            };

            page.NumberOfGroceryDeleted += (source, args) =>
            {
                if (NumberOfGroceriesSummary != null)
                    NumberOfGroceriesSummary.Remove(args);
            };

            await Navigation.PushAsync(page);
        }

        async void CalculateClicked(System.Object sender, System.EventArgs e)
        {
            if (!await VaildateCalculatorData())
                return;

            App globalVariables = Application.Current as App;

            if (Helper.ConvertToFloat(glucose.Text, out float glucoseFloat))
            {
                //Data is valid, continue with calculations
                DayProfileModel dayProfile = pickerDayprofiles.SelectedItem as DayProfileModel;

                float insulinForFood = GetCarbsFromFood() * dayProfile.CarbScalar / globalVariables.InsulinToCarbohydratesRatio;

                float insulinForCorrection = (glucoseFloat - dayProfile.TargetGlucoseValue) * dayProfile.GlucoseScalar / globalVariables.InsulinToGlucoseRatio;

                //If it is a pure correction dose, no food (carbs)
                if (insulinForFood == 0)
                    insulinForCorrection *= globalVariables.InsulinOnlyCorrectionScalar;

                float totalInsulin = insulinForFood + insulinForCorrection;


                insulinEstimate.Text = String.Format("{0:F1}", totalInsulin);
                insulinEstimate.IsVisible = true;
                logInsulinButton.IsEnabled = true;
                _insulinEstimate = totalInsulin;
            }
            else
            {
                ClearCalculatorData();
                await DisplayAlert("Invalid data", "Glucose and carbohydrates must be numbers", "OK");
            }
        }

        private float GetCarbsFromFood()
        {
            float totalCarbs = 0.0f;

            if (NumberOfGroceriesSummary != null)
                foreach (NumberOfGroceryModel numberOfGrocery in NumberOfGroceriesSummary)
                    totalCarbs += numberOfGrocery.NumberOfGrocery * numberOfGrocery.Grocery.GramsPerPortion * (numberOfGrocery.Grocery.CarbsPer100Grams / 100) * numberOfGrocery.Grocery.CarbScalar;

            return totalCarbs;
        }

        async void LogInsulinClicked(System.Object sender, System.EventArgs e)
        {
            if (!await VaildateCalculatorData() ||
                _insulinEstimate == null ||
                !Helper.ConvertToFloat(insulinEstimate.Text, out float insulinFromUserFloat) ||
                !Helper.ConvertToFloat(glucose.Text, out float glucoseAtMealFloat))
                return;

            LogModel newLogEntry = new((pickerDayprofiles.SelectedItem as DayProfileModel).DayProfileID, DateTime.Now, (float)_insulinEstimate, insulinFromUserFloat, glucoseAtMealFloat, NumberOfGroceriesSummary?.ToList<NumberOfGroceryModel>());

            await logDatabase.InsertLogAsync(newLogEntry);
            
            ClearCalculatorData();
        }

        private void ClearCalculatorData()
        {
            glucose.Text = insulinEstimate.Text = "";
            insulinEstimate.IsVisible = logInsulinButton.IsEnabled = false;
            NumberOfGroceriesSummary?.Clear();
        }

        async private Task<bool> VaildateCalculatorData()
        {
            App globalVariables = Application.Current as App;

            bool propertiesChanged = false;

            if (globalVariables.InsulinToCarbohydratesRatio == -1.0f)
            {
                propertiesChanged = true;
                string result = await DisplayPromptAsync("We don't have your insulin-carbs-ratio", "How many units of insulin did you set yesterday?", keyboard: Keyboard.Numeric);
                if (Helper.ConvertToFloat(result, out float resultFloat))
                    globalVariables.InsulinToCarbohydratesRatio = 500 / resultFloat;
            }
            if (globalVariables.InsulinToGlucoseRatio == -1.0f)
            {
                propertiesChanged = true;
                string result = await DisplayPromptAsync("We don't have your insulin-glucose-ratio", "How many units of insulin did you set yesterday?", keyboard: Keyboard.Numeric);
                if (Helper.ConvertToFloat(result, out float resultFloat))
                    globalVariables.InsulinToGlucoseRatio = 100 / resultFloat;
            }
            if (globalVariables.InsulinOnlyCorrectionScalar == -1.0f)
            {
                propertiesChanged = true;
                string result = await DisplayPromptAsync("We don't have your sensetivity to only correction insulin", "How many percent (as decimal e.g. 0.7) of your normal dose do you use, if only setting for correction?", keyboard: Keyboard.Numeric);
                if (Helper.ConvertToFloat(result, out float resultFloat))
                    globalVariables.InsulinOnlyCorrectionScalar = resultFloat;
            }

            if (propertiesChanged)
                await globalVariables.SavePropertiesAsync();

            if (String.IsNullOrWhiteSpace(glucose.Text))
            {
                ClearCalculatorData();
                await DisplayAlert("Missing data", "Glucose must be given", "OK");
                return false;
            }
            else if (pickerDayprofiles.SelectedItem == null)
            {
                ClearCalculatorData();
                await DisplayAlert("Missing data", "A day profile must be created", "OK");
                return false;
            }

            return true; //If no errers occur
        }

        /*
         * This method is used to move the entry over the keyboard
         * that appears, so that the user can see what they type in
         */
        void InsulinEstimateEntryFocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            Content.LayoutTo(new Rectangle(30, -200, Content.Bounds.Width, Content.Bounds.Height));
        }

        /*
         * This method is used to move the entry down after the keyboard
         * dissapears
         * 
         * See: void InsulinEstimateEntryFocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
         */
        void InsulinEstimateEntryUnfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            Content.LayoutTo(new Rectangle(30, 0, Content.Bounds.Width, Content.Bounds.Height));
        }
    }
}
