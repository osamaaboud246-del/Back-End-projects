// =====================================================================
// MockChatProvider — SESSION PROJECT (Style Guide Rule 20/35/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 21 — Block 2 (ungrounded) / Block 3 (grounded)
//
// This class is where today's central lesson becomes code: a model
// with no private data should say so, not guess — and a model given
// the real retrieved data can answer correctly because it is reading
// real facts, not because it got smarter.
// =====================================================================
namespace StudentPortalAI
{
    public class MockChatProvider : IChatProvider
    {
        // TODO 2: Implement Complete(systemPrompt, groundingContext,
        //         userPrompt). If groundingContext is null or blank,
        //         this is the UNGROUNDED case — return a sentence that
        //         honestly declines to answer because there is no
        //         private data available, rather than inventing one.
        //         Otherwise, this is the GROUNDED case — return a
        //         sentence that states the answer came from the
        //         StudentPortal records, followed by the groundingContext
        //         text itself, so the answer is built directly from the
        //         real retrieved row rather than guessed.

        public string Complete(string systemPrompt, string? groundingContext, string userPrompt)
        {
            if (string.IsNullOrWhiteSpace(groundingContext))
            {
                return $"I Don't have access to any private student records , so i can't answer"
                    + $"accuratley , I can guess , but i'd rather tell you i don't know that make up something";
            }

            return $"Based on the StudentPortal records i was given : {groundingContext}";
            
        }

        #region 📋 Full TODO Checklist
        // TODO 2: Complete(...) — ungrounded decline vs. grounded
        //         answer-from-context, decided by whether groundingContext
        //         has anything in it.
        #endregion
    }
}
