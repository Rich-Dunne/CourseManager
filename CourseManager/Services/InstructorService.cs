using CourseManager.Models;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

            await database.InsertAsync(instructor);
            Debug.WriteLine($"({instructor.Id}) \"{instructor.FirstName} {instructor.LastName}\" added.");

            await ImportInstructors();
        }

        public static Instructor GetInstructor(int id) => database.FindAsync<Instructor>(id).Result;

        public static async Task UpdateInstructor(Instructor instructor)
        {
            await database.UpdateAsync(instructor);
            await ImportInstructors();
        }

        public static async Task RemoveInstructor(Instructor instructor)
        {
            await Init();

            Instructors.Remove(instructor);

            CourseService.RemoveInstructorFromCourse(instructor);

            await database.DeleteAsync<Instructor>(instructor.Id);
            Debug.WriteLine($"({instructor.Id}) \"{instructor.FirstName} {instructor.LastName}\" removed.");
        }

        public static async Task ClearTable()
        {
            await Init();

            foreach (Instructor instructor in Instructors)
            {
                CourseService.RemoveInstructorFromCourse(instructor);
            }

            Instructors.Clear();
            await database.DeleteAllAsync<Instructor>();
            Debug.WriteLine($"The Instructors table has been cleared.");
        }

        public static async Task DropTable()
        {
            await Init();

            await database.DropTableAsync<Instructor>().ContinueWith((results) => Debug.WriteLine($"Table dropped."));
        }
    }
}
