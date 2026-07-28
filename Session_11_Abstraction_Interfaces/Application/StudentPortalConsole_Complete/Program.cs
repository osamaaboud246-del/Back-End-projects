// =====================================================================
// StudentPortalConsole_Complete — FULL WORKING FALLBACK (Rule 20)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 11 — Abstraction + Interfaces + OOP Capstone
//
// Complete, correct, runnable version of everything taught live today.
// Matches Instructor_Guide_EN.md and Student_Guide.md exactly (Rule 15).
// Extends Session 10's Person/Student/Instructor with:
//   - Person becomes abstract (cannot be instantiated on its own)
//   - Person.PrintBasicInfo() becomes virtual; Student/Instructor override it
//   - Person gains an abstract GetRoleDescription(), overridden by both
//   - A new IPrintable interface, implemented by Student, Instructor, AND
//     Course (which is NOT a Person at all — proves interfaces cross
//     unrelated hierarchies)
// This is the OOP capstone: Encapsulation (Session 8), Polymorphism +
// Class Relationships (Session 9), Inheritance (Session 10), and today's
// Abstraction + Interfaces are all exercised together through one menu.
// Run with: dotnet run (or Visual Studio's Start Without Debugging)
// =====================================================================

namespace StudentPortalConsole
{
    internal class Program
    {
        // An interface: pure contract, no fields, no shared code, no
        // constructor. Any class — related or not — can implement it.
        interface IPrintable
        {
            void PrintDetails();
        }

        // Person is now abstract: "new Person(...)" is a compile error
        // anywhere in this project. It still holds real shared state
        // (fullName) and a real shared constructor — abstract does NOT
        // mean empty, it means "not directly buildable."
        abstract class Person
        {
            protected string fullName;
            public string FullName
            {
                get { return fullName; }
                set { fullName = value; }
            }
            public Person(string fullName)
            {
                this.fullName = fullName;
            }

            // virtual: Person supplies a sensible default. Subclasses
            // MAY replace it, but aren't required to.
            public virtual void PrintBasicInfo()
            {
                Console.WriteLine($"Person: {FullName}");
            }

            // abstract: no default makes sense here. Every concrete
            // subclass MUST supply its own — enforced at compile time.
            public abstract string GetRoleDescription();
        }

        class Student : Person, IPrintable
        {
            private static int totalStudentsCreated = 0;

            private int yearOfStudy;
            private double gpa;

            public int YearOfStudy
            {
                get { return yearOfStudy; }
                set
                {
                    if (value >= 1 && value <= 4)
                    {
                        yearOfStudy = value;
                    }
                }
            }

            public double Gpa
            {
                get { return gpa; }
                set
                {
                    if (value >= 0.0 && value <= 4.0)
                    {
                        gpa = value;
                    }
                }
            }

            public Student(string fullName, int yearOfStudy)
                : this(fullName, yearOfStudy, 0.0)
            {
            }

            public Student(string fullName, int yearOfStudy, double gpa)
                : base(fullName)
            {
                YearOfStudy = yearOfStudy;
                Gpa = gpa;
                totalStudentsCreated++;
            }

            public void UpdateGpa(double newGpa)
            {
                UpdateGpa(newGpa, "No reason given");
            }

            public void UpdateGpa(double newGpa, string reason)
            {
                Gpa = newGpa;
                Console.WriteLine($"{fullName}'s GPA updated to {newGpa:F2}. Reason: {reason}");
            }

            // override of Person's virtual method — adds Student-specific
            // detail on top of the reused base line.
            public override void PrintBasicInfo()
            {
                base.PrintBasicInfo();
                Console.WriteLine($"  Year {YearOfStudy}, GPA {Gpa:F2}");
            }

            // override of Person's abstract method — mandatory, no default existed.
            public override string GetRoleDescription()
            {
                return "Student";
            }

            // IPrintable implementation — reuses PrintBasicInfo rather than
            // duplicating its logic.
            public void PrintDetails()
            {
                PrintBasicInfo();
            }

            public static int GetTotalStudents()
            {
                return totalStudentsCreated;
            }

            public static bool operator >(Student a, Student b)
            {
                return a.Gpa > b.Gpa;
            }

            public static bool operator <(Student a, Student b)
            {
                return a.Gpa < b.Gpa;
            }

            public static bool operator ==(Student a, Student b)
            {
                if (ReferenceEquals(a, b)) return true;
                if (a is null || b is null) return false;
                return a.fullName == b.fullName && a.Gpa == b.Gpa;
            }

            public static bool operator !=(Student a, Student b)
            {
                return !(a == b);
            }

            public override bool Equals(object? obj)
            {
                if (obj is Student other) return this == other;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(fullName, gpa);
            }
        }

        class Instructor : Person, IPrintable
        {
            private int yearsOfExperience;

            public int YearsOfExperience
            {
                get { return yearsOfExperience; }
                set
                {
                    if (value >= 0)
                        yearsOfExperience = value;
                }
            }

            public string? AssignedCourseName { get; set; }

            public Instructor(string fullName, int yearsOfExperience)
                : base(fullName)
            {
                YearsOfExperience = yearsOfExperience;
            }

            public override void PrintBasicInfo()
            {
                base.PrintBasicInfo();
                Console.WriteLine($"  {YearsOfExperience} years of experience");
                if (AssignedCourseName != null)
                {
                    Console.WriteLine($"  Currently associated with {AssignedCourseName}");
                }
            }

            public override string GetRoleDescription()
            {
                return "Instructor";
            }

            public void PrintDetails()
            {
                PrintBasicInfo();
            }
        }

        // Course is NOT a Person and never derives from it — it implements
        // IPrintable on its own, proving the interface unifies classes with
        // no shared inheritance at all.
        class Course : IPrintable
        {
            private static int totalCoursesCreated = 0;

            private string courseName;
            private int credits;
            private List<Student> enrolledStudents = new List<Student>();

            public string CourseName
            {
                get { return courseName; }
                set { courseName = value; }
            }

            public int Credits
            {
                get { return credits; }
                set
                {
                    if (value >= 1 && value <= 6)
                    {
                        credits = value;
                    }
                }
            }

            public Course(string courseName, int credits)
            {
                CourseName = courseName;
                Credits = credits;
                totalCoursesCreated++;
            }

            public void EnrollStudent(Student s)
            {
                enrolledStudents.Add(s);
            }

            public void PrintRoster()
            {
                Console.WriteLine($"=== {courseName} Roster ({enrolledStudents.Count} enrolled) ===");
                foreach (Student s in enrolledStudents)
                {
                    s.PrintBasicInfo();
                }
            }

            public void PrintDetails()
            {
                PrintRoster();
            }

            public static int GetTotalCourses()
            {
                return totalCoursesCreated;
            }
        }

        static int ReadValidYear()
        {
            int year = 0;
            bool isValid = false;
            while (!isValid)
            {
                Console.WriteLine("Enter year of study (1-4):");
                string? input = Console.ReadLine();
                isValid = int.TryParse(input, out year) && year >= 1 && year <= 4;
                if (!isValid) Console.WriteLine("That's not a valid year. Try again.");
            }
            return year;
        }

        static double ReadValidGpa()
        {
            double gpa = 0;
            bool isValid = false;
            while (!isValid)
            {
                Console.WriteLine("Enter GPA (0.0-4.0):");
                string? input = Console.ReadLine();
                isValid = double.TryParse(input, out gpa) && gpa >= 0.0 && gpa <= 4.0;
                if (!isValid) Console.WriteLine("That's not a valid GPA. Try again.");
            }
            return gpa;
        }

        static Course? FindCourseByName(List<Course> courses, string name)
        {
            foreach (Course c in courses)
            {
                if (c.CourseName == name) return c;
            }
            return null;
        }

        static Student? FindStudentByName(List<Student> students, string name)
        {
            foreach (Student s in students)
            {
                if (s.FullName == name) return s;
            }
            return null;
        }

        static void Main(string[] args)
        {
            List<Student> students = new List<Student>();
            List<Instructor> instructors = new List<Instructor>();
            List<Course> courses = new List<Course>();
            bool keepRunning = true;

            do
            {
                Console.WriteLine();
                Console.WriteLine("=== StudentPortal Menu (Session 11 Capstone) ===");
                Console.WriteLine("1. Register a new student");
                Console.WriteLine("2. Register a new instructor");
                Console.WriteLine("3. Create a course");
                Console.WriteLine("4. Enroll a student in a course");
                Console.WriteLine("5. Compare two students by GPA (>)");
                Console.WriteLine("6. Print everyone (List<Person> — Abstraction)");
                Console.WriteLine("7. Print everything printable (List<IPrintable> — Interfaces)");
                Console.WriteLine("8. Show total students/courses ever created");
                Console.WriteLine("0. Quit");
                Console.WriteLine("Choose an option:");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Enter the student's full name:");
                        string name1 = Console.ReadLine() ?? "";
                        int year1 = ReadValidYear();
                        double gpa1 = ReadValidGpa();
                        students.Add(new Student(name1, year1, gpa1));
                        Console.WriteLine("Student registered.");
                        break;

                    case "2":
                        Console.WriteLine("Enter the instructor's full name:");
                        string name2 = Console.ReadLine() ?? "";
                        Console.WriteLine("Enter years of experience:");
                        int.TryParse(Console.ReadLine(), out int years);
                        instructors.Add(new Instructor(name2, years));
                        Console.WriteLine("Instructor registered.");
                        break;

                    case "3":
                        Console.WriteLine("Enter the course name:");
                        string courseName = Console.ReadLine() ?? "";
                        Console.WriteLine("Enter credits (1-6):");
                        int.TryParse(Console.ReadLine(), out int credits);
                        courses.Add(new Course(courseName, credits));
                        Console.WriteLine("Course created.");
                        break;

                    case "4":
                        Console.WriteLine("Enter the student's full name:");
                        string enrollStudentName = Console.ReadLine() ?? "";
                        Console.WriteLine("Enter the course name:");
                        string enrollCourseName = Console.ReadLine() ?? "";
                        Student? studentToEnroll = FindStudentByName(students, enrollStudentName);
                        Course? courseToEnrollIn = FindCourseByName(courses, enrollCourseName);
                        if (studentToEnroll == null || courseToEnrollIn == null)
                        {
                            Console.WriteLine("Student or course not found.");
                            break;
                        }
                        courseToEnrollIn.EnrollStudent(studentToEnroll);
                        Console.WriteLine("Student enrolled.");
                        break;

                    case "5":
                        Console.WriteLine("Enter the first student's full name:");
                        string firstName = Console.ReadLine() ?? "";
                        Console.WriteLine("Enter the second student's full name:");
                        string secondName = Console.ReadLine() ?? "";
                        Student? first = FindStudentByName(students, firstName);
                        Student? second = FindStudentByName(students, secondName);
                        if (first == null || second == null)
                        {
                            Console.WriteLine("One or both students not found.");
                            break;
                        }
                        Console.WriteLine($"{first.FullName} > {second.FullName} by GPA: {first > second}");
                        break;

                    case "6":
                        // Abstraction payoff: one List<Person>, virtual/override
                        // dispatch, zero casts, correct output per real type.
                        List<Person> everyone = new List<Person>();
                        everyone.AddRange(students);
                        everyone.AddRange(instructors);
                        foreach (Person p in everyone)
                        {
                            p.PrintBasicInfo();
                            Console.WriteLine($"  Role: {p.GetRoleDescription()}");
                        }
                        break;

                    case "7":
                        // Interfaces payoff: List<IPrintable> crosses the
                        // Person hierarchy AND Course, which isn't a Person.
                        List<IPrintable> printables = new List<IPrintable>();
                        printables.AddRange(students);
                        printables.AddRange(instructors);
                        printables.AddRange(courses);
                        foreach (IPrintable item in printables)
                        {
                            item.PrintDetails();
                        }
                        break;

                    case "8":
                        Console.WriteLine($"Total students ever created: {Student.GetTotalStudents()}");
                        Console.WriteLine($"Total courses ever created: {Course.GetTotalCourses()}");
                        break;

                    case "0":
                        keepRunning = false;
                        break;

                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }

            } while (keepRunning);

            Console.WriteLine("Goodbye!");
        }
    }
}
