using System;
using System.Globalization;
using System.Windows;

using Xamarin.Forms;

namespace DiabetesContolApp.GlobalLogic
{
    /*
     * This class is a converter for the datetype DateTime -> string.
     * It is used to bind a DateTime to a XAML object directly,
     * and get the wanted format, this returns the format dd/ MMM,
     * e.g. 1. may
     */
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            DateTime dateTime = (DateTime)value;

            return dateTime != null ? dateTime.Date.ToString("dd/ MMM") : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string dateString = value as string;

            if (DateTime.TryParse(dateString, out DateTime dateFromString))
                return dateFromString;
            return null;
        }
    }
}
