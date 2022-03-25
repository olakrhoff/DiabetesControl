using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            connection.CreateTableAsync<ReminderModelDAO>().Wait();
            //connection.DropTableAsync<LogModel>().Wait();
            connection.CreateTableAsync<LogModel>().Wait();
            //connection.DropTableAsync<GroceryLogModel>().Wait();
            connection.CreateTableAsync<GroceryLogModel>().Wait();
            connection.CreateTableAsync<AverageTDDModel>().Wait();
        }

        public abstract Task<List<IModel>> GetAllAsync();
        public abstract string HeaderForCSVFile();
    }
}