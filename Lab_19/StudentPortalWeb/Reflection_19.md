# Reflection 19 — Osama Aboud (Lab ID: 29)

1. Values:
   - Lab ID: 29
   - MIN_GRADE_LAB: 1.0 + (29 mod 4) * 0.5 = 1.0 + (1 * 0.5) = 1.5
   - COURSE_COUNT: (29 mod 3) + 2 = 2 + 2 = 4

2. Three places data can be rejected:
   - Client-side validation (HTML5 / validation scripts in the browser).
   - Server-side validation (ModelState in the controller, 
     where my Part D change lives).
   - Database level (SQL Server constraints, like the UNIQUE index).

3. Connecting Student and Course directly with just a foreign key fails 
   because it limits the relationship to one-to-many 
   (e.g., a student can only take one course). To fix this, 
   a junction table (Enrollment) is required to hold two foreign keys,
   properly establishing a many-to-many relationship.

4. I would choose `Cascade` delete for an `Assignment` belonging to a `Course`. 
   If a Course is completely deleted from the system, all of its specific assignments 
   should logically be destroyed along with it, as they have no standalone value or purpose 
   without their parent course.
