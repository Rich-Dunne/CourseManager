using CourseManager.Models;
using CourseManager.Views;
using Plugin.LocalNotifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    [QueryProperty(nameof(Username), nameof(Username))]
    public class DegreePlanViewModel : BaseViewModel
    {
        private string _username;
        public string Username { get => _username; set => SetProperty(ref _username, value); }

        public ObservableCollection<TermGroup> TermGroups { get; } = Services.TermService.TermGroups;
        public ObservableCollection<Course> Courses { get; } = Services.CourseService.Courses;

        private Term _selectedTerm;
        public Term SelectedTerm { get => _selectedTerm; set => SetProperty(ref _selectedTerm, value); }

        private Course _recentlySelectedCourse;
        public Course RecentlySelectedCourse { get => _recentlySelectedCourse; set => SetProperty(ref _recentlySelectedCourse, value); }

        private Course _selectedCourse = null;
        public Course SelectedCourse
        {
            get => _selectedCourse;
            set
            {
                RecentlySelectedCourse = value;
                SetProperty(ref _selectedCourse, value);
                if (value != null)
                {
                    NavigateViewCourse();
                    _selectedCourse = null;
                }
            }
        }

        private bool _termsExist = false;
        public bool TermsExist { get => _termsExist; set => SetProperty(ref _termsExist, value); }

        #region Command Properties
        public Command LoadItemsCommand { get; }
        public Command ClearTermTableCommand { get; }
        public Command ClearCourseTableCommand { get; }
        public Command ClearInstructorTableCommand { get; }
        public Command ClearAssessmentTableCommand { get; }
        public Command NavigateAddTermCommand { get; }
        public Command<TermGroup> NavigateModifyTermCommand { get; }
        public Command NavigateAddCourseCommand { get; }
        public Command NavigateViewCourseCommand { get; }
        #endregion

        public DegreePlanViewModel()
        {
            Title = "Degree Plan";

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            var initInstructors = Services.InstructorService.Init();
            var initCourses = Services.CourseService.Init();
            var initAssessments = Services.AssessmentService.Init();
            var initTerms = Services.TermService.Init();

            ClearTermTableCommand = new Command(ClearTermTable);
            ClearCourseTableCommand = new Command(ClearCourseTable);
            ClearInstructorTableCommand = new Command(ClearInstructorTable);
            ClearAssessmentTableCommand = new Command(ClearAssessmentTable);


            NavigateAddTermCommand = new Command(NavigateAddTerm);
            NavigateModifyTermCommand = new Command<TermGroup>(NavigateModifyTerm);
            NavigateAddCourseCommand = new Command(NavigateAddCourse);
            NavigateViewCourseCommand = new Command(NavigateViewCourse);

            TermsExist = TermGroups.Count > 0;
        }

        private async void ClearTermTable()
        {
            await Services.TermService.ClearTable();
            await Shell.Current.DisplayAlert("Table cleared", "The Terms table has been cleared successfully.", "Ok");
        }

        private async void ClearCourseTable()
        {
            await Services.CourseService.ClearTable();
            await Shell.Current.DisplayAlert("Table cleared", "The Courses table has been cleared successfully.", "Ok");
            await Services.TermService.ImportTerms();
        }

        private async void ClearInstructorTable()
        {
            await Services.InstructorService.ClearTable();
            await Shell.Current.DisplayAlert("Table cleared", "The Instructors table has been cleared successfully.", "Ok");
        }

        private async void ClearAssessmentTable()
        {
            await Services.AssessmentService.ClearTable();
            await Shell.Current.DisplayAlert("Table cleared", "The Assessments table has been cleared successfully.", "Ok");
        }

        private async void NavigateAddTerm()
        {
            var route = $"{nameof(AddTermPage)}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateModifyTerm(TermGroup termGroup)
        {
            var route = $"{nameof(EditTermPage)}?Id={termGroup.Id}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateAddCourse()
        {
            if(TermGroups.Count == 0)
            {
                return;
            }

            var route = $"{nameof(AddCoursePage)}";
            await Shell.Current.GoToAsync(route);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            await Task.Delay(1000);
            await Services.TermService.ImportTerms();

            IsBusy = false;
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedTerm = null;
        }

        async void NavigateViewCourse()
        {
            var route = $"{nameof(ViewCoursePage)}?CourseId={SelectedCourse.Id}";
            await Shell.Current.GoToAsync(route);
        }
    }
}