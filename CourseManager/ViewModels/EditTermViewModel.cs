using CourseManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CourseManager.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    public class EditTermViewModel : BaseViewModel
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                SetProperty(ref _id, value);
                TermToModify = Services.TermService.GetTerm(Id);
                InitializeProperties();
            }
        }

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

        public string TERM_NAME_REQUIRED { get; } = "Required";
        public string TERM_NAME_TAKEN { get; } = "A term with this name already exists";

        private bool _showTermNameRequiredErrorMessage = false;
        public bool ShowTermNameRequiredErrorMessage { get => _showTermNameRequiredErrorMessage; set => SetProperty(ref _showTermNameRequiredErrorMessage, value); }

        private bool _showTermNameTakenErrorMessage = false;
        public bool ShowTermNameTakenErrorMessage { get => _showTermNameTakenErrorMessage; set => SetProperty(ref _showTermNameTakenErrorMessage, value); }

        public Command SaveCommand { get; }
        public Command RemoveCommand { get; }
        public Command NavigateBackCommand { get; }

        public Term TermToModify;

        public EditTermViewModel()
        {
            Title = "Edit Term";
            SaveCommand = new Command(Save);
            RemoveCommand = new Command(Remove);
            NavigateBackCommand = new Command(NavigateBack);

            MinStartDate = DateTime.Now.AddDays(-365);
            MaxEndDate = DateTime.Now.AddDays(365);
            MaxStartDate = MaxEndDate.AddDays(-1);
            MinEndDate = MinStartDate.AddDays(1);

            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddDays(1);
        }

        private async void Save()
        {
            Validate();
            if(HasErrors)
            {
                await Shell.Current.DisplayAlert("Oops!", "It looks like your form has some errors that need to be fixed before continuing.", "OK");
                return;
            }

            TermToModify.TermName = TermName;
            TermToModify.StartDate = StartDate;
            TermToModify.EndDate = EndDate;
            await Services.TermService.UpdateTerm(TermToModify);
            Debug.WriteLine($"Updated term {TermToModify.TermName}");

            await Shell.Current.GoToAsync("..");
        }

        private async void Remove()
        {
            var termToRemove = Services.TermService.GetTerm(Id);
            await Services.TermService.RemoveTerm(termToRemove);

            await Shell.Current.GoToAsync("..");
        }

        private async void NavigateBack() => await Shell.Current.GoToAsync("..");

        private void InitializeProperties()
        {
            var termGroupToEdit = Services.TermService.TermGroups.FirstOrDefault(x => x.Id == Id);
            if (termGroupToEdit == null)
            {
                Debug.WriteLine($"Term group not found.");
                return;
            }
            TermName = termGroupToEdit.Name;
            StartDate = termGroupToEdit.StartDate;
            EndDate = termGroupToEdit.EndDate;

            MinStartDate = DateTime.Now;
            MaxStartDate = EndDate.AddDays(-1);
            MinEndDate = StartDate.AddDays(1);
            MaxEndDate = MaxStartDate.AddDays(30);
        }

        private void Validate()
        {
            ShowTermNameRequiredErrorMessage = string.IsNullOrWhiteSpace(TermName);
            ShowTermNameTakenErrorMessage = TermName != TermToModify.TermName && Services.TermService.TermGroups.Any(x => x.Name == TermName);
            
            
            HasErrors = ShowTermNameRequiredErrorMessage || ShowTermNameTakenErrorMessage;
        }
    }
}
