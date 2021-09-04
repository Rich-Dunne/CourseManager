using CourseManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseManager.Models
{
    public static class DataStore
    {
        public static List<Assessment> Assessments = new List<Assessment>();
        public static List<Course> Courses = new List<Course>();
        public static List<Instructor> Instructors = new List<Instructor>();

        public static void InitializeData()
        {
            Instructors.Add(new Instructor("Richard", "Noggin", "555-123-4567", "rnoggin@wgu.edu"));
            Instructors.Add(new Instructor("Anita", "Baff", "555-867-5309", "abaff@wgu.edu"));

            Assessments.Add(new Assessment("Mobile App Assessment One", AssessmentType.Objective, new DateTime(2021, 10, 15), true));
            Assessments.Add(new Assessment("Mobile App Assessment Two", AssessmentType.Performance, new DateTime(2021, 10, 30), true));

            Courses.Add(new Course("C971 Mobile Software Development", new DateTime(2021,10,1), new DateTime(2021,10,31), Instructors.First(), Status.Active, "No notes", "This is a course about mobile app development.", Assessments, true));
        }
    }
}
