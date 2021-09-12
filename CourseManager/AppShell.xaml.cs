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
            Routing.RegisterRoute(nameof(ModifyTermPage), typeof(ModifyTermPage));
            Routing.RegisterRoute(nameof(AddCoursePage), typeof(AddCoursePage));
            Routing.RegisterRoute(nameof(AddInstructorPage), typeof(AddInstructorPage));
            Routing.RegisterRoute(nameof(AddAssessmentsPage), typeof(AddAssessmentsPage));
            Routing.RegisterRoute(nameof(ViewCoursePage), typeof(ViewCoursePage));
        }

        //private async void OnMenuItemClicked(object sender, EventArgs e)
        //{
        //    await Shell.Current.GoToAsync("//LoginPage");
        //}
    }
}
