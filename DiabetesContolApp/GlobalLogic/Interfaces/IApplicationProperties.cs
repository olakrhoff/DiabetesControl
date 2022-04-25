using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DiabetesContolApp.GlobalLogic.Interfaces
{
    public interface IApplicationProperties
    {
        public T GetProperty<T>(string key);
        public bool SetProperty<T>(string key, T value);
        Task SavePropertiesAsync();
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
    }
}
