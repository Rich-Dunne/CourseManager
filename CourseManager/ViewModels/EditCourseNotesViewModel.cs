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
    public class EditCourseNotesViewModel : BaseViewModel
    {
        private int _courseId;
        public int CourseId 
        { 
            get => _courseId;
            set
            {
                SetProperty(ref _courseId, value);
                GetCourse();
            }
        }

        public string _courseNotes;
        public string CourseNotes { get => _courseNotes; set => SetProperty(ref _courseNotes, value); }


        private bool _hasErrors = false;
        public bool HasErrors { get => _hasErrors; set => SetProperty(ref _hasErrors, value); }

        public string CourseNameErrorMessage { get; } = "Required";

        private bool _showCourseNameErrorMessage = false;
        public bool ShowCourseNameErrorMessage { get => _showCourseNameErrorMessage; set => SetProperty(ref _showCourseNameErrorMessage, value); }

        public Course CurrentCourse;
        public Command SaveCommand { get; }
        public Command NavigateBackCommand { get; }

        public EditCourseNotesViewModel()
        {
            Title = "Edit Course Notes";
            SaveCommand = new Command(Save);
            NavigateBackCommand = new Command(NavigateBack);
        }



        private async void Save()
        {
            CurrentCourse.Notes = CourseNotes;
            await Services.CourseService.UpdateCourse(CurrentCourse);
            NavigateBack();
        }

        private async void NavigateBack() => await Shell.Current.GoToAsync("..");

        private void GetCourse()
        {
            CurrentCourse = Services.CourseService.GetCourse(CourseId);
        }
    }
}
