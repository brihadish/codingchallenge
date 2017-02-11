using Functional.Maybe;

namespace CodingChallenge.Lib.Domain
{
    public struct StringIndexerSearchInput
    {
        public string SearchTerm { get; private set; }

        public Maybe<int> Take { get; private set; }

        public Maybe<string> ContinuationToken { get; private set; }

        public StringIndexerSearchInput(string searchTerm, Maybe<int> take, Maybe<string> continuationToken)
        {
            SearchTerm = searchTerm;
            Take = take;
            ContinuationToken = continuationToken;
        }
    }
}
