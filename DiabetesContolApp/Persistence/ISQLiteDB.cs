using System;
using SQLite;

namespace DiabetesContolApp.Persistence
{
    public interface ISQLiteDB
    {
        SQLiteAsyncConnection GetConnection();
    }
}
