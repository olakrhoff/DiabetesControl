using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.DAO;

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

            connection.CreateTableAsync<DayProfileModelDAO>().Wait();
            connection.CreateTableAsync<GroceryModelDAO>().Wait();
            //connection.DropTableAsync<ReminderModelDAO>().Wait();
            connection.CreateTableAsync<ReminderModelDAO>().Wait();
            //connection.DropTableAsync<LogModelDAO>().Wait();
            connection.CreateTableAsync<LogModelDAO>().Wait();
            //connection.DropTableAsync<GroceryLogModelDAO>().Wait();
            connection.CreateTableAsync<GroceryLogModelDAO>().Wait();
        }

        public abstract Task<List<IModelDAO>> GetAllAsync();
        public abstract string HeaderForCSVFile();
    }
}