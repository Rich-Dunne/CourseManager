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
    [QueryProperty(nameof(FirstAssessmentId), nameof(FirstAssessmentId))]
    [QueryProperty(nameof(SecondAssessmentId), nameof(SecondAssessmentId))]
    public class EditAssessmentsViewModel : BaseViewModel
    {
        #region Passed Form Values
        private int _courseId;
        public int CourseId
        {
            get => _courseId;
            set
            {
                SetProperty(ref _courseId, value);
                GetAssociatedCourse();
            }
        }

        private int _firstAssessmentId;
        public int FirstAssessmentId
        {
            get => _firstAssessmentId;
            set
            {
                SetProperty(ref _firstAssessmentId, value);
                GetAssessments();
            }
        }

        private int _secondAssessmentId;
        public int SecondAssessmentId
        {
            get => _secondAssessmentId;
            set
            {
                SetProperty(ref _secondAssessmentId, value);
                GetAssessments();
            }
        }
        #endregion

        private string _assessmentName;
        public string AssessmentName 
        { 
            get => _assessmentName;
            set
            {
                SetProperty(ref _assessmentName, value);
                ShowAssessmentNameErrorMessage = string.IsNullOrWhiteSpace(AssessmentName);
            }
        }

        private string _secondAssessmentName;
        public string SecondAssessmentName 
        { 
            get => _secondAssessmentName;
            set
            {
                SetProperty(ref _secondAssessmentName, value);
                ShowSecondAssessmentNameErrorMessage = string.IsNullOrWhiteSpace(SecondAssessmentName);
            }
        }

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
        public string AssessmentType 
        { 
            get => _assessmentType;
            set
            {
                SetProperty(ref _assessmentType, value);
                SecondAssessmentType = AssessmentTypes.First(x => x != AssessmentType);
                if (PickerIndex == 0)
                {
                    SecondPickerIndex = 1;
                }
                else
                {
                    SecondPickerIndex = 0;
                }
            }
        }

        private string _secondAssessmentType;
        public string SecondAssessmentType { get => _secondAssessmentType; set => SetProperty(ref _secondAssessmentType, value); }

        private bool _hasSecondAssessment = false;
        public bool HasSecondAssessment 
        { 
            get => _hasSecondAssessment;
            set
            {
                SetProperty(ref _hasSecondAssessment, value);
                SecondAssessmentType = AssessmentTypes.First(x => x != AssessmentType);
                ShowAddAssessmentButton = !HasSecondAssessment;
            }
        }

        private bool _showAddAssessmentButton = true;
        public bool ShowAddAssessmentButton { get => _showAddAssessmentButton; set => SetProperty(ref _showAddAssessmentButton, value); }

        private bool _hasErrors = false;
        public bool HasErrors { get => _hasErrors; set => SetProperty(ref _hasErrors, value); }

        public string AssessmentNameErrorMessage { get; } = "Required";

        private bool _showAssessmentNameErrorMessage = false;
        public bool ShowAssessmentNameErrorMessage { get => _showAssessmentNameErrorMessage; set => SetProperty(ref _showAssessmentNameErrorMessage, value); }

        private bool _showSecondAssessmentNameErrorMessage = false;
        public bool ShowSecondAssessmentNameErrorMessage { get => _showSecondAssessmentNameErrorMessage; set => SetProperty(ref _showSecondAssessmentNameErrorMessage, value); }

        public Course Course;
        public Assessment FirstAssessment, SecondAssessment;

        public Command SaveCommand { get; }
        public Command NavigateBackCommand { get; }
        public Command AddSecondAssessmentCommand { get; }
        public Command RemoveSecondAssessmentCommand { get; }

        public EditAssessmentsViewModel()
        {
            Title = "Edit Assessments";
            SaveCommand = new Command(Save);
            NavigateBackCommand = new Command(NavigateBack);
            AddSecondAssessmentCommand = new Command(() => { HasSecondAssessment = true; ShowAddAssessmentButton = false; SecondDueDate = DueDate; });
            RemoveSecondAssessmentCommand = new Command(() => { HasSecondAssessment = false; ShowAddAssessmentButton = true; Course.SecondAssessmentId = 0; });

            MinDueDate = DateTime.Today.AddDays(-365);
            MaxDueDate = DateTime.Today.AddDays(365);

            AssessmentType = "Objective";
            SecondAssessmentType = "Performance";
        }

        private void GetAssessments()
        {
            if (FirstAssessment == null && FirstAssessmentId != 0)
            {
                FirstAssessment = Services.AssessmentService.GetAssessment(FirstAssessmentId);
                if(FirstAssessment == null)
                {
                    Debug.WriteLine($"First assessment is null.");
                    return;
                }

                AssessmentName = FirstAssessment.Name;
                DueDate = FirstAssessment.DueDate;
                AssessmentType = FirstAssessment.AssessmentType.ToString();
                EnableAlerts = FirstAssessment.EnableNotifications;
            }
            
            if (SecondAssessment == null && SecondAssessmentId != 0)
            {
                SecondAssessment = Services.AssessmentService.GetAssessment(SecondAssessmentId);
                if(SecondAssessment == null)
                {
                    Debug.WriteLine($"Second assessment is null.");
                    return;
                }

                HasSecondAssessment = true;
                SecondAssessmentName = SecondAssessment.Name;
                SecondDueDate = SecondAssessment.DueDate;
                SecondAssessmentType = SecondAssessment.AssessmentType.ToString();
            }
        }

        private void GetAssociatedCourse()
        {
            Course = Services.CourseService.GetCourse(CourseId);
            MinDueDate = Course.StartDate.AddDays(1);
            MaxDueDate = Course.EndDate;
        }

        private async void Save()
        {
            Validate();
            if(HasErrors)
            {
                await Shell.Current.DisplayAlert("Oops!", "It looks like your form has some errors that need to be fixed before continuing.", "OK");
                return;
            }

            UpdateFirstAssessment();

            if (HasSecondAssessment)
            {
                if (SecondAssessmentId != 0)
                {
                    UpdateSecondAssessment();
                }
                else
                {
                    CreateSecondAssessment();
                    await Services.AssessmentService.AddAssessment(SecondAssessment);
                    Course.SecondAssessmentId = SecondAssessment.Id;
                }
            }
            else
            {
                Course.SecondAssessmentId = 0;
            }

            await Services.CourseService.UpdateCourse(Course);

            NavigateBack();
        }

        private async void UpdateFirstAssessment()
        {
            var assessmentType = Enum.TryParse(AssessmentType, out Enums.AssessmentType firstAssessmentType);

            FirstAssessment.Name = AssessmentName;
            FirstAssessment.DueDate = DueDate;
            FirstAssessment.AssessmentType = firstAssessmentType;
            FirstAssessment.EnableNotifications = EnableAlerts;
            await Services.AssessmentService.UpdateAssessment(FirstAssessment);
        }

        private async void UpdateSecondAssessment()
        {
            var assessmentType = Enum.TryParse(SecondAssessmentType, out Enums.AssessmentType secondAssessmentType);

            SecondAssessment.Name = SecondAssessmentName;
            SecondAssessment.DueDate = SecondDueDate;
            SecondAssessment.AssessmentType = secondAssessmentType;
            SecondAssessment.EnableNotifications = EnableAlerts;
            await Services.AssessmentService.UpdateAssessment(SecondAssessment);
        }

        private async void NavigateBack() => await Shell.Current.GoToAsync("..");

        private void CreateSecondAssessment()
        {
            var assessmentType = Enum.TryParse(SecondAssessmentType, out Enums.AssessmentType secondAssessmentType);
            SecondAssessment = new Assessment
            {
                Name = SecondAssessmentName,
                AssessmentType = secondAssessmentType,
                DueDate = SecondDueDate,
                EnableNotifications = EnableAlerts,
            };
            Debug.WriteLine($"Second assessment created: {SecondAssessment.Name}");
        }

        private void Validate()
        {
            HasErrors = false;

            ShowAssessmentNameErrorMessage = string.IsNullOrWhiteSpace(AssessmentName);

            if (HasSecondAssessment)
            {
                ShowSecondAssessmentNameErrorMessage = string.IsNullOrWhiteSpace(SecondAssessmentName);
            }


            if(ShowAssessmentNameErrorMessage || (HasSecondAssessment && ShowSecondAssessmentNameErrorMessage))
            {
                HasErrors = true;
            }
        }
    }
}
