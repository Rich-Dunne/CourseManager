using CourseManager.Models;
using CourseManager.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    public class AddAssessmentsViewModel : BaseViewModel
    {
        private string _assessmentName;
        public string AssessmentName { get => _assessmentName; set => SetProperty(ref _assessmentName, value); }

        private string _secondAssessmentName;
        public string SecondAssessmentName { get => _secondAssessmentName; set => SetProperty(ref _secondAssessmentName, value); }

        private DateTime _minDueDate;
        public DateTime MinDueDate { get => _minDueDate; set => SetProperty(ref _minDueDate, value); }

        private DateTime _maxDueDate;
        public DateTime MaxDueDate { get => _maxDueDate; set => SetProperty(ref _maxDueDate, value); }

        private DateTime _dueDate;
        public DateTime DueDate { get => _dueDate; set => SetProperty(ref _dueDate, value); }

        private DateTime _secondDueDate;
        public DateTime SecondDueDate { get => _secondDueDate; set => SetProperty(ref _secondDueDate, value); }

        private int _pickerIndex = 0;
        public int PickerIndex { get => _pickerIndex; set => SetProperty(ref _pickerIndex, value); }

        private int _secondPickerIndex = 1;
        public int SecondPickerIndex { get => _secondPickerIndex; set => SetProperty(ref _secondPickerIndex, value); }

        private List<string> _assessmentTypes = new List<string>() { "Objective", "Performance" };
        public List<string> AssessmentTypes { get => _assessmentTypes; set => SetProperty(ref _assessmentTypes, value); }

        private bool _enableAlerts;
        public bool EnableAlerts { get => _enableAlerts; set => SetProperty(ref _enableAlerts, value); }

        private string _assessmentType;
        public string AssessmentType { get => _assessmentType; set => SetProperty(ref _assessmentType, value); }

        private string _secondAssessmentType;
        public string SecondAssessmentType { get => _secondAssessmentType; set => SetProperty(ref _secondAssessmentType, value); }

        private bool _secondAssessment = false;
        public bool SecondAssessment { get => _secondAssessment; set => SetProperty(ref _secondAssessment, value); }

        private bool _showAddAssessmentButton = true;
        public bool ShowAddAssessmentButton { get => _showAddAssessmentButton; set => SetProperty(ref _showAddAssessmentButton, value); }

        public Command SaveCommand { get; }
        public Command NavigateBackCommand { get; }
        public Command AddSecondAssessmentCommand { get; }
        public Command RemoveSecondAssessmentCommand { get; }

        public AddAssessmentsViewModel()
        {
            Title = "Add Assessments";
            SaveCommand = new Command(Save);
            NavigateBackCommand = new Command(NavigateBack);
            AddSecondAssessmentCommand = new Command(() => { SecondAssessment = true; ShowAddAssessmentButton = false; });
            RemoveSecondAssessmentCommand = new Command(() => { SecondAssessment = false; ShowAddAssessmentButton = true; });

            MinDueDate = DateTime.Today;
            MaxDueDate = DateTime.Today.AddDays(30);
        }

        private async void Save()
        {
            if(string.IsNullOrWhiteSpace(_assessmentName))
            {
                return;
            }

            var route = $"/{nameof(DegreePlanPage)}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        private void AddSecondAssessment()
        {
            _secondAssessment = true;
        }
    }
}
