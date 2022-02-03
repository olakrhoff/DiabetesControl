using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using DiabetesContolApp.iOS.Persistence;
using DiabetesContolApp.Persistence;

[assembly: Dependency(typeof(SQLiteDB))]

namespace DiabetesContolApp.iOS.Persistence
{
    public class SQLiteDB : ISQLiteDB
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentPath, "MySQLite.db");
            return new SQLiteAsyncConnection(path);
        }
    }
}
