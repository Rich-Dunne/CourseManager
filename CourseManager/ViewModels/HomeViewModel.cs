using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using CourseManager.Views;

namespace CourseManager.ViewModels
{
    [QueryProperty(nameof(Username), nameof(Username))]
    public class HomeViewModel : BaseViewModel
    {
        private string _username;
        public string Username 
        { 
            get => _username; 
            set
            {
                SetProperty(ref _username, value);
            }
        }

        public ICommand NavigateDegreePlanCommand { get; }

        public HomeViewModel()
        {
            Title = "Home";
            NavigateDegreePlanCommand = new Command(async () => await Shell.Current.GoToAsync($"///{nameof(DegreePlanPage)}?Username={Username}"));
        }
    }
}