// =====================================================================
// StudentPortalContext — COMPLETE / FALLBACK VERSION (Rule 20)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 15 — ASP.NET MVC Overview, Dependency Injection, Middleware
//
// This is the Session 14 context AFTER today's one structural change.
// Compare it side by side with Session 14's version and exactly two
// things are different:
//
//   1. It now has a constructor that ACCEPTS its configuration.
//   2. OnConfiguring is gone, and with it the connection string.
//
// Everything else — the entities, the annotations, the Fluent API
// relationship, the DbSet properties — is byte-for-byte yesterday's
// code. That is the honest measure of what Dependency Injection costs
// you: one constructor, one deletion.
//
// ⚠️ MIGRATION OWNERSHIP: the Session 14 CONSOLE project still owns
//    this database's migration history. This web project has no
//    Migrations/ folder on purpose. Do not run Add-Migration or
//    Update-Database from here — today the web app READS a database
//    yesterday already built.
// =====================================================================

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortalWeb_Complete.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        public int YearOfStudy { get; set; }

        public double Gpa { get; set; }
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

    public class StudentPortalContext : DbContext
    {
        // TODO 1 (done) — the whole of today's change to this class.
        //
        // DbContextOptions<StudentPortalContext> is the sealed envelope
        // containing "which provider, which connection string, which
        // logging" — decided elsewhere and handed in. The base class
        // knows how to open it; this class never looks inside.
        //
        // The generic argument matters. It is NOT decoration. If two
        // different contexts existed in this app, DbContextOptions<A>
        // and DbContextOptions<B> would be genuinely different types,
        // so the container could never hand the wrong configuration to
        // the wrong context. The type system enforces the pairing.
        public StudentPortalContext(DbContextOptions<StudentPortalContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        // TODO 2 (done) — OnConfiguring is DELETED.
        //
        // Yesterday it lived here and hardcoded the connection string.
        // Its absence is the point: this class can no longer decide for
        // itself where the data lives. It has to be told. That is the
        // difference between a class that owns its dependencies and a
        // class that declares them.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Session 14 Block 2 — Fluent API wins over annotations.
            modelBuilder.Entity<Student>()
                .Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(100);

            // Session 14 Block 3 — the real relationship. Restrict means
            // the database refuses to delete an instructor who still has
            // courses, rather than silently deleting the courses too.
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
