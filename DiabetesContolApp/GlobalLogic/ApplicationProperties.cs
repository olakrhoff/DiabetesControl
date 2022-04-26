using System;
using System.Threading.Tasks;
using DiabetesContolApp.GlobalLogic.Interfaces;

using Xamarin.Forms;

namespace DiabetesContolApp.GlobalLogic
{
    /// <summary>
    /// This class is used to make it possible to test code that uses
    /// Application.Current from Xamarin.
    /// </summary>
    public class ApplicationProperties : IApplicationProperties
    {
        private readonly Application _application;

        public ApplicationProperties(Application application)
        {
            _application = application;
        }

        async public Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await _application.MainPage.DisplayAlert(title, message, accept, cancel);
        }

        public T GetProperty<T>(string key)
        {
            return (T)(_application.Properties.TryGetValue(key, out object result) ? result : null);
        }

        async public Task SavePropertiesAsync()
        {
            await _application.SavePropertiesAsync();
        }

        public bool SetProperty<T>(string key, T value)
        {
            if (!_application.Properties.ContainsKey(key))
                return false;
            _application.Properties[key] = value;
            return true;
        }
    }
}
