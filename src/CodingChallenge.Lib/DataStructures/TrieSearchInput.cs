using Functional.Maybe;

namespace CodingChallenge.Lib.DataStructures
{
    public struct TrieSearchInput<T>
    {
        public Maybe<T> SearchValue { get; }

        public Maybe<int> PageSize { get; }

        public Maybe<string> ContinuationToken { get; }

        private TrieSearchInput(T searchValue, Maybe<int> pageSize, Maybe<string> continuationToken)
        {
            SearchValue = searchValue.ToMaybe();
            PageSize = pageSize;
            ContinuationToken = continuationToken;
        }

        public static TrieSearchInput<T> Create(T searchValue)
        {
            return new TrieSearchInput<T>(searchValue, Maybe<int>.Nothing, Maybe<string>.Nothing);
        }

        public static TrieSearchInput<T> Create(T searchValue, Maybe<int> pageSize, Maybe<string> continuationToken)
        {
            return new TrieSearchInput<T>(searchValue, pageSize, continuationToken);
        }
    }
}
