namespace CodingChallenge.Lib.DataStructures
{
    public interface ITrie<T>
    {
        long ApproximateSizeInBytes { get; }

        Result<bool> AddIndex(T indexValue);

        Result<TrieSearchOutput<T>> Search(TrieSearchInput<T> searchInput);
    }
}
