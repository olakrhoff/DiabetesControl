using System;
using DiabetesContolApp.Models;
using SQLite;
using Xamarin.Forms;

namespace DiabetesContolApp.Persistence
{
    public abstract class ModelDatabaseAbstract
    {
        protected readonly SQLiteAsyncConnection connection;

        public ModelDatabaseAbstract()
        {
            connection = DependencyService.Get<ISQLiteDB>().GetConnection();

            connection.CreateTableAsync<DayProfileModel>().Wait();
            connection.CreateTableAsync<GroceryModel>().Wait();
            //connection.DropTableAsync<ReminderModel>().Wait();
            connection.CreateTableAsync<ReminderModel>().Wait();
            //connection.DropTableAsync<LogModel>().Wait();
            connection.CreateTableAsync<LogModel>().Wait();
            //connection.DropTableAsync<GroceryLogModel>().Wait();
            connection.CreateTableAsync<GroceryLogModel>().Wait();
        }
    }
}