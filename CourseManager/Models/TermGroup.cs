using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CourseManager.Models
{
    public class TermGroup : ObservableCollection<Course>
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        
        public TermGroup(int id, string name, DateTime startDate, DateTime endDate, ObservableCollection<Course> courses) : base(courses)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
