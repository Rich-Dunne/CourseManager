using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using CourseManager.Views;

namespace CourseManager.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ICommand NavigateDegreePlanCommand { get; }

        public HomeViewModel()
        {
            Title = "Home";
            NavigateDegreePlanCommand = new Command(async () => await Shell.Current.GoToAsync($"///{nameof(DegreePlanPage)}"));
        }
    }
}