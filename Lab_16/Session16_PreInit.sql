-- ==============================================================================
-- 🎓 Session 16 Pre-Initialization Script — VERIFY-ONLY
-- ITI Summer Training | Web Development Using .NET | Morning Group
-- ==============================================================================
-- ⚠️ Rule 38: Real, Persistent Environment Continuity
--
-- This script CREATES nothing, DROPS nothing and ALTERS nothing. It only reads.
--
-- 🔴 SESSION 16 CHANGES ZERO SCHEMA. Routing is decided entirely in C#, before
--    a controller exists and long before a database connection does. There is no
--    migration today, from any project. The Session 14 CONSOLE project remains
--    the owner of this database's migration history, exactly as in Session 15.
--
-- ==============================================================================
-- 📌 TWO RECONCILIATION NOTES FOR HAMDY — please confirm or correct (Rule 32)
-- ==============================================================================
--
-- (1) THE DATABASE NAME WAS WRONG IN SESSIONS 13-15 AND HAS BEEN CORRECTED.
--     Your real, live classroom code — the Session 14 console project that owns
--     the migrations, and the Session 15 web project you typed in front of the
--     room — both connect to:
--
--         ITI_StudentPortal
--
--     The Session 13/14/15 _Complete projects, Session15_PreInit.sql and 16
--     places across those sessions' guides said ITI_StudentPortalDB_EF, which is
--     a database that the migration history does not live in. Session 16 follows
--     the live code. The earlier files have been corrected to match.
--
-- (2) TWO VALUES THIS SESSION'S EXPECTED-OUTPUT BLOCKS ASSUME.
--     Section 5 below prints the REAL values. If either disagrees with what the
--     guides say, the guides are wrong and the database is right — tell me and
--     I will correct them, do not adjust the database.
--
--       (a) Nada Samir's GPA is assumed to be 4.00, because Session 14's live
--           classroom code changed it from 3.9 and saved. If it is still 3.90,
--           NOTHING in today's demos breaks: 3.90 and 4.00 both land in the
--           "first" honours band, so /students/honours/first shows the same two
--           students either way. Only the printed GPA differs.
--
--       (b) The student used in the /students/3 demo is assumed to be the row
--           with Id = 3. Section 5 prints the real Id for every student. If your
--           Ids differ, use the real one and say that number aloud instead — no
--           other part of the session depends on it.
--
-- Run this in SSMS before the session starts. Read every line of output.
-- ==============================================================================

USE [master];
GO

PRINT '=================================================';
PRINT '  VERIFYING SESSION 16 PRE-REQUISITES';
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
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ITI_StudentPortal')
BEGIN
    PRINT 'Database ITI_StudentPortal       : MISSING ❌';
    PRINT '';
    PRINT '>> STOP. Do not continue and hope.';
    PRINT '>> FIX: open the SESSION 14 console project, run Update-Database,';
    PRINT '>>      run it once to re-seed, then re-run this script.';
    PRINT '>> Every check below this line is meaningless until this one passes.';
    RAISERROR('Session 16 pre-requisite failed: database ITI_StudentPortal not found.', 16, 1);
END
ELSE
    PRINT 'Database ITI_StudentPortal       : FOUND ✅';
GO

USE [ITI_StudentPortal];
GO

-- ------------------------------------------------------------------------------
-- 2. Migration history — proof this database was built by EF, and how far.
--    Expect 3 after Session 14: InitialCreate, AddStudentContraints,
--    AddInstructorCourseRelationship. Session 15 added none. Session 16 adds
--    none. If this number is not 3, something migrated that should not have.
-- ------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    DECLARE @MigrationCount INT;
    SELECT @MigrationCount = COUNT(*) FROM [__EFMigrationsHistory];

    PRINT 'Migration history                : FOUND (' + CAST(@MigrationCount AS VARCHAR(10)) + ' applied) ✅';

    IF @MigrationCount < 3
        PRINT '   ⚠️  Expected 3 after Session 14. Run Update-Database from the CONSOLE project.';

    IF @MigrationCount > 3
        PRINT '   🔴 More than 3. Somebody ran Add-Migration from the WEB project. Stop and check.';

    PRINT '   Applied migrations, oldest first:';
    SELECT MigrationId, ProductVersion FROM [__EFMigrationsHistory] ORDER BY MigrationId;
END
ELSE
    PRINT 'Migration history                : MISSING ❌ (__EFMigrationsHistory not found)';
GO

-- ------------------------------------------------------------------------------
-- 3. The Students table and its rows.
--    An empty Students table is the single most likely reason today's live demos
--    look broken: every route will match perfectly and every page will render an
--    empty table. Catch it here, not in front of the room.
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
        PRINT '   >> Every route today will match and every page will look blank.';
        PRINT '   >> FIX: run the Session 14 Complete project once to re-seed.';
    END
END
ELSE
    PRINT 'Table Students                   : MISSING ❌';
GO

-- ------------------------------------------------------------------------------
-- 4. Every academic year today's by-year route can reach.
--    The route accepts years 1 through 4. Any year in that range with zero rows
--    will render an empty (but correct) page — worth knowing BEFORE you type it
--    in front of the room and it looks like the route failed.
-- ------------------------------------------------------------------------------
PRINT '';
PRINT 'Rows per academic year (the route accepts 1-4 only):';
SELECT YearOfStudy, COUNT(*) AS StudentCount
FROM Students
GROUP BY YearOfStudy
ORDER BY YearOfStudy;
GO

-- ------------------------------------------------------------------------------
-- 5. Exactly what each of today's routes should render, so the browser can be
--    checked against SSMS. Same queries, same ordering, as the actions.
-- ------------------------------------------------------------------------------
PRINT '';
PRINT '--- /students  (ordered by FullName, as the Index action orders it) ---';
SELECT Id, FullName, YearOfStudy, CAST(Gpa AS DECIMAL(4,2)) AS Gpa
FROM Students
ORDER BY FullName;
GO

PRINT '';
PRINT '--- /students/3  (whichever student really has Id 3 — see note 2b) ---';
SELECT Id, FullName, YearOfStudy, CAST(Gpa AS DECIMAL(4,2)) AS Gpa
FROM Students
WHERE Id = 3;
GO

PRINT '';
PRINT '--- /students/year/2 ---';
SELECT Id, FullName, YearOfStudy, CAST(Gpa AS DECIMAL(4,2)) AS Gpa
FROM Students
WHERE YearOfStudy = 2
ORDER BY FullName;
GO

PRINT '';
PRINT '--- /students/honours/first   (Gpa >= 3.5) ---';
SELECT Id, FullName, CAST(Gpa AS DECIMAL(4,2)) AS Gpa
FROM Students
WHERE Gpa >= 3.5
ORDER BY FullName;
GO

PRINT '';
PRINT '--- /students/honours/second  (3.0 <= Gpa < 3.5) ---';
SELECT Id, FullName, CAST(Gpa AS DECIMAL(4,2)) AS Gpa
FROM Students
WHERE Gpa >= 3.0 AND Gpa < 3.5
ORDER BY FullName;
GO

PRINT '';
PRINT '--- /students/honours/pass    (Gpa < 3.0) ---';
SELECT Id, FullName, CAST(Gpa AS DECIMAL(4,2)) AS Gpa
FROM Students
WHERE Gpa < 3.0
ORDER BY FullName;
GO

PRINT '';
PRINT '--- /students/search?name=a   (FullName LIKE %a%, as Contains translates) ---';
SELECT Id, FullName, CAST(Gpa AS DECIMAL(4,2)) AS Gpa
FROM Students
WHERE FullName LIKE '%a%'
ORDER BY FullName;
GO

PRINT '=================================================';
PRINT '  PreInit check complete. Nothing was modified.';
PRINT '=================================================';
GO
