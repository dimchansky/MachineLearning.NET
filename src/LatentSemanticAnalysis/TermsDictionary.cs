namespace MachineLearning.LatentSemanticAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public sealed class TermsDictionary<TTerm>
        where TTerm : IEquatable<TTerm>
    {
        #region Fields and Properties

        // term -> term id
        private readonly Dictionary<TTerm, int> term2Id = new Dictionary<TTerm, int>();

        // reverse mapping: term id -> term
        private readonly Dictionary<int, TTerm> id2Term = new Dictionary<int, TTerm>();

        // document frequencies: term id -> in how many documents this term appeared
        private readonly Dictionary<int, int> df = new Dictionary<int, int>();

        // number of terms in dictionary
        public long TermsCount
        {
            get
            {
                return this.term2Id.Count;
            }
        }

        // number of documents processed
        public long DocumentsCount { get; private set; }

        // total number of processed terms in all documents
        public long TotalTermsProcessed { get; private set; }

        // total number of non-zeroes in the BOW (Bag-Of-Words) matrix
        public long TotalNonZeroMatrixElements { get; private set; }

        #endregion

        public TermsDictionary()
            : this(Enumerable.Empty<TTerm[]>())
        {
        }

        public TermsDictionary(IEnumerable<TTerm[]> documents)
        {
            if (documents == null)
            {
                throw new ArgumentNullException("documents");
            }

            this.AddDocuments(documents);
        }

        #region Public Methods

        public void AddDocuments(IEnumerable<TTerm[]> documents)
        {
            if (documents == null)
            {
                throw new ArgumentNullException("documents");
            }
            foreach (var document in documents)
            {
                this.DocumentToSparceVector(document, true);
            }
        }

        public SparceVector<int> AddDocument(TTerm[] document)
        {
            return this.DocumentToSparceVector(document, true);
        }

        public SparceVector<int> DocumentToVector(TTerm[] document)
        {
            return this.DocumentToSparceVector(document);
        }

        public int TermToId(TTerm term)
        {
            return this.term2Id[term];
        }

        public bool TryTermToId(TTerm term, out int termId)
        {
            return this.term2Id.TryGetValue(term, out termId);
        }

        public TTerm IdToTerm(int termId)
        {
            return this.id2Term[termId];
        }

        public bool TryIdToTerm(int termId, out TTerm term)
        {
            return this.id2Term.TryGetValue(termId, out term);
        }

        public int DocumentFrequencyById(int termId)
        {
            return this.df[termId];
        }

        public int DocumentFrequencyByTerm(TTerm term)
        {
            int termId = this.TermToId(term);
            return this.DocumentFrequencyById(termId);
        }

        #endregion

        #region Helpers

        private SparceVector<int> DocumentToSparceVector(ICollection<TTerm> document, bool allowUpdate = false)
        {
            var result = new SparceVector<int>();

            int termsInDocument = document.Count;

            if (termsInDocument > 0)
            {
                foreach (var groupByTerm in document.GroupBy(term => term))
                {
                    TTerm term = groupByTerm.Key;
                    // try to get term id
                    int termId;
                    if (!this.term2Id.TryGetValue(term, out termId))
                    {
                        // no term in dictionary
                        if (allowUpdate)
                        {
                            // assign new id to term
                            termId = this.term2Id.Count;
                            this.term2Id.Add(term, termId);
                            // update reverse mapping
                            this.id2Term.Add(termId, term);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    // update how many times a term appeared in the document
                    int termFrequency = groupByTerm.Count();
                    result[termId] = termFrequency;

                    if (allowUpdate)
                    {
                        // increase document count for each unique term that appeared in the document
                        int termDf;
                        if (!this.df.TryGetValue(termId, out termDf))
                        {
                            termDf = 0;
                        }
                        this.df[termId] = termDf + 1;

                        // increase total number of non-zeroes in the BOW matrix
                        this.TotalNonZeroMatrixElements += 1;
                    }
                }

                if (allowUpdate)
                {
                    // increase total document number
                    this.DocumentsCount += 1;
                    // increase total number of processed terms
                    this.TotalTermsProcessed += termsInDocument;
                }
            }

            return result;
        }

        #endregion
    }
}
