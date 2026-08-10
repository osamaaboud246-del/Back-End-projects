// =====================================================================
// StudentLookupTool — SESSION PROJECT (Style Guide Rule 20/35/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 21 — Block 8: AI Agents & Agentic Workflows (the payoff)
//
// A precise tool: an exact database lookup, not a similarity search.
// =====================================================================
namespace StudentPortalAI
{
    public class StudentLookupTool : ITool
    {
        private readonly StudentPortalReader _reader;

        public string Name => "student_lookup";
        public string Description => "Looks up ONE named student's exact year and GPA by full name.";

        public StudentLookupTool(StudentPortalReader reader)
        {
            _reader = reader;
        }

        // Populated at startup from the real database (see Program.cs)
        // with every real student's name currently in ITI_StudentPortal.
        public List<string> KnownStudentNames { get; set; } = new();

        // TODO 5 (part one): Implement CanHandle(question). Return true
        //         if the question text contains any of the words
        //         "student", "honour", "honor", or "gpa" (case
        //         shouldn't matter). This is the agent's routing rule —
        //         it decides WHETHER this tool is even a candidate,
        //         before Execute ever runs.

        public bool CanHandle(string question)
        {
            return question.Contains("student", StringComparison.OrdinalIgnoreCase) ||
                   question.Contains("honour", StringComparison.OrdinalIgnoreCase) ||
                   question.Contains("honor", StringComparison.OrdinalIgnoreCase) ||
                   question.Contains("gpa", StringComparison.OrdinalIgnoreCase);
        }

        // TODO 5 (part two): Implement Execute(question). Loop through
        //         KnownStudentNames and check whether the question
        //         contains that exact name. On the first match, call the
        //         reader's FindStudentByName with that name. If found,
        //         build and return the same sentence shape
        //         StudentPortalReader already uses for a student
        //         document (name, year, GPA to two decimal places, and
        //         the honour band from StudentPortalReader.HonourBandFor).
        //         If the loop finishes with no match at all, return a
        //         short sentence saying no matching student name was
        //         found in the question.

        public string Execute(string question)
        {
            foreach (var candidate in KnownStudentNames)
            {
                if (question.Contains(candidate, StringComparison.OrdinalIgnoreCase))
                {
                    var (found, fullName, yearOfStudy, gpa) = _reader.FindStudentByName(candidate);
                    if (found)
                    {
                        var band = StudentPortalReader.HonourBandFor(gpa);
                        return $"{fullName} is a Year {yearOfStudy} student with a GPA of {gpa:F2}, which places them in the {band} honour band.";
                    }
                }
            }
            return $"No matching student name was found in the question.";
        }

        #region 📋 Full TODO Checklist
        // TODO 5 (part one): CanHandle(question) — keyword check.
        // TODO 5 (part two): Execute(question) — find the named student,
        //         look them up for real, build the sentence.
        #endregion
    }
}
