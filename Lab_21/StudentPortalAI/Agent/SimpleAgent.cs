// =====================================================================
// SimpleAgent — SESSION PROJECT (Style Guide Rule 20/35/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 21 — Block 8: AI Agents & Agentic Workflows (the payoff)
//
// The whole day converges here: try each precise tool in order; if none
// can handle the question, fall back to the RAG pipeline built in
// Block 3.
// =====================================================================
namespace StudentPortalAI
{
    public class SimpleAgent
    {
        private readonly List<ITool> _tools;
        private readonly RagPipeline _ragFallback;

        public SimpleAgent(List<ITool> tools, RagPipeline ragFallback)
        {
            _tools = tools;
            _ragFallback = ragFallback;
        }

        // TODO 7: Implement Handle(question). Loop through _tools IN
        //         ORDER; for the first one whose CanHandle(question) is
        //         true, call its Execute(question) and return a new
        //         AgentResult with ToolUsed set to that tool's Name and
        //         Answer set to what Execute returned — stop looping
        //         once you've done this. If the loop finishes with NO
        //         tool willing to handle it, return an AgentResult with
        //         ToolUsed set to the literal text "rag_fallback" and
        //         Answer set to whatever the RAG pipeline's Ask(question)
        //         returns.

        public AgentResult Handle(string question)
        {
            foreach (var tool in _tools)
            {
                if (tool.CanHandle(question))
                {
                    return new AgentResult
                    {
                        ToolUsed = tool.Name,
                        Answer = tool.Execute(question)
                    };
                }
            }
            return new AgentResult
            {
                ToolUsed = "rag_fallback",
                Answer = _ragFallback.Ask(question)
            };
        }

        #region 📋 Full TODO Checklist
        // TODO 7: Handle(question) — try each tool in order, fall back
        //         to RAG if none can handle it.
        #endregion
    }

    public class AgentResult
    {
        public string ToolUsed { get; set; } = "";
        public string Answer { get; set; } = "";
    }
}
