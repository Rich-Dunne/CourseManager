﻿using CourseManager.Enums;
using CourseManager.Models;
using Plugin.LocalNotifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManager.Services
{
    public static class CourseService
    {
        public static SQLiteAsyncConnection database;
        public static ObservableCollection<Course> Courses { get; } = new ObservableCollection<Course>();

        public static async Task Init()
        {
            if (database == null)
            {
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WGUCourses.db");
                database = new SQLiteAsyncConnection(databasePath);
            }

            await database.CreateTableAsync<Course>();
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

        public static async Task ImportCourses()
        {
            await Init();

            var tableInfo = await database.GetTableInfoAsync("Courses");
            if(tableInfo.Count == 0)
            {
                Debug.WriteLine($"Courses table doesn't exist - Creating new table.");
                return;
            }

            Courses.Clear();

            var query = await database.Table<Course>().ToListAsync();
            Debug.WriteLine($"Courses being imported from DB: {query.Count}.");
            foreach (var result in query)
            {
                Debug.WriteLine($"ID: {result.Id}, Name: {result.CourseName}, Associated term: {result.AssociatedTermId}, Associated Instructor: {result.AssociatedInstructorId}");
                Courses.Add(result);

                if (result.EnableNotifications && (result.StartDate - DateTime.Now).TotalDays < 7)
                {
                    Debug.WriteLine($"Course starting soon");
                    CrossLocalNotifications.Current.Show("Course starting soon", $"{result.CourseName} is starting on {result.StartDate.ToShortDateString()}", result.Id, DateTime.Now.AddSeconds(5));
                }

                if (result.EnableNotifications && (result.EndDate - DateTime.Now).TotalDays < 30)
                {
                    Debug.WriteLine($"Course ending soon");
                    CrossLocalNotifications.Current.Show("Course ending soon", $"{result.CourseName} is ending on {result.EndDate.ToShortDateString()}", result.Id, DateTime.Now.AddSeconds(5));
                }
            }
        }


        //public static void GetTables()
        //{
        //    var table = database.Table<Course>();
        //    if(table == null)
        //    {
        //        Debug.WriteLine($"The Courses table does not exist.");
        //        return;
        //    }

        //    var query = table.Where(x => x.Id >= 0);
        //    if(query.CountAsync().Result == 0)
        //    {
        //        Debug.WriteLine($"There are no courses in the table.");
        //    }

        //    query.ToListAsync().ContinueWith((t) =>
        //    {
        //        foreach (var course in t.Result)
        //            Debug.WriteLine($"Course: {course.CourseName}");
        //    });
        //}

        public static async Task AddCourse(Course course)
        {
            await Init();

            var query = await database.Table<Course>().FirstOrDefaultAsync(x => x.CourseName == course.CourseName);
            if (query != null)
            {
                Debug.WriteLine($"\"{course.CourseName}\" already exists.");
                return;
            }

            await database.InsertAsync(course);
            Debug.WriteLine($"({course.Id}) \"{course.CourseName}\" added.");

            await ImportCourses();
        }

        public static Course GetCourse(int id)
        {
            var course = database.FindAsync<Course>(id).Result;
            return course;
        }

        public static async Task UpdateCourse(Course course)
        {
            await database.UpdateAsync(course);
            Debug.WriteLine($"({course.Id}) \"{course.CourseName}\" updated.");

            await ImportCourses();
        }

        public static async Task RemoveCourse(Course course)
        {
            await Init();

            var matchingCourse = Courses.FirstOrDefault(x => x.CourseName == course.CourseName);
            if (matchingCourse != null)
            {
                Courses.Remove(matchingCourse);
            }

            await TermService.RemoveCourseFromTerm(course);

            await database.DeleteAsync<Course>(course.Id);
            Debug.WriteLine($"({course.Id}) \"{course.CourseName}\" removed.");
        }

        //public static async Task<IEnumerable<Course>> GetCourses()
        //{
        //    await Init();

        //    var course = await database.Table<Course>().ToListAsync();
        //    return course;
        //}

        //public static async Task DropTable()
        //{
        //    await Init();

        //    await database.DropTableAsync<Course>().ContinueWith((results) => Debug.WriteLine($"Table deleted."));
        //}

        public static async Task ClearTable()
        {
            var table = database.Table<Course>();
            if (table == null)
            {
                Debug.WriteLine($"The Courses table does not exist.");
                return;
            }

            foreach(TermGroup termGroup in TermService.TermGroups)
            {
                termGroup.Clear();
            }

            Courses.Clear();
            await database.DeleteAllAsync<Course>();
            Debug.WriteLine($"The Courses table has been cleared.");
        }

        public static async void RemoveInstructorFromCourse(Instructor instructor)
        {
            foreach(Course course in Courses)
            {
                if(course.AssociatedInstructorId == instructor.Id)
                {
                    course.AssociatedInstructorId = 0;
                }
                await database.UpdateAsync(course);
            }
        }

        public static async void AddInstructorToCourse(Instructor instructor, int courseId)
        {
            var course = Courses.FirstOrDefault(x => x.Id == courseId);
            if(course == null)
            {
                Debug.WriteLine($"No matching course found.");
                return;
            }

            course.AssociatedInstructorId = instructor.Id;
            await UpdateCourse(course);
        }

        public static async void RemoveAssessmentFromCourse(Assessment assessment)
        {
            foreach (Course course in Courses)
            {
                if (course.FirstAssessmentId == assessment.Id)
                {
                    course.FirstAssessmentId = 0;
                }
                else if (course.SecondAssessmentId == assessment.Id)
                {
                    course.SecondAssessmentId = 0;
                }
                await database.UpdateAsync(course);
            }
        }

        //public static void GetTableRows()
        //{
        //    var query = database.QueryAsync<Course>("SELECT * from Course");

        //    if(query.Result.Count == 0)
        //    {
        //        Debug.WriteLine($"There are no courses.");
        //    }

        //    foreach(var c in query.Result)
        //    {
        //        Debug.WriteLine($"Result: {c.CourseName}");
        //    }
        //}
    }
}
