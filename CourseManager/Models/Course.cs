using CourseManager.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseManager.Models
{
    [Table("Courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Instructor Instructor { get; set; }
        public Status Status { get; set; }
        public string Notes { get; set; }
        public string CourseInformation { get; set; }
        public bool EnableNotifications { get; set; }
        public int AssociatedTermId { get; set; }

        public Course() { }
    }
}
