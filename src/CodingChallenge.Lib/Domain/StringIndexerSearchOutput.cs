using System.Collections.Generic;

namespace CodingChallenge.Lib.Domain
{
    public struct StringIndexerSearchOutput
    {
        public IEnumerable<string> SearchResults { get; private set; }

        public string ContinuationToken { get; private set; }

        public StringIndexerSearchOutput(IEnumerable<string> searchResults, string continuationToken)
        {
            SearchResults = searchResults;
            ContinuationToken = continuationToken;
        }
    }
}
