namespace MachineLearning.LatentSemanticAnalysis.Corpora
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MachineLearning.Collections;

    /// <summary>
    /// <see cref="TermsDictionary&lt;TTerm&gt;"/> encapsulates the mapping between terms (normalized words) and their integer ids.    
    /// </summary>
    /// <typeparam name="TTerm">The type of the term.</typeparam>
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

        /// <summary>
        /// Gets the count of unique terms in dictionary.
        /// </summary>
        /// <value>The terms count.</value>
        public long TermsCount
        {
            get
            {
                return this.term2Id.Count;
            }
        }

        /// <summary>
        /// Gets or sets the processed documents count.
        /// </summary>
        /// <value>The documents count.</value>
        public long DocumentsCount { get; private set; }

        /// <summary>
        /// Gets or sets the total count of processed terms in all documents.
        /// </summary>
        /// <value>The total terms processed.</value>
        public long TotalTermsProcessed { get; private set; }

        /// <summary>
        /// Gets or sets the total count of non-zeros in the BOW (Bag-Of-Words) matrix.
        /// </summary>
        /// <value>The total non zero matrix elements.</value>
        public long TotalNonZeroMatrixElements { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TermsDictionary&lt;TTerm&gt;"/> class.
        /// </summary>
        public TermsDictionary()
            : this(Enumerable.Empty<TTerm[]>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TermsDictionary&lt;TTerm&gt;"/> class.
        /// </summary>
        /// <param name="documents">The documents.</param>
        public TermsDictionary(IEnumerable<TTerm[]> documents)
        {
            if (documents == null)
            {
                throw new ArgumentNullException("documents");
            }

            this.AddDocuments(documents);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates dictionary from a collection of documents. Each document is an array
        /// of terms (normalized words).
        /// </summary>
        /// <param name="documents">The documents.</param>
        public void AddDocuments(IEnumerable<TTerm[]> documents)
        {
            if (documents == null)
            {
                throw new ArgumentNullException("documents");
            }
            foreach (var document in documents)
            {
                this.DocumentToSparseVector(document, true);
            }
        }

        /// <summary>
        /// Updates the dictionary in the process: create ids for new words. At the same time, update document frequency -
        /// for for each word appearing in this document, increase its DF (Document-Frequency) by one.
        /// Also convert document (an array of terms) into the bag-of-words representation. Each term is assumed to be a normalized word.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>The sparse vector of bag-of-words. Each vector index is term id and vector elements are term counts.</returns>
        public SparseVector<int> AddDocument(TTerm[] document)
        {
            return this.DocumentToSparseVector(document, true);
        }

        /// <summary>
        /// Convert document (an array of terms) into the bag-of-words representation. Each term is assumed to be a normalized word.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>The sparse vector of bag-of-words. Each vector index is term id and vector elements are term counts.</returns>
        public SparseVector<int> DocumentToVector(TTerm[] document)
        {
            return this.DocumentToSparseVector(document);
        }

        /// <summary>
        /// Determines whether the dictionary contains the given <paramref name="term"/>.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>
        /// 	<c>true</c> if the dictionary contains term; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsTerm(TTerm term)
        {
            return term2Id.ContainsKey(term);
        }

        /// <summary>
        /// Determines whether the dictionary contains the given <paramref name="termId"/>.
        /// </summary>
        /// <param name="termId">The term id.</param>
        /// <returns>
        ///     <c>true</c> if the dictionary contains term id; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsTermId(int termId)
        {
            return id2Term.ContainsKey(termId);
        }

        /// <summary>
        /// Convert term to term id.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The term id.</returns>
        /// <exception cref="KeyNotFoundException">if the given term is not in dictionary.</exception>
        public int ConvertTermToTermId(TTerm term)
        {
            return this.term2Id[term];
        }

        /// <summary>
        /// Tries to convert the term to term id.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="termId">The term id.</param>
        /// <returns><c>true</c> if the given term is in dictionary; otherwise, <c>false</c>.</returns>
        public bool TryConvertTermToTermId(TTerm term, out int termId)
        {
            return this.term2Id.TryGetValue(term, out termId);
        }

        /// <summary>
        /// Convert term id to to term.
        /// </summary>
        /// <param name="termId">The term id.</param>
        /// <returns>The term.</returns>
        /// <exception cref="KeyNotFoundException">if the given term id is not in dictionary.</exception>
        public TTerm ConvertTermIdToTerm(int termId)
        {
            return this.id2Term[termId];
        }

        /// <summary>
        /// Tries to convert the term id to term.
        /// </summary>
        /// <param name="termId">The term id.</param>
        /// <param name="term">The term.</param>
        /// <returns><c>true</c> if the given term id is in dictionary; otherwise, <c>false</c>.</returns>
        public bool TryConvertTermIdToTerm(int termId, out TTerm term)
        {
            return this.id2Term.TryGetValue(termId, out term);
        }

        /// <summary>
        /// Returns documents frequency for the given <paramref name="termId"/>.
        /// </summary>
        /// <param name="termId">The term id.</param>
        /// <returns>The document frequency.</returns>
        public int DocumentFrequencyByTermId(int termId)
        {
            int value;
            return this.df.TryGetValue(termId, out value) ? value : 0;
        }

        /// <summary>
        /// Returns documents frequency for the given <paramref name="term"/>.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The document frequency.</returns>
        public int DocumentFrequencyByTerm(TTerm term)
        {
            int termId;
            return this.TryConvertTermToTermId(term, out termId) ? this.DocumentFrequencyByTermId(termId) : 0;
        }

        #endregion

        #region Helpers

        private SparseVector<int> DocumentToSparseVector(ICollection<TTerm> document, bool allowUpdate = false)
        {
            var result = new SparseVector<int>();

            int termsInDocument = document.Count;

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

            return result;
        }

        #endregion
    }
}
