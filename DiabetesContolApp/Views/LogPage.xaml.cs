﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using DiabetesContolApp.Models;
using DiabetesContolApp.Persistence;
using DiabetesContolApp.GlobalLogic;

using Xamarin.Forms;
using System.Threading.Tasks;

namespace DiabetesContolApp.Views
{
    public partial class LogPage : ContentPage
    {
        public ObservableCollection<LogModel> Logs { get; set; }
        LogDatabase logDatabase = LogDatabase.GetInstance();
        DateTime localDate = DateTime.Now;

        private string dateString
        {
            get
            {
                return localDate.ToString("dd/MM");
            }
        }

        public LogPage()
        {
            InitializeComponent();

            Logs = new();

            logList.ItemsSource = Logs;
        }

        async protected override void OnAppearing()
        {
            GetLogsForDate();

            labelDate.Text = localDate.Date.ToString("dd/ MMM");

            base.OnAppearing();
        }

        async private void GetLogsForDate()
        {
            var logs = await logDatabase.GetLogsAsync(localDate);
            logs.Sort();
            logs.Reverse();
            Logs = new(logs);

            logList.ItemsSource = Logs;
        }

        async void LogListItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            LogDetailPage page = new(e.Item as LogModel);

            logList.SelectedItem = null;

            page.LogSaved += async (source, args) =>
            {
                await logDatabase.UpdateLogAsync(args);
            };

            await Navigation.PushAsync(page);
        }

        async void AddNewClicked(System.Object sender, System.EventArgs e)
        {
            LogDetailPage page = new();

            page.LogAdded += async (source, args) =>
            {
                await logDatabase.InsertLogAsync(args);
            };

            await Navigation.PushAsync(page);
        }

        async void OnDeleteClicked(System.Object sender, System.EventArgs e)
        {
            await logDatabase.DeleteLogAsync((sender as MenuItem).CommandParameter as LogModel);
        }

        void PreviousDateClicked(System.Object sender, System.EventArgs e)
        {
            localDate = localDate.AddDays(-1);
            GetLogsForDate();
            labelDate.Text = localDate.Date.ToString("dd/ MMM");
            nextDateButton.IsEnabled = true;
        }

        void NextDateClicked(System.Object sender, System.EventArgs e)
        {
            localDate = localDate.AddDays(1);
            GetLogsForDate();
            labelDate.Text = localDate.Date.ToString("dd/ MMM");

            if (localDate.Date.Equals(DateTime.Now.Date))
                nextDateButton.IsEnabled = false;
        }
    }
}
