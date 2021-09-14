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
    [QueryProperty(nameof(CourseValues), nameof(CourseValues))]
    [QueryProperty(nameof(InstructorValues), nameof(InstructorValues))]
    public class AddAssessmentsViewModel : BaseViewModel
    {
        #region Passed Form Values
        private string _courseValues;
        public string CourseValues
        {
            get => _courseValues;
            set
            {
                SetProperty(ref _courseValues, value);
                InitializeCourseProperties();
            }
        }

        private string _instructorValues;
        public string InstructorValues
        {
            get => _instructorValues;
            set
            {
                SetProperty(ref _instructorValues, value);
                InitializeInstructorProperties();
            }
        }
        #endregion

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
            }
        }

        private bool _showAddAssessmentButton = true;
        public bool ShowAddAssessmentButton { get => _showAddAssessmentButton; set => SetProperty(ref _showAddAssessmentButton, value); }

        public Course Course;
        public Instructor Instructor;
        public Assessment FirstAssessment, SecondAssessment;

        public Command SaveCommand { get; }
        public Command NavigateBackCommand { get; }
        public Command AddSecondAssessmentCommand { get; }
        public Command RemoveSecondAssessmentCommand { get; }

        public AddAssessmentsViewModel()
        {
            Title = "Add Assessments";
            SaveCommand = new Command(Save);
            NavigateBackCommand = new Command(NavigateBack);
            AddSecondAssessmentCommand = new Command(() => { HasSecondAssessment = true; ShowAddAssessmentButton = false; SecondDueDate = DueDate; });
            RemoveSecondAssessmentCommand = new Command(() => { HasSecondAssessment = false; ShowAddAssessmentButton = true; });

            MinDueDate = DateTime.Today.AddDays(-365);
            MaxDueDate = DateTime.Today.AddDays(365);

            AssessmentType = "Objective";
            SecondAssessmentType = "Performance";
        }

        private async void Save()
        {
            if (string.IsNullOrWhiteSpace(_assessmentName))
            {
                return;
            }

            CreateAssessments();

            await Services.AssessmentService.AddAssessment(FirstAssessment);
            Course.FirstAssessmentId = FirstAssessment.Id;

            if(HasSecondAssessment)
            {
                await Services.AssessmentService.AddAssessment(SecondAssessment);
                Course.SecondAssessmentId = SecondAssessment.Id;
            }

            await Services.InstructorService.AddInstructor(Instructor);     
            Course.AssociatedInstructorId = Instructor.Id;

            await Services.CourseService.AddCourse(Course);

            await Services.TermService.AddCourseToTerm(Course);

            var route = $"///{nameof(DegreePlanPage)}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        private void InitializeCourseProperties()
        {
            string[] courseValues = CourseValues.Split(',');
            var status = Enum.TryParse(courseValues[4], out Enums.Status myStatus);

            Course = new Course
            {
                CourseName = courseValues[0],
                StartDate = DateTime.Parse(courseValues[1]),
                EndDate = DateTime.Parse(courseValues[2]),
                EnableNotifications = bool.Parse(courseValues[3]),
                Status = myStatus,
                Notes = courseValues[5],
                AssociatedTermId = int.Parse(courseValues[6]),
            };

            MinDueDate = Course.StartDate.AddDays(1);
            MaxDueDate = Course.EndDate;

            Debug.WriteLine($"Course/Id: {Course.CourseName}/{Course.Id}");
            Debug.WriteLine($"Start: {Course.StartDate}");
            Debug.WriteLine($"End: {Course.EndDate}");
            Debug.WriteLine($"Notifications: {Course.EnableNotifications}");
            Debug.WriteLine($"Status: {Course.Status}");
            Debug.WriteLine($"Notes: {Course.Notes}");
            Debug.WriteLine($"Associated Term: {Course.AssociatedTermId}");
            Debug.WriteLine($"Associated Instructor: {Course.AssociatedInstructorId}");
        }

        private void InitializeInstructorProperties()
        {
            int.TryParse(InstructorValues, out int instructorID);
            Instructor = Services.InstructorService.GetInstructor(instructorID);

            Debug.WriteLine($"Instructor Id: {Instructor.Id}");
            Debug.WriteLine($"Instructor First Name: {Instructor.FirstName}");
            Debug.WriteLine($"Instructor Last Name: {Instructor.LastName}");
            Debug.WriteLine($"Phone Number: {Instructor.PhoneNumber}");
            Debug.WriteLine($"Email: {Instructor.Email}");
        }

        private void CreateAssessments()
        {
            var assessmentType = Enum.TryParse(AssessmentType, out Enums.AssessmentType firstAssessmentType);
            FirstAssessment = new Assessment
            {
                Name = AssessmentName,
                AssessmentType = firstAssessmentType,
                DueDate = DueDate,
                EnableNotifications = EnableAlerts,
            };
            Debug.WriteLine($"First assessment created: {FirstAssessment.Name}");

            if (HasSecondAssessment)
            {
                assessmentType = Enum.TryParse(SecondAssessmentType, out Enums.AssessmentType secondAssessmentType);
                SecondAssessment = new Assessment
                {
                    Name = SecondAssessmentName,
                    AssessmentType = secondAssessmentType,
                    DueDate = SecondDueDate,
                    EnableNotifications = EnableAlerts,
                };
                Debug.WriteLine($"Second assessment created: {SecondAssessment.Name}");
            }
        }
    }
}
