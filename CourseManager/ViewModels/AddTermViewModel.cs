using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    [QueryProperty(nameof(TermName), nameof(TermName))]
    public class AddTermViewModel : BaseViewModel
    {
        private string _termName;
        public string TermName 
        { 
            get => _termName;
            set
            {
                SetProperty(ref _termName, value);
                Validate();
            }
        }

        private DateTime _minStartDate = DateTime.Now;
        public DateTime MinStartDate { get => _minStartDate; set => SetProperty(ref _minStartDate, value); }

        private DateTime _maxStartDate;
        public DateTime MaxStartDate { get => _maxStartDate; set => SetProperty(ref _maxStartDate, value); }

        private DateTime _startDate;
        public DateTime StartDate 
        { 
            get => _startDate;
            set
            {
                SetProperty(ref _startDate, value);
                MinEndDate = _startDate.AddDays(1);
            }
        }

        private DateTime _minEndDate;
        public DateTime MinEndDate { get => _minEndDate; set => SetProperty(ref _minEndDate, value); }

        private DateTime _maxEndDate;
        public DateTime MaxEndDate { get => _maxEndDate; set => SetProperty(ref _maxEndDate, value); }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set
            { 
                SetProperty(ref _endDate, value);
                MaxStartDate = EndDate.AddDays(-1);
            }
        }

        private bool _hasErrors = false;
        public bool HasErrors { get => _hasErrors; set => SetProperty(ref _hasErrors, value); }

        public string TermNameErrorMessage { get; } = "Required";

        private bool _showTermNameErrorMessage = false;
        public bool ShowTermNameErrorMessage { get => _showTermNameErrorMessage; set => SetProperty(ref _showTermNameErrorMessage, value); }

        public Command SaveCommand { get; }
        public Command NavigateBackCommand { get; }

        public AddTermViewModel()
        {
            Title = "Add Term";
            SaveCommand = new Command(Save);
            NavigateBackCommand = new Command(NavigateBack);

            MinStartDate = DateTime.Now;
            StartDate = MinStartDate;

            MinEndDate = StartDate.AddDays(1);
            EndDate = StartDate.AddDays(1);

            MaxStartDate = EndDate;
            MaxEndDate = MaxStartDate.AddDays(30);
            
        }

        private async void Save()
        {
            Validate();
            if(HasErrors)
            {
                await Shell.Current.DisplayAlert("Oops!", "It looks like your form has some errors that need to be fixed before continuing.", "OK");
                return;
            }

            await Services.TermService.AddTerm(TermName, StartDate, EndDate);

            await Shell.Current.GoToAsync("..");
        }

        public async void NavigateBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        private void Validate()
        {
            ShowTermNameErrorMessage = string.IsNullOrWhiteSpace(TermName);
            HasErrors = ShowTermNameErrorMessage;
        }
    }
}
