using CourseManager.Models;
using Plugin.LocalNotifications;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CourseManager.Services
{
    public static class AssessmentService
    {
        public static SQLiteAsyncConnection database;
        public static ObservableCollection<Assessment> Assessments { get; } = new ObservableCollection<Assessment>();
        private static bool _notificationsDisplayed = false;

        public static async Task Init()
        {
            if (database == null)
            {
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WGUAssessments.db");
                database = new SQLiteAsyncConnection(databasePath);
            }

            await database.CreateTableAsync<Assessment>();
        }

        public static async Task ImportAssessments()
        {
            await Init();

            Assessments.Clear();

            var query = await database.Table<Assessment>().ToListAsync();
            Debug.WriteLine($"Assessments being imported from DB: {query.Count}.");
            foreach (var result in query)
            {
                Debug.WriteLine($"ID: {result.Id}, Name: {result.Name}");
                Assessments.Add(result);
                if (!_notificationsDisplayed)
                {
                    if (result.EnableNotifications && CourseService.Courses.Any(x => x.FirstAssessmentId == result.Id || x.SecondAssessmentId == result.Id) && (result.DueDate - DateTime.Now).TotalDays < 30)
                    {
                        Debug.WriteLine($"Assessment due soon");
                        CrossLocalNotifications.Current.Show("Upcoming assessment", $"{result.Name} is on {result.DueDate.ToShortDateString()}", result.Id, DateTime.Now.AddSeconds(5));
                    }
                }
            }

            _notificationsDisplayed = true;
        }

        public static async Task AddAssessment(Assessment assessment)
        {
            await Init();

            await database.InsertAsync(assessment);
            Debug.WriteLine($"({assessment.Id}) \"{assessment.Name}\" added.");

            await ImportAssessments();
        }

        public static Assessment GetAssessment(int id) => database.FindAsync<Assessment>(id).Result;

        public static async Task UpdateAssessment(Assessment assessment)
        {
            await database.UpdateAsync(assessment);
            await ImportAssessments();
        }

        // This method is never used, but may be used in later development
        //public static async Task RemoveAssessment(Assessment assessment)
        //{
        //    await Init();

        //    Assessments.Remove(assessment);
        //    await database.DeleteAsync<Assessment>(assessment.Id);
        //    Debug.WriteLine($"({assessment.Id}) \"{assessment.Name}\" removed.");
        //}

        public static async Task ClearTable()
        {
            await Init();

            foreach (Assessment assessment in Assessments)
            {
                CourseService.RemoveAssessmentFromCourse(assessment);
            }

            Assessments.Clear();
            await database.DeleteAllAsync<Assessment>();
            Debug.WriteLine($"The Assessments table has been cleared.");
        }

        public static async Task DropTable()
        {
            await Init();

            await database.DropTableAsync<Assessment>().ContinueWith((results) => Debug.WriteLine($"Table dropped."));
        }
    }
}
