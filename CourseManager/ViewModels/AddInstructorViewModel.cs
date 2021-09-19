using CourseManager.Models;
using CourseManager.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    [QueryProperty(nameof(CourseValues), nameof(CourseValues))]
    public class AddInstructorViewModel : BaseViewModel
    {
        private string _courseValues;
        public string CourseValues
        {
            get => _courseValues;
            set
            {
                SetProperty(ref _courseValues, value);
            }
        }

        private string _instructorFirstName;
        public string InstructorFirstName 
        { 
            get => _instructorFirstName;
            set
            {
                SetProperty(ref _instructorFirstName, value);
                ShowFirstNameErrorMessage = string.IsNullOrWhiteSpace(InstructorFirstName);
                ShowNameTakenErrorMessage = Services.InstructorService.Instructors.Any(x => x.FirstName == InstructorFirstName && x.LastName == InstructorLastName);
                HasErrors = ShowFirstNameErrorMessage || ShowNameTakenErrorMessage;
            }
        }

        private string _instructorLastName;
        public string InstructorLastName 
        { 
            get => _instructorLastName;
            set
            {
                SetProperty(ref _instructorLastName, value);
                ShowLastNameErrorMessage = string.IsNullOrWhiteSpace(InstructorLastName);
                ShowNameTakenErrorMessage = Services.InstructorService.Instructors.Any(x => x.FirstName == InstructorFirstName && x.LastName == InstructorLastName);
                HasErrors = ShowFirstNameErrorMessage || ShowNameTakenErrorMessage;
            }
        }

        private string _instructorPhoneNumber;
        public string InstructorPhoneNumber 
        { 
            get => _instructorPhoneNumber;
            set
            {
                SetProperty(ref _instructorPhoneNumber, value);
                ShowPhoneNumberErrorMessage = string.IsNullOrWhiteSpace(InstructorPhoneNumber);
                if (ShowPhoneNumberErrorMessage)
                {
                    PhoneNumberErrorMessage = "Required";
                    HasErrors = ShowPhoneNumberErrorMessage;
                    return;
                }

                ShowPhoneNumberErrorMessage = !Regex.IsMatch(InstructorPhoneNumber, _PHONE_NUMBER_REGEX);
                PhoneNumberErrorMessage = "Invalid format";

                HasErrors = ShowPhoneNumberErrorMessage;
            }
        }

        private string _instructorEmail;
        public string InstructorEmail 
        { 
            get => _instructorEmail;
            set
            {
                SetProperty(ref _instructorEmail, value);
                ShowEmailErrorMessage = string.IsNullOrWhiteSpace(InstructorEmail);
                if (ShowEmailErrorMessage)
                {
                    EmailErrorMessage = "Required";
                    HasErrors = ShowEmailErrorMessage;
                    return;
                }

                ShowEmailErrorMessage = !Regex.IsMatch(InstructorEmail, _EMAIL_REGEX);
                EmailErrorMessage = "Invalid format";

                HasErrors = ShowEmailErrorMessage;
            }
        }

        private bool _showFirstNameErrorMessage;
        public bool ShowFirstNameErrorMessage { get => _showFirstNameErrorMessage; set => SetProperty(ref _showFirstNameErrorMessage, value); }
        
        private bool _showLastNameErrorMessage;
        public bool ShowLastNameErrorMessage { get => _showLastNameErrorMessage; set => SetProperty(ref _showLastNameErrorMessage, value); }

        private bool _showNameTakenErrorMessage;
        public bool ShowNameTakenErrorMessage { get => _showNameTakenErrorMessage; set => SetProperty(ref _showNameTakenErrorMessage, value); }

        private bool _showPhoneNumberErrorMessage;
        public bool ShowPhoneNumberErrorMessage { get => _showPhoneNumberErrorMessage; set => SetProperty(ref _showPhoneNumberErrorMessage, value); }

        private bool _showEmailErrorMessage;
        public bool ShowEmailErrorMessage { get => _showEmailErrorMessage; set => SetProperty(ref _showEmailErrorMessage, value); }

        private bool _hasErrors = false;
        public bool HasErrors
        {
            get => _hasErrors;
            set => SetProperty(ref _hasErrors, value);
        }

        public string FirstNameErrorMessage { get; } = "Required";
        public string LastNameErrorMessage { get; } = "Required";
        public string INSTRUCTOR_NAME_TAKEN { get; } = "An instructor with this first and last name already exists";
        
        private string _phoneNumberErrorMessage;
        public string PhoneNumberErrorMessage { get => _phoneNumberErrorMessage; private set => SetProperty(ref _phoneNumberErrorMessage, value); }

        private string _emailErrorMessage;
        public string EmailErrorMessage { get => _emailErrorMessage; private set => SetProperty(ref _emailErrorMessage, value); }

        private Color _backgroundColor;
        public Color BackgroundColor
        {
            get =>_backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        private string _PHONE_NUMBER_REGEX = "^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$";
        private string _EMAIL_REGEX = "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$";

        public Command NavigateAddAssessmentsCommand { get; }
        public Command NavigateBackCommand { get; }

        public AddInstructorViewModel()
        {
            Title = "Add Instructor";
            NavigateAddAssessmentsCommand = new Command(NavigateAddAssessments);
            NavigateBackCommand = new Command(NavigateBack);
        }

        private async void NavigateAddAssessments()
        {
            ValidateInput();
            if(HasErrors)
            {
                await Shell.Current.DisplayAlert("Oops!", "It looks like you have some errors in the form that need to be fixed before we continue.", "Ok");
                return;
            }

            var instructorValues = $"{InstructorFirstName},{InstructorLastName},{InstructorPhoneNumber},{InstructorEmail}";
            var route = $"{nameof(AddAssessmentsPage)}?CourseValues={CourseValues}&InstructorValues={instructorValues}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        private void ValidateInput()
        {
            ShowFirstNameErrorMessage = string.IsNullOrWhiteSpace(InstructorFirstName);
            ShowLastNameErrorMessage = string.IsNullOrWhiteSpace(InstructorLastName);
            ShowNameTakenErrorMessage = Services.InstructorService.Instructors.Any(x => x.FirstName == InstructorFirstName && x.LastName == InstructorLastName);

            ShowPhoneNumberErrorMessage = string.IsNullOrWhiteSpace(InstructorPhoneNumber);
            PhoneNumberErrorMessage = "Required";

            if (!ShowPhoneNumberErrorMessage)
            {
                ShowPhoneNumberErrorMessage = !Regex.IsMatch(InstructorPhoneNumber, _PHONE_NUMBER_REGEX);
                PhoneNumberErrorMessage = "Invalid format";
            }

            ShowEmailErrorMessage = string.IsNullOrWhiteSpace(InstructorEmail);
            EmailErrorMessage = "Required";

            if(!ShowEmailErrorMessage)
            {
                ShowEmailErrorMessage = !Regex.IsMatch(InstructorEmail, _EMAIL_REGEX);
                EmailErrorMessage = "Invalid format";
            }

            if(ShowFirstNameErrorMessage || ShowLastNameErrorMessage || ShowPhoneNumberErrorMessage || ShowEmailErrorMessage || ShowNameTakenErrorMessage)
            {
                HasErrors = true;
            }
            else
            {
                HasErrors = false;
            }
        }
    }
}
