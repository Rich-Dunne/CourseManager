using CourseManager.Models;
using CourseManager.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    public class AddInstructorViewModel : BaseViewModel
    {
        private string _instructorName;
        public string InstructorName { get => _instructorName; set => SetProperty(ref _instructorName, value); }

        private string _instructorPhoneNumber;
        public string InstructorPhoneNumber { get => _instructorPhoneNumber; set => SetProperty(ref _instructorPhoneNumber, value); }

        private string _instructorEmail;
        public string InstructorEmail { get => _instructorEmail; set => SetProperty(ref _instructorEmail, value); }

        public Command NavigateAddAssessmentsCommand { get; }
        public Command NavigateBackCommand { get; }

        public AddInstructorViewModel()
        {
            Title = "Add Instructor";
            NavigateAddAssessmentsCommand = new Command(NavigateAddAssessments);
            NavigateBackCommand = new Command(NavigateBack);

        }

        private async void NavigateAddAssessments()
        {
            if(string.IsNullOrWhiteSpace(_instructorName))
            {
                return;
            }

            var route = $"{nameof(AddAssessmentsPage)}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
