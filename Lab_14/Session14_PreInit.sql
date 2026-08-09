-- =====================================================================
-- Session14_PreInit.sql
-- ITI Summer Training | Web Development Using .NET | Morning Group
-- Session 14 — EF Core: CRUD, Relationships, and Loading
--
-- STYLE GUIDE RULE 32 — Pre-Session Initialization Script.
--
-- 🔴 CRITICAL DIFFERENCE FROM SESSION 13'S PREINIT:
--    THIS SCRIPT IS READ-ONLY. It creates nothing, drops nothing, and
--    alters nothing.
--
--    Session 13's PreInit deliberately DROPPED ITI_StudentPortalDB_EF,
--    so that Update-Database would visibly create it in front of the
--    room. Doing that today would destroy the entire point of Session
--    14: every migration generated today lands on a table that ALREADY
--    HAS ROWS IN IT. That is what makes Blocks 2 and 3 meaningful —
--    AlterColumn against real data can fail in ways CreateTable never
--    can, and proving the data survives an additive migration is a
--    graded part of the Lab.
--
--    This also honours the migrate-forward-never-recreate principle:
--    a database with real data is brought forward by migrations, not
--    rebuilt from scratch.
--
-- WHAT THIS SCRIPT DOES: verifies that Session 13 left the environment
--   in the state Session 14 assumes, and reports anything missing
--   loudly enough to act on BEFORE the session starts.
--
-- ⚠️ HONEST LIMITATION: written against Microsoft's T-SQL documentation
--   and reviewed by hand. NOT executed against a real SQL Server — there
--   is none reachable from the authoring environment. Run it yourself
--   from Chapter 0 of the Instructor Guide, well before the session.
--
-- HOW TO RUN: open in SSMS, connect to the same server as Sessions 3-4
--   and 13, press Execute (F5). Read the Messages pane.
-- =====================================================================

SET NOCOUNT ON;
GO

USE master;
GO

PRINT '=====================================================';
PRINT ' Session 14 PreInit — VERIFY ONLY (nothing is changed)';
PRINT '=====================================================';
GO

-- ---------------------------------------------------------------------
-- STEP 1 — The database Session 13 created must still exist.
-- ---------------------------------------------------------------------
IF DB_ID(N'ITI_StudentPortalDB_EF') IS NULL
BEGIN
    PRINT '*** STOP ***';
    PRINT 'Database ITI_StudentPortalDB_EF        : NOT FOUND';
    PRINT '  Session 14 cannot run without it. Session 13 created it via';
    PRINT '  Add-Migration InitialCreate + Update-Database.';
    PRINT '  FIX: open Session 13''s project and run Update-Database, then';
    PRINT '       seed it, then re-run this script.';
    PRINT '  DO NOT run Session 13''s PreInit script first — it drops this';
    PRINT '       database, which is the opposite of what you need today.';
END
ELSE
BEGIN
    PRINT 'Database ITI_StudentPortalDB_EF        : FOUND';
END
GO

-- Everything below only makes sense if the database exists.
IF DB_ID(N'ITI_StudentPortalDB_EF') IS NULL
BEGIN
    PRINT 'Skipping remaining checks — database missing.';
    RETURN;
END
GO

USE ITI_StudentPortalDB_EF;
GO

-- ---------------------------------------------------------------------
-- STEP 2 — The three tables from Session 13's InitialCreate.
--          Students must also contain rows: an empty table makes
--          Blocks 2 and 3 prove nothing about data survival.
-- ---------------------------------------------------------------------
DECLARE @studentRows INT = 0;

IF OBJECT_ID(N'dbo.Students', N'U') IS NOT NULL
BEGIN
    SELECT @studentRows = COUNT(*) FROM dbo.Students;

    IF @studentRows > 0
        PRINT 'Table Students                          : FOUND ('
              + CAST(@studentRows AS VARCHAR(10)) + ' row(s))';
    ELSE
    BEGIN
        PRINT '*** WARNING ***';
        PRINT 'Table Students                          : FOUND but EMPTY';
        PRINT '  Today''s Blocks 2 and 3 migrate a table that is supposed to';
        PRINT '  already hold data — that is the whole lesson. With zero rows,';
        PRINT '  AlterColumn cannot demonstrate a conflict and the Lab cannot';
        PRINT '  show data surviving an additive migration.';
        PRINT '  FIX: run Session 13''s Complete project once to seed the four';
        PRINT '       students (Yara Adel, Omar Hesham, Nada Samir, Kareem Fouad).';
    END
END
ELSE
BEGIN
    PRINT '*** STOP ***';
    PRINT 'Table Students                          : NOT FOUND';
    PRINT '  Session 13''s InitialCreate migration was never applied.';
END
GO

IF OBJECT_ID(N'dbo.Courses', N'U') IS NOT NULL
    PRINT 'Table Courses                           : FOUND';
ELSE
    PRINT '*** STOP *** Table Courses              : NOT FOUND';
GO

IF OBJECT_ID(N'dbo.Instructors', N'U') IS NOT NULL
    PRINT 'Table Instructors                       : FOUND';
ELSE
    PRINT '*** STOP *** Table Instructors          : NOT FOUND';
GO

-- ---------------------------------------------------------------------
-- STEP 3 — Migration history. Today ADDS to this; it must already exist.
-- ---------------------------------------------------------------------
IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NOT NULL
BEGIN
    DECLARE @migrations INT;
    SELECT @migrations = COUNT(*) FROM dbo.__EFMigrationsHistory;
    PRINT 'Migration history                       : '
          + CAST(@migrations AS VARCHAR(10)) + ' migration(s) applied';
END
ELSE
BEGIN
    PRINT '*** STOP ***';
    PRINT 'Migration history                       : NOT FOUND';
    PRINT '  This database was not created by EF migrations. Today''s';
    PRINT '  Add-Migration will not know what state to compare against.';
END
GO

-- ---------------------------------------------------------------------
-- STEP 4 — AssignedCourseName should still be PRESENT.
--          Block 3 deletes it and replaces it with a real foreign key.
--          If it is already gone, someone has run today's migration.
-- ---------------------------------------------------------------------
IF COL_LENGTH(N'dbo.Instructors', N'AssignedCourseName') IS NOT NULL
BEGIN
    PRINT 'Instructors.AssignedCourseName          : PRESENT (expected — Block 3 removes it)';
END
ELSE
BEGIN
    PRINT '*** WARNING ***';
    PRINT 'Instructors.AssignedCourseName          : ALREADY GONE';
    PRINT '  Block 3''s migration appears to have been applied already —';
    PRINT '  probably from a rehearsal run. The live demo will still work,';
    PRINT '  but Add-Migration will generate an empty migration because';
    PRINT '  nothing has changed.';
    PRINT '  FIX: roll back to Session 13''s state before the session with';
    PRINT '       Update-Database InitialCreate, then Remove-Migration.';
END
GO

-- ---------------------------------------------------------------------
-- STEP 5 — Courses.InstructorId should NOT exist yet (Block 3 adds it).
-- ---------------------------------------------------------------------
IF COL_LENGTH(N'dbo.Courses', N'InstructorId') IS NULL
    PRINT 'Courses.InstructorId                    : ABSENT (expected — Block 3 adds it)';
ELSE
    PRINT 'Courses.InstructorId                    : ALREADY PRESENT (see the warning above)';
GO

PRINT '=====================================================';
PRINT ' PreInit complete. Ready for Session 14.';
PRINT ' Nothing was created, dropped, or altered by this script.';
PRINT '=====================================================';
GO

USE ITI_StudentPortalDB_EF;
GO

INSERT INTO Students (FullName, YearOfStudy, Gpa, CreditsCompleted)
VALUES 
(N'Nada Samir', 3, 3.2, 90),
(N'Ahmed Ali', 2, 3.5, 60),
(N'Mona Hassan', 4, 3.8, 120),
(N'Kareem Omar', 1, 2.9, 30);


USE ITI_StudentPortalDB_EF;
GO


IF NOT EXISTS (SELECT 1 FROM Instructors WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT Instructors ON;
    INSERT INTO Instructors (Id, FullName) VALUES (1, N'Hamdy');
    SET IDENTITY_INSERT Instructors OFF;
END


IF NOT EXISTS (SELECT 1 FROM Courses WHERE CourseName LIKE N'%Web Development%')
BEGIN
    INSERT INTO Courses (CourseName, Credits, InstructorId) 
    VALUES (N'Web Development Using .NET', 3, NULL);
END

-- =====================================================================
-- AFTER THE SESSION
--
-- Session 15 builds an ASP.NET Core MVC application against this same
-- database, with the relationship Block 3 creates today. Do not drop or
-- recreate it — Session 15 continues migrating it forward.
-- =====================================================================
