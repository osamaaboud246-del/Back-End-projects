-- ==============================================================================
-- 🎓 Session 15 Pre-Initialization Script — VERIFY-ONLY
-- ITI Summer Training | Web Development Using .NET | Morning Group
-- ==============================================================================
-- ⚠️ Rule 38: Real, Persistent Environment Continuity
--
-- This script CREATES nothing, DROPS nothing and ALTERS nothing. It only reads.
--
-- Session 15 expects `ITI_StudentPortalDB_EF` to already exist, already migrated
-- through Session 14's three migrations, and already holding its seeded rows.
-- Today's web application connects to that same database with that same
-- connection string, and only READS from it.
--
-- 🔴 SESSION 15 IS NOT THE MIGRATION OWNER.
--    The Session 14 CONSOLE project still owns this database's migration
--    history. The web project has no Migrations/ folder on purpose. Nobody —
--    instructor or trainee — runs Add-Migration or Update-Database from the web
--    project today. Doing so generates a migration with no history behind it,
--    which then tries to create tables that already exist.
--
-- Run this in SSMS before the session starts. Read every line of output.
-- ==============================================================================

USE [master];
GO

PRINT '=================================================';
PRINT '  VERIFYING SESSION 15 PRE-REQUISITES';
PRINT '=================================================';
GO

-- ------------------------------------------------------------------------------
-- 1. The database itself.
--    This check gets its own batch and a real error, not just a PRINT, because
--    every check below assumes the database exists. RETURN would only exit this
--    batch and execution would carry straight on into the USE statement below,
--    producing a cascade of unrelated errors instead of one clear message.
--    RAISERROR at severity 16 stops the script in SSMS.
-- ------------------------------------------------------------------------------
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ITI_StudentPortalDB_EF')
BEGIN
    PRINT 'Database ITI_StudentPortalDB_EF  : MISSING ❌';
    PRINT '';
    PRINT '>> STOP. Do not continue and hope.';
    PRINT '>> FIX: open the SESSION 14 console project, run Update-Database,';
    PRINT '>>      run it once to re-seed, then re-run this script.';
    PRINT '>> Every check below this line is meaningless until this one passes.';
    RAISERROR('Session 15 pre-requisite failed: database ITI_StudentPortalDB_EF not found.', 16, 1);
END
ELSE
    PRINT 'Database ITI_StudentPortalDB_EF  : FOUND ✅';
GO

USE [ITI_StudentPortalDB_EF];
GO

-- ------------------------------------------------------------------------------
-- 2. Migration history — proof this database was built by EF, and how far.
--    Expect 3 after Session 14: InitialCreate, AddStudentContraints,
--    AddInstructorCourseRelationship.
-- ------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    DECLARE @MigrationCount INT;
    SELECT @MigrationCount = COUNT(*) FROM [__EFMigrationsHistory];

    PRINT 'Migration history                : FOUND (' + CAST(@MigrationCount AS VARCHAR(10)) + ' applied) ✅';

    IF @MigrationCount < 3
        PRINT '   ⚠️  Expected 3 after Session 14. Run Update-Database from the CONSOLE project.';

    PRINT '   Applied migrations, oldest first:';
    SELECT MigrationId, ProductVersion FROM [__EFMigrationsHistory] ORDER BY MigrationId;
END
ELSE
    PRINT 'Migration history                : MISSING ❌ (__EFMigrationsHistory not found)';
GO

-- ------------------------------------------------------------------------------
-- 3. Tables and seed data.
--    An empty Students table is the single most likely reason today's live demo
--    looks broken: DI will work perfectly and hand the controller a context
--    pointing at nothing. Catch it here, not in front of the room.
-- ------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
BEGIN
    DECLARE @StudentCount INT;
    SELECT @StudentCount = COUNT(*) FROM Students;

    IF @StudentCount > 0
        PRINT 'Table Students                   : FOUND (' + CAST(@StudentCount AS VARCHAR(10)) + ' row(s)) ✅'
    ELSE
    BEGIN
        PRINT 'Table Students                   : FOUND, but EMPTY ⚠️';
        PRINT '   >> Today''s page will render an empty table and look broken.';
        PRINT '   >> FIX: run the Session 14 Complete project once to re-seed.';
    END
END
ELSE
    PRINT 'Table Students                   : MISSING ❌';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Courses')
    PRINT 'Table Courses                    : FOUND ✅'
ELSE
    PRINT 'Table Courses                    : MISSING ❌';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Instructors')
    PRINT 'Table Instructors                : FOUND ✅'
ELSE
    PRINT 'Table Instructors                : MISSING ❌';
GO

-- ------------------------------------------------------------------------------
-- 4. Session 14's relationship survived.
--    Courses.InstructorId is the foreign key Session 14 Block 3 introduced.
--    Today's page does not use it, but its absence means the database is not at
--    Session 14's end state, which changes what the trainees will see.
-- ------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.columns
           WHERE object_id = OBJECT_ID('Courses') AND name = 'InstructorId')
    PRINT 'Courses.InstructorId FK column   : FOUND ✅ (Session 14 Block 3 intact)'
ELSE
    PRINT 'Courses.InstructorId FK column   : MISSING ❌ (database is behind Session 14)';
GO

-- ------------------------------------------------------------------------------
-- 5. Exactly what today's page should render, so you can compare it against the
--    browser. Same query, same ordering, as the controller's Index action.
-- ------------------------------------------------------------------------------
PRINT '';
PRINT 'What today''s home page should show, in this order:';
SELECT FullName, YearOfStudy, CAST(Gpa AS DECIMAL(4,2)) AS Gpa
FROM Students
ORDER BY FullName;
GO

PRINT '=================================================';
PRINT '  PreInit check complete. Nothing was modified.';
PRINT '=================================================';
GO
