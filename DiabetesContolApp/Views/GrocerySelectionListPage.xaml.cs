using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SQLite;

namespace DiabetesContolApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GrocerySelectionListPage : ContentPage
    {
        public ObservableCollection<NumberOfGroceryModel> Groceries { get; set; }
        private SQLiteAsyncConnection connection;

        public GrocerySelectionListPage(List<NumberOfGroceryModel> groceries = null)
        {
            if (groceries != null)
                throw new NotImplementedException("Må implementere at tidligere lagt inn groceries dukker opp om man går inn på nytt");

            InitializeComponent();

            connection = DependencyService.Get<ISQLiteDB>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            Groceries = new ObservableCollection<NumberOfGroceryModel>(await GetNumberOfGroceries());
            groceriesList.ItemsSource = Groceries;

            base.OnAppearing();
        }

        async private Task<List<NumberOfGroceryModel>> GetNumberOfGroceries(string searchText = null)
        {
            await connection.CreateTableAsync<GroceryModel>(); //Creates table if it does not already exist

            var numberOfGroceryList = NumberOfGroceryModel.GetNumberOfGroceries(await connection.Table<GroceryModel>().ToListAsync());

            if (String.IsNullOrWhiteSpace(searchText))
                return numberOfGroceryList;

            return numberOfGroceryList.Where(numberOfGrocery => numberOfGrocery.Grocery.Name.ToLower().Contains(searchText.ToLower())).ToList<NumberOfGroceryModel>();
        }

        async void AddNewClicked(System.Object sender, System.EventArgs e)
        {
            var grocery = new GroceryModel
            {
                GroceryID = -1,
                CarbScalar = 1.0f
            };

            var page = new GroceryDetailPage(grocery);

            page.GroceryAdded += async (source, args) =>
            {
                await connection.InsertAsync(args);
            };

            await Navigation.PushAsync(page);
        }

        void GroceriesListItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (e == null)
                return;

            var selectedGrocery = (e as ItemTappedEventArgs).Item as NumberOfGroceryModel;
            groceriesList.SelectedItem = null;
            selectedGrocery.NumberOfGrocery++;
        }

        async void DeleteClicked(System.Object sender, System.EventArgs e)
        {
            NumberOfGroceryModel grocery = (sender as MenuItem).CommandParameter as NumberOfGroceryModel;
            if (await DisplayAlert("Deleting", $"Are you sure you want to delete {grocery.Grocery.Name}?", "Delete", "Cancel"))
            {
                Groceries.Remove(grocery);
                await connection.DeleteAsync(grocery);
            }
        }

        void SaveClicked(System.Object sender, System.EventArgs e)
        {
        }

        async void EditClicked(System.Object sender, System.EventArgs e)
        {
            if ((sender as MenuItem).CommandParameter is not NumberOfGroceryModel selectedGrocery)
                return;

            groceriesList.SelectedItem = null;

            var page = new GroceryDetailPage(selectedGrocery.Grocery);

            page.GrocerySaved += async (source, args) =>
            {
                await connection.UpdateAsync(args);
            };

            await Navigation.PushAsync(page);
        }

        async void SearchBarTextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            groceriesList.ItemsSource = Groceries = new ObservableCollection<NumberOfGroceryModel>(await GetNumberOfGroceries(e.NewTextValue));
        }
    }
}
