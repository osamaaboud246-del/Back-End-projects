// =====================================================================
// StudentPortalConsole — TODO GUIDE ONLY (Style Guide Rule 20 / Rule 35)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 11 — Abstraction + Interfaces + OOP Capstone
//
// This file holds VERBAL/TODO guidance only — NOT working code. Build
// this yourself, from scratch, following the TODOs below in order.
// Every pattern here was demonstrated live in today's lecture — the
// Student Guide has the exact worked examples if you get stuck.
//
// This EXTENDS Session 10's Person/Student/Instructor: Person becomes
// abstract with one virtual and one abstract method, Student/Instructor
// override both, and a new IPrintable interface is implemented by
// Student, Instructor, AND Course (which is NOT a Person at all). The
// Main menu then wires everything from Sessions 8-11 together into one
// working capstone app.
//
// For the full, correct, runnable version (do NOT peek until you've
// tried it yourself, or you're using it to check your own work), see:
// ../StudentPortalConsole_Complete/Student.cs
// =====================================================================

namespace StudentPortalConsole
{
    public class Student : Person, IPrintable// Student Is A Person
    {
        private int yearOfStudy;
        private double gpa;

        public int YearOfStudy
        {
            get { return yearOfStudy; }
            set { yearOfStudy = value; }
        }

        public double Gpa
        {
            get { return gpa; }
            set { gpa = value; }
        }
        public Student(string fullName, int yearOfStudy, double gpa)
            : base(fullName)
        {
            Console.WriteLine("Student Constructor");
            YearOfStudy = yearOfStudy;
            Gpa = gpa;
        }

        //private string GetShortName()
        //{
        //    return fullName.Substring(0, 3).ToUpper();
        //}
        public override void PrintBasicInfo()
        {
            base.PrintBasicInfo(); // Student -> person -> baseClass
            Console.WriteLine($"Year of Study : {YearOfStudy}");
            Console.WriteLine($"GPA : {Gpa}");
        }

        public override string GetRoleDescription()
        {
            return "Student";
        }

        public void PrintDetails()
        {
            PrintBasicInfo();
        }

        public double GetGPA()
        {
            return Gpa;
        }
    }

    #region 📋 Full TODO Checklist (collapse this region for a quick overview)
    // 1.  Define IPrintable — one method signature, no body, no access modifier
    // 2.  Define Person as ABSTRACT — protected fullName field, public FullName property, constructor
    // 3.  Add Person's VIRTUAL PrintBasicInfo() — has a real default, optional to override
    // 4.  Add Person's ABSTRACT GetRoleDescription() — no body, mandatory to override
    // 5.  Define Student : Person, IPrintable — private fields (year, GPA) + static total count
    // 6.  Add YearOfStudy/Gpa properties + both constructors (carried forward)
    // 7.  Add UpdateGpa overloads + operator overloads (carried forward)
    // 8.  Override PrintBasicInfo() — calls base first, then year/GPA line
    // 9.  Override GetRoleDescription() — returns "Student"
    // 10. Implement IPrintable's method — calls TODO 8's method
    // 11. Add the static total-Student-count getter
    // 12. Define Instructor : Person, IPrintable — years of experience + AssignedCourseName (carried forward)
    // 13. Override PrintBasicInfo() — calls base first, then years-of-experience (+ course) line
    // 14. Override GetRoleDescription() — returns "Instructor"
    // 15. Implement IPrintable's method — calls TODO 13's method
    // 16. Define Course (NOT : Person) — fields/properties/EnrollStudent/PrintRoster (carried forward) + static total count
    // 17. Implement IPrintable's method — calls Course's roster-printing method
    // 18. Add the static total-Course-count getter
    // 19. Add the validated year-reading helper (1-4)
    // 20. Add the validated GPA-reading helper (0.0-4.0)
    // 21. Add the find-Student-by-name helper
    // 22. Add the find-Course-by-name helper
    // 23. Declare the three lists + the loop flag
    // 24. Build the do-while menu loop with all 9 options
    // 25. Wire: register a student
    // 26. Wire: register an instructor
    // 27. Wire: create a course
    // 28. Wire: enroll a student in a course
    // 29. Wire: compare two students by GPA
    // 30. Wire: print everyone (List<Person>, virtual/override dispatch)
    // 31. Wire: print everything printable (List<IPrintable>, crosses Course)
    // 32. Wire: show total students/courses ever created
    // 33. Wire: quit + goodbye message
    #endregion
}
