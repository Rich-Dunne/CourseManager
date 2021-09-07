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
        public string TermName { get => _termName; set => SetProperty(ref _termName, value); }

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
        public DateTime EndDate { get => _endDate; set => SetProperty(ref _endDate, value); }

        public Command SaveCommand { get; }

        public AddTermViewModel()
        {
            Title = "Add Term";
            SaveCommand = new Command(Save);

            MinStartDate = DateTime.Now;
            MaxStartDate = DateTime.Now.AddDays(30);
            StartDate = MinStartDate;

            MinEndDate = StartDate.AddDays(1);
            MaxEndDate = MaxStartDate.AddDays(30);
            EndDate = StartDate.AddDays(1);
        }

        private async void Save()
        {
            if(string.IsNullOrWhiteSpace(_termName))
            {
                return;
            }

            await Services.TermService.AddTerm(TermName, StartDate, EndDate);

            await Shell.Current.GoToAsync("..");
        }
    }
}
