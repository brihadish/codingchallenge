using System.Collections.Generic;
using System.Linq;

namespace CodingChallenge.Lib.DataStructures
{
    public struct TrieSearchOutput<T>
    {
        public IEnumerable<T> SearchResults { get; }

        public string ContinuationToken { get; }

        public TrieSearchOutput(IEnumerable<T> searchResults, string continuationToken)
        {
            SearchResults = searchResults;
            ContinuationToken = continuationToken;
        }

        public static TrieSearchOutput<T> Empty()
        {
            return new TrieSearchOutput<T>(Enumerable.Empty<T>(), string.Empty);
        }
    }
}
