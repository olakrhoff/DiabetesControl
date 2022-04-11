﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface ILogRepo
    {
        public Task<bool> InsertLogAsync(LogModel newLog);
        public Task<bool> DeleteLogAsync(int logID);
        public Task<List<LogModel>> GetAllLogsWithReminderIDAsync(int reminderID);
        public Task<List<LogModel>> GetAllLogsWithDayProfileIDAsync(int dayProfileID);
        public Task<List<LogModel>> GetAllLogsAsync();
        public Task<bool> DeleteAllLogsAsync(List<int> logIDs);
        public Task<LogModel> GetLogAsync(int logID);
        public Task<bool> UpdateLogAsync(LogModel log);
    }
}
