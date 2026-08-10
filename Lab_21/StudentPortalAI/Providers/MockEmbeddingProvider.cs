// =====================================================================
// MockEmbeddingProvider — SESSION PROJECT (Style Guide Rule 20/35/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 21 — Block 2: Core Concepts II (embeddings)
//
// Today's new idea: turn a sentence into a list of numbers ("a vector")
// so two sentences that mean similar things end up as similar numbers.
// Our offline stand-in does this with shared WORDS, not real meaning —
// disclosed on purpose (see Instructor Guide).
// =====================================================================
namespace StudentPortalAI
{
    public class MockEmbeddingProvider : IEmbeddingProvider
    {
        // A shared word-to-position lookup, reused across every call, so
        // the same word always lands in the same vector position no
        // matter which text is embedded first.
        private readonly Dictionary<string, int> _vocabulary = new(StringComparer.OrdinalIgnoreCase);

        // TODO 1: Implement Embed(text). Break the text into lowercase
        //         words (split on spaces and basic punctuation — a
        //         helper for this, Tokenize, is already written for you
        //         below). For every word that hasn't been seen before,
        //         give it the next free position in the shared
        //         vocabulary dictionary. Then build a float array sized
        //         to the current vocabulary count, and set a 1 at the
        //         position of every word this text contains (repeats
        //         just set the same position to 1 again — that's fine).
        //         Return that array.

        public float[] Embed(string text)
        {
            var words = Tokenize(text);
            foreach (var word in words)
            {
                if (!_vocabulary.ContainsKey(word))
                {
                    _vocabulary[word] = _vocabulary.Count; //0
                }
            }
            var vector = new float[Math.Max(_vocabulary.Count,1)];
            foreach (var word in words)
            {
                vector[_vocabulary[word]] = 1f;
            }
            return vector; // [1,0,1,0,0,1,0]

            // "ahmed is smart"
             /// [1,1,1]
        }

        // Already written for you — called by the Retriever after every
        // document AND the question are embedded, so a word that first
        // appeared in the question doesn't leave earlier vectors too
        // short to compare.
        public float[] Resize(float[] vector)
        {
            if (vector.Length == _vocabulary.Count)
            {
                return vector;
            }

            var resized = new float[_vocabulary.Count];
            Array.Copy(vector, resized, Math.Min(vector.Length, resized.Length));
            return resized;
        }

        private static string[] Tokenize(string text)
        {
            return text
                .ToLowerInvariant()
                .Split(new[] { ' ', '.', ',', '?', '!', ':', ';', '\'', '"' },
                       StringSplitOptions.RemoveEmptyEntries);
        }

        #region 📋 Full TODO Checklist
        // TODO 1: Embed(text) — tokenize, grow the shared vocabulary,
        //         build and return the bag-of-words vector.
        #endregion
    }
}
