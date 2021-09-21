using CourseManager.Enums;
using SQLite;
using System;

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
        public Status Status { get; set; }
        public string Notes { get; set; }
        public bool EnableNotifications { get; set; }
        public int AssociatedTermId { get; set; }
        public int AssociatedInstructorId { get; set; }
        public int FirstAssessmentId { get; set; }
        public int SecondAssessmentId { get; set; }

        public Course() { }
    }
}
