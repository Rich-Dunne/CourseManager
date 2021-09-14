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
    public class ViewCourseViewModel : BaseViewModel
    {
        private bool _propertiesInitialized = false;
        private int _courseId;
        public int CourseId
        {
            get => _courseId;
            set
            {
                SetProperty(ref _courseId, value);
                if (!_propertiesInitialized)
                {
                    InitializeSelectedCourse();
                    _propertiesInitialized = true;
                }
            }
        }

        private Instructor _instructor;
        public Instructor Instructor
        {
            get => _instructor;
            set
            {
                SetProperty(ref _instructor, value);
            }
        }

        private Assessment _firstAssessment;
        public Assessment FirstAssessment
        {
            get => _firstAssessment;
            set
            {
                SetProperty(ref _firstAssessment, value);
            }
        }

        private string _firstAssessmentDueDate;
        public string FirstAssessmentDueDate { get => _firstAssessmentDueDate; set => SetProperty(ref _firstAssessmentDueDate, value); }

        private Assessment _secondAssessment;
        public Assessment SecondAssessment
        {
            get => _secondAssessment;
            set
            {
                SetProperty(ref _secondAssessment, value);
            }
        }

        private string _secondAssessmentDueDate;
        public string SecondAssessmentDueDate { get => _secondAssessmentDueDate; set => SetProperty(ref _secondAssessmentDueDate, value); }

        private Course _selectedCourse;
        public Course SelectedCourse 
        { 
            get => _selectedCourse;
            set
            {
                SetProperty(ref _selectedCourse, value);
            }
        }

        private bool _hasSecondAssessment = false;
        public bool HasSecondAssessment { get => _hasSecondAssessment; set => SetProperty(ref _hasSecondAssessment, value); }

        public Command NavigateBackCommand { get; }
        public Command EditCourseCommand { get; }
        public Command RemoveCourseCommand { get; }

        public ViewCourseViewModel()
        {
            Title = "View Course";
            NavigateBackCommand = new Command(NavigateBack);
            EditCourseCommand = new Command(EditCourse);
            RemoveCourseCommand = new Command(RemoveCourse);
        }

        private async void NavigateBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        private void InitializeSelectedCourse()
        {
            SelectedCourse = Services.CourseService.GetCourse(CourseId);
            if(SelectedCourse == null)
            {
                return;
            }

            if (SelectedCourse.AssociatedInstructorId != 0)
            {
                Instructor = Services.InstructorService.GetInstructor(SelectedCourse.AssociatedInstructorId);
            }

            if (SelectedCourse.FirstAssessmentId != 0)
            {
                FirstAssessment = Services.AssessmentService.GetAssessment(SelectedCourse.FirstAssessmentId);
                FirstAssessmentDueDate = FirstAssessment.DueDate.ToShortDateString();
            }

            if(SelectedCourse.SecondAssessmentId != 0)
            {
                HasSecondAssessment = true;
                SecondAssessment = Services.AssessmentService.GetAssessment(SelectedCourse.SecondAssessmentId);
                SecondAssessmentDueDate = SecondAssessment.DueDate.ToShortDateString();
            }

            Debug.WriteLine($"Viewing course \"{SelectedCourse.CourseName}\"");
        }

        private void EditCourse()
        {

        }

        private async void RemoveCourse()
        {
            await Services.CourseService.RemoveCourse(SelectedCourse);

            var route = $"///{nameof(DegreePlanPage)}";
            await Shell.Current.GoToAsync(route);
        }
    }
}
