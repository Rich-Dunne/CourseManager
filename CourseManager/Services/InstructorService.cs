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
    public static class InstructorService
    {
        public static SQLiteAsyncConnection database;
        public static ObservableCollection<Instructor> Instructors { get; } = new ObservableCollection<Instructor>();

        public static async Task Init()
        {
            if (database == null)
            {
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WGUInstructors.db");
                database = new SQLiteAsyncConnection(databasePath);
            }

            await database.CreateTableAsync<Instructor>();
        }

        public static async Task ImportInstructors()
        {
            await Init();

            var tableInfo = await database.GetTableInfoAsync("Instructors");
            if(tableInfo.Count == 0)
            {
                Debug.WriteLine($"Instructors table doesn't exist - Creating new table.");
                return;
            }

            Instructors.Clear();

            var query = await database.Table<Instructor>().ToListAsync();
            Debug.WriteLine($"Instructors being imported from DB: {query.Count}.");
            foreach (var result in query)
            {
                Debug.WriteLine($"ID: {result.Id}, Name: {result.FirstName} {result.LastName}");
                Instructors.Add(result);
            }
        }

        public static async Task AddInstructor(Instructor instructor)
        {
            await Init();

            var insertInstructor = database.InsertAsync(instructor).Result;
            Debug.WriteLine($"({instructor.Id}) \"{instructor.FirstName} {instructor.LastName}\" added.");

            await ImportInstructors();
        }

        public static Instructor GetInstructor(int id)
        {
            var course = database.FindAsync<Instructor>(id).Result;
            return course;
        }

        public static async Task UpdateInstructor(Instructor instructor)
        {
            var matchingInstructor = Instructors.FirstOrDefault(x => x.Id == instructor.Id);
            if (matchingInstructor == null)
            {
                Debug.WriteLine($"Matching instructor not found.");
                return;
            }

            //Instructors.Remove(matchingInstructor);
            await database.UpdateAsync(instructor);
            await ImportInstructors();
        }

        public static async Task RemoveInstructor(Instructor instructor)
        {
            await Init();

            var matchingCourse = Instructors.FirstOrDefault(x => x.Id == instructor.Id);
            if (matchingCourse != null)
            {
                Instructors.Remove(matchingCourse);
            }

            CourseService.RemoveInstructorFromCourse(instructor);

            await database.DeleteAsync<Instructor>(instructor.Id);
            Debug.WriteLine($"({instructor.Id}) \"{instructor.FirstName} {instructor.LastName}\" removed.");
        }

        public static async Task ClearTable()
        {
            var table = database.Table<Instructor>();
            if (table == null)
            {
                Debug.WriteLine($"The Instructors table does not exist.");
                return;
            }

            foreach (Instructor instructor in Instructors)
            {
                CourseService.RemoveInstructorFromCourse(instructor);
            }

            Instructors.Clear();
            await database.DeleteAllAsync<Instructor>();
            Debug.WriteLine($"The Instructors table has been cleared.");
        }
    }
}
