using System;
using System.Collections.Generic;
using DiabetesContolApp.Models;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiabetesContolApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroceryPage : ContentPage
    {
        public ObservableCollection<GroceryModel> Groceries { get; set; }

        public GroceryPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var groceries = new ObservableCollection<GroceryModel>
            {
                new GroceryModel
                {
                    Name = "Kake",
                    CarbsPer100Grams = 54.3f,
                    GramsPerPortion = 50,
                    NameOfPortion = "Stykke"
                }
            };

            Groceries = groceries;
            groceriesList.ItemsSource = Groceries;

            base.OnAppearing();
        }

        void AddNewClicked(System.Object sender, System.EventArgs e)
        {
        }

        async void GroceriesListItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            GroceryModel selectedGrocery = e.Item as GroceryModel;
            groceriesList.SelectedItem = null;

            var page = new GroceryDetailPage(selectedGrocery);

            await Navigation.PushAsync(page);
        }

        async void OnDeleteClicked(System.Object sender, System.EventArgs e)
        {
            GroceryModel grocery = (sender as MenuItem).CommandParameter as GroceryModel;
            if (await DisplayAlert("Deleting", $"Are you sure you want to delete {grocery.Name}?", "Delete", "Cancel"))
            {
                Groceries.Remove(grocery);
                //TODO: Database call
            }
        }

        void OnEditClicked(System.Object sender, System.EventArgs e)
        {

        }
    }
}
