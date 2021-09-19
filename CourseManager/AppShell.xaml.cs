using CourseManager.ViewModels;
using CourseManager.Views;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CourseManager
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AddTermPage), typeof(AddTermPage));
            Routing.RegisterRoute(nameof(EditTermPage), typeof(EditTermPage));
            Routing.RegisterRoute(nameof(AddCoursePage), typeof(AddCoursePage));
            Routing.RegisterRoute(nameof(AddInstructorPage), typeof(AddInstructorPage));
            Routing.RegisterRoute(nameof(AddAssessmentsPage), typeof(AddAssessmentsPage));
            Routing.RegisterRoute(nameof(ViewCoursePage), typeof(ViewCoursePage));
            Routing.RegisterRoute(nameof(EditInstructorPage), typeof(EditInstructorPage));
            Routing.RegisterRoute(nameof(EditAssessmentsPage), typeof(EditAssessmentsPage));
            Routing.RegisterRoute(nameof(EditCourseNotesPage), typeof(EditCourseNotesPage));
            Routing.RegisterRoute(nameof(EditCoursePage), typeof(EditCoursePage));
        }

        //private async void OnMenuItemClicked(object sender, EventArgs e)
        //{
        //    await Shell.Current.GoToAsync("//LoginPage");
        //}
    }
}
