using CourseManager.Models;
using CourseManager.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    [QueryProperty(nameof(CourseName), nameof(CourseName))]
    public class AddCourseViewModel : BaseViewModel
    {
        public List<string> StatusList { get; } = new List<string>() { "Inactive", "Active", "Planned", "Dropped", "Completed" };
        public List<string> Instructors { get; } = new List<string>() {  "New Instructor"  };
        public List<string> Terms { get; } = new List<string>();

        private string _courseName;
        public string CourseName 
        { 
            get => _courseName;
            set
            {
                SetProperty(ref _courseName, value);
                Validate();
            }
        }

        private DateTime _minStartDate;
        public DateTime MinStartDate { get => _minStartDate; set => SetProperty(ref _minStartDate, value); }

        private DateTime _maxStartDate;
        public DateTime MaxStartDate { get => _maxStartDate; set => SetProperty(ref _maxStartDate, value); }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                SetProperty(ref _startDate, value);
                MinEndDate = _startDate.AddDays(1);
            }
        }

        private DateTime _minEndDate;
        public DateTime MinEndDate { get => _minEndDate; set => SetProperty(ref _minEndDate, value); }

        private DateTime _maxEndDate;
        public DateTime MaxEndDate { get => _maxEndDate; set => SetProperty(ref _maxEndDate, value); }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                SetProperty(ref _endDate, value);
                MaxStartDate = EndDate.AddDays(-1);
            }
        }

        private bool _enableAlerts;
        public bool EnableAlerts { get => _enableAlerts; set => SetProperty(ref _enableAlerts, value); }

        private string _status = "Inactive";
        public string Status { get => _status; set => SetProperty(ref _status, value); }

        private int _pickerIndex = 0;
        public int PickerIndex { get => _pickerIndex; set => SetProperty(ref _pickerIndex, value); }

        private string _selectedTerm = Services.TermService.TermGroups.First(x => x.Count < 6).Name;
        public string SelectedTerm
        {
            get => _selectedTerm;
            set
            {
                SetProperty(ref _selectedTerm, value);
                InitializeSelectedTerm();
            }
        }

        private string _selectedInstructor;
        public string SelectedInstructor
        {
            get => _selectedInstructor;
            set
            {
                SetProperty(ref _selectedInstructor, value);
                if(SelectedInstructor == "New Instructor")
                {
                    NewInstructor = true;
                }
                else
                {
                    NewInstructor = false;
                }
            }
        }

        public string _courseNotes;
        public string CourseNotes { get => _courseNotes; set => SetProperty(ref _courseNotes, value); }

        private bool _newInstructor = true;
        public bool NewInstructor { get => _newInstructor; set => SetProperty(ref _newInstructor, value); }

        private bool _hasErrors = false;
        public bool HasErrors { get => _hasErrors; set => SetProperty(ref _hasErrors, value); }

        public string COURSE_NAME_REQUIRED { get; } = "Required";
        public string COURSE_NAME_TAKEN { get; } = "A course with that name already exists";

        private bool _showCourseNameRequiredErrorMessage = false;
        public bool ShowCourseNameRequiredErrorMessage { get => _showCourseNameRequiredErrorMessage; set => SetProperty(ref _showCourseNameRequiredErrorMessage, value); }

        private bool _showCourseNameTakenErrorMessage = false;
        public bool ShowCourseNameTakenErrorMessage { get => _showCourseNameTakenErrorMessage; set => SetProperty(ref _showCourseNameTakenErrorMessage, value); }

        public Command NavigateAddInstructorCommand { get; }
        public Command NavigateAddAssessmentsCommand { get; }
        public Command NavigateBackCommand { get; }

        public AddCourseViewModel()
        {
            Title = "Add Course";
            NavigateAddInstructorCommand = new Command(NavigateAddInstructor);
            NavigateAddAssessmentsCommand = new Command(NavigateAddAssessments);
            NavigateBackCommand = new Command(NavigateBack);

            MinStartDate = DateTime.Now;
            StartDate = MinStartDate;

            MinEndDate = StartDate.AddDays(1);
            EndDate = StartDate.AddDays(1);

            MaxStartDate = EndDate;

            GetTerms();
            GetInstructors();

            SelectedInstructor = Instructors.First();
        }

        private void GetTerms()
        {
            var terms = Services.TermService.TermGroups;
            foreach (TermGroup termGroup in terms)
            {
                if (termGroup.Count < 6)
                {
                    Terms.Add(termGroup.Name);
                }
            }
        }

        private void GetInstructors()
        {
            var instructors = Services.InstructorService.Instructors;
            foreach (Instructor instructor in instructors)
            {
                Instructors.Add($"{instructor.FirstName} {instructor.LastName}");
            }
        }

        private async void NavigateAddInstructor()
        {
            Validate();
            if(HasErrors)
            {
                await Shell.Current.DisplayAlert("Oops!", "It looks like your form has some errors that need to be fixed before continuing.", "OK");
                return;
            }

            var termID = Services.TermService.TermGroups.FirstOrDefault(x => x.Name == SelectedTerm).Id;
            string courseValues = $"{CourseName},{StartDate.ToShortDateString()},{EndDate.ToShortDateString()},{EnableAlerts},{Status},{CourseNotes},{termID}";
            var route = $"{nameof(AddInstructorPage)}?CourseValues={courseValues}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateAddAssessments()
        {
            Validate();
            if(HasErrors)
            {
                await Shell.Current.DisplayAlert("Oops!", "It looks like your form has some errors that need to be fixed before continuing.", "OK");
                return;
            }
            var termID = Services.TermService.TermGroups.FirstOrDefault(x => x.Name == SelectedTerm).Id;
            string courseValues = $"{CourseName},{StartDate.ToShortDateString()},{EndDate.ToShortDateString()},{EnableAlerts},{Status},{CourseNotes},{termID}";
            var instructor = Services.InstructorService.Instructors.FirstOrDefault(x => $"{x.FirstName} {x.LastName}" == SelectedInstructor);
            if (instructor == null)
            {
                Debug.WriteLine($"Instructor \"{SelectedInstructor}\" doesn't exist.");
                return;
            }

            var route = $"{nameof(AddAssessmentsPage)}?CourseValues={courseValues}&InstructorValues={instructor.Id}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateBack() => await Shell.Current.GoToAsync("..");

        private void InitializeSelectedTerm()
        {
            var term = Services.TermService.TermGroups.FirstOrDefault(x => x.Name == SelectedTerm);
            if(term == null)
            {
                Debug.WriteLine($"Term doesn't exist");
                return;
            }

            // Initial values must be initialized in such a way that changing max/min start/end dates does not constrain themselves, or cause invalid values (min being more than max for example)
            MinStartDate = DateTime.Now.AddDays(-365);
            MaxEndDate = DateTime.Now.AddDays(365);
            MaxStartDate = MaxEndDate.AddDays(-1);
            MinEndDate = MinStartDate.AddDays(1);

            EndDate = term.EndDate;
            MaxEndDate = EndDate;
            MaxStartDate = MaxEndDate.AddDays(-1);

            StartDate = term.StartDate;
            MinStartDate = StartDate;
            MinEndDate = StartDate.AddDays(1);
        }

        private void Validate()
        {
            ShowCourseNameRequiredErrorMessage = string.IsNullOrWhiteSpace(CourseName);
            ShowCourseNameTakenErrorMessage = Services.CourseService.Courses.Any(x => x.CourseName == CourseName);
            
            HasErrors = ShowCourseNameRequiredErrorMessage || ShowCourseNameTakenErrorMessage;
        }
    }
}
