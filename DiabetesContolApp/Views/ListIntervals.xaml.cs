using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiabetesContolApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListIntervals : ContentPage
    {
        private SQLiteAsyncConnection connection;
        public ObservableCollection<Interval> Intervals { get; set; }

        public ListIntervals()
        {
            InitializeComponent();

            connection = DependencyService.Get<ISQLiteDB>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            //When the DB needs to be rebuilt
            //await connection.DropTableAsync<Interval>();

            await connection.CreateTableAsync<Interval>();
            var intervals = await connection.Table<Interval>().ToListAsync();
            intervals.Sort(); //Sort the elements
            Intervals = new ObservableCollection<Interval>(intervals);
            MyListView.ItemsSource = Intervals;


            base.OnAppearing();
        }

        async void IntervalTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {
                return;
            }

            var selectedInterval = e.Item as Interval;
            MyListView.SelectedItem = null;

            var page = new IntervalDetail(selectedInterval);

            page.IntervalSaved += async (source, args) =>
            {
                selectedInterval.Name = args.Name;
                selectedInterval.TimeStart = args.TimeStart;
                selectedInterval.BloodSkalar = args.BloodSkalar;
                selectedInterval.KarbSkalar = args.KarbSkalar;
                selectedInterval.TargetBloodSugar = args.TargetBloodSugar;
                await connection.UpdateAsync(selectedInterval); //TODO: Add exception handling of this
            };

            await Navigation.PushAsync(page);
        }

        async void OnAdd(System.Object sender, System.EventArgs e)
        {
            var interval = new Interval
            {
                ID = -1,
                TimeStart = 0
            };


            var page = new IntervalDetail(interval);

            page.IntervalAdded += async (source, args) =>
            {
                interval.Name = args.Name;
                interval.TimeStart = args.TimeStart;
                interval.BloodSkalar = args.BloodSkalar;
                interval.KarbSkalar = args.KarbSkalar;
                interval.TargetBloodSugar = args.TargetBloodSugar;
                await connection.InsertAsync(interval); //TODO: Add exception handling of this
            };

            Intervals.Add(interval);
            await Navigation.PushAsync(page);
        }

        async void OnRemove(System.Object sender, System.EventArgs e)
        {
            var interval = (sender as MenuItem).CommandParameter as Interval;

            if (await DisplayAlert("Deleting", $"Are you sure you want to delete {interval.Name}?", "Delete", "Cancel"))
            {
                Intervals.Remove(interval);
                await connection.DeleteAsync(interval);
            }
        }
    }
}
