-- ==============================================================================
-- 🎓 Session 17 Pre-Initialization Script — VERIFY-ONLY
-- ITI Summer Training | Web Development Using .NET | Morning Group
-- ==============================================================================
-- ⚠️ Rule 38: Real, Persistent Environment Continuity
--
-- This script CREATES nothing, DROPS nothing and ALTERS nothing. It only reads.
--
-- 🔴 SESSION 17 CHANGES NO SCHEMA. Not one migration, from any project. The
--    Session 14 CONSOLE project remains the migration owner, exactly as in
--    Sessions 15 and 16.
--
--    Today's new attributes are [Range(...)], which is a VALIDATION-only
--    attribute. Entity Framework does not map it to a column, so it changes
--    nothing about the database. Compare with [Required] and [MaxLength], which
--    ARE mapped and were migrated back in Session 14. Block 4 of the lecture
--    makes that distinction explicitly, because getting it wrong produces a
--    migration nobody intended.
--
-- ⚠️ TODAY DOES ADD ROWS, AND THAT IS THE POINT.
--    The whole session builds a form that inserts real students, and the lab
--    edits real instructors. Expect the Students table to grow by roughly one
--    row per trainee, plus the duplicates deliberately created in Block 5's
--    F5 demonstration.
--
--    Section 5 below holds a CLEAN-UP statement, commented out, for AFTER the
--    session. Read it before you run it.
--
-- Run this in SSMS before the session starts. Read every line of output.
-- ==============================================================================

USE [master];
GO

PRINT '=================================================';
PRINT '  VERIFYING SESSION 17 PRE-REQUISITES';
PRINT '=================================================';
GO

-- ------------------------------------------------------------------------------
-- 1. The database itself.
-- ------------------------------------------------------------------------------
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ITI_StudentPortal')
BEGIN
    PRINT 'Database ITI_StudentPortal       : MISSING ❌';
    PRINT '';
    PRINT '>> STOP. Do not continue and hope.';
    PRINT '>> FIX: open the SESSION 14 console project, run Update-Database,';
    PRINT '>>      run it once to re-seed, then re-run this script.';
    RAISERROR('Session 17 pre-requisite failed: database ITI_StudentPortal not found.', 16, 1);
END
ELSE
    PRINT 'Database ITI_StudentPortal       : FOUND ✅';
GO

USE [ITI_StudentPortal];
GO

-- ------------------------------------------------------------------------------
-- 2. Migration history. Expect 3, unchanged since Session 14.
--    Sessions 15, 16 and 17 all add zero. If this is more than 3, somebody ran
--    Add-Migration from the web project and that needs sorting out before the
--    session, not during it.
-- ------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    DECLARE @MigrationCount INT;
    SELECT @MigrationCount = COUNT(*) FROM [__EFMigrationsHistory];

    PRINT 'Migration history                : FOUND (' + CAST(@MigrationCount AS VARCHAR(10)) + ' applied) ✅';

    IF @MigrationCount <> 3
        PRINT '   🔴 Expected exactly 3. Stop and check before teaching.';

    SELECT MigrationId, ProductVersion FROM [__EFMigrationsHistory] ORDER BY MigrationId;
END
ELSE
    PRINT 'Migration history                : MISSING ❌';
GO

-- ------------------------------------------------------------------------------
-- 3. Row counts BEFORE the session. Write these two numbers down.
--    The difference afterwards is exactly what the room created.
-- ------------------------------------------------------------------------------
PRINT '';
PRINT 'Row counts BEFORE today (write these down):';
SELECT
    (SELECT COUNT(*) FROM Students)    AS StudentsBefore,
    (SELECT COUNT(*) FROM Instructors) AS InstructorsBefore,
    (SELECT COUNT(*) FROM Courses)     AS CoursesBefore;
GO

-- ------------------------------------------------------------------------------
-- 4. The two tables today actually touches.
--
--    STUDENTS — the lecture's Create form inserts into this one.
--    INSTRUCTORS — the LAB's Edit form updates this one. Every trainee needs at
--    least one instructor row to edit, so if this comes back empty the lab
--    cannot start. That is the single most likely blocker today: check it now.
-- ------------------------------------------------------------------------------
PRINT '';
PRINT '--- Students (what /students should show, ordered by name) ---';
SELECT Id, FullName, YearOfStudy, CAST(Gpa AS DECIMAL(4,2)) AS Gpa
FROM Students
ORDER BY FullName;
GO

PRINT '';
PRINT '--- Instructors (what the LAB edits — must not be empty) ---';
SELECT Id, FullName, YearsOfExperience
FROM Instructors
ORDER BY FullName;
GO

IF NOT EXISTS (SELECT 1 FROM Instructors)
BEGIN
    PRINT '   🔴 INSTRUCTORS TABLE IS EMPTY — THE LAB CANNOT RUN.';
    PRINT '   >> FIX: run the Session 14 Complete project once, which seeds';
    PRINT '   >>      instructors, then re-run this script.';
END
GO

-- ------------------------------------------------------------------------------
-- 5. AFTER THE SESSION — clean-up, deliberately commented out.
--
--    Read this before running it. It deletes students created TODAY only, by
--    Id, above the highest Id that existed before the session. Substitute the
--    real number from section 3 for <LAST_ID_BEFORE_TODAY>.
--
--    ⚠️ It does NOT reset the IDENTITY seed, and that is deliberate: gaps in Id
--    numbers are normal and harmless, and resetting the seed on a table that
--    other tables reference is how you create two rows with the same Id six
--    months later. If you genuinely want the seed reset, it is
--    DBCC CHECKIDENT ('Students', RESEED, <n>) — and think twice first.
--
--    ⚠️ Do NOT delete instructors. The Courses table has a foreign key to them
--    with DeleteBehavior.Restrict (Session 14), so the delete would be refused —
--    correctly. The lab only ever UPDATES instructors, never inserts them.
-- ------------------------------------------------------------------------------

-- DELETE FROM Students WHERE Id > <LAST_ID_BEFORE_TODAY>;
-- SELECT COUNT(*) AS StudentsAfterCleanup FROM Students;

PRINT '=================================================';
PRINT '  PreInit check complete. Nothing was modified.';
PRINT '=================================================';
GO
