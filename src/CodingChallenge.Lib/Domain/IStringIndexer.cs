using System.Threading.Tasks;

namespace CodingChallenge.Lib.Domain
{
    public interface IStringIndexer
    {
        Task<Result> AddIndexAsync(string indexValue);

        Task<Result<StringIndexerSearchOutput>> SearchAsync(StringIndexerSearchInput searchInput);
    }
}
