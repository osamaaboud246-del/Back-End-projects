// Lab ID: 29
// Name: Osama Aboud

// Part B: Predict-the-Output Drills
// 
// B1:
// Vehicle constructor: Toyota
// Car constructor: 4 doors
//
// B2:
// Does Bird.CompareToDog's line compile? No.
// Why: legCount is protected. A derived class (Bird) can access inherited protected 
// members on its own instances, but cannot access them on instances of a sibling 
// derived class (Dog) because they are different branches of the hierarchy.
//
// B3:
// Does the last line compile? Yes.
// What prints: 12.56

using System;
using System.Collections.Generic;

namespace StudentPortalConsole
{
    public class Person
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

        public void PrintBasicInfo()
        {
            Console.WriteLine($"Person Name: {fullName}");
        }

        // Part C: Protected Helper Method
        // Lab ID: 29
        // Arithmetic: (29 mod 3) + 2 -> 2 + 2 -> 4 letters
        // Why protected: FormatTag is protected so derived classes (Student, Instructor, Admin) can reuse it directly, whereas private would prevent inherited access and public would break encapsulation by exposing formatting logic to outside classes.
        protected string FormatTag()
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "TAGG";
            }

            string clean = fullName.Replace(" ", "");
            if (clean.Length >= 4)
            {
                return clean.Substring(0, 4).ToUpper();
            }
            return clean.PadRight(4, 'X').ToUpper();
        }
    }

    public class Student : Person
    {
        private int yearOfStudy;
        private double gpa;

        public int YearOfStudy
        {
            get { return yearOfStudy; }
            set
            {
                if (value >= 1 && value <= 4) yearOfStudy = value;
            }
        }

        public double Gpa
        {
            get { return gpa; }
            set
            {
                if (value >= 0.0 && value <= 4.0) gpa = value;
            }
        }

        public Student(string fullName, int yearOfStudy, double gpa) : base(fullName)
        {
            YearOfStudy = yearOfStudy;
            Gpa = gpa;
        }

        public void PrintSummary()
        {
            string tag = FormatTag();
            Console.WriteLine($"[{tag}] Student: {fullName} | Year: {yearOfStudy} | GPA: {gpa:F2}");
        }
    }

    public class Instructor : Person
    {
        private int yearsOfExperience;

        public int YearsOfExperience
        {
            get { return yearsOfExperience; }
            set
            {
                if (value >= 0) yearsOfExperience = value;
            }
        }

        public string AssignedCourseName { get; set; } = string.Empty;

        public Instructor(string fullName, int yearsOfExperience) : base(fullName)
        {
            YearsOfExperience = yearsOfExperience;
        }

        public void PrintSummary()
        {
            string tag = FormatTag();
            string courseInfo = string.IsNullOrWhiteSpace(AssignedCourseName)
                ? "No course assigned"
                : $"Course: {AssignedCourseName}";

            Console.WriteLine($"[{tag}] Instructor: {fullName} | Exp: {yearsOfExperience} yrs | {courseInfo}");
        }
    }

    // Supporting class to model Composition inside Admin
    public class AdminBadge
    {
        public string BadgeCode { get; private set; }

        public AdminBadge(string ownerName)
        {
            BadgeCode = "BADGE-" + Math.Abs(ownerName.GetHashCode()).ToString();
        }
    }

    // Part D: Build a Third Derived Class
    public class Admin : Person
    {
        private int accessLevel = 1;
        private AdminBadge badge; // Composition: Admin owns its AdminBadge

        // Lab ID: 29
        // Valid Range: 1 to (29 mod 3) + 2 -> 1 to 4
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
            this.badge = new AdminBadge(fullName); // Badge is created and owned strictly by Admin
        }

        public void PrintSummary()
        {
            base.PrintBasicInfo();
            string tag = FormatTag();
            Console.WriteLine($"[{tag}] Admin Access Level: {accessLevel} | Badge: {badge.BadgeCode}");
        }
    }

    // Supporting Class for Relationships
    public class Course
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
    }

    internal class Program
    {
        private static List<Person> people = new List<Person>();

        static void Main(string[] args)
        {
            // Adding initial objects including myself as an Admin
            people.Add(new Admin("Osama Aboud", 4));
            people.Add(new Student("Omar Tarek", 2, 3.5));
            people.Add(new Instructor("Dr. Khaled", 10));

            RunMenuLoop();
        }

        private static void RunMenuLoop()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("\nStudent Portal Menu");
                Console.WriteLine("1. Register Student");
                Console.WriteLine("2. Register Instructor");
                Console.WriteLine("3. Register Admin");
                Console.WriteLine("4. Display All People (Basic Info)");
                Console.WriteLine("5. Lookup Person by Name (Full Summary)");
                Console.WriteLine("6. Exit");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        RegisterStudent();
                        break;
                    case "2":
                        RegisterInstructor();
                        break;
                    case "3":
                        RegisterAdmin();
                        break;
                    case "4":
                        DisplayAllPeople();
                        break;
                    case "5":
                        LookupPersonByName();
                        break;
                    case "6":
                        running = false;
                        Console.WriteLine("Exiting...");
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        private static void RegisterStudent()
        {
            Console.Write("Enter Student Full Name: ");
            string name = Console.ReadLine() ?? "Unknown";

            Console.Write("Enter Year of Study (1-4): ");
            int.TryParse(Console.ReadLine(), out int year);

            Console.Write("Enter GPA: ");
            double.TryParse(Console.ReadLine(), out double gpa);

            people.Add(new Student(name, year, gpa));
            Console.WriteLine("Student registered.");
        }

        private static void RegisterInstructor()
        {
            Console.Write("Enter Instructor Full Name: ");
            string name = Console.ReadLine() ?? "Unknown";

            Console.Write("Enter Years of Experience: ");
            int.TryParse(Console.ReadLine(), out int exp);

            people.Add(new Instructor(name, exp));
            Console.WriteLine("Instructor registered.");
        }

        private static void RegisterAdmin()
        {
            Console.Write("Enter Admin Full Name: ");
            string name = Console.ReadLine() ?? "Unknown";

            Console.Write("Enter Access Level (1-4): ");
            int.TryParse(Console.ReadLine(), out int level);

            people.Add(new Admin(name, level));
            Console.WriteLine("Admin registered.");
        }

        private static void DisplayAllPeople()
        {
            Console.WriteLine("All Registered People:");
            foreach (Person p in people)
            {
                p.PrintBasicInfo();
            }
        }

        private static void LookupPersonByName()
        {
            Console.Write("Enter full name to look up: ");
            string searchName = (Console.ReadLine() ?? string.Empty).Trim();

            Person foundPerson = null;
            foreach (Person p in people)
            {
                if (p.FullName.Trim().Equals(searchName, StringComparison.OrdinalIgnoreCase))
                {
                    foundPerson = p;
                    break;
                }
            }

            if (foundPerson == null)
            {
                Console.WriteLine("Person not found.");
                return;
            }

            Student student = foundPerson as Student;
            Instructor instructor = foundPerson as Instructor;
            Admin admin = foundPerson as Admin;

            if (student != null)
            {
                student.PrintSummary();
            }
            else if (instructor != null)
            {
                instructor.PrintSummary();
            }
            else if (admin != null)
            {
                admin.PrintSummary();
            }
            else
            {
                foundPerson.PrintBasicInfo();
            }
        }
    }
}

/*
Part F: Wrap-Up Reflection

For my Lab ID 29, my FormatTag length is 4 letters and my Admin access-level range is 1 to 4.
Inheritance establishes an "is-a" relationship where Admin is-a Person, allowing Admin to inherit the shared fullName property and FormatTag() helper without duplicating code.
Association exists between Instructor and Course, where an Instructor references an assigned course name without owning the course lifecycle.
Aggregation is demonstrated by Course holding a List<Student>, where students are shared objects that continue to exist independently even if the course is deleted.
Composition is shown inside Admin, which instantiates and strictly owns its internal AdminBadge object, meaning the badge cannot exist without its Admin.
If we had not used Inheritance and instead modeled Admin using another relationship, we would have lost code reusability by having duplicated fullName validation logic and FormatTag() in every derived class.
Additionally, we would lose polymorphic behavior, making it impossible to manage all individuals in a single shared List<Person> or perform unified menu lookup operations.
*/
