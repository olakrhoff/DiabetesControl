using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;
using DiabetesContolApp.GlobalLogic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SQLite;

namespace DiabetesContolApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GrocerySelectionListPage : ContentPage
    {
        public event EventHandler<List<NumberOfGroceryModel>> NumberOfGroceryListSaved;
        public event EventHandler<NumberOfGroceryModel> NumberOfGroceryDeleted;

        public ObservableCollection<NumberOfGroceryModel> Groceries { get; set; }
        private List<NumberOfGroceryModel> GroceriesAdded { get; set; }
        private readonly SQLiteAsyncConnection connection;

        public GrocerySelectionListPage(List<NumberOfGroceryModel> groceries = null)
        {
            GroceriesAdded = groceries;
            connection = DependencyService.Get<ISQLiteDB>().GetConnection();

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            if (Groceries == null)
                Groceries = new ObservableCollection<NumberOfGroceryModel>(await GetNumberOfGroceries());
            groceriesList.ItemsSource = Groceries;

            if (GroceriesAdded != null)
                foreach (NumberOfGroceryModel numberOfGrocery in GroceriesAdded)
                {
                    var index = Groceries.IndexOf(numberOfGrocery);
                    if (index < 0)
                        continue;
                    Groceries.ElementAt(index).NumberOfGrocery = numberOfGrocery.NumberOfGrocery;
                }

            base.OnAppearing();
        }

        async private Task<List<NumberOfGroceryModel>> GetNumberOfGroceries()
        {
            await connection.CreateTableAsync<GroceryModel>(); //Creates table if it does not already exist
            var groceries = await connection.Table<GroceryModel>().ToListAsync();
            groceries.Sort();

            return NumberOfGroceryModel.GetNumberOfGroceries(groceries);
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
                Groceries.Add(new NumberOfGroceryModel(args));
                Groceries = Helper.SortObservableCollection(Groceries);
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
                NumberOfGroceryDeleted?.Invoke(this, grocery);
                await connection.DeleteAsync(grocery.Grocery);
            }
        }

        async void SaveClicked(System.Object sender, System.EventArgs e)
        {
            NumberOfGroceryListSaved?.Invoke(this, Groceries.Where(g => g.NumberOfGrocery > 0).ToList());

            await Navigation.PopAsync();
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

        void SearchBarTextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(e.NewTextValue))
            {
                groceriesList.ItemsSource = Groceries;
                return;
            }
            groceriesList.ItemsSource = Groceries.Where(numberOfGrocery => numberOfGrocery.Grocery.Name.ToLower().Contains(e.NewTextValue.ToLower())).ToList<NumberOfGroceryModel>();
        }
    }
}
