﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiabetesContolApp.Models;

namespace DiabetesContolApp.Service
{
    public class LogService
    {
        public LogService()
        {
        }

        async public Task<bool> InsertLogAsync(LogModel newLog)
        {
            throw new NotImplementedException();
        }

        async public Task<bool> DeleteLogAsync(int logID)
        {
            throw new NotImplementedException();
        }

        async public Task<List<LogModel>> GetLogsAsync(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        async public Task<bool> UpdateLogAsync(LogModel log)
        {
            throw new NotImplementedException();
        }

        async public Task<LogModel> GetNewestLogAsync()
        {
            throw new NotImplementedException();
        }
    }
}
