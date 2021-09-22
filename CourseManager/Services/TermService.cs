﻿using CourseManager.Enums;
using CourseManager.Models;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CourseManager.Services
{
    public static class TermService
    {
        public static SQLiteAsyncConnection database;

        public static ObservableCollection<TermGroup> TermGroups { get; } = new ObservableCollection<TermGroup>();

        public static async Task Init()
        {
            if(database == null)
            {
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WGUTerms.db");
                database = new SQLiteAsyncConnection(databasePath);
            }

            await database.CreateTableAsync<Term>();
        }

        //public static async Task ListTables()
        //{
        //    Debug.WriteLine($"Below is a list of tables which exist in the database:");
        //    foreach (var table in database.TableMappings)
        //    {
        //        var tableInfo = await database.GetTableInfoAsync(table.TableName);
        //        if (tableInfo.Count > 0)
        //        {
        //            Debug.WriteLine($"{table.TableName} ({tableInfo.Count} columns)");
        //        }
        //    }
        //}

        public static async Task ImportTerms()
        {
            await Init();

            TermGroups.Clear();

            var query = database.Table<Term>().ToListAsync();
            if(query.Result.Count == 0)
            {
                Debug.WriteLine($"Creating mock data term");
                CreateMockData();
                return;
            }

            foreach (var result in query.Result)
            {
                var existingTermGroup = TermGroups.FirstOrDefault(x => x.Name == result.TermName);
                if (existingTermGroup == null)
                {
                    var courses = GetCourses(result);
                    AddTermGroup(result, courses);
                }
            }

            ObservableCollection<Course> GetCourses(Term term)
            {
                var courses = new ObservableCollection<Course>();
                foreach (Course course in CourseService.Courses)
                {
                    Debug.WriteLine($"Course: {course.CourseName}, Associated Term: {course.AssociatedTermId}");
                    if (course.AssociatedTermId == term.Id && !courses.Any(x => x.Id == course.Id))
                    {
                        courses.Add(course);
                        Debug.WriteLine($"Added \"{course.CourseName}\" to term group's course collection.");
                    }
                }

                return courses;
            }    
        }

        private static void AddTermGroup(Term term, ObservableCollection<Course> courses)
        {
            var newTermGroup = new TermGroup(term.Id, term.TermName, term.StartDate, term.EndDate, courses);

            TermGroups.Add(newTermGroup);
            Debug.WriteLine($"Added \"{newTermGroup.Name}\" to collection.");
        }

        private static async void CreateMockData()
        {
            var mockAssessmentOne = new Assessment
            {
                Name = "Mock Assessment One",
                AssessmentType = AssessmentType.Objective,
                DueDate = new DateTime(2021, 10, 31),
                EnableNotifications = true
            };
            await AssessmentService.AddAssessment(mockAssessmentOne);

            var mockAssessmentTwo = new Assessment
            {
                Name = "Mock Assessment Two",
                AssessmentType = AssessmentType.Performance,
                DueDate = new DateTime(2021, 10, 31),
                EnableNotifications = true
            };
            await AssessmentService.AddAssessment(mockAssessmentTwo);

            var mockInstructor = new Instructor
            {
                FirstName = "Rich",
                LastName = "Dunne",
                PhoneNumber = "2072192352",
                Email = "rdunne5@wgu.edu"
            };
            await InstructorService.AddInstructor(mockInstructor);

            var newCourse = new Course
            {
                CourseName = "Mock Course",
                StartDate = new DateTime(2021, 10, 1),
                EndDate = new DateTime(2021, 10, 31),
                Status = Status.Active,
                Notes = "This is a mock data course.",
                EnableNotifications = true
            };
            newCourse.AssociatedInstructorId = mockInstructor.Id;
            newCourse.FirstAssessmentId = mockAssessmentOne.Id;
            newCourse.SecondAssessmentId = mockAssessmentTwo.Id;

            Term mockTerm = new Term
            {
                TermName = "Mock Data Term",
                StartDate = new DateTime(2021, 10, 1),
                EndDate = new DateTime(2021, 10, 31)
            };

            await AddTerm(mockTerm);
            newCourse.AssociatedTermId = mockTerm.Id;

            await CourseService.AddCourse(newCourse);

            await ImportTerms();
        }

        public static async Task AddTerm(Term term)
        {
            await Init();

            await database.InsertAsync(term);
            Debug.WriteLine($"({term.Id}) \"{term.TermName}\" added.");

            await ImportTerms();
        }

        public static Term GetTerm(int id) => database.FindAsync<Term>(id).Result;

        public static async Task UpdateTerm(Term term)
        {
            await database.UpdateAsync(term);
            await ImportTerms();
        }

        public static async Task RemoveTerm(Term term)
        {
            await Init();

            var termGroup = TermGroups.FirstOrDefault(x => x.Name == term.TermName);
            if(termGroup != null)
            {
                TermGroups.Remove(termGroup);
            }

            await database.DeleteAsync<Term>(term.Id);
            Debug.WriteLine($"({term.Id}) \"{term.TermName}\" removed.");
        }

        public static async Task AddCourseToTerm(Course course)
        {
            var matchingGroup = TermGroups.FirstOrDefault(x => x.Id == course.AssociatedTermId);
            if (matchingGroup == null)
            {
                Debug.WriteLine($"Matching group not found.");
                return;
            }

            matchingGroup.Add(course);
            await ImportTerms();
        }

        public static void RemoveCourseFromTerm(Course course)
        {
            var matchingGroup = TermGroups.FirstOrDefault(x => x.Id == course.AssociatedTermId);
            if (matchingGroup == null)
            {
                Debug.WriteLine($"Matching group not found in RemoveCourseFromTerm.");
                return;
            }

            matchingGroup.Remove(course);
            course.AssociatedTermId = 0;
            Debug.WriteLine($"Course \"{course.CourseName}\" removed from term \"{matchingGroup.Name}\".");
            //await ImportTerms();
        }

        public static async Task DropTable()
        {
            await Init();

            await database.DropTableAsync<Term>().ContinueWith((results) => Debug.WriteLine($"Table dropped."));
        }

        public static async Task ClearTable()
        {
            await Init();

            TermGroups.Clear();

            await database.DeleteAllAsync<Term>();
            Debug.WriteLine($"The Terms table has been cleared.");

            await ImportTerms();
        }
    }
}
