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

            var tableInfo = database.GetTableInfoAsync("Terms");
            if(tableInfo.Result.Count == 0)
            {
                Debug.WriteLine($"Terms table doesn't exist - Creating new table.");
                return;
            }

            var query = database.Table<Term>().ToListAsync();
            Debug.WriteLine($"Terms being imported from DB: {query.Result.Count}.");
            foreach (var result in query.Result)
            {
                Debug.WriteLine($"ID: {result.Id}, Name: {result.TermName}");

                var existingTermGroup = TermGroups.FirstOrDefault(x => x.Name == result.TermName);
                if (existingTermGroup == null)
                {
                    var courses = new ObservableCollection<Course>();
                    foreach (Course course in CourseService.Courses)
                    {
                        if (course.AssociatedTermId == result.Id)
                        {
                            courses.Add(course);
                            Debug.WriteLine($"Added \"{course.CourseName}\" to term group's course collection.");
                        }
                    }

                    var newTermGroup = new TermGroup(result.Id, result.TermName, result.StartDate, result.EndDate, courses);

                    TermGroups.Add(newTermGroup);
                    Debug.WriteLine($"Added \"{newTermGroup.Name}\" to collection.");
                }
            }
        }


        //public static void GetTables()
        //{
        //    var table = database.Table<Term>();
        //    if(table == null)
        //    {
        //        Debug.WriteLine($"The Terms table does not exist.");
        //        return;
        //    }

        //    var query = table.Where(x => x.Id >= 0);
        //    if(query.CountAsync().Result == 0)
        //    {
        //        Debug.WriteLine($"There are no terms in the table.");
        //    }

        //    query.ToListAsync().ContinueWith((t) =>
        //    {
        //        foreach (var term in t.Result)
        //            Debug.WriteLine($"Term: {term.TermName}");
        //    });
        //}

        public static async Task AddTerm(string termName, DateTime startDate, DateTime endDate)
        {
            await Init();

            var query = await database.Table<Term>().FirstOrDefaultAsync(x => x.TermName == termName);
            if(query != null)
            {
                Debug.WriteLine($"\"{termName}\" already exists.");
                return;
            }

            var term = new Term
            {
                TermName = termName,
                StartDate = startDate,
                EndDate = endDate,
            };

            await database.InsertAsync(term);
            Debug.WriteLine($"({term.Id}) \"{termName}\" added.");

            await ImportTerms();
        }

        public static Term GetTerm(int id)
        {
            var term = database.FindAsync<Term>(id).Result;
            return term;
        }

        public static async Task UpdateTerm(Term term)
        {
            var matchingGroup = TermGroups.FirstOrDefault(x => x.Id == term.Id);
            if(matchingGroup == null)
            {
                Debug.WriteLine($"Matching group not found.");
                return;
            }
            TermGroups.Remove(matchingGroup);

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

        public static async Task RemoveCourseFromTerm(Course course)
        {
            var matchingGroup = TermGroups.FirstOrDefault(x => x.Id == course.AssociatedTermId);
            if (matchingGroup == null)
            {
                Debug.WriteLine($"Matching group not found.");
                return;
            }

            matchingGroup.Remove(course);
            Debug.WriteLine($"Course \"{course.CourseName}\" removed from term \"{matchingGroup.Name}\".");
            await ImportTerms();
        }

        //public static async Task<IEnumerable<Term>> GetTerms()
        //{
        //    await Init();

        //    var term = await database.Table<Term>().ToListAsync();
        //    return term;
        //}

        //public static async Task DropTable()
        //{
        //    await Init();

        //    await database.DropTableAsync<Term>().ContinueWith((results) => Debug.WriteLine($"Table deleted."));
        //}

        public static async Task ClearTable()
        {
            var table = database.Table<Term>();
            if (table == null)
            {
                Debug.WriteLine($"The Terms table does not exist.");
                return;
            }

            await database.DeleteAllAsync<Term>();
            Debug.WriteLine($"The Terms table has been cleared.");

            TermGroups.Clear();

            await ImportTerms();
        }

        //public static void GetTableRows()
        //{
        //    var query = database.QueryAsync<Term>("SELECT * from Term");

        //    if(query.Result.Count == 0)
        //    {
        //        Debug.WriteLine($"There are no terms.");
        //    }

        //    foreach(var t in query.Result)
        //    {
        //        Debug.WriteLine($"Result: {t.TermName}");
        //    }
        //}
    }
}
