using System;
using System.Collections.Generic;
using DiabetesContolApp.Models;
using DiabetesContolApp.GlobalLogic;

using Xamarin.Forms;

namespace DiabetesContolApp.Views
{
    public partial class GroceryDetailPage : ContentPage
    {
        public GroceryModel Grocery { get; set; }
        public event EventHandler<GroceryModel> GrocerySaved;
        public event EventHandler<GroceryModel> GroceryAdded;

        public GroceryDetailPage(GroceryModel grocery)
        {
            if (grocery == null)
                return;

            Grocery = grocery;

            InitializeComponent();

            BindingContext = Grocery;
        }

        async void SaveClicked(System.Object sender, System.EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(name.Text) ||
                String.IsNullOrWhiteSpace(carbsPer100g.Text) ||
                String.IsNullOrWhiteSpace(nameOfPortion.Text) ||
                String.IsNullOrWhiteSpace(gramsPerPortion.Text) ||
                !Helper.ConvertToFloat(carbsPer100g.Text, out float carbsPer100gFloat) ||
                !Helper.ConvertToFloat(gramsPerPortion.Text, out float gramsPerPortionFloat))
            {
                await DisplayAlert("Error", "All fields must be filled out", "OK");
                return;
            }

            Grocery.Name = name.Text;
            Grocery.CarbsPer100Grams = carbsPer100gFloat;
            Grocery.NameOfPortion = nameOfPortion.Text;
            Grocery.GramsPerPortion = gramsPerPortionFloat;

            if (Grocery.GroceryID == -1) //The grocery is not in the database
                GroceryAdded?.Invoke(this, Grocery);
            else
                GrocerySaved?.Invoke(this, Grocery);

            await Navigation.PopAsync();
        }
    }
}
