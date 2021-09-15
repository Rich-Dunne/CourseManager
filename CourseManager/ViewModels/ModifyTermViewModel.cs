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
    public class ModifyTermViewModel : BaseViewModel
    {
        private bool _propertiesInitialized = false;

        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                SetProperty(ref _id, value);
                if (!_propertiesInitialized)
                {
                    InitializeProperties();
                    _propertiesInitialized = true;
                }
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

        public string TermNameErrorMessage { get; } = "Required";

        private bool _showTermNameErrorMessage = false;
        public bool ShowTermNameErrorMessage { get => _showTermNameErrorMessage; set => SetProperty(ref _showTermNameErrorMessage, value); }

        public Command SaveCommand { get; }
        public Command RemoveCommand { get; }
        public Command NavigateBackCommand { get; }

        public ModifyTermViewModel()
        {
            Title = "Modify Term";
            SaveCommand = new Command(Save);
            RemoveCommand = new Command(Remove);
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

            var termToModify = Services.TermService.GetTerm(Id);
            termToModify.TermName = TermName;
            termToModify.StartDate = StartDate;
            termToModify.EndDate = EndDate;
            await Services.TermService.UpdateTerm(termToModify);
            Debug.WriteLine($"Updated term {termToModify.TermName}");

            await Shell.Current.GoToAsync("..");
        }

        private async void Remove()
        {
            var termToRemove = Services.TermService.GetTerm(Id);
            await Services.TermService.RemoveTerm(termToRemove);

            await Shell.Current.GoToAsync("..");
        }

        private async void NavigateBack()
        {
            await Shell.Current.GoToAsync("..");
        }


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
            ShowTermNameErrorMessage = string.IsNullOrWhiteSpace(TermName);
            HasErrors = ShowTermNameErrorMessage;
        }
    }
}
