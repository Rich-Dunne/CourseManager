using CourseManager.Models;
using CourseManager.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

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
                InitializeSelectedCourse();
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
        public Command NavigateEditInstructorCommand { get; }
        public Command NavigateEditAssessmentsCommand { get; }
        public Command NavigateEditCourseNotesCommand { get; }
        public Command EditCourseCommand { get; }
        public Command RemoveCourseCommand { get; }
        public Command ShareCommand { get; }

        public ViewCourseViewModel()
        {
            Title = "View Course";
            NavigateBackCommand = new Command(NavigateBack);
            NavigateEditInstructorCommand = new Command(NavigateEditInstructor);
            NavigateEditAssessmentsCommand = new Command(NavigateEditAssessments);
            NavigateEditCourseNotesCommand = new Command(NavigateEditCourseNotes);
            EditCourseCommand = new Command(EditCourse);
            RemoveCourseCommand = new Command(RemoveCourse);
            ShareCommand = new Command(Share);

            if(!_propertiesInitialized)
            {
                InitializeSelectedCourse();
                _propertiesInitialized = true;
            }
        }

        private async void NavigateBack() => await Shell.Current.GoToAsync("..");

        private async void NavigateEditInstructor()
        {
            var route = $"{nameof(EditInstructorPage)}?InstructorId={Instructor.Id}&CourseId={CourseId}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateEditAssessments()
        {
            int secondAssessmentId = 0;
            if(SecondAssessment != null)
            {
                secondAssessmentId = SecondAssessment.Id;
            }

            var route = $"{nameof(EditAssessmentsPage)}?CourseId={CourseId}&FirstAssessmentId={FirstAssessment.Id}&SecondAssessmentId={secondAssessmentId}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateEditCourseNotes()
        {
            var route = $"{nameof(EditCourseNotesPage)}?CourseId={CourseId}";
            await Shell.Current.GoToAsync(route);
        }

        private void InitializeSelectedCourse()
        {
            SelectedCourse = Services.CourseService.GetCourse(CourseId);
            if (SelectedCourse == null)
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

            if (SelectedCourse.SecondAssessmentId != 0)
            {
                HasSecondAssessment = true;
                SecondAssessment = Services.AssessmentService.GetAssessment(SelectedCourse.SecondAssessmentId);
                SecondAssessmentDueDate = SecondAssessment.DueDate.ToShortDateString();
            }
            else
            {
                HasSecondAssessment = false;
                SecondAssessment = null;
            }

            Debug.WriteLine($"Viewing course \"{SelectedCourse.CourseName}\"");
        }

        private async void EditCourse()
        {
            var route = $"{nameof(EditCoursePage)}?CourseId={CourseId}";
            await Shell.Current.GoToAsync(route);
        }

        private async void RemoveCourse()
        {
            await Services.CourseService.RemoveCourse(SelectedCourse);

            NavigateBack();
        }

        private async void Share()
        {
            await Xamarin.Essentials.Share.RequestAsync(new ShareTextRequest
            {
                Text = SelectedCourse.Notes,
                Title = "Share course notes"
            });
        }
    }
}
