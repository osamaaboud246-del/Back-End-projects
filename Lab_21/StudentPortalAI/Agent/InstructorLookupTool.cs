using System.Collections.Generic;
using System.Linq;

namespace StudentPortalAI.Agent
{
    public class InstructorLookupTool : ITool
    {
        private readonly List<KnowledgeDocument> _courseDocs;

        public InstructorLookupTool(List<KnowledgeDocument> courseDocs)
        {
            _courseDocs = courseDocs;
        }

        public string Name => "InstructorLookupTool";

        public string Description => "Finds out who teaches a specific course.";

        public bool CanHandle(string userPrompt)
        {
            var p = userPrompt.ToLower();
            return p.Contains("who teaches") || p.Contains("instructor");
        }

        public string Execute(string userPrompt)
        {
            var doc = _courseDocs.FirstOrDefault(d =>
                userPrompt.ToLower().Split(' ').Any(w => w.Length > 4 && d.Text.ToLower().Contains(w)));

            if (doc != null) return $"Here is the course info: {doc.Text}";
            return "Instructor not found for this course.";
        }
    }
}