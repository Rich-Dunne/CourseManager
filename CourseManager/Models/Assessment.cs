using CourseManager.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseManager.Models
{
    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public AssessmentType AssessmentType { get; set; }
        public DateTime DueDate { get; set; }
        public bool EnableNotifications { get; set; }

        public Assessment() { }
    }
}
