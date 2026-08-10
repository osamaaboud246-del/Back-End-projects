// =====================================================================
// StudentPortalConsole — SESSION PROJECT (Style Guide Rule 20/34/35/39/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 19 — Models: the Many-to-Many Relationship and Migration Ownership
//
// ⚠️ THIS IS STILL THE SAME PROJECT (Rule 40). This console project has not
//    been open since Session 14 — Sessions 15-18 all correctly left it
//    alone, because none of them changed the database schema. Today is
//    the first schema change since Session 14, and this project is where
//    every schema change in this course happens (Rule 38: ONE migration
//    owner, always).
//
// EVERYTHING BELOW BLOCKS 1-4'S MARKER IS CARRIED FORWARD, REAL, WORKING
// CODE FROM SESSION 14 (Rule 39) — CRUD, the Student/Course FK, Include/
// AsNoTracking. Press F5 right now, before a single TODO is done, and it
// prints the roster and proves every Session 14 behaviour again, exactly
// as it did that day.
//
// TODAY'S THREE TODOs, IN THE ORDER THE LECTURE VISITS THEM:
//   TODO 1   Block 1   The Enrollment entity — the associative table
//   TODO 2   Block 2   The Fluent API — two relationships + a unique index
//   TODO 3   Block 2   Prove it against the real database
//
// ⚠️ AFTER TODO 1 AND TODO 2, BEFORE TODO 3: this is the one day this
//    course adds real schema outside Session 14. Open the Package
//    Manager Console (Tools → NuGet Package Manager → Package Manager
//    Console) with THIS project selected as the Default project, and run,
//    in order:
//       Add-Migration AddEnrollment
//       Update-Database
//    Read the generated migration file before running Update-Database —
//    Block 2 explains exactly what to check for. Only once that succeeds
//    does TODO 3 have a real Enrollments table to insert into.
//
// ⚠️ IF THE MIGRATION FAILS: roll back, do NOT drop the database.
//     Update-Database AddInstructorCourseRelationship   (steps back)
//     Remove-Migration                                  (deletes the bad file)
//
// For the full, correct, runnable version (do NOT peek until you've
// tried it yourself, or you're checking your own work), see:
// ../StudentPortalConsole_Complete/Program.cs
// =====================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace StudentPortalConsole
{
    // =================================================================
    // CARRIED FORWARD FROM SESSION 14 — already working. Unchanged today
    // except for the two additions TODO 1 marks.
    // =================================================================
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        public int YearOfStudy { get; set; }
        public double Gpa { get; set; }

        // TODO 1 (part one): Add ONE navigation property here — a list of
        //         Enrollment objects, initialized to a new empty list, the
        //         same shape as Instructor's Courses list from Session 14.
        //         This is NOT a column. It exists so C# code can write
        //         student.Enrollments without a separate query. Name it
        //         Enrollments, plural, matching the DbSet you add below.
        public List<Enrollment> Enrollments { get; set; } = new();
    }

    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string CourseName { get; set; } = "";

        public int Credits { get; set; }

        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;

        // TODO 1 (part two): The SAME addition as Student's, above — a
        //         list of Enrollment, initialized to new(), named
        //         Enrollments. Course is the other "many" side of the
        //         same relationship.
        public List<Enrollment> Enrollments { get; set; } = new();
    }

    public class Instructor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        public int YearsOfExperience { get; set; }
        public List<Course> Courses { get; set; } = new();
    }

    // TODO 1 (part three): Declare the associative entity itself, as a
    //         brand-new class sitting HERE — after Instructor, before the
    //         DbContext, because this is the physical point in the file
    //         where a new entity belongs (Rule 40).
    //
    //         Name it Enrollment. It needs, in this order:
    //           - a whole-number Id, the primary key, same shape as every
    //             other entity in this file;
    //           - a whole-number StudentId — a REAL foreign-key column,
    //             same pattern as Course.InstructorId;
    //           - a navigation property of type Student, named Student,
    //             marked non-null with the null-forgiving operator, since
    //             EF populates it, not your code;
    //           - a whole-number CourseId — the second real foreign-key
    //             column;
    //           - a navigation property of type Course, named Course,
    //             also null-forgiving;
    //           - a point-in-time value recording when the enrollment
    //             happened, named EnrollmentDate, of the type C# uses for
    //             dates and times;
    //           - a grade, named Grade, of the NULLABLE version of the
    //             type you use for GPA — write the type name followed by
    //             a question mark. A grade that does not exist yet (the
    //             course just started) must be representable as "no
    //             value", not as a fake number like zero, because zero
    //             would silently look like a real failing grade.
    //         Give Grade the SAME validation attribute Session 17 gave
    //         Student.Gpa — the one meaning "must fall between these two
    //         numbers, inclusive" — with bounds 0.0 and 4.0, and your own
    //         plain-English ErrorMessage.
    //         ⚠️ This class has TWO foreign keys and TWO navigation
    //         properties, twice as many as Course had. That is not a
    //         mistake — it is the entire shape of an associative entity:
    //         one foot in each of the two tables it connects.

    public class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
        [Range(0.0, 4.0, ErrorMessage = "Grade must be between 0.0 and 4.0")]
        public double? Grade { get; set; }
    }

    // =================================================================
    // THE CONTEXT — unchanged since Session 14 except where TODO 2 says.
    // =================================================================
    public class StudentPortalContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        // TODO 1 (part four): Add ONE more DbSet property here, for
        //         Enrollment, named Enrollments — the same pattern as the
        //         three above it. Without this, EF does not know the new
        //         class is a table at all; it would just be an ordinary
        //         C# class no query can ever reach.
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=.;Database=ITI_StudentPortal;Trusted_Connection=True;TrustServerCertificate=True;")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // TODO 2: Three more Fluent API statements, all configuring
            //         Enrollment, all belonging HERE — physically inside
            //         this same method, after the Course/Instructor
            //         config above it, because this is where every
            //         relationship in this project gets configured.
            //
            //         First relationship — Enrollment to Student:
            //         start from Enrollment, say it HasOne Student
            //         (through Enrollment's Student navigation property),
            //         say Student WithMany Enrollments (through Student's
            //         Enrollments list), name the foreign key
            //         (Enrollment's StudentId), and choose the delete
            //         behaviour that DELETES an enrollment when its
            //         student is deleted — write the word for "cascade".
            //
            //         Second relationship — Enrollment to Course: the
            //         SAME four-call shape, substituting Course for
            //         Student throughout (HasOne Course, Course WithMany
            //         Enrollments, foreign key CourseId), same cascade
            //         delete choice.
            //         ⚠️ Both use Cascade, not Session 14's Restrict.
            //         Block 2 explains why the correct choice is
            //         DIFFERENT here even though both are still "best
            //         practice": an Enrollment has no meaning once its
            //         Student or its Course is gone, where an
            //         Instructor's Courses very much do.
            //
            //         Third — the unique index. Start from Enrollment,
            //         call the method meaning "create an index" naming
            //         BOTH StudentId and CourseId together inside a `new
            //         { }` anonymous object, then chain the call meaning
            //         "and this combination must be unique". This is a
            //         DATABASE-enforced rule: the same student cannot be
            //         inserted twice against the same course, no matter
            //         which application, which developer, or which bug
            //         tries it.
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            using (var context = new StudentPortalContext())
            {
                Console.WriteLine("Students currently in the database:");
                foreach (var s in await context.Students.ToListAsync())
                {
                    Console.WriteLine($"  {s.FullName} — Year {s.YearOfStudy}, GPA {s.Gpa:F2}");
                }

                // =====================================================
                // CARRIED FORWARD FROM SESSION 14 — unchanged. Blocks 1-4
                // there proved CRUD, constraints, the Course/Instructor
                // relationship, and loading strategies. None of that is
                // today's topic; it stays exactly as it was.
                // =====================================================
                Console.WriteLine();
                Console.WriteLine("===== SESSION 14 RECAP (unchanged) =====");

                var nada = await context.Students.FirstAsync(s => s.FullName == "Nada Samir");
                Console.WriteLine($"  {nada.FullName} — GPA {nada.Gpa:F2}");

                var withCourses = await context.Instructors.Include(i => i.Courses).ToListAsync();
                foreach (var i in withCourses)
                {
                    Console.WriteLine($"  {i.FullName} teaches {i.Courses.Count} course(s)");
                }

                // =====================================================
                // TODO 3 — Block 2's live proof, AFTER Add-Migration AND
                // Update-Database have both succeeded against the real
                // database. Do not attempt this before then; there is no
                // Enrollments table yet and it will throw a SQL error
                // about an invalid object name.
                //
                // In this exact order:
                //   1. Load the real Student named "Nada Samir" (you
                //      already have her loaded two lines above — reuse
                //      that variable) and the real Course named "Web
                //      Development Using .NET" (FirstAsync, matching on
                //      CourseName).
                //   📌 Idempotency: this Main() may genuinely run more than
                //      once against the same real database (a rehearsal,
                //      then again live). Check with AnyAsync whether this
                //      exact pairing already exists BEFORE step 2 — if it
                //      does, skip straight to step 4 instead of attempting
                //      an unguarded insert that would throw unhandled on a
                //      second run.
                //   2. If not already enrolled: create a new Enrollment
                //      linking their two Ids, with EnrollmentDate set to
                //      right now (the type's static "Now" member) and
                //      Grade left unset (it defaults to having no value,
                //      which is exactly correct — she has not finished the
                //      course yet).
                //   3. Add it to the Enrollments set and save. Print a
                //      confirmation line naming both the student and the
                //      course (or, if it already existed, print that it
                //      was already there from an earlier run).
                //   4. Immediately try to do the EXACT SAME THING again —
                //      same student, same course, a second brand-new
                //      Enrollment object. Wrap this second attempt in a
                //      try/catch catching DbUpdateException, the same
                //      exception type Session 14 caught for the NULL
                //      name and the bad foreign key. In the catch block,
                //      print that the duplicate was rejected. In the
                //      (nonexistent, if this is written correctly) code
                //      path where it succeeds, print a warning that the
                //      unique index is not in place — that line should
                //      never run.
                //      ⚠️ This is Block 2's whole payoff: nothing in
                //      your C# code checks for a duplicate. The database
                //      itself refuses the second row, because of the
                //      unique index from TODO 2 — a rule enforced in
                //      exactly one place, impossible to bypass by a
                //      different program, a different bug, or a typo in
                //      a future TODO.


                Console.WriteLine("============= Try Enrollment=============");
                var webCourse = await context.Courses
                    .FirstAsync(c => c.CourseName == "Web Development Using .NET");
               
                var alreadyEnrolled = await context.Enrollments
                                .AnyAsync(e => e.StudentId == nada.Id && e.CourseId == webCourse.Id); 
                
                if (!alreadyEnrolled)
                {
                    var firstEnrollment = new Enrollment
                    {
                        StudentId = nada.Id,
                        CourseId = webCourse.Id,
                        EnrollmentDate = DateTime.Now
                    };
                    await context.Enrollments.AddAsync(firstEnrollment);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"  Enrolled {nada.FullName} in {webCourse.CourseName}.");
                }
                else
                {
                    Console.WriteLine($"  {nada.FullName} is already enrolled in {webCourse.CourseName}.");
                }

                Console.WriteLine("============= Try Duplicate Enrollment=============");

                try
                {
                    var duplicate = new Enrollment
                    {
                        StudentId = nada.Id,
                        CourseId = webCourse.Id,
                        EnrollmentDate = DateTime.Now
                    };
                    await context.Enrollments.AddAsync(duplicate);
                    await context.SaveChangesAsync();
                    Console.WriteLine($" Duplicate enrollment: {nada.FullName} in {webCourse.CourseName}.");
                }
                catch (DbUpdateException)
                {
                    Console.WriteLine($" Rejected duplicate enrollment By Unique Index: {nada.FullName} in {webCourse.CourseName}.");
                    foreach (var entry in context.ChangeTracker.Entries<Enrollment>()
                        .Where(e=>e.State == EntityState.Added))
                    {
                        entry.State = EntityState.Detached;
                    }
                }

            }


            Console.WriteLine();
            Console.WriteLine("Done.");
        }
    }
}

#region 📋 Full TODO Checklist
// ---------------------------------------------------------------------
// Program.cs (this file)
//   TODO 1: The Enrollment entity — four parts, four different physical
//           locations (Student's nav property, Course's nav property,
//           the class itself, the DbSet)                        [Block 1]
//   TODO 2: Fluent API — two relationships + one unique index    [Block 2]
//   TODO 3: Prove it — enroll once, then try the same pair twice [Block 2]
//
// (run Add-Migration AddEnrollment / Update-Database between TODO 2
//  and TODO 3 — see the file header and the Instructor Guide)
// ---------------------------------------------------------------------
#endregion
