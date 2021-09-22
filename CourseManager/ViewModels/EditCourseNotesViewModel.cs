using CourseManager.Models;
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
        public Course CurrentCourse;

        #region Form Properties
        public string _courseNotes;
        public string CourseNotes { get => _courseNotes; set => SetProperty(ref _courseNotes, value); }
        #endregion

        #region Validation Properties
        private bool _hasErrors = false;
        public bool HasErrors { get => _hasErrors; set => SetProperty(ref _hasErrors, value); }

        public string CourseNameErrorMessage { get; } = "Required";

        private bool _showCourseNameErrorMessage = false;
        public bool ShowCourseNameErrorMessage { get => _showCourseNameErrorMessage; set => SetProperty(ref _showCourseNameErrorMessage, value); }
        #endregion

        #region Command Properties
        public Command SaveCommand { get; }
        public Command NavigateBackCommand { get; }
        #endregion

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
            CourseNotes = CurrentCourse.Notes;
        }
    }
}
