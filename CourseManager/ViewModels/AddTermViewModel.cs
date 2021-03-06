using CourseManager.Models;
using System;
using System.Linq;
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

        #region Form Properties
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
        #endregion

        #region Validation Properties
        private bool _hasErrors = false;
        public bool HasErrors { get => _hasErrors; set => SetProperty(ref _hasErrors, value); }

        public string TERM_NAME_REQUIRED { get; } = "Required";
        public string TERM_NAME_TAKEN { get; } = "A term with this name already exists";

        private bool _showTermNameRequiredErrorMessage = false;
        public bool ShowTermNameRequiredErrorMessage { get => _showTermNameRequiredErrorMessage; set => SetProperty(ref _showTermNameRequiredErrorMessage, value); }

        private bool _showTermNameTakenErrorMessage = false;
        public bool ShowTermNameTakenErrorMessage { get => _showTermNameTakenErrorMessage; set => SetProperty(ref _showTermNameTakenErrorMessage, value); }
        #endregion

        #region Command Properties
        public Command SaveCommand { get; }
        public Command NavigateBackCommand { get; }
        #endregion

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

            Term newTerm = new Term
            {
                TermName = TermName,
                StartDate = StartDate,
                EndDate = EndDate
            };

            await Services.TermService.AddTerm(newTerm);

            await Shell.Current.GoToAsync("..");
        }

        public async void NavigateBack() => await Shell.Current.GoToAsync("..");

        private void Validate()
        {
            ShowTermNameRequiredErrorMessage = string.IsNullOrWhiteSpace(TermName);
            ShowTermNameTakenErrorMessage = Services.TermService.TermGroups.Any(x => x.Name == TermName);
            
            HasErrors = ShowTermNameRequiredErrorMessage || ShowTermNameTakenErrorMessage;
        }
    }
}
