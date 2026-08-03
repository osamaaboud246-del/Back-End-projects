// Lab ID: 7
// Name: Osama Aboud
// Lab 12 

using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentPortalConsole
{
    public interface IPrintable
    {
        void PrintDetails();
    }

    public abstract class Person
    {
        public string FullName { get; set; } = string.Empty;

        public Person(string fullName)
        {
            FullName = fullName;
        }

        public virtual void PrintBasicInfo()
        {
            Console.WriteLine($"Name: {FullName}");
        }

        public abstract string GetRoleDescription();
    }

    public class Student : Person, IPrintable
    {
        public int YearOfStudy { get; set; }
        public double Gpa { get; set; }

        public Student(string fullName, int yearOfStudy, double gpa) : base(fullName)
        {
            YearOfStudy = yearOfStudy;
            Gpa = gpa;
        }

        public override string GetRoleDescription() => "Student";

        public override void PrintBasicInfo()
        {
            Console.WriteLine($"Student: {FullName} | Year: {YearOfStudy} | GPA: {Gpa:F2}");
        }

        public void PrintDetails()
        {
            PrintBasicInfo();
        }
    }

    public class Instructor : Person, IPrintable
    {
        public int YearsOfExperience { get; set; }

        public Instructor(string fullName, int yearsOfExperience) : base(fullName)
        {
            YearsOfExperience = yearsOfExperience;
        }

        public override string GetRoleDescription() => "Instructor";

        public override void PrintBasicInfo()
        {
            Console.WriteLine($"Instructor: {FullName} | Experience: {YearsOfExperience} yrs");
        }

        public void PrintDetails()
        {
            PrintBasicInfo();
        }
    }

    public class Course : IPrintable
    {
        public string CourseName { get; set; }
        public int Credits { get; set; }

        public Course(string courseName, int credits)
        {
            CourseName = courseName;
            Credits = credits;
        }

        public void PrintDetails()
        {
            Console.WriteLine($"Course: {CourseName} ({Credits} credits)");
        }
    }

    // Part D3: Generic Class Tracker
    // Lab ID 7 -> Capacity: (7 mod 3) + 2 = 3
    public class Tracker<T>
    {
        private List<T> items = new List<T>();
        private int maxCapacity = 3;

        public void Add(T item)
        {
            if (items.Count >= maxCapacity)
            {
                Console.WriteLine($"Capacity limit ({maxCapacity}) reached!");
                return;
            }
            items.Add(item);
            Console.WriteLine($"Item added. Total items: {items.Count}");
        }

        public List<T> GetAll()
        {
            return items;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Seed Data from Part A
            List<Student> students = new List<Student>
            {
                new Student("Yara Adel", 2, 3.5),
                new Student("Omar Hesham", 3, 2.8),
                new Student("Nada Samir", 1, 3.9),
                new Student("Kareem Fouad", 4, 3.2)
            };

            List<Instructor> instructors = new List<Instructor>
            {
                new Instructor("Hamdy", 10),
                new Instructor("Mona Khalil", 6)
            };

            List<Course> courses = new List<Course>
            {
                new Course("Web Development Using .NET", 4),
                new Course("Database Fundamentals", 3)
            };

            Console.WriteLine("--- Part C ---");

            // Part C Threshold: 2.0 + ((7 mod 5) * 0.4) = 2.8
            List<Student> filteredByNamed = FilterStudents(students, IsAboveMyThreshold);
            Console.WriteLine($"Named method count: {filteredByNamed.Count}");

            List<Student> filteredByLambda = FilterStudents(students, s => s.Gpa > 2.8);
            Console.WriteLine($"Lambda count: {filteredByLambda.Count}");

            Console.WriteLine("\nApplyToAll result:");
            ApplyToAll(students, s => Console.WriteLine($"Name: {s.FullName}, GPA: {s.Gpa}"));

            Console.WriteLine("\n--- Part D ---");

            Student foundStudent = FindFirst(students, s => s.Gpa > 3.5);
            Console.WriteLine($"FindFirst Student: {foundStudent?.FullName ?? "None"}");

            Instructor foundInstructor = FindFirst(instructors, i => i.YearsOfExperience > 8);
            Console.WriteLine($"FindFirst Instructor: {foundInstructor?.FullName ?? "None"}");

            Course foundCourse = FindFirst(courses, c => c.Credits == 4);
            Console.WriteLine($"FindFirst Course: {foundCourse?.CourseName ?? "None"}");

            Console.WriteLine("\nTracker capacity test:");
            Tracker<Student> studentTracker = new Tracker<Student>();
            studentTracker.Add(students[0]);
            studentTracker.Add(students[1]);
            studentTracker.Add(students[2]);
            studentTracker.Add(students[3]); // refused

            Tracker<Course> courseTracker = new Tracker<Course>();
            courseTracker.Add(courses[0]);

            // studentTracker.Add(courses[0]); 
            // Error CS1503: cannot convert from Course to Student

            Console.WriteLine("\nPrintAllNames output:");
            PrintAllNames(students);

            Console.WriteLine("\n--- Part E ---");

            // Part E Year Filter: ((7 mod 4) + 1) = 4
            // Q1: Method syntax
            List<string> q1 = students.Where(s => s.YearOfStudy == 4)
                                      .Select(s => s.FullName)
                                      .ToList();
            Console.WriteLine("Q1: " + string.Join(", ", q1));

            // Q2: Method syntax
            Console.WriteLine("\nQ2:");
            var q2 = students.OrderByDescending(s => s.Gpa);
            foreach (var s in q2)
            {
                Console.WriteLine($"{s.FullName} - {s.Gpa:F2}");
            }

            // Q3: Method syntax
            List<string> q3 = students.Where(s => s.Gpa > 2.8)
                                      .OrderBy(s => s.FullName)
                                      .Select(s => s.FullName)
                                      .ToList();
            Console.WriteLine("\nQ3:");
            foreach (string name in q3)
            {
                Console.WriteLine(name);
            }

            // Q4: Query syntax
            var q4 = from s in students
                     where s.Gpa > 3.0
                     orderby s.FullName
                     select s.FullName;

            Console.WriteLine("\nQ4:");
            foreach (var name in q4)
            {
                Console.WriteLine(name);
            }

            // Q5: Query syntax
            var q5 = from s in students
                     where s.YearOfStudy <= 2
                     select $"{s.FullName} - Year {s.YearOfStudy}";

            Console.WriteLine("\nQ5:");
            foreach (var info in q5)
            {
                Console.WriteLine(info);
            }
        }

        // Helper Methods

        public static List<Student> FilterStudents(List<Student> list, Func<Student, bool> predicate)
        {
            List<Student> result = new List<Student>();
            foreach (var s in list)
            {
                if (predicate(s))
                {
                    result.Add(s);
                }
            }
            return result;
        }

        // Threshold = 2.8
        public static bool IsAboveMyThreshold(Student s)
        {
            return s.Gpa > 2.8;
        }

        public static void ApplyToAll(List<Student> list, Action<Student> action)
        {
            foreach (var s in list)
            {
                action(s);
            }
        }

        /*
        Part C5:
        Func requires a return value type. Step 4 performs a void action (printing to console), 
        so C# requires Action<Student> instead of Func.
        */

        // Constraint 'where T : class' is needed to allow returning null when no match is found.
        public static T FindFirst<T>(List<T> items, Func<T, bool> predicate) where T : class
        {
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return null;
        }

        // Original Error Code: CS1061
        // The compiler checks generics independently and doesn't know T has FullName without a constraint.
        public static void PrintAllNames<T>(List<T> items) where T : Person
        {
            foreach (T item in items)
            {
                Console.WriteLine(item.FullName);
            }
        }
    }
}

/*
Part B - Predict-the-Output Drills

B1:
Does NOT compile.
Reason: Func must return a value, it cannot have void as return type. Action<Student> should be used instead.

B2:
Does NOT compile.
Reason: IsTopStudent() calls the method instead of passing it as a reference. Need to remove parentheses.

B3:
Does NOT compile.
Reason: Error CS0029. Select returns IEnumerable<bool>, so ToList() makes List<bool> which cannot be assigned to List<Student>.
*/

/*
Part E Question 6:
If Select is placed before OrderBy in Q3, the list elements become strings after Select. 
Then OrderBy(s => s.FullName) will cause compiler error CS1061 because string does not have a FullName property.

Part F - Wrap-Up Reflection

1. Lab ID: 7
   - Part C threshold: 2.0 + ((7 mod 5) * 0.4) = 2.0 + (2 * 0.4) = 2.8
   - Part D capacity: (7 mod 3) + 2 = 1 + 2 = 3
   - Part E year filter: ((7 mod 4) + 1) = 3 + 1 = 4

2. Delegates allow passing methods/behavior dynamically as arguments without needing to create new classes or inherit interfaces. In Part C, Func<Student, bool> let us pass any condition directly without writing extra classes.

3. Generics are compiled independently of call sites. Without the constraint 'where T : Person', the compiler cannot assume T has a FullName property because T could be any type.

4. Students in my Q3 result:
   - Kareem Fouad
   - Nada Samir
   - Yara Adel
   Omar Hesham is excluded because his GPA (2.8) is not strictly greater than 2.8.
   Yes, a different Lab ID would give a different threshold and different results.
*/