using CourseManager.Models;
using CourseManager.Views;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    [QueryProperty(nameof(Username), nameof(Username))]
    public class DegreePlanViewModel : BaseViewModel
    {
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
            }
        }

        private Term _selectedTerm;
        public Term SelectedTerm
        {
            get => _selectedTerm;
            set
            {
                SetProperty(ref _selectedTerm, value);
            }
        }

        private Course _recentlySelectedCourse;
        public Course RecentlySelectedCourse
        {
            get => _recentlySelectedCourse;
            set
            {
                SetProperty(ref _recentlySelectedCourse, value);
            }
        }

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

        public ObservableCollection<TermGroup> TermGroups { get; } = Services.TermService.TermGroups;
        public ObservableCollection<Course> Courses { get; } = Services.CourseService.Courses;
        public Command LoadItemsCommand { get; }
        //public Command<Term> TermTapped { get; }
        //public Command GetTablesCommand { get; }
        //public Command ShowTableContentsCommand { get; }
        //public Command DropTableCommand { get; }
        public Command ClearTermTableCommand { get; }
        public Command ClearCourseTableCommand { get; }
        public Command ClearInstructorTableCommand { get; }
        public Command ClearAssessmentTableCommand { get; }
        public Command NavigateAddTermCommand { get; }
        public Command<TermGroup> NavigateModifyTermCommand { get; }
        public Command NavigateAddCourseCommand { get; }
        public Command NavigateViewCourseCommand { get; }

        public DegreePlanViewModel()
        {
            Title = "Degree Plan";

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            var initInstructors = Services.InstructorService.Init();
            var initCourses = Services.CourseService.Init();
            var initAssessments = Services.AssessmentService.Init();
            var initTerms = Services.TermService.Init();

            //GetTablesCommand = new Command(ListTables);
            //ShowTableContentsCommand = new Command(ShowTableContents);
            //DropTableCommand = new Command(DropTable);

            ClearTermTableCommand = new Command(ClearTermTable);
            ClearCourseTableCommand = new Command(ClearCourseTable);
            ClearInstructorTableCommand = new Command(ClearInstructorTable);
            ClearAssessmentTableCommand = new Command(ClearAssessmentTable);


            NavigateAddTermCommand = new Command(NavigateAddTerm);
            NavigateModifyTermCommand = new Command<TermGroup>(NavigateModifyTerm);
            NavigateAddCourseCommand = new Command(NavigateAddCourse);
            NavigateViewCourseCommand = new Command(NavigateViewCourse);

            PopulateTermsView();
            //Debug.WriteLine($"SelectedCourse: {SelectedCourse}");
            TermsExist = TermGroups.Count > 0;
        }

        private async void PopulateTermsView()
        {
            await Services.InstructorService.ImportInstructors();
            await Services.CourseService.ImportCourses();
            await Services.AssessmentService.ImportAssessments();
            await Services.TermService.ImportTerms();
        }

            //private async void ListTables()
            //{
            //    await Services.TermService.ListTables();
            //}

            //private void ShowTableContents()
            //{
            //    Services.TermService.GetTables();
            //}

            //private void DropTable()
            //{
            //    var dropTable = Services.TermService.DropTable();
            //}

        private async void ClearTermTable()
        {
            await Services.TermService.ClearTable();
        }

        private async void ClearCourseTable()
        {
            await Services.CourseService.ClearTable();
            await Services.TermService.ImportTerms();
        }

        private async void ClearInstructorTable()
        {
            await Services.InstructorService.ClearTable();
        }

        private async void ClearAssessmentTable()
        {
            await Services.AssessmentService.ClearTable();
        }

        private async void NavigateAddTerm()
        {
            var route = $"{nameof(AddTermPage)}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateModifyTerm(TermGroup termGroup)
        {
            var route = $"{nameof(ModifyTermPage)}?Id={termGroup.Id}";
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

        //private async void ClearDatabase()
        //{
        //    await Services.TermService.DropTable();
        //}

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