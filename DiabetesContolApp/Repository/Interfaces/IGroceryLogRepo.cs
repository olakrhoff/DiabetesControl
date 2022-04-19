﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface IGroceryLogRepo
    {
        Task<bool> InsertAllGroceryLogsAsync(List<GroceryLogModel> groceryLogs, int logID);
        Task<bool> DeleteAllGroceryLogsWithLogIDAsync(int logID);
    }
}
