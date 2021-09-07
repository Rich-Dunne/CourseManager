using System;
using System.Collections.Generic;
using System.Text;

namespace CourseManager.Models
{
    public class Instructor
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int AssociatedCourseId { get; set; }

        public Instructor() { }
    }
}
