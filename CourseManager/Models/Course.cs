using CourseManager.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseManager.Models
{
    public class Course
    {
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Instructor Instructor { get; set; }
        public Status Status { get; set; }
        public string Notes { get; set; }
        public string CourseInformation { get; set; }
        public List<Assessment> Assessments { get; set; }
        public bool EnableNotifications { get; set; }

        public Course(string courseName, DateTime startDate, DateTime endDate, Instructor instructor, Status status, string notes, string courseInformation, List<Assessment> assessments, bool enableNotifications)
        {
            CourseName = courseName;
            StartDate = startDate;
            EndDate = endDate;
            Instructor = instructor;
            Status = status;
            Notes = notes;
            CourseInformation = courseInformation;
            Assessments = assessments;
            EnableNotifications = enableNotifications;
        }
    }
}
