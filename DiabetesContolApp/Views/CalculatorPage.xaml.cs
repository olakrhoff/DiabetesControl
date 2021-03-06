using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using DiabetesContolApp.Models;
using DiabetesContolApp.Service;
using DiabetesContolApp.GlobalLogic;

using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace DiabetesContolApp.Views
{

    public partial class CalculatorPage : ContentPage
    {

        public ObservableCollection<DayProfileModel> DayProfiles { get; set; }
        public ObservableCollection<NumberOfGroceryModel> NumberOfGroceriesSummary { get; set; }
        private LogModel _tempLog = new();

        private DayProfileService dayProfileService = DayProfileService.GetDayProfileService();
        private LogService logService = LogService.GetLogService();

        public CalculatorPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            List<DayProfileModel> dayProfiles = await dayProfileService.GetAllDayProfilesAsync();
            dayProfiles.Sort(); //Sort the elements

            DayProfiles = new ObservableCollection<DayProfileModel>(dayProfiles);
            pickerDayprofiles.ItemsSource = DayProfiles;
            pickerDayprofiles.SelectedItem = GetDayProfileByTime();

            if (pickerDayprofiles.SelectedItem == null && DayProfiles.Count > 0)
                pickerDayprofiles.SelectedItem = DayProfiles[0];

            base.OnAppearing();
        }

        /// <summary>
        /// This method disables the glucose field and shows a label
        /// if there is an overlapping log entry. If not, it sets it back.
        /// </summary>
        /// <param name="isOverlapping">
        /// True if there is an overlap, else false
        /// </param>
        /// <param name="previousLog">
        /// The previous log entry that overlapps with the potensial new one,
        /// needed to get the TargetGlucoseValue from the respective DayProfile.
        /// </param>
        /// <returns>void</returns>
        private void SetOverlappingMeals(bool isOverlapping, LogModel previousLog)
        {
            overlappingMealLabel.IsVisible = isOverlapping; //Visible if overlapping
            glucose.IsEnabled = !isOverlapping; //Enabled if not overlapping
            if (isOverlapping)
                glucose.Text = previousLog.DayProfile.TargetGlucoseValue.ToString();

            _tempLog.Reminder = isOverlapping ? previousLog.Reminder : null;
        }

        private DayProfileModel GetDayProfileByTime()
        {

            if (DayProfiles.Count == 0)
                return null;

            bool valid = false;
            DayProfileModel prev = new();
            foreach (DayProfileModel dayProfile in DayProfiles)
            {
                if (dayProfile.StartTime.TimeOfDay <= DateTime.Now.TimeOfDay && dayProfile.StartTime.TimeOfDay >= prev.StartTime.TimeOfDay)
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

            if (Helper.ConvertToFloat(glucose.Text, out float glucoseFloat))
            {
                DayProfileModel dayProfile = pickerDayprofiles.SelectedItem as DayProfileModel;

                _tempLog.GlucoseAtMeal = glucoseFloat;
                _tempLog.DayProfile = dayProfile;
                if (NumberOfGroceriesSummary != null)
                    _tempLog.NumberOfGroceries = NumberOfGroceriesSummary.ToList();

                //Data is valid, continue with calculations
                Helper.CalculateInsulin(ref _tempLog);

                insulinEstimate.Text = String.Format("{0:F1}", _tempLog.InsulinEstimate);
                insulinEstimate.IsVisible = true;
                logInsulinButton.IsEnabled = true;
            }
            else
            {
                ClearCalculatorData();
                await DisplayAlert("Invalid data", "Glucose and carbohydrates must be numbers", "OK");
            }
        }

        async void LogInsulinClicked(System.Object sender, System.EventArgs e)
        {
            if (!await VaildateCalculatorData() ||
                _tempLog.InsulinEstimate < 0 ||
                !Helper.ConvertToFloat(insulinEstimate.Text, out float insulinFromUserFloat))
                return;

            _tempLog.InsulinFromUser = insulinFromUserFloat;
            _tempLog.DateTimeValue = DateTime.Now;

            if (!await logService.InsertLogAsync(_tempLog))
                await DisplayAlert("Error", "Something went wrong when added the new log.", "OK");

            ClearCalculatorData();
            SetOverlappingMeals(false, null); //Enable the glucose entry
        }

        private void ClearCalculatorData()
        {
            glucose.Text = insulinEstimate.Text = "";
            insulinEstimate.IsVisible = logInsulinButton.IsEnabled = false;
            NumberOfGroceriesSummary?.Clear();
        }

        /*
         * This method validates that all the neccesary fileds
         * on the calculator page is valid, so the calculations
         * can be done.
         * 
         * Parmas: None
         * 
         * Return: Task<bool>, Task for async, returns true if every field
         * is valid, else false.
         */
        async private Task<bool> VaildateCalculatorData()
        {
            App globalVariables = Application.Current as App;

            bool propertiesChanged = false;

            if (globalVariables.InsulinToCarbohydratesRatio == -1.0f)
            {
                propertiesChanged = true;
                string result = await DisplayPromptAsync("We don't have your insulin-carbs-ratio", "How many units of insulin did you set yesterday?", keyboard: Keyboard.Numeric);
                if (Helper.ConvertToFloat(result, out float resultFloat))
                    globalVariables.InsulinToCarbohydratesRatio = Helper.Calculate500Rule(resultFloat);
            }
            if (globalVariables.InsulinToGlucoseRatio == -1.0f)
            {
                propertiesChanged = true;
                string result = await DisplayPromptAsync("We don't have your insulin-glucose-ratio", "How many units of insulin did you set yesterday?", keyboard: Keyboard.Numeric);
                if (Helper.ConvertToFloat(result, out float resultFloat))
                    globalVariables.InsulinToGlucoseRatio = Helper.Calculate100Rule(resultFloat);
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
            Content.LayoutTo(new Rectangle(Content.Bounds.Left, -200, Content.Bounds.Width, Content.Bounds.Height));
        }

        /*
         * This method is used to move the entry down after the keyboard
         * dissapears
         * 
         * See: void InsulinEstimateEntryFocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
         */
        void InsulinEstimateEntryUnfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            Content.LayoutTo(new Rectangle(Content.Bounds.Left, 0, Content.Bounds.Width, Content.Bounds.Height));
        }

        /// <summary>
        /// When the glucose entry is focused it checks
        /// if there is an active reminder which overlaps with
        /// the current time, then if it is overlapping
        /// then the filed is overridden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void GlucoseEntryFocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            LogModel log = await logService.GetNewestLogAsync();
            if (log == null || log.Reminder == null)
                SetOverlappingMeals(false, null); //No log, then there is no overlap

            //The previous log overlaps in time with the
            //new log, if it is to be added now
            else
                SetOverlappingMeals(!log.Reminder.ReadyToHandle(), log);
        }
    }
}
