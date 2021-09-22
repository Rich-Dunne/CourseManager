using CourseManager.Views;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Form Properties
        private string _username;
        public string Username 
        { 
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                Validate();
            }
        }
        #endregion

        #region Validation Properties
        private string _usernameErrorMessage = "Required";
        public string UsernameErrorMessage { get => _usernameErrorMessage; }

        private bool _showUsernameErrorMessage = false;
        public bool ShowUsernameErrorMessage { get => _showUsernameErrorMessage; set => SetProperty(ref _showUsernameErrorMessage, value); }

        private bool _hasErrors = false;
        public bool HasErrors { get => _hasErrors; set => SetProperty(ref _hasErrors, value); }
        #endregion

        #region Command Properties
        public Command LoginCommand { get; }
        #endregion

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            Validate();
            if(HasErrors)
            {
                await Shell.Current.DisplayAlert("Oops!", "It looks like your username has an error that needs to be fixed before continuing.", "OK");
                return;
            }

            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}?Username={Username}");
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(_username))
            {
                ShowUsernameErrorMessage = true;
                HasErrors = ShowUsernameErrorMessage;
                return;
            }

            ShowUsernameErrorMessage = false;
            HasErrors = ShowUsernameErrorMessage;
        }
    }
}
