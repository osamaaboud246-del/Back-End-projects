-- ==============================================================================
-- 🎓 Session 19 Pre-Initialization Script — VERIFY-ONLY (run BEFORE the migration)
-- ITI Summer Training | Web Development Using .NET | Morning Group
-- ==============================================================================
-- ⚠️ Rule 38: Real, Persistent Environment Continuity
--
-- This script CREATES nothing, DROPS nothing and ALTERS nothing. It only reads.
--
-- 🔴 UNLIKE SESSIONS 15-18, TODAY GENUINELY CHANGES SCHEMA — but not from this
--    script. Today's new Enrollments table is created LIVE, in Block 2, by
--    running Add-Migration AddEnrollment / Update-Database from the SESSION 14
--    CONSOLE PROJECT. Run this script BEFORE the room arrives, before that
--    migration has happened — it should report exactly 3 migrations applied
--    (unchanged since Session 14) and NO Enrollments table yet. That is the
--    correct, expected state to start the day in.
--
--    If you have already rehearsed today's migration and this script instead
--    reports 4 migrations and a real Enrollments table, that is ALSO fine —
--    it just means you ran Block 2 once already. Either state is a valid
--    starting point; the script tells you which one you're in.
--
-- Run this in SSMS before the session starts. Read every line of output.
-- ==============================================================================

USE [master];
GO

PRINT '=================================================';
PRINT '  VERIFYING SESSION 19 PRE-REQUISITES';
PRINT '=================================================';
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ITI_StudentPortal')
BEGIN
    PRINT 'Database ITI_StudentPortal       : MISSING ❌';
    PRINT '>> FIX: open the SESSION 14 console project, run Update-Database,';
    PRINT '>>      then re-run this script.';
    RAISERROR('Session 19 pre-requisite failed: database ITI_StudentPortal not found.', 16, 1);
END
ELSE
    PRINT 'Database ITI_StudentPortal       : FOUND ✅';
GO

USE [ITI_StudentPortal];
GO

-- ------------------------------------------------------------------------------
-- Migration history. Before Block 2 runs live: expect exactly 3 (unchanged
-- since Session 14). After Block 2 (or a rehearsal): expect exactly 4, the
-- fourth named AddEnrollment.
-- ------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    DECLARE @MigrationCount INT;
    SELECT @MigrationCount = COUNT(*) FROM [__EFMigrationsHistory];

    PRINT 'Migration history                : FOUND (' + CAST(@MigrationCount AS VARCHAR(10)) + ' applied)';

    IF @MigrationCount = 3
        PRINT '   ✅ Exactly 3 — today has NOT been migrated yet. This is the expected starting state.';
    ELSE IF @MigrationCount = 4
    BEGIN
        IF EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE MigrationId LIKE '%AddEnrollment')
            PRINT '   ✅ 4, and the 4th is AddEnrollment — you have already run Block 2''s migration (a rehearsal). Fine to teach from this state.';
        ELSE
            PRINT '   🔴 4 migrations, but the 4th is NOT AddEnrollment. Somebody migrated something unexpected — check before teaching.';
    END
    ELSE
        PRINT '   🔴 Expected 3 (not yet migrated) or 4 (already migrated). Investigate before teaching.';

    PRINT '';
    PRINT 'Full migration list:';
    SELECT MigrationId, ProductVersion FROM [__EFMigrationsHistory] ORDER BY MigrationId;
END
ELSE
    PRINT 'Migration history                : MISSING ❌';
GO

-- ------------------------------------------------------------------------------
-- Does the Enrollments table exist yet? Mirrors the migration count above.
-- ------------------------------------------------------------------------------
PRINT '';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Enrollments')
    PRINT 'dbo.Enrollments table             : EXISTS — today''s migration has already run.';
ELSE
    PRINT 'dbo.Enrollments table             : does not exist yet — correct, if teaching from a fresh state.';
GO

-- ------------------------------------------------------------------------------
-- The real roster, courses and instructors — Block 1's Warm-Up and Block 2's
-- console demo both name real rows. Confirm they still exist before teaching.
-- ------------------------------------------------------------------------------
PRINT '';
PRINT 'Student count and roster (Block 1''s Warm-Up references this):';
SELECT COUNT(*) AS StudentCount FROM Students;
SELECT Id, FullName, YearOfStudy, CAST(Gpa AS DECIMAL(4,2)) AS Gpa
FROM Students ORDER BY FullName;
GO

PRINT '';
PRINT 'Confirm "Nada Samir" still exists — Block 2''s console demo and Block 5''s';
PRINT 'payoff both enroll her by name:';
SELECT Id, FullName FROM Students WHERE FullName = 'Nada Samir';
IF NOT EXISTS (SELECT * FROM Students WHERE FullName = 'Nada Samir')
    PRINT '   🔴 NOT FOUND. Update the console project''s TODO 3 and this session''s demos to a real student name before teaching.';
GO

PRINT '';
PRINT 'Course count and list (CoursesController.Index shows all of these):';
SELECT COUNT(*) AS CourseCount FROM Courses;
SELECT Id, CourseName, Credits, InstructorId FROM Courses ORDER BY CourseName;
IF (SELECT COUNT(*) FROM Courses) < 4
    PRINT '   ⚠️ Fewer than 4 real courses exist. Any trainee whose COURSE_COUNT (Lab ID mod 3 = 2) needs 4 will be short — see Lab_19_Instructor_Notes.md.';
GO

PRINT '';
PRINT 'Confirm "Web Development Using .NET" still exists — Block 2/Block 5''s demos';
PRINT 'enroll Nada in this exact course:';
SELECT Id, CourseName FROM Courses WHERE CourseName = 'Web Development Using .NET';
IF NOT EXISTS (SELECT * FROM Courses WHERE CourseName = 'Web Development Using .NET')
    PRINT '   🔴 NOT FOUND. Update the demos to a real course name before teaching.';
GO

PRINT '';
PRINT 'Instructor count (for reference only — unchanged today):';
SELECT COUNT(*) AS InstructorCount FROM Instructors;
GO

-- ------------------------------------------------------------------------------
-- If Enrollments already exists (rehearsal), show what's in it, so Hamdy can
-- decide whether to clean it up before the real room arrives.
-- ------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Enrollments')
BEGIN
    PRINT '';
    PRINT 'Existing Enrollments rows (from a rehearsal, if any):';
    SELECT e.Id, s.FullName AS Student, c.CourseName AS Course, e.EnrollmentDate,
           CAST(e.Grade AS DECIMAL(4,2)) AS Grade
    FROM Enrollments e
    JOIN Students s ON s.Id = e.StudentId
    JOIN Courses c ON c.Id = e.CourseId
    ORDER BY e.Id;
    PRINT '(Rehearsal rows are harmless — today''s lecture and lab both ADD rows, never assume a specific starting count.)';
END
GO

PRINT '=================================================';
PRINT '  PreInit check complete. Nothing was modified.';
PRINT '=================================================';
GO
