using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace StudentPortalConsole
{
    public class StudentPortalContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=.;Database=ITI_StudentPortalDB_EF;Trusted_Connection=True;TrustServerCertificate=True;")
                .LogTo(Console.WriteLine, LogLevel.Information); 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Part D: Fluent API Constraint
            modelBuilder.Entity<Student>()
                .Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(100);

            // Part E: SetNull Relationship for Lab ID 7 (7 % 2 = 1)
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}