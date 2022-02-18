using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace DiabetesContolApp.GlobalLogic
{
    static public class Helper
    {
        /*
         * This method converts a string to a float.
         * It ignores if it uses a ',' or '.' as a seperator,
         * but it can only have one of them in the hole string.
         * 
         * Returns false if the format of the number is wrong. Set the float_val to 0.0f.
         * Returns true if format is correct, then convert the string to a float and store it in float_val.
         * 
         */
        static public bool ConvertToFloat(string string_val, out float float_val)
        {
            float_val = 0.0f; //This is just a temp value, if the convertion is successful the value will be changed
            if (string_val == null)
                return false;
            char[] seperators = { ',', '.' };

            ushort counter = (ushort)string_val.Count(count => (count == seperators[0] || count == seperators[1]));

            if (counter > 1)
                return false;

            //If there isn't a seperator we can treat it as an integer
            if (counter != 0)
            {
                //The string has only one comma or dot in it
                int choosenSeperator;
                if (string_val.Contains(seperators[0]))
                    choosenSeperator = 0;
                else
                    choosenSeperator = 1;

                //Check that the number seperator is in a valid place
                var seperatorIndex = string_val.IndexOf(seperators[choosenSeperator]);
                if (seperatorIndex == 0 || seperatorIndex == string_val.Length - 1)
                    return false; //Not a valid placment of the seperator

                //If the seperator doesn't match the current culture, then swap it with the other correct one
                if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator != seperators[choosenSeperator].ToString())
                    string_val = string_val.Replace(seperators[choosenSeperator], System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]);
            }
           

            //Try to convert the string to float
            try
            {
                float_val = Convert.ToSingle(string_val);
            }
            catch (FormatException fe)
            {
                //Format was not valid
                return false;
            }
            catch (Exception e)
            {
                //This should not happen, but for safety it is still here
                return false;
            }
            
            return true;
        }

        static public ObservableCollection<T> SortObservableCollection<T>(ObservableCollection<T> observableCollection)
        {
            var list = observableCollection.ToList();
            list.Sort();

            return new(list);
        }
    }
}
