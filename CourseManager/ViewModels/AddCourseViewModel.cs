using CourseManager.Models;
using CourseManager.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    [QueryProperty(nameof(CourseName), nameof(CourseName))]
    public class AddCourseViewModel : BaseViewModel
    {
        public List<string> StatusList { get; } = new List<string>() { "Inactive", "Active", "Planned", "Dropped", "Completed" };
        public List<string> Terms { get; } = new List<string>();

        private string _courseName;
        public string CourseName { get => _courseName; set => SetProperty(ref _courseName, value); }

        private DateTime _minStartDate = DateTime.Now;
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

        private string _status;
        public string Status { get => _status; set => SetProperty(ref _status, value); }

        public string _courseNotes;
        public string CourseNotes { get => _courseNotes; set => SetProperty(ref _courseNotes, value); }


        public Command NavigateAddInstructorCommand { get; }

        public AddCourseViewModel()
        {
            Title = "Add Course";
            NavigateAddInstructorCommand = new Command(NavigateAddInstructor);

            MinStartDate = DateTime.Now;
            StartDate = MinStartDate;

            MinEndDate = StartDate.AddDays(1);
            EndDate = StartDate.AddDays(1);

            MaxStartDate = EndDate;
            MaxEndDate = MaxStartDate.AddDays(30);

            GetTerms();
        }

        private void GetTerms()
        {
            var terms = Services.TermService.TermGroups;
            foreach (TermGroup termGroup in terms)
            {
                if (termGroup.Courses.Count < 6)
                {
                    Terms.Add(termGroup.Name);
                }
            }
        }

        private async void NavigateAddInstructor()
        {
            if(string.IsNullOrWhiteSpace(_courseName))
            {
                return;
            }

            var route = $"{nameof(AddInstructorPage)}";
            await Shell.Current.GoToAsync(route);
        }
    }
}
