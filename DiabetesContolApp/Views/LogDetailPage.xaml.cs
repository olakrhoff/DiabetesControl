using System;
using System.Collections.Generic;

using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;

using Xamarin.Forms;

namespace DiabetesContolApp.Views
{
    public partial class LogDetailPage : ContentPage
    {
        public LogModel Log { get; set; }

        public LogDetailPage(LogModel log = null)
        {
            Log = log == null ? new() : log;

            InitializeComponent();

            BindingContext = Log;
        }
    }
}
