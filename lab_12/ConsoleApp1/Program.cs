// Lab ID: 29
// Name: Osama Aboud
// Lab 11 — Abstraction + Interfaces + OOP Capstone

using System;
using System.Collections.Generic;

namespace StudentPortalConsole
{
    // Part D: Interface Definition
    public interface IPrintable
    {
        void PrintDetails();
    }

    // Part E4: Second Interface Definition
    public interface IRankable
    {
        int GetRankScore();
    }

    // Part C: Abstract Person Base Class
    public abstract class Person
    {
        protected string fullName = string.Empty;

        public string FullName
        {
            get { return fullName; }
            set { fullName = value ?? string.Empty; }
        }

        public Person(string fullName)
        {
            FullName = fullName;
        }

        // Virtual method carried forward from Session 10
        public virtual void PrintBasicInfo()
        {
            Console.WriteLine($"Name: {fullName}");
        }

        // Abstract method - must be overridden in derived classes
        public abstract string GetRoleDescription();

        // Protected Helper Method from Lab 10
        // Lab ID: 29 -> (29 mod 3) + 2 = 4 letters
        protected string FormatTag()
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "TAGG";
            string clean = fullName.Replace(" ", "");
            if (clean.Length >= 4) return clean.Substring(0, 4).ToUpper();
            return clean.PadRight(4, 'X').ToUpper();
        }
    }

    // Student Class
    public class Student : Person, IPrintable
    {
        private int yearOfStudy;
        private double gpa;

        public int YearOfStudy
        {
            get { return yearOfStudy; }
            set { if (value >= 1 && value <= 4) yearOfStudy = value; }
        }

        public double Gpa
        {
            get { return gpa; }
            set { if (value >= 0.0 && value <= 4.0) gpa = value; }
        }

        public Student(string fullName, int yearOfStudy, double gpa) : base(fullName)
        {
            YearOfStudy = yearOfStudy;
            Gpa = gpa;
        }

        public override string GetRoleDescription()
        {
            return "Student";
        }

        public override void PrintBasicInfo()
        {
            base.PrintBasicInfo();
            Console.WriteLine($"Role: {GetRoleDescription()} | Year: {yearOfStudy} | GPA: {gpa:F2}");
        }

        public void PrintDetails()
        {
            string tag = FormatTag();
            Console.WriteLine($"[{tag}] Student: {fullName} | Year: {yearOfStudy} | GPA: {gpa:F2}");
        }
    }

    // Instructor Class
    public class Instructor : Person, IPrintable
    {
        private int yearsOfExperience;

        public int YearsOfExperience
        {
            get { return yearsOfExperience; }
            set { if (value >= 0) yearsOfExperience = value; }
        }

        public string AssignedCourseName { get; set; } = string.Empty;

        public Instructor(string fullName, int yearsOfExperience) : base(fullName)
        {
            YearsOfExperience = yearsOfExperience;
        }

        public override string GetRoleDescription()
        {
            return "Instructor";
        }

        public override void PrintBasicInfo()
        {
            base.PrintBasicInfo();
            Console.WriteLine($"Role: {GetRoleDescription()} | Experience: {yearsOfExperience} yrs");
        }

        public void PrintDetails()
        {
            string tag = FormatTag();
            string courseInfo = string.IsNullOrWhiteSpace(AssignedCourseName) ? "No course assigned" : $"Course: {AssignedCourseName}";
            Console.WriteLine($"[{tag}] Instructor: {fullName} | Exp: {yearsOfExperience} yrs | {courseInfo}");
        }
    }

    // Course Class (Does NOT derive from Person)
    public class Course : IPrintable
    {
        public string CourseName { get; set; }
        private List<Student> enrolledStudents = new List<Student>();

        public Course(string courseName)
        {
            CourseName = courseName;
        }

        public void EnrollStudent(Student s)
        {
            if (s != null) enrolledStudents.Add(s);
        }

        public void PrintDetails()
        {
            Console.WriteLine($"Course Name: {CourseName} | Enrolled Count: {enrolledStudents.Count}");
            foreach (Student s in enrolledStudents)
            {
                Console.WriteLine($"   - Student: {s.FullName}");
            }
        }
    }

    // Part E: Admin Class implementing Person, IPrintable, and IRankable
    public class Admin : Person, IPrintable, IRankable
    {
        // Lab ID: 29 -> Range: 1 to (29 mod 3) + 2 -> 1 to 4
        private int accessLevel = 1;

        public int AccessLevel
        {
            get { return accessLevel; }
            set
            {
                if (value >= 1 && value <= 4)
                {
                    accessLevel = value;
                }
            }
        }

        public Admin(string fullName, int accessLevel) : base(fullName)
        {
            AccessLevel = accessLevel;
        }

        public override string GetRoleDescription()
        {
            return "Admin";
        }

        public override void PrintBasicInfo()
        {
            base.PrintBasicInfo();
            Console.WriteLine($"Role: {GetRoleDescription()} | Access Level: {accessLevel}");
        }

        public void PrintDetails()
        {
            string tag = FormatTag();
            Console.WriteLine($"[{tag}] Admin: {fullName} | Access Level: {accessLevel}");
        }

        // Part E4: Implementation of IRankable
        // Lab ID 29: (29 mod 4) + 1 = 2
        public int GetRankScore()
        {
            return 2;
        }
    }

    internal class Program
    {
        private static List<Person> people = new List<Person>();
        private static List<IPrintable> printables = new List<IPrintable>();
        private static List<IRankable> rankables = new List<IRankable>();

        static void Main(string[] args)
        {
            // Seed data using student's own name for Admin, Access level up to 4 now
            Admin adminMe = new Admin("Osama Aboud", 4);
            Student s1 = new Student("Omar Tarek", 2, 3.5);
            Instructor i1 = new Instructor("Dr. Khaled", 10);
            Course c1 = new Course("Web Development .NET");
            c1.EnrollStudent(s1);

            // Add objects to Person list
            people.Add(adminMe);
            people.Add(s1);
            people.Add(i1);

            // Add objects to Printable list
            printables.Add(adminMe);
            printables.Add(s1);
            printables.Add(i1);
            printables.Add(c1);

            // Add objects to Rankable list
            rankables.Add(adminMe);

            RunMenu();
        }

        private static void RunMenu()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n--- Student Portal Capstone Menu ---");
                Console.WriteLine("1. Print Everyone (List<Person>)");
                Console.WriteLine("2. Print Everything Printable (List<IPrintable>)");
                Console.WriteLine("3. Show Rank Scores (List<IRankable>)");
                Console.WriteLine("4. Exit");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        PrintEveryone();
                        break;
                    case "2":
                        PrintEverythingPrintable();
                        break;
                    case "3":
                        ShowRankScores();
                        break;
                    case "4":
                        running = false;
                        Console.WriteLine("Exiting...");
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        // Part F1: Print everyone using List<Person> with zero casts
        private static void PrintEveryone()
        {
            Console.WriteLine("=== All People ===");
            foreach (Person p in people)
            {
                Console.WriteLine($"Role: {p.GetRoleDescription()}");
                p.PrintBasicInfo();
                Console.WriteLine("----------------");
            }
        }

        // Part F2: Print everything printable using List<IPrintable>
        private static void PrintEverythingPrintable()
        {
            Console.WriteLine("=== All Printable Items ===");
            foreach (IPrintable item in printables)
            {
                item.PrintDetails();
            }
        }

        // Part F3: Show rank scores using List<IRankable>
        private static void ShowRankScores()
        {
            Console.WriteLine("=== Rank Scores ===");
            foreach (IRankable item in rankables)
            {
                Console.WriteLine($"Rank Score: {item.GetRankScore()}");
            }
        }
    }
}

/*
Part B — Predict-the-Output Drills

B1. What happens?
Does NOT compile.
Reason: Cannot create an instance of the abstract class 'Shape'.

B2. What happens?
Does NOT compile.
Reason: Class 'Square' does not implement abstract member 'Shape.GetArea()'.

B3. What happens?
Compiles and prints:
Meow
*/

/*
Part G — Wrap-Up Reflection

1. Personal Lab ID: 29
   - Admin AccessLevel range arithmetic: (29 mod 3) + 2 = 2 + 2 = 4. Range is 1 to 4.
   - Admin GetRankScore result arithmetic: (29 mod 4) + 1 = 1 + 1 = 2. Result is 2.

2. Course implementing IPrintable is valid because an interface defines a capability ("what an object can do") rather than an identity ("what an object is"). Course is not a Person, but it has printable details, so implementing IPrintable allows it to be treated as printable alongside Persons without inheriting from Person.

3. If I tried to give Admin its GetRankScore capability by inheriting from a second base class instead of implementing an interface, C# would give a compiler error because C# does not support multiple class inheritance. A class in C# can only inherit from one base class (CS1721 error), so using interfaces is the only way to give a class multiple separate sets of behaviors.
*/
