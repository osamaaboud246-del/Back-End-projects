using System.ComponentModel.DataAnnotations;

namespace StudentPortalConsole
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public int YearOfStudy { get; set; }
        public double Gpa { get; set; }
        public int CreditsCompleted { get; set; }
    }
}