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
// ../StudentPortalConsole_Complete/Program.cs
// =====================================================================

namespace StudentPortalConsole
{

    public interface IPrintable
    {
        //public string name; // No Fields allowed
        void PrintDetails()
        {
            Console.WriteLine("PrintDetails"); // default implementation
        }
        


    }

    //interface IHasGPA
    //{
    //    double GetGPA();
    //}


    internal class Program
    {
        // ===== IPrintable interface =====

        // TODO 1: Define an interface named IPrintable, at the same
        //         level as the classes below (not inside any of them).
        //         Give it exactly one method signature, with no body
        //         and no access modifier written on it (interface
        //         members are public by default): a parameterless
        //         method that returns nothing, meant to print whatever
        //         details a class wants to expose. Remember interfaces
        //         hold signatures only — no fields, no constructor, no
        //         implementation of any kind.

        // ===== Person base class, now abstract =====

        // TODO 2: Define a class named Person, marking the class itself
        //         as ABSTRACT — this makes it impossible for any code,
        //         anywhere in this project, to write `new Person(...)`
        //         directly. Give it one protected text field for the
        //         full name, a public property that reads/writes it,
        //         and a constructor taking a full name and assigning
        //         it to the field. None of this changes because of the
        //         abstract keyword — abstract only blocks direct
        //         instantiation, it does not empty the class out.

        // TODO 3: Give Person a public method with no parameters that
        //         prints a single line naming the object's own full
        //         name — mark this method VIRTUAL. Virtual means
        //         subclasses are ALLOWED to replace this method's
        //         behavior with their own version, but are not
        //         required to — a subclass that writes no override at
        //         all simply inherits and uses this exact version.

        // TODO 4: Give Person a second method: a public method with no
        //         parameters that returns text, with NO body at all —
        //         just the signature ending in a semicolon — and mark
        //         it ABSTRACT. This one has no sensible default answer
        //         Person itself could give, so unlike TODO 3, every
        //         concrete subclass will be REQUIRED by the compiler
        //         to supply its own version, with no way to skip it.

        // ===== Student class, now overriding both Person methods and implementing IPrintable =====

        // TODO 5: Define a class named Student that DERIVES from
        //         Person AND implements IPrintable — write both after
        //         the colon, base class first, then the interface,
        //         separated by a comma. This exact order is required
        //         syntax. Give it the same private fields as Session
        //         10's version (year of study, GPA, plus one private
        //         static whole-number field tracking how many Student
        //         objects have been created in total) — nothing here
        //         changes from what you already built.

        // TODO 6: Give Student the same YearOfStudy and Gpa properties,
        //         with the same validation ranges as Session 09/10 (1-4
        //         for year, 0.0-4.0 for GPA), and the same two
        //         constructors (one taking full name/year/GPA, one
        //         taking just full name/year and chaining to the other
        //         with `: this(...)`, defaulting GPA to 0.0) — carry
        //         these forward unchanged.

        // TODO 7: Give Student the same two UpdateGpa method overloads
        //         and the same four operator overloads (`>`, `<`,
        //         `==`, `!=`) as Session 09/10 — carry these forward
        //         unchanged as well.

        // TODO 8: Give Student a method that OVERRIDES the virtual
        //         method from TODO 3. Inside, first call the inherited
        //         Person-level version using the colon-base syntax (so
        //         the name line still prints, reused rather than
        //         retyped), then print a second line with the
        //         student's own year of study and GPA.

        // TODO 9: Give Student a method that OVERRIDES the abstract
        //         method from TODO 4. This one is not optional — the
        //         project will not compile without it. Have it simply
        //         return the literal text "Student".

        // TODO 10: Give Student the method IPrintable requires — same
        //          name and shape as TODO 1's signature. Its body
        //          should just call TODO 8's method — reuse the logic
        //          that already exists rather than duplicating it.

        // TODO 11: Give Student a PUBLIC STATIC method that returns
        //          the current value of the TODO 5 total-count field.

        // ===== Instructor class, same treatment as Student =====

        // TODO 12: Define a class named Instructor that DERIVES from
        //          Person AND implements IPrintable, same order as
        //          TODO 5. Give it the same private years-of-experience
        //          field and public property (rejecting negative
        //          values) as Session 10, plus the same auto-property
        //          for an associated course name (plain text, the
        //          Association pattern from Session 09) — carry these
        //          forward unchanged. Give it the same constructor,
        //          chaining to `base(fullName)`.

        // TODO 13: Give Instructor a method OVERRIDING TODO 3's virtual
        //          method — call the inherited Person-level version
        //          first via colon-base syntax, then print a second
        //          line with years of experience (and, if an
        //          associated course name is set, a third line naming
        //          it).

        // TODO 14: Give Instructor a method OVERRIDING TODO 4's
        //          abstract method, returning the literal text
        //          "Instructor".

        // TODO 15: Give Instructor the method IPrintable requires, same
        //          shape as TODO 1/10 — its body should call TODO 13's
        //          method.

        // ===== Course class — NOT derived from Person, but still implements IPrintable =====

        // TODO 16: Define a class named Course that does NOT derive
        //          from Person at all (Course was never a kind of
        //          Person, and forcing that relationship just to reuse
        //          a printing pattern would be exactly the design
        //          mistake Block 4 of today's lecture warns against) —
        //          but DOES implement IPrintable on its own. Give it
        //          the same private fields, properties, constructor,
        //          EnrollStudent method, and PrintRoster method as
        //          Session 09, carried forward unchanged, plus one
        //          private static whole-number field tracking how many
        //          Course objects have been created in total.

        // TODO 17: Give Course the method IPrintable requires, same
        //          shape as TODO 1/10/15 — its body should call
        //          Course's own existing roster-printing method.

        // TODO 18: Give Course a PUBLIC STATIC method that returns the
        //          current value of TODO 16's total-count field.

        // ===== Helper methods (carried forward from Session 09/10 patterns) =====

        // TODO 19: Write a helper method that reads a whole number from
        //          the console in a validated loop, only accepting
        //          values from 1 to 4, re-prompting on anything else,
        //          and returns the valid value once entered.

        // TODO 20: Write a helper method that reads a decimal number
        //          from the console in a validated loop, only
        //          accepting values from 0.0 to 4.0, re-prompting on
        //          anything else, and returns the valid value once
        //          entered.

        // TODO 21: Write a helper method that searches a list of
        //          Student objects for one whose full name matches a
        //          given name, returning that Student if found or
        //          nothing if not.

        // TODO 22: Write a helper method that searches a list of Course
        //          objects for one whose course name matches a given
        //          name, returning that Course if found or nothing if
        //          not.

        static void Main(string[] args)
        {

            //List<Person> people = new List<Person>()
            //{
            //    new Student("John Doe", 3,3.5),
            //    new Instructor("Jane Doe", 5),
            //};

            //foreach (var person in people)
            //{
            //    Console.WriteLine($"This is {person.GetRoleDescription()}");
            //    person.PrintBasicInfo(); // Polymorphic Override of PrintBasicInfo
            //}

            List<IPrintable> printables = new List<IPrintable>();
            printables.Add(new Student("John Doe", 3, 3.5));
            printables.Add(new Instructor("Jane Doe", 5));
            printables.Add(new Course("Math 101",4));

            foreach (var item in printables)
            {
                item.PrintDetails();
            }

            //List<IHasGPA> hasGPAs = new List<IHasGPA>();
            //hasGPAs.Add(new Student("John Doe", 3, 3.5));
            //hasGPAs.Add(new Instructor("Jane Doe", 5));
            //foreach (var item in hasGPAs)
            //{
            //    item.GetGPA();
            //}
            //var hasGPA = new Student("John Doe", 3, 3.5);
            //Console.WriteLine(hasGPA.GetGPA()); 


            //Person person1 = new Person("John Doe"); // error because Person is abstract
            // ===== Capstone menu wiring =====

            // TODO 23: Declare three empty lists to hold every Student,
            //          Instructor, and Course created this run, plus a
            //          true/false flag controlling the main menu loop.

            // TODO 24: Build a do-while loop that prints a numbered
            //          menu offering: register a student, register an
            //          instructor, create a course, enroll a student in
            //          a course, compare two students by GPA using the
            //          `>` operator, print everyone (every Student and
            //          Instructor together, through a single list typed
            //          as Person), print everything printable (every
            //          Student, Instructor, AND Course together,
            //          through a single list typed as IPrintable), show
            //          total students/courses ever created, and quit.
            //          Read the trainee's numeric choice as text.

            // TODO 25: Inside a switch on that choice, wire the
            //          register-student option: prompt for full name
            //          (TODO 19's helper for year, TODO 20's helper for
            //          GPA), build a new Student, add it to the
            //          students list.

            // TODO 26: Wire the register-instructor option: prompt for
            //          full name and years of experience, build a new
            //          Instructor, add it to the instructors list.

            // TODO 27: Wire the create-course option: prompt for course
            //          name and credits, build a new Course, add it to
            //          the courses list.

            // TODO 28: Wire the enroll-a-student option: prompt for a
            //          student's name and a course's name, look both up
            //          with TODO 21/22's helpers, print an error if
            //          either isn't found, otherwise enroll the student
            //          in the course.

            // TODO 29: Wire the compare-two-students option: prompt for
            //          two student names, look both up, print an error
            //          if either isn't found, otherwise print the
            //          result of comparing them with the `>` operator.

            // TODO 30: Wire the print-everyone option: build ONE list
            //          typed as Person, add every Student and every
            //          Instructor into it, then loop over it calling
            //          TODO 3/8/13's virtual/override method on each —
            //          notice this single loop correctly prints
            //          role-specific detail for every object, with NO
            //          casts and NO type-checking if/else chain
            //          anywhere, because of dynamic dispatch. Also
            //          print each object's role using TODO 4/9/14's
            //          abstract method.

            // TODO 31: Wire the print-everything-printable option:
            //          build ONE list typed as IPrintable, add every
            //          Student, every Instructor, AND every Course into
            //          it, then loop over it calling TODO 1/10/15/17's
            //          interface method on each — notice this list can
            //          hold a Course even though Course shares no
            //          inheritance with Person at all.

            // TODO 32: Wire the total-counts option: print the results
            //          of TODO 11 and TODO 18's static methods.

            // TODO 33: Wire the quit option to end the loop, and print
            //          a goodbye message after the loop ends.
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
