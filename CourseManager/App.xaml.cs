using CourseManager.Models;
using CourseManager.Services;
using CourseManager.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CourseManager
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
