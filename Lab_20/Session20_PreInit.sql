-- =====================================================================
--  Session20_PreInit.sql  —  VERIFY-ONLY. THIS SCRIPT CHANGES NOTHING.
--  ITI Summer Training | Web Development Using .NET | Morning Group
--  Session 20 — ASP.NET Core Razor Pages
--
--  ⚠️ READ THIS HEADER. It behaves DIFFERENTLY from earlier PreInit
--     scripts, and the difference matters.
--
--     Sessions up to 14 shipped PreInit scripts that could CREATE or
--     RESET state. This one does neither, and nor did Session 19's.
--     Style Guide Rule 38 (Real, Persistent Environment Continuity)
--     applies: ITI_StudentPortal is a REAL database the room has been
--     inserting into since Session 15, and nothing may drop, reseed or
--     reconcile it.
--
--  ⚠️ SESSION 20 CREATES NO MIGRATION AND NO SCHEMA CHANGE OF ANY KIND.
--     No new table, no new column, no new index. Razor Pages is a way of
--     SERVING data. It has nothing to say about what the data is.
--
--     So this script is genuinely optional. Run it only if you want
--     reassurance that the database is exactly as Session 19 left it.
--     Nothing in the session depends on running it.
--
--  HOW TO RUN: open in SSMS, connect to  .  (local), press F5.
--              Read the four result sets. Compare against the notes.
-- =====================================================================

SET NOCOUNT ON;

-- ---------------------------------------------------------------------
-- 1. Does the database exist at all?
--    Expected: exactly one row, named ITI_StudentPortal.
--    If zero rows: something is very wrong — the room's whole project
--    from Session 15 onward points at this database. Do NOT create it
--    from this script. Stop and investigate.
-- ---------------------------------------------------------------------
SELECT
    N'1. DATABASE'                          AS [Check],
    name                                    AS [Value],
    CASE WHEN state_desc = N'ONLINE'
         THEN N'OK — online'
         ELSE N'PROBLEM — ' + state_desc END AS [Verdict]
FROM sys.databases
WHERE name = N'ITI_StudentPortal';

USE ITI_StudentPortal;
GO

-- ---------------------------------------------------------------------
-- 2. Which migrations are actually applied?
--    Expected: FOUR rows, ending with ..._AddEnrollment
--      20260728095231_InitialCreate
--      20260729072536_AddStudentContraints      (sic — the room's spelling)
--      20260729082109_AddInstructorCourseRelationship
--      20260805064111_AddEnrollment             (Session 19, Aug 5 2026)
--
--    ⚠️ NO FIFTH ROW SHOULD APPEAR TODAY, before or after the session.
--       If one does, someone ran Add-Migration when they should not have.
-- ---------------------------------------------------------------------
SELECT
    N'2. MIGRATIONS' AS [Check],
    MigrationId      AS [Value],
    ProductVersion   AS [EF Core]
FROM __EFMigrationsHistory
ORDER BY MigrationId;

-- ---------------------------------------------------------------------
-- 3. Row counts.
--    There is no "expected" number here — the room has been inserting
--    real rows since Session 15 and will insert more in today's lab.
--    What matters is that Enrollments has AT LEAST ONE row, because
--    Block 5's payoff needs a student who actually has a course.
-- ---------------------------------------------------------------------
SELECT N'3. ROWS' AS [Check], N'Students'    AS [Table], COUNT(*) AS [N] FROM dbo.Students
UNION ALL
SELECT N'3. ROWS',            N'Courses',              COUNT(*) FROM dbo.Courses
UNION ALL
SELECT N'3. ROWS',            N'Instructors',          COUNT(*) FROM dbo.Instructors
UNION ALL
SELECT N'3. ROWS',            N'Enrollments',          COUNT(*) FROM dbo.Enrollments;

-- ---------------------------------------------------------------------
-- 4. WHICH STUDENT SHOULD YOU DEMO WITH?
--    This is the one genuinely useful thing in this script.
--    Block 5 needs a student who HAS an enrollment. Take the top row of
--    this result set, write the StudentId on the board, and use it all
--    morning — Warm-Up, Block 4, and Block 5's three tabs.
--
--    If this returns ZERO rows, the room has no enrollments and Block 5
--    has nothing to show. In that case, add one by hand before class:
--      INSERT INTO dbo.Enrollments (StudentId, CourseId, EnrollmentDate, Grade)
--      VALUES (<a real student id>, <a real course id>, GETDATE(), NULL);
--    (Kept as a comment deliberately. Do not run it blind.)
-- ---------------------------------------------------------------------
SELECT TOP (10)
    N'4. DEMO ROW'      AS [Check],
    s.Id                AS [StudentId],
    s.FullName          AS [Student],
    c.Id                AS [CourseId],
    c.CourseName        AS [Course],
    e.EnrollmentDate,
    e.Grade
FROM dbo.Enrollments e
JOIN dbo.Students s ON s.Id = e.StudentId
JOIN dbo.Courses  c ON c.Id = e.CourseId
ORDER BY e.Id;

-- ---------------------------------------------------------------------
-- 5. The composite unique index from Session 19 — still there?
--    Expected: one row with is_unique = 1 covering (StudentId, CourseId).
--    Nothing today touches it. This is confirmation, not a requirement.
-- ---------------------------------------------------------------------
SELECT
    N'5. UNIQUE INDEX' AS [Check],
    i.name             AS [Index],
    i.is_unique        AS [IsUnique],
    i.type_desc        AS [Type]
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID(N'dbo.Enrollments')
  AND i.is_unique = 1;
GO

-- =====================================================================
--  END. Nothing above modified anything.
-- =====================================================================
