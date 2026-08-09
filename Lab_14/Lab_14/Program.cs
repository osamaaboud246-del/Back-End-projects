using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StudentPortalConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var context = new StudentPortalContext();

            

            // B1: Filtering (GPA >= 3.0)
            Console.WriteLine("=== B1: Filtering (GPA >= 3.0) ===");
            var highGpaStudents = await context.Students
                .Where(s => s.Gpa >= 3.0)
                .ToListAsync();

            foreach (var student in highGpaStudents)
            {
                Console.WriteLine($"Student: {student.FullName}, GPA: {student.Gpa}");
            }

            // B2: Projection (Select Specific Fields)
            Console.WriteLine("\n=== B2: Projection ===");
            var studentSummaries = await context.Students
                .Select(s => new
                {
                    s.FullName,
                    s.YearOfStudy
                })
                .ToListAsync();

            foreach (var summary in studentSummaries)
            {
                Console.WriteLine($"Name: {summary.FullName}, Year: {summary.YearOfStudy}");
            }

            // B3: Aggregation & Ordering (Average GPA & Top Student)
            Console.WriteLine("\n=== B3: Aggregation & Ordering ===");
            var averageGpa = await context.Students.AverageAsync(s => (double?)s.Gpa) ?? 0.0;
            Console.WriteLine($"Average GPA of all students: {averageGpa:F2}");

            var topStudent = await context.Students
                .OrderByDescending(s => s.Gpa)
                .FirstOrDefaultAsync();

            if (topStudent != null)
            {
                Console.WriteLine($"Top Student: {topStudent.FullName} with GPA: {topStudent.Gpa}");
            }

            
            Console.WriteLine("\n=== Processing Parts C to G ===");
            context.ChangeTracker.Clear();

            Console.WriteLine("\nLab 14 executed successfully!");
        }
    }
}