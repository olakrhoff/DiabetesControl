﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;

using Xamarin.Forms;

namespace DiabetesContolApp.Views
{
    public partial class LogPage : ContentPage
    {
        public ObservableCollection<LogModel> Logs { get; set; }
        LogDatabase logDatabase = LogDatabase.GetInstance();
        DateTime localDate = DateTime.Now;

        public LogPage()
        {
            InitializeComponent();
        }

        async protected override void OnAppearing()
        {
            var logs = await logDatabase.GetLogsAsync(localDate);

            Logs = new(logs);

            logList.ItemsSource = Logs;

            base.OnAppearing();
        }

        void LogListItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
        }

        async void AddNewClicked(System.Object sender, System.EventArgs e)
        {
            LogDetailPage page = new();

            await Navigation.PushAsync(page);
        }

        void OnDeleteClicked(System.Object sender, System.EventArgs e)
        {
        }
    }
}
