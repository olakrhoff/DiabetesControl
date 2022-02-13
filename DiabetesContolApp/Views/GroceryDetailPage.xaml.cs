using System;
using System.Collections.Generic;
using DiabetesContolApp.Models;

using Xamarin.Forms;

namespace DiabetesContolApp.Views
{
    public partial class GroceryDetailPage : ContentPage
    {
        public GroceryModel Grocery { get; set; }

        public GroceryDetailPage(GroceryModel grocery)
        {
            Grocery = grocery;

            InitializeComponent();

            BindingContext = Grocery;

            if (Grocery.GetID() == -1)
            {

            }
            else
            {

            }

        }
    }
}
