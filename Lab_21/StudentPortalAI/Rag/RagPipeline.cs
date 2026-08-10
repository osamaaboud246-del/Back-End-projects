// =====================================================================
// RagPipeline — SESSION PROJECT (Style Guide Rule 20/35/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 21 — Block 3: The RAG hands-on mini-exercise
//
// Retrieve + Augment + Generate, as one method.
// =====================================================================
namespace StudentPortalAI
{
    public class RagPipeline
    {
        private readonly Retriever _retriever;
        private readonly IChatProvider _chatProvider;
        private const int TopK = 2;

        public const string SystemPrompt =
            "You are the StudentPortal assistant. Answer only using the retrieved " +
            "StudentPortal records you are given. If they don't contain the answer, say so.";

        public RagPipeline(Retriever retriever, IChatProvider chatProvider)
        {
            _retriever = retriever;
            _chatProvider = chatProvider;
        }

        // TODO 4 (part one): Implement Ask(question) — the full RAG
        //         path. Call the retriever's TopK for the top 2 matching
        //         documents. Join their Text values together into one
        //         string, separated by spaces — this is the "augment"
        //         step, and it really is just string joining. Pass that
        //         joined string as the groundingContext argument to the
        //         chat provider's Complete method, along with
        //         SystemPrompt and the original question.

        public string Ask(string question)
        {
            var retrieved = _retriever.TopK(question, TopK);
            var context = string.Join(" ", retrieved.Select(r => r.Document.Text));
            return _chatProvider.Complete(SystemPrompt, context, question);
        }

        // TODO 4 (part two): Implement AskUngrounded(question) — the
        //         SAME chat provider call as Ask, but pass null for
        //         groundingContext instead of retrieving anything. This
        //         is what Block 2's live demo runs.

        public string AskUngrounded(string question)
        {
            return _chatProvider.Complete(SystemPrompt, groundingContext: null, question);
        }

        #region 📋 Full TODO Checklist
        // TODO 4 (part one): Ask(question) — retrieve top 2, join their
        //         Text, call Complete with that as context.
        // TODO 4 (part two): AskUngrounded(question) — same call, no
        //         context.
        #endregion
    }
}
