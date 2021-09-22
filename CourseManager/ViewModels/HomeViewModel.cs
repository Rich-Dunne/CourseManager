using System.Windows.Input;
using Xamarin.Forms;
using CourseManager.Views;

namespace CourseManager.ViewModels
{
    [QueryProperty(nameof(Username), nameof(Username))]
    public class HomeViewModel : BaseViewModel
    {
        private string _username;
        public string Username { get => _username; set => SetProperty(ref _username, value); }

        public ICommand NavigateDegreePlanCommand { get; }

        public HomeViewModel()
        {
            Title = "Home";
            NavigateDegreePlanCommand = new Command(async () => await Shell.Current.GoToAsync($"///{nameof(DegreePlanPage)}?Username={Username}"));
            ImportData();
        }

        private async void ImportData()
        {
            await Services.InstructorService.ImportInstructors();
            await Services.CourseService.ImportCourses();
            await Services.AssessmentService.ImportAssessments();
            await Services.TermService.ImportTerms();
        }
    }
}