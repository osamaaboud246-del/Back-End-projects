// =====================================================================
// CourseLookupTool — SESSION PROJECT (Style Guide Rule 20/35/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 21 — Block 8: AI Agents & Agentic Workflows
//
// The second tool — same shape as StudentLookupTool by design, so
// building this one is recognition, not new learning.
// =====================================================================
namespace StudentPortalAI
{
    public class CourseLookupTool : ITool
    {
        private readonly List<KnowledgeDocument> _courseDocuments;

        public string Name => "course_lookup";
        public string Description => "Looks up ONE named course's credits and instructor.";

        public CourseLookupTool(List<KnowledgeDocument> courseDocuments)
        {
            _courseDocuments = courseDocuments;
        }

        // TODO 6: Implement BOTH CanHandle and Execute for this tool,
        //         mirroring TODO 5 exactly. CanHandle: true if the
        //         question contains "course", "credit", or "instructor".
        //         Execute: loop through _courseDocuments, and for each
        //         one, take the part of its Text before " is a " as the
        //         course's name (string splitting you already used
        //         earlier this course) — if the question contains that
        //         name, return that document's whole Text as the answer.
        //         If nothing matches after the loop, return a short
        //         sentence saying no matching course name was found.

        public bool CanHandle(string question)
        {
            return question.Contains("course", StringComparison.OrdinalIgnoreCase) ||
                   question.Contains("credit", StringComparison.OrdinalIgnoreCase) ||
                   question.Contains("instructor", StringComparison.OrdinalIgnoreCase);
        }

        public string Execute(string question)
        {
            foreach (var doc in _courseDocuments)
            {
                var courseName = doc.Text.Split(" is a ")[0]; // C#  is a 3-credit course
                if (question.Contains(courseName,StringComparison.OrdinalIgnoreCase))
                {
                    return doc.Text;
                }
            }
            return "No matching course name was found.";
        }

        #region 📋 Full TODO Checklist
        // TODO 6: CanHandle + Execute — same shape as TODO 5, applied to
        //         courses instead of students.
        #endregion
    }
}
