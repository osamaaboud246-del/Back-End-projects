// =====================================================================
// Retriever — SESSION PROJECT (Style Guide Rule 20/35/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 21 — Block 3: The RAG hands-on mini-exercise
//
// The "R" in RAG. Given a question, find the K real documents whose
// vector is closest to the question's vector.
// =====================================================================
using System.Numerics;

namespace StudentPortalAI
{
    public class Retriever
    {
        private readonly IEmbeddingProvider _embeddingProvider;
        private readonly List<KnowledgeDocument> _documents;

        public Retriever(IEmbeddingProvider embeddingProvider, List<KnowledgeDocument> documents)
        {
            _embeddingProvider = embeddingProvider;
            _documents = documents;
        }

        // TODO 3: Implement TopK(question, k). Embed every document's
        //         Text with the embedding provider, and embed the
        //         question too. If the embedding provider is a
        //         MockEmbeddingProvider, call its Resize helper on every
        //         one of those vectors AFTER embedding the question, so
        //         they're all the same length before comparing (a plain
        //         type check and cast is enough — see the "is" pattern
        //         you already used back in Session 11). Score every
        //         document with CosineSimilarity(questionVector,
        //         documentVector) — already written for you below — sort
        //         by score highest first, and return only the top k as a
        //         list of RetrievedDocument (Document + Score).

        public List<RetrievedDocument> TopK(string question, int k)
        {
            var docVectors = _documents
                .Select(d => (Doc: d, Vector: _embeddingProvider.Embed(d.Text)))
                .ToList();

            var questionVector = _embeddingProvider.Embed(question);


            if (_embeddingProvider is MockEmbeddingProvider mock)
            {
                docVectors = docVectors
                    .Select(dv => (dv.Doc, Vector: mock.Resize(dv.Vector)))
                    .ToList();
                questionVector = mock.Resize(questionVector);
            }

            return docVectors
                .Select(dv => new RetrievedDocument
                {
                    Document = dv.Doc,
                    Score = CosineSimilarity(questionVector, dv.Vector)
                })
                .OrderByDescending(r => r.Score)
                .Take(k)
                .ToList();

            // READ : Pagination (Take + Skip + TakeWhile + SkipWhile)
        }

        // Already written for you — the standard formula for "how
        // similar are two vectors' directions," used by real vector
        // databases the same way.
        public static double CosineSimilarity(float[] a, float[] b)
        {
            int length = Math.Min(a.Length, b.Length);
            double dot = 0, magnitudeA = 0, magnitudeB = 0;

            for (int i = 0; i < length; i++)
            {
                dot += a[i] * b[i];
                magnitudeA += a[i] * a[i];
                magnitudeB += b[i] * b[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0)
            {
                return 0;
            }

            return dot / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        }

        #region 📋 Full TODO Checklist
        // TODO 3: TopK(question, k) — embed everything, resize mock
        //         vectors, score, sort, take k.
        #endregion
    }
}
