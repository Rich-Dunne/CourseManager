using CourseManager.Enums;
using CourseManager.Models;
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
            if(database == null)
            {
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WGUCourses.db");
                database = new SQLiteAsyncConnection(databasePath);
            }

            await database.CreateTableAsync<Course>();
        }

        public static async Task ListTables()
        {
            Debug.WriteLine($"Below is a list of tables which exist in the database:");
            foreach (var table in database.TableMappings)
            {
                var tableInfo = await database.GetTableInfoAsync(table.TableName);
                if (tableInfo.Count > 0)
                {
                    Debug.WriteLine($"{table.TableName} ({tableInfo.Count} columns)");
                }
            }
        }

        public static async Task ImportTerms()
        {
            var tableInfo = database.GetTableInfoAsync(nameof(Courses));
            if(tableInfo.Result.Count == 0)
            {
                Debug.WriteLine($"Courses table doesn't exist - Creating new table.");
                await database.CreateTableAsync<Term>();
                return;
            }

            var query = database.Table<Course>().ToListAsync();
            Debug.WriteLine($"Terms being imported from DB: {query.Result.Count}.");
            foreach (var result in query.Result)
            {
                Debug.WriteLine($"ID: {result.Id}, Name: {result.CourseName}");
                Courses.Add(result);
            }
        }


        public static void GetTables()
        {
            var table = database.Table<Course>();
            if(table == null)
            {
                Debug.WriteLine($"The Courses table does not exist.");
                return;
            }

            var query = table.Where(x => x.Id >= 0);
            if(query.CountAsync().Result == 0)
            {
                Debug.WriteLine($"There are no courses in the table.");
            }

            query.ToListAsync().ContinueWith((t) =>
            {
                foreach (var course in t.Result)
                    Debug.WriteLine($"Course: {course.CourseName}");
            });
        }

        public static async Task AddCourse(string termName, DateTime startDate, DateTime endDate, int associatedTermId)
        {
            await Init();

            var course = new Course
            {
                CourseName = termName,
                StartDate = startDate,
                EndDate = endDate,
                Instructor = new Instructor(),
                Status = Status.Active,
                Notes = "Some course notes",
                CourseInformation = "Detailed course information",
                EnableNotifications = true,
                AssociatedTermId = associatedTermId
            };

            await database.InsertAsync(course);
            Debug.WriteLine($"Course added.");
        }

        public static async Task RemoveCourse(int id)
        {
            await Init();

            await database.DeleteAsync<Course>(id);
            Debug.WriteLine($"Course removed.");
        }

        public static async Task<IEnumerable<Course>> GetCourses()
        {
            await Init();

            var course = await database.Table<Course>().ToListAsync();
            return course;
        }

        public static async Task DropTable()
        {
            await Init();

            await database.DropTableAsync<Course>().ContinueWith((results) => Debug.WriteLine($"Table deleted."));
        }

        public static async Task ClearTable()
        {
            var table = database.Table<Course>();
            if (table == null)
            {
                Debug.WriteLine($"The Courses table does not exist.");
                return;
            }

            await database.DeleteAllAsync<Course>();
            Debug.WriteLine($"The Courses table has been cleared.");
        }

        public static void GetTableRows()
        {
            var query = database.QueryAsync<Course>("SELECT * from Course");

            if(query.Result.Count == 0)
            {
                Debug.WriteLine($"There are no courses.");
            }

            foreach(var c in query.Result)
            {
                Debug.WriteLine($"Result: {c.CourseName}");
            }
        }
    }
}
