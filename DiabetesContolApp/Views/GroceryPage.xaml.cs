using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SQLite;

namespace DiabetesContolApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroceryPage : ContentPage
    {
        public ObservableCollection<GroceryModel> Groceries { get; set; }
        private GroceryDatabase groceryDatabase = GroceryDatabase.GetInstance();

        public GroceryPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var groceries = await groceryDatabase.GetGroceriesAsync();
            groceries.Sort();
            Groceries = new(groceries);
            groceriesList.ItemsSource = Groceries;

            base.OnAppearing();
        }

        async void AddNewClicked(System.Object sender, System.EventArgs e)
        {
            GroceryModel grocery = new();

            var page = new GroceryDetailPage(grocery);

            page.GroceryAdded += async (source, args) =>
            {
                await groceryDatabase.InsertGroceryAsync(args);
            };

            await Navigation.PushAsync(page);
        }

        async void GroceriesListItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            GroceryModel selectedGrocery = e.Item as GroceryModel;
            groceriesList.SelectedItem = null;

            var page = new GroceryDetailPage(selectedGrocery);

            page.GrocerySaved += async (source, args) =>
            { 
                await groceryDatabase.UpdateGroceryAsync(args);
            };

            await Navigation.PushAsync(page);
        }

        async void OnDeleteClicked(System.Object sender, System.EventArgs e)
        {
            GroceryModel grocery = (sender as MenuItem).CommandParameter as GroceryModel;
            if (await DisplayAlert("Deleting", $"Are you sure you want to delete {grocery.Name}?", "Delete", "Cancel"))
            {
                Groceries.Remove(grocery);
                await groceryDatabase.DeleteGroceryAsync(grocery);
            }
        }
    }
}
