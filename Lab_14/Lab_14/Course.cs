using System.ComponentModel.DataAnnotations;

namespace StudentPortalConsole
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string CourseName { get; set; } = string.Empty;

        public int Credits { get; set; }

        public int? InstructorId { get; set; }
        public Instructor? Instructor { get; set; }
    }
}