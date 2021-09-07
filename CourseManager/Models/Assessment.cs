using CourseManager.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseManager.Models
{
    public class Assessment
    {
        public string Name { get; set; }
        public AssessmentType AssessmentType { get; set; }
        public DateTime DueDate { get; set; }
        public bool EnableNotifications { get; set; }
        public int AssociatedCourseId { get; set; }

        public Assessment() { }
    }
}
