using CourseManager.Models;
using CourseManager.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    [QueryProperty(nameof(CourseValues), nameof(CourseValues))]
    public class AddInstructorViewModel : BaseViewModel
    {
        private string _courseValues;
        public string CourseValues
        {
            get => _courseValues;
            set
            {
                SetProperty(ref _courseValues, value);
            }
        }

        private string _instructorFirstName;
        public string InstructorFirstName { get => _instructorFirstName; set => SetProperty(ref _instructorFirstName, value); }

        private string _instructorLastName;
        public string InstructorLastName { get => _instructorLastName; set => SetProperty(ref _instructorLastName, value); }

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
            if(string.IsNullOrWhiteSpace(_instructorFirstName))
            {
                return;
            }

            var instructorValues = $"{InstructorFirstName},{InstructorLastName},{InstructorPhoneNumber},{InstructorEmail}";
            var route = $"{nameof(AddAssessmentsPage)}?CourseValues={CourseValues}&InstructorValues={instructorValues}";
            await Shell.Current.GoToAsync(route);
        }

        private async void NavigateBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
