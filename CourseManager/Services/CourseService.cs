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
    public static class CourseService
    {
        public static SQLiteAsyncConnection database;
        public static ObservableCollection<Course> Courses { get; } = new ObservableCollection<Course>();
        private static bool _notificationsDisplayed = false;

        public static async Task Init()
        {
            if (database == null)
            {
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WGUCourses.db");
                database = new SQLiteAsyncConnection(databasePath);
            }

            await database.CreateTableAsync<Course>();
        }

        public static async Task ImportCourses()
        {
            await Init();

            Courses.Clear();

            var query = await database.Table<Course>().ToListAsync();
            Debug.WriteLine($"Courses being imported from DB: {query.Count}.");
            foreach (var result in query)
            {
                Debug.WriteLine($"ID: {result.Id}, Name: {result.CourseName}, Associated term: {result.AssociatedTermId}, Associated Instructor: {result.AssociatedInstructorId}");
                Courses.Add(result);

                if(!_notificationsDisplayed)
                {
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

                    foreach(Assessment assessment in AssessmentService.Assessments.Where(x => x.EnableNotifications && (result.FirstAssessmentId == x.Id || result.SecondAssessmentId == x.Id) && (x.DueDate - DateTime.Now).TotalDays < 30))
                    {
                        Debug.WriteLine($"Assessment due soon");
                        CrossLocalNotifications.Current.Show("Upcoming assessment", $"{assessment.Name} is due on {assessment.DueDate.ToShortDateString()}", assessment.Id, DateTime.Now.AddSeconds(5));
                    }
                }
            }

            _notificationsDisplayed = true;
        }

        public static async Task AddCourse(Course course)
        {
            await Init();

            await database.InsertAsync(course);
            Debug.WriteLine($"({course.Id}) \"{course.CourseName}\" added.");

            await ImportCourses();
        }

        public static Course GetCourse(int id) => database.FindAsync<Course>(id).Result;

        public static async Task UpdateCourse(Course course)
        {
            await database.UpdateAsync(course);
            Debug.WriteLine($"({course.Id}) \"{course.CourseName}\" updated.");

            await ImportCourses();
        }

        public static async Task RemoveCourse(Course course)
        {
            await Init();

            Courses.Remove(course);
            TermService.RemoveCourseFromTerm(course);
            await database.DeleteAsync<Course>(course.Id);
            Debug.WriteLine($"({course.Id}) \"{course.CourseName}\" removed.");
        }

        public static async Task DropTable()
        {
            await Init();

            await database.DropTableAsync<Course>().ContinueWith((results) => Debug.WriteLine($"Table dropped."));
        }

        public static async Task ClearTable()
        {
            await Init();

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
    }
}
