using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

using DiabetesContolApp.Service;
using DiabetesContolApp.Service.Interfaces;
using DiabetesContolApp.Models;
using DiabetesContolApp.GlobalLogic.Interfaces;

using MathNet.Numerics;

using Xamarin.Forms;

namespace DiabetesContolApp.GlobalLogic
{
    public static class Algorithm
    {
        async public static Task<bool> RunAlgorithmOnReminder(ReminderModel reminder)
        {
            return await AlgorithmBase.RunStatisticsOnReminder(reminder);
        }
    }
}