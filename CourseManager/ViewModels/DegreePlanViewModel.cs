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
    public class DegreePlanViewModel : BaseViewModel
    {
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
                    _selectedCourse = null;
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
        public Command NavigateAddTermCommand { get; }
        public Command<TermGroup> NavigateModifyTermCommand { get; }
        public Command NavigateAddCourseCommand { get; }

        public DegreePlanViewModel()
        {
            Title = "Degree Plan";

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            var initCourses = Services.CourseService.Init();
            var initTerms = Services.TermService.Init();
            //GetTablesCommand = new Command(ListTables);
            //ShowTableContentsCommand = new Command(ShowTableContents);
            //DropTableCommand = new Command(DropTable);
            ClearTermTableCommand = new Command(ClearTermTable);
            ClearCourseTableCommand = new Command(ClearCourseTable);
            NavigateAddTermCommand = new Command(NavigateAddTerm);
            NavigateModifyTermCommand = new Command<TermGroup>(NavigateModifyTerm);
            NavigateAddCourseCommand = new Command(NavigateAddCourse);

            PopulateTermsView();
            Debug.WriteLine($"TermGroups Count: {TermGroups.Count}");
            TermsExist = TermGroups.Count > 0;
        }

        private async void PopulateTermsView()
        {
            await Services.CourseService.ImportCourses();
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
            await Services.TermService.ImportTerms();

            IsBusy = false;
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedTerm = null;
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(Term term)
        {
            if (term == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={term.Id}");
        }
    }
}