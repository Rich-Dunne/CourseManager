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
    [QueryProperty(nameof(CourseId), nameof(CourseId))]
    public class EditCourseViewModel : BaseViewModel
    {
        public List<string> StatusList { get; } = new List<string>() { "Inactive", "Active", "Planned", "Dropped", "Completed" };
        public List<string> Instructors { get; } = new List<string>() {  "New Instructor"  };

        private int _courseId;
        public int CourseId 
        { 
            get => _courseId;
            set
            {
                SetProperty(ref _courseId, value);
                GetCourse();
            }
        }

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

        private bool _hasErrors = false;
        public bool HasErrors { get => _hasErrors; set => SetProperty(ref _hasErrors, value); }

        public string COURSE_NAME_REQUIRED { get; } = "Required";
        public string COURSE_NAME_TAKEN { get; } = "A course with that name already exists";

        private bool _showCourseNameRequiredErrorMessage = false;
        public bool ShowCourseNameRequiredErrorMessage { get => _showCourseNameRequiredErrorMessage; set => SetProperty(ref _showCourseNameRequiredErrorMessage, value); }

        private bool _showCourseNameTakenErrorMessage = false;
        public bool ShowCourseNameTakenErrorMessage { get => _showCourseNameTakenErrorMessage; set => SetProperty(ref _showCourseNameTakenErrorMessage, value); }

        public Course Course;
        public Command SaveCommand { get; }
        public Command NavigateBackCommand { get; }

        public EditCourseViewModel()
        {
            Title = "Edit Course";
            SaveCommand = new Command(Save);
            NavigateBackCommand = new Command(NavigateBack);

            MinStartDate = DateTime.Now.AddDays(-365);
            MaxEndDate = DateTime.Now.AddDays(365);
            MaxStartDate = MaxEndDate.AddDays(-1);
            MinEndDate = MinStartDate.AddDays(1);

            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddDays(1);
        }

        private async void Save()
        {
            Validate();
            if(HasErrors)
            {
                await Shell.Current.DisplayAlert("Oops!", "It looks like your form has some errors that need to be fixed before continuing.", "OK");
                return;
            }

            var status = Enum.TryParse(Status, out Enums.Status myStatus);
            Course.CourseName = CourseName;
            Course.StartDate = StartDate;
            Course.EndDate = EndDate;
            Course.Status = myStatus;
            Course.EnableNotifications = EnableAlerts;

            await Services.CourseService.UpdateCourse(Course);

            NavigateBack();
        }

        private async void NavigateBack() => await Shell.Current.GoToAsync("..");

        private void GetCourse()
        {
            Debug.WriteLine($"Getting course");
            Course = Services.CourseService.GetCourse(CourseId);
            if(Course == null)
            {
                Debug.WriteLine($"Course doesn't exist");
                return;
            }

            var courseTerm = Services.TermService.GetTerm(Course.AssociatedTermId);

            CourseName = Course.CourseName;
            Status = Course.Status.ToString();
            EnableAlerts = Course.EnableNotifications;
            
            MaxEndDate = courseTerm.EndDate;
            EndDate = Course.EndDate;
            MaxStartDate = MaxEndDate.AddDays(-1);

            MinStartDate = Course.StartDate;
            StartDate = MinStartDate;
            MinEndDate = StartDate.AddDays(1);
        }

        private void Validate()
        {
            ShowCourseNameRequiredErrorMessage = string.IsNullOrWhiteSpace(CourseName);
            ShowCourseNameTakenErrorMessage = CourseName != Course.CourseName && Services.CourseService.Courses.Any(x => x.CourseName == CourseName);

            HasErrors = ShowCourseNameRequiredErrorMessage || ShowCourseNameTakenErrorMessage;
        }
    }
}
