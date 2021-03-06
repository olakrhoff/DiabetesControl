using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using DiabetesContolApp.Models;
using DiabetesContolApp.Service;

using SQLite;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiabetesContolApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DayProfilePage : ContentPage
    {
        public ObservableCollection<DayProfileModel> DayProfiles { get; set; }

        private DayProfileService dayProfileService = DayProfileService.GetDayProfileService();

        public DayProfilePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var dayProfiles = await dayProfileService.GetAllDayProfilesAsync();
            dayProfiles.Sort();
            DayProfiles = new ObservableCollection<DayProfileModel>(dayProfiles);
            dayProfilesList.ItemsSource = DayProfiles;

            base.OnAppearing();
        }

        async void AddNewClicked(System.Object sender, System.EventArgs e)
        {
            var dayProfile = new DayProfileModel
            {
                DayProfileID = -1,
                CarbScalar = 1.0f,
                GlucoseScalar = 1.0f,
                StartTime = DateTime.Now
            };

            var page = new DayProfileDetailPage(dayProfile);

            page.DayProfileAdded += async (source, args) =>
                {
                    await dayProfileService.InsertDayProfileAsync(args);
                };

            await Navigation.PushAsync(page);
        }

        async void DayProfilesListItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            DayProfileModel selectedDayProfile = e.Item as DayProfileModel;
            dayProfilesList.SelectedItem = null; //Set the selected item to null so that it isn't "greyed out"

            var page = new DayProfileDetailPage(selectedDayProfile);

            page.DayProfileSaved += async (source, args) =>
            {
                await dayProfileService.UpdateDayProfileAsync(args);
            };

            await Navigation.PushAsync(page);
        }

        async void OnDeleteClicked(System.Object sender, System.EventArgs e)
        {
            DayProfileModel dayProfile = (sender as MenuItem).CommandParameter as DayProfileModel;
            if (await DisplayAlert("Deleting", $"Are you sure you want to delete {dayProfile.Name}?", "Delete", "Cancel"))
            {
                DayProfiles.Remove(dayProfile);
                await dayProfileService.DeleteDayProfileAsync(dayProfile.DayProfileID);
            }
        }
    }
}
