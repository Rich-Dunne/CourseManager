using System;
using System.Collections.Generic;
using System.Text;

namespace CourseManager.Models
{
    public class Instructor
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get => FirstName + " " + LastName; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public Instructor(string firstName, string lastName, string phoneNumber, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
