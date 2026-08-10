using System.Collections.Generic;
using System.Linq;

namespace StudentPortalConsole
{
	public static class StudentExtensions
	{
		public static IEnumerable<Student> MyTopStudents(this IEnumerable<Student> source)
		{
			return source.Where(s => s.Gpa >= 3.4);
		}
	}
}