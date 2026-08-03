using Microsoft.EntityFrameworkCore;
using StudentPortalConsole;

using var context = new StudentPortalContext();

// PART C — Full CRUD Operations

var nada = await context.Students.FirstAsync(s => s.FullName == "Nada Samir");
Console.WriteLine($"Nada Current GPA: {nada.Gpa}");

nada.Gpa = 3.0; 
Console.WriteLine($"Nada Memory GPA: {nada.Gpa}");


await context.SaveChangesAsync();


var me = new Student { FullName = "Osama Aboud", YearOfStudy = 2, Gpa = 3.0, CreditsCompleted = 30 };
Console.WriteLine($"ID before Save: {me.Id}");
context.Students.Add(me);
await context.SaveChangesAsync();
Console.WriteLine($"ID after Save: {me.Id}");

me.YearOfStudy = 3;
await context.SaveChangesAsync();

context.Students.Remove(me);
await context.SaveChangesAsync();


// PART D — Constraint Exception Verification

try
{
    context.Students.Add(new Student { FullName = null!, YearOfStudy = 1, Gpa = 3.0 });
    await context.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    Console.WriteLine($"Part D Exception Caught: {ex.GetType().Name}");
    context.ChangeTracker.Clear(); 
}


// PART E — Relationship Verification

var webCourse = await context.Courses.FirstAsync(c => c.CourseName.Contains("Web Development"));
webCourse.InstructorId = 1; 
await context.SaveChangesAsync();

try
{
    context.Courses.Add(new Course { CourseName = "Invalid FK", Credits = 3, InstructorId = 9999 });
    await context.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    Console.WriteLine($"Part E FK Constraint Exception Caught: {ex.InnerException?.Message}");
    context.ChangeTracker.Clear(); 
}


// PART F — Loading Strategies

if (!await context.Courses.AnyAsync(c => c.CourseName == "Advanced C#"))
{
    context.Courses.AddRange(
        new Course { CourseName = "Advanced C#", Credits = 4, InstructorId = 1 },
        new Course { CourseName = "EF Core Deep Dive", Credits = 3, InstructorId = 1 },
        new Course { CourseName = "SQL Optimization", Credits = 3, InstructorId = 1 }
    );
    await context.SaveChangesAsync();
}


var instructorsNoInclude = await context.Instructors.ToListAsync();
foreach (var i in instructorsNoInclude)
    Console.WriteLine($"No Include - {i.FullName}: {i.Courses.Count}");

var instructorsEager = await context.Instructors.Include(i => i.Courses).ToListAsync();
foreach (var i in instructorsEager)
    foreach (var c in i.Courses)
        Console.WriteLine($"Include - {i.FullName} -> {c.CourseName}");

var singleInst = await context.Instructors.FirstAsync(i => i.Id == 1);
Console.WriteLine($"Explicit Before: {singleInst.Courses.Count}");
await context.Entry(singleInst).Collection(i => i.Courses).LoadAsync();
Console.WriteLine($"Explicit After: {singleInst.Courses.Count}");

var untracked = await context.Students.AsNoTracking().FirstAsync();
untracked.Gpa = 2.0;
await context.SaveChangesAsync();

/*
PART G — LAB REFLECTION
1. Personal Lab ID: 7
   - Part C GPA Threshold : 3.0 + ((7 % 7) * 0.1) = 3.0
   - Part E DeleteBehavior: 7 % 2 = 1 -> SetNull (Requires int? InstructorId)
   - Part F Course Count  : ((7 % 3) + 2) = 3 Extra Courses

2. Part E Delete Behavior Explanation (SetNull):
   Configuring OnDelete to SetNull ensures that deleting an Instructor automatically 
   sets the associated Courses' InstructorId to NULL. This prevents course deletion 
   and protects course records, which requires InstructorId to be nullable (int?).

3. Rollback Statement:
   No rollback needed.
   If needed, commands are:
   - Update-Database <PreviousMigrationName>
   - Remove-Migration

4. N+1 & Multiple Enumeration Analysis:
   Both issue multiple roundtrips to SQL Server due to delayed query execution. 
   Part F evidence shows Eager loading executes 1 query (JOIN), while missing Include 
   requires separate queries to fetch collection data.
*/