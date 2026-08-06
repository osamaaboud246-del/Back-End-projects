namespace StudentPortalConsole
{
    public class Instructor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;

        // AssignedCourseName deleted as required in Part E
        public List<Course> Courses { get; set; } = new();
    }
}