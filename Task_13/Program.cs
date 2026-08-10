using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace StudentPortalConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("LINQ Warm-Up:");

            List<Student> students = new List<Student>
            {
                new Student { Id = 1, FullName = "Yara Adel", YearOfStudy = 2, Gpa = 3.5 },
                new Student { Id = 2, FullName = "Omar Hesham", YearOfStudy = 3, Gpa = 2.8 },
                new Student { Id = 3, FullName = "Nada Samir", YearOfStudy = 1, Gpa = 3.9 },
                new Student { Id = 4, FullName = "Kareem Fouad", YearOfStudy = 4, Gpa = 3.2 }
            };

            List<Instructor> instructors = new List<Instructor>
            {
                new Instructor { Id = 1, Name = "Hamdy", YearsOfExperience = 10, AssignedCourseName = "Web Development Using .NET" },
                new Instructor { Id = 2, Name = "Mona Khalil", YearsOfExperience = 6, AssignedCourseName = "Database Fundamentals" }
            };

            List<Course> courses = new List<Course>
            {
                new Course { Id = 1, Name = "Web Development Using .NET", Credits = 4 },
                new Course { Id = 2, Name = "Database Fundamentals", Credits = 3 }
            };

            var warmUp = students.Where(s => s.Gpa > 3.0)
                                 .OrderByDescending(s => s.Gpa)
                                 .Select(s => s.FullName);

            foreach (var name in warmUp)
            {
                Console.WriteLine($"  {name}");
            }

            // Part B - Predictions
            Console.WriteLine("\nPart B:");

            // Part C - Aggregates
            Console.WriteLine("\nPart C:");
            Console.WriteLine($"Total Count: {students.Count()}");
            Console.WriteLine($"Count above 3.4: {students.Count(s => s.Gpa > 3.4)}");
            Console.WriteLine($"Average GPA: {students.Average(s => s.Gpa):F2}");
            Console.WriteLine($"Highest GPA: {students.Max(s => s.Gpa)}");
            Console.WriteLine($"Lowest GPA: {students.Min(s => s.Gpa)}");
            Console.WriteLine($"Any below 2.0: {students.Any(s => s.Gpa < 2.0)}");
            Console.WriteLine($"All above 2.0: {students.All(s => s.Gpa >= 2.0)}");

            List<Student> emptyList = new List<Student>();
            try
            {
                var crash = emptyList.Average(s => s.Gpa);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.GetType().Name} - {ex.Message}");
            }

            if (emptyList.Any())
            {
                Console.WriteLine($"Average: {emptyList.Average(s => s.Gpa)}");
            }

            var groupedByYear = students.GroupBy(s => s.YearOfStudy);
            foreach (var group in groupedByYear)
            {
                Console.WriteLine($"Year: {group.Key}, Count: {group.Count()}");
            }

            var groupedByStatus = students.GroupBy(s => s.Gpa >= 3.4 ? "High Achiever" : "Standard Student");
            foreach (var group in groupedByStatus)
            {
                Console.WriteLine($"Bucket [{group.Key}]: {group.Count()}");
            }

            var sortedGrouped = students.GroupBy(s => s.YearOfStudy).OrderBy(g => g.Key);

            // Part D - Join
            Console.WriteLine("\nPart D:");
            instructors.Add(new Instructor
            {
                Id = 3,
                Name = "Osama Aboud",
                YearsOfExperience = 5,
                AssignedCourseName = "Machine Learning"
            });

            var joinResult = instructors.Join(
                courses,
                inst => inst.AssignedCourseName,
                crs => crs.Name,
                (inst, crs) => new { Instructor = inst.Name, Course = crs.Name }
            ).ToList();

            Console.WriteLine($"Instructors In: {instructors.Count}, Rows Out: {joinResult.Count}");

            // Part E - Deferred Execution
            Console.WriteLine("\nPart E:");
            var query = students.Where(s => s.Gpa > 3.0);
            var layla = new Student { Id = 5, FullName = "Layla Mostafa", YearOfStudy = 2, Gpa = 3.7 };
            students.Add(layla);
            Console.WriteLine($"Count: {query.Count()}");
            students.Remove(layla);

            var topStudents = students.MyTopStudents().OrderBy(s => s.FullName).Select(s => s.FullName).ToList();
            foreach (var name in topStudents)
            {
                Console.WriteLine($"Top Student: {name}");
            }

            // Part H - Database Queries
            Console.WriteLine("\nPart H:");
            using (var db = new StudentPortalContext())
            {
                if (!db.Students.Any())
                {
                    db.Students.AddRange(
                        new Student { FullName = "Yara Adel", YearOfStudy = 2, Gpa = 3.5, CreditsCompleted = 60 },
                        new Student { FullName = "Omar Hesham", YearOfStudy = 3, Gpa = 2.8, CreditsCompleted = 90 },
                        new Student { FullName = "Nada Samir", YearOfStudy = 1, Gpa = 3.9, CreditsCompleted = 30 },
                        new Student { FullName = "Kareem Fouad", YearOfStudy = 4, Gpa = 3.2, CreditsCompleted = 120 }
                    );
                    db.SaveChanges();
                    Console.WriteLine("data entered");
                }

                var dbResults = db.Students.Where(s => s.Gpa > 3.0).ToList();
                foreach (var st in dbResults)
                {
                    Console.WriteLine($"DB Student: {st.FullName}");
                }
            }
        }
    }
}

/*

Part F:
- Up method creates: Students, Instructors, Courses.
- Gpa type is float, FullName is nvarchar max.
- Down method drops created tables.
- Database does not exist before Update-Database command.

Part G:
- Added property CreditsCompleted for Lab ID 29.
- Up operation adds column to existing table.
- 4 rows preserved in SSMS.

Part I Reflection:
1. Lab ID 29 parameters: GPA threshold 3.4, experience 5, property CreditsCompleted.
2. Silent join failure drops missing records without alert.
3. Separation of Add-Migration and Update-Database allows review before changing database.
4. Database LINQ translates code to SQL. Deferred execution avoids extra network requests.
*/
