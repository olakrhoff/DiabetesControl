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
        protected readonly SQLiteAsyncConnection _connection;

        public ModelDatabaseAbstract(SQLiteAsyncConnection connection = null)
        {
            if (connection == null)
                _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            else
                _connection = connection;

            _connection.CreateTableAsync<DayProfileModelDAO>().Wait();
            _connection.CreateTableAsync<GroceryModelDAO>().Wait();
            //connection.DropTableAsync<ReminderModelDAO>().Wait();
            _connection.CreateTableAsync<ReminderModelDAO>().Wait();
            //connection.DropTableAsync<LogModelDAO>().Wait();
            _connection.CreateTableAsync<LogModelDAO>().Wait();
            //connection.DropTableAsync<GroceryLogModelDAO>().Wait();
            _connection.CreateTableAsync<GroceryLogModelDAO>().Wait();
            //connection.DropTableAsync<ScalarModelDAO>().Wait();
            _connection.CreateTableAsync<ScalarModelDAO>().Wait();
        }

        public abstract Task<List<IModelDAO>> GetAllAsync();
        public abstract string HeaderForCSVFile();
    }
}