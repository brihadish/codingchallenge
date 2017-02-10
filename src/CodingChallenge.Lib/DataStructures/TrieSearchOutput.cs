using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.DataStructures
{
    [DataContract]
    public struct TrieSearchOutput<T>
    {
        [DataMember]
        public IEnumerable<T> SearchResults { get; private set; }

        [DataMember]
        public string ContinuationToken { get; private set; }

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
