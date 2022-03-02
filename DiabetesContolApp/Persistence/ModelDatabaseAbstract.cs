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
            connection.CreateTableAsync<ReminderModel>().Wait();
            connection.CreateTableAsync<LogModel>().Wait();
            connection.CreateTableAsync<GroceryLogModel>().Wait();
        }
    }
}