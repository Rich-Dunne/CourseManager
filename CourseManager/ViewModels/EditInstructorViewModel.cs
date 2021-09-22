using CourseManager.Models;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    [QueryProperty(nameof(CourseId), nameof(CourseId))]
    [QueryProperty(nameof(InstructorId), nameof(InstructorId))]
    public class EditInstructorViewModel : BaseViewModel
    {
        private int _courseId;
        public int CourseId
        {
            get => _courseId;
            set
            {
                SetProperty(ref _courseId, value);
                GetCourseInformation();
            }
        }

        private int _instructorId;
        public int InstructorId
        {
            get => _instructorId;
            set
            {
                SetProperty(ref _instructorId, value);
                GetInstructorInformation();
            }
        }

        private Instructor _currentInstructor;
        public Instructor CurrentInstructor { get => _currentInstructor; set => SetProperty(ref _currentInstructor, value); }

        private Course _associatedCourse;
        public Course AssociatedCourse { get => _associatedCourse; set => SetProperty(ref _associatedCourse, value); }

        private string _PHONE_NUMBER_REGEX = "^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$";
        private string _EMAIL_REGEX = "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$";

        #region Form Properties
        private string _instructorFirstName;
        public string InstructorFirstName
        {
            get => _instructorFirstName;
            set
            {
                SetProperty(ref _instructorFirstName, value);
                ShowFirstNameErrorMessage = string.IsNullOrWhiteSpace(InstructorFirstName);
                ShowNameTakenErrorMessage = NewInstructorChecked && Services.InstructorService.Instructors.Any(x => x.FirstName == InstructorFirstName && x.LastName == InstructorLastName);
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
                ShowNameTakenErrorMessage = NewInstructorChecked && Services.InstructorService.Instructors.Any(x => x.FirstName == InstructorFirstName && x.LastName == InstructorLastName);
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

        private bool _currentInstructorChecked = true;
        public bool CurrentInstructorChecked
        {
            get => _currentInstructorChecked;
            set
            {
                SetProperty(ref _currentInstructorChecked, value);
                if (_currentInstructorChecked && CurrentInstructor != null)
                {
                    InstructorFirstName = CurrentInstructor.FirstName;
                    InstructorLastName = CurrentInstructor.LastName;
                    InstructorPhoneNumber = CurrentInstructor.PhoneNumber;
                    InstructorEmail = CurrentInstructor.Email;
                    ShowNameTakenErrorMessage = false;
                }
            }
        }

        private bool _newInstructorChecked = false;
        public bool NewInstructorChecked
        {
            get => _newInstructorChecked;
            set
            {
                SetProperty(ref _newInstructorChecked, value);
                if (_newInstructorChecked)
                {
                    InstructorFirstName = "";
                    InstructorLastName = "";
                    InstructorPhoneNumber = "";
                    InstructorEmail = "";

                    ShowFirstNameErrorMessage = false;
                    ShowLastNameErrorMessage = false;
                    ShowNameTakenErrorMessage = false;
                    ShowPhoneNumberErrorMessage = false;
                    ShowEmailErrorMessage = false;
                }
            }
        }
        #endregion

        #region Validation Properties
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
        public bool HasErrors { get => _hasErrors; set => SetProperty(ref _hasErrors, value); }

        public string FirstNameErrorMessage { get; } = "Required";
        public string LastNameErrorMessage { get; } = "Required";
        public string INSTRUCTOR_NAME_TAKEN { get; } = "An instructor with this first and last name already exists";

        private string _phoneNumberErrorMessage;
        public string PhoneNumberErrorMessage { get => _phoneNumberErrorMessage; private set => SetProperty(ref _phoneNumberErrorMessage, value); }

        private string _emailErrorMessage;
        public string EmailErrorMessage { get => _emailErrorMessage; private set => SetProperty(ref _emailErrorMessage, value); }
        #endregion

        #region Command Properties
        public Command NavigateBackCommand { get; }
        public Command NavigateSaveCommand { get; }
        #endregion

        public EditInstructorViewModel()
        {
            Title = "Edit Instructor";
            NavigateBackCommand = new Command(NavigateBack);
            NavigateSaveCommand = new Command(Save);
        }

        private async void NavigateBack() => await Shell.Current.GoToAsync("..");

        private void GetInstructorInformation()
        {
            CurrentInstructor = Services.InstructorService.GetInstructor(InstructorId);
            if(CurrentInstructor == null)
            {
                Debug.WriteLine($"CurrentInstructor not found.");
                return;
            }

            InstructorFirstName = CurrentInstructor.FirstName;
            InstructorLastName = CurrentInstructor.LastName;
            InstructorPhoneNumber = CurrentInstructor.PhoneNumber;
            InstructorEmail = CurrentInstructor.Email;
        }

        private void GetCourseInformation()
        {
            AssociatedCourse = Services.CourseService.GetCourse(CourseId);
            if(AssociatedCourse == null)
            {
                Debug.WriteLine($"AssociatedCourse not found.");
            }
        }

        private async void Save()
        {
            ValidateInput();
            if (HasErrors)
            {
                await Shell.Current.DisplayAlert("Oops!", "It looks like your form has some errors that need to be fixed before continuing.", "OK");
                return;
            }

            CurrentInstructor.FirstName = InstructorFirstName;
            CurrentInstructor.LastName = InstructorLastName;
            CurrentInstructor.PhoneNumber = InstructorPhoneNumber;
            CurrentInstructor.Email = InstructorEmail;

            if (CurrentInstructorChecked)
            {
                await Services.InstructorService.UpdateInstructor(CurrentInstructor);
                Debug.WriteLine($"Updated instructor {CurrentInstructor.FirstName} {CurrentInstructor.LastName}");
            }
            else if (NewInstructorChecked)
            {
                var newInstructor = new Instructor
                {
                    FirstName = InstructorFirstName,
                    LastName = InstructorLastName,
                    PhoneNumber = InstructorPhoneNumber,
                    Email = InstructorEmail
                };
                await Services.InstructorService.AddInstructor(newInstructor);
                AssociatedCourse.AssociatedInstructorId = newInstructor.Id;

                await Services.CourseService.UpdateCourse(AssociatedCourse);

                Debug.WriteLine($"Added new instructor {newInstructor.FirstName} {newInstructor.LastName}");
            }

            NavigateBack();
        }

        private void ValidateInput()
        {
            ShowFirstNameErrorMessage = string.IsNullOrWhiteSpace(InstructorFirstName);
            ShowLastNameErrorMessage = string.IsNullOrWhiteSpace(InstructorLastName);
            if (CurrentInstructorChecked)
            {
                ShowNameTakenErrorMessage = CurrentInstructor.FirstName != InstructorFirstName && CurrentInstructor.LastName != InstructorLastName && Services.InstructorService.Instructors.Any(x => x.FirstName == InstructorFirstName && x.LastName == InstructorLastName);
            }
            else if (NewInstructorChecked)
            {
                ShowNameTakenErrorMessage = Services.InstructorService.Instructors.Any(x => x.FirstName == InstructorFirstName && x.LastName == InstructorLastName);
            }

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
