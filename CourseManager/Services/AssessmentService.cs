using CourseManager.Enums;
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
    public static class AssessmentService
    {
        public static SQLiteAsyncConnection database;
        public static ObservableCollection<Assessment> Assessments { get; } = new ObservableCollection<Assessment>();

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

            var tableInfo = await database.GetTableInfoAsync("Assessments");
            if(tableInfo.Count == 0)
            {
                Debug.WriteLine($"Assessments table doesn't exist - Creating new table.");
                return;
            }

            Assessments.Clear();

            var query = await database.Table<Assessment>().ToListAsync();
            Debug.WriteLine($"Assessments being imported from DB: {query.Count}.");
            foreach (var result in query)
            {
                Debug.WriteLine($"ID: {result.Id}, Name: {result.Name}");
                Assessments.Add(result);
                if (result.EnableNotifications && (result.DueDate - DateTime.Now).TotalDays < 30)
                {
                    Debug.WriteLine($"Assessment due soon");
                    CrossLocalNotifications.Current.Show("Upcoming assessment", $"{result.Name} is on {result.DueDate.ToShortDateString()}", new Random().Next(0, 100), DateTime.Now.AddSeconds(5));
                }
            }
        }

        public static async Task AddAssessment(Assessment assessment)
        {
            await Init();

            var query = await database.Table<Assessment>().FirstOrDefaultAsync(x => x.Id == assessment.Id);
            if (query != null)
            {
                Debug.WriteLine($"Assessment with ID \"{assessment.Id}\" already exists.");
                return;
            }

            var insertAssessment = database.InsertAsync(assessment).Result;
            Debug.WriteLine($"({assessment.Id}) \"{assessment.Name}\" added.");

            await ImportAssessments();
        }

        public static Assessment GetAssessment(int id)
        {
            var course = database.FindAsync<Assessment>(id).Result;
            return course;
        }

        public static async Task UpdateAssessment(Assessment assessment)
        {
            var matchingAssessment = Assessments.FirstOrDefault(x => x.Id == assessment.Id);
            if (matchingAssessment == null)
            {
                Debug.WriteLine($"Matching assessment not found.");
                return;
            }

            //Assessments.Remove(matchingAssessment);
            await database.UpdateAsync(assessment);
            await ImportAssessments();
        }

        public static async Task RemoveAssessment(Assessment assessment)
        {
            await Init();

            var matchingCourse = Assessments.FirstOrDefault(x => x.Id == assessment.Id);
            if (matchingCourse != null)
            {
                Assessments.Remove(matchingCourse);
            }

            await database.DeleteAsync<Assessment>(assessment.Id);
            Debug.WriteLine($"({assessment.Id}) \"{assessment.Name}\" removed.");
        }

        public static async Task ClearTable()
        {
            var table = database.Table<Assessment>();
            if (table == null)
            {
                Debug.WriteLine($"The Assessments table does not exist.");
                return;
            }

            foreach (Assessment assessment in Assessments)
            {
                CourseService.RemoveAssessmentFromCourse(assessment);
            }

            Assessments.Clear();
            await database.DeleteAllAsync<Assessment>();
            Debug.WriteLine($"The Assessments table has been cleared.");
        }
    }
}
