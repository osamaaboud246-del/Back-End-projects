-- ==============================================================================
-- 🎓 Session 18 Pre-Initialization Script — VERIFY-ONLY
-- ITI Summer Training | Web Development Using .NET | Morning Group
-- ==============================================================================
-- ⚠️ Rule 38: Real, Persistent Environment Continuity
--
-- This script CREATES nothing, DROPS nothing and ALTERS nothing. It only reads.
--
-- 🔴 SESSION 18 IS THE QUIETEST SESSION OF THE WEEK, DATABASE-WISE:
--      · no schema change
--      · no migration, from any project
--      · no new rows
--      · not one line of C# in the controller or the model changes
--
--    Everything today happens in .cshtml files and one new TagHelpers/*.cs file.
--    If anybody suggests running Add-Migration today, the answer is no.
--
-- ⚠️ THE STUDENT COUNT WILL BE HIGHER THAN 4, AND THAT IS CORRECT.
--    Session 17's Create form inserted real students, its Block 5 F5 demonstration
--    inserted deliberate duplicates, and the lab inserted more. Write the number
--    down: Block 1 of today's lecture prints a live count on the roster page, and
--    it must agree with this.
--
-- Run this in SSMS before the session starts. Read every line of output.
-- ==============================================================================

USE [master];
GO

PRINT '=================================================';
PRINT '  VERIFYING SESSION 18 PRE-REQUISITES';
PRINT '=================================================';
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ITI_StudentPortal')
BEGIN
    PRINT 'Database ITI_StudentPortal       : MISSING ❌';
    PRINT '>> FIX: open the SESSION 14 console project, run Update-Database,';
    PRINT '>>      run it once to re-seed, then re-run this script.';
    RAISERROR('Session 18 pre-requisite failed: database ITI_StudentPortal not found.', 16, 1);
END
ELSE
    PRINT 'Database ITI_StudentPortal       : FOUND ✅';
GO

USE [ITI_StudentPortal];
GO

-- ------------------------------------------------------------------------------
-- Migration history. Expect exactly 3, unchanged since Session 14.
-- Sessions 15, 16, 17 and 18 all add zero.
-- ------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    DECLARE @MigrationCount INT;
    SELECT @MigrationCount = COUNT(*) FROM [__EFMigrationsHistory];

    PRINT 'Migration history                : FOUND (' + CAST(@MigrationCount AS VARCHAR(10)) + ' applied) ✅';

    IF @MigrationCount <> 3
        PRINT '   🔴 Expected exactly 3. Somebody migrated from a web project. Check before teaching.';
END
ELSE
    PRINT 'Migration history                : MISSING ❌';
GO

-- ------------------------------------------------------------------------------
-- The live roster. TODAY'S BLOCK 1 PRINTS THIS COUNT AND AVERAGE ON THE PAGE —
-- they must match. Write both down.
-- ------------------------------------------------------------------------------
PRINT '';
PRINT 'Roster count and average GPA — Block 1 prints these on screen:';
SELECT
    COUNT(*)                                  AS StudentCount,
    CAST(AVG(Gpa) AS DECIMAL(4,2))            AS AverageGpa
FROM Students;
GO

PRINT '';
PRINT 'The roster, ordered by name (what /students shows):';
SELECT Id, FullName, YearOfStudy, CAST(Gpa AS DECIMAL(4,2)) AS Gpa
FROM Students
ORDER BY FullName;
GO

-- ------------------------------------------------------------------------------
-- The three GPA bands today's tag helper colours. Same boundaries as Session 16's
-- HonourBandConstraint, deliberately — a green badge and /students/honours/first
-- must never disagree.
--
-- ⚠️ If any band is EMPTY, the payoff demo shows fewer colours than the guides
--    promise. That is not a bug, but know it before you say "green, blue and grey"
--    out loud to a room that can only see two of them.
-- ------------------------------------------------------------------------------
PRINT '';
PRINT 'Badge colours the roster will show (green / blue / grey):';
SELECT
    SUM(CASE WHEN Gpa >= 3.5                  THEN 1 ELSE 0 END) AS First_Green,
    SUM(CASE WHEN Gpa >= 3.0 AND Gpa < 3.5    THEN 1 ELSE 0 END) AS Second_Blue,
    SUM(CASE WHEN Gpa < 3.0                   THEN 1 ELSE 0 END) AS Pass_Grey
FROM Students;
GO

-- ------------------------------------------------------------------------------
-- Block 1 saves a student called <b>Nada</b> to demonstrate HTML encoding.
-- This finds any left over from a rehearsal, and the clean-up is below,
-- commented out. Leaving one is harmless and makes a good visible reminder.
-- ------------------------------------------------------------------------------
PRINT '';
PRINT 'Any encoding-demo students left from a rehearsal:';
SELECT Id, FullName FROM Students WHERE FullName LIKE '%<%';
GO

-- DELETE FROM Students WHERE FullName LIKE '%<%';

PRINT '=================================================';
PRINT '  PreInit check complete. Nothing was modified.';
PRINT '=================================================';
GO
