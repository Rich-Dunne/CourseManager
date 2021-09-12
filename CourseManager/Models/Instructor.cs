using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseManager.Models
{
    [Table("Instructors")]
    public class Instructor
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public Instructor() { }
    }
}
