using Functional.Maybe;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.Domain
{
    public interface IZipCodeIndexer
    {
        Task<Result> AddZipCodeAsync(int zipCode);

        Task<Result<ZipCodeIndexSearchOutput>> SearchAsync(ZipCodeIndexSearchInput input);
    }

    public enum ZipCodeIndexerErrorType
    {
        InvalidZipCode,
        ZipCodeNotFound
    }

    public sealed class ZipCodeIndexer : IZipCodeIndexer
    {
        private readonly IStringIndexer _stringIndexer;

        public ZipCodeIndexer(IStringIndexer stringIndexer)
        {
            _stringIndexer = stringIndexer;
        }

        public async Task<Result> AddZipCodeAsync(int zipCode)
        {
            if (zipCode <= 0)
            {
                return Result.Failure(Error.CreateFromEnum(ZipCodeIndexerErrorType.InvalidZipCode));
            }
            var zipCodeStr = zipCode.ToString();
            var result = await _stringIndexer.AddIndexAsync(zipCodeStr);
            if (result.IsFailure)
            {
                // Should not happen unless there is a bug
                return Result.Failure(result.Error);
            }
            return Result.Ok();
        }

        public async Task<Result<ZipCodeIndexSearchOutput>> SearchAsync(ZipCodeIndexSearchInput input)
        {
            if (input.SearchTerm < 0)
            {
                return Result<ZipCodeIndexSearchOutput>.Failure(Error.CreateFromEnum(ZipCodeIndexerErrorType.InvalidZipCode));
            }
            var take = input.Take <= 0 ? Maybe<int>.Nothing : input.Take.ToMaybe();
            var searchResult = await _stringIndexer.SearchAsync(
                new StringIndexerSearchInput(input.SearchTerm.ToString(), take, input.ContinuationToken.ToMaybe()));
            if (searchResult.IsSuccess)
            {
                var result = new ZipCodeIndexSearchOutput(
                    searchResult.ResultValue.SearchResults.Select(t => int.Parse(t)),
                    searchResult.ResultValue.ContinuationToken);
                return Result<ZipCodeIndexSearchOutput>.Ok(result);
            }
            return Result<ZipCodeIndexSearchOutput>.Failure(searchResult.Error);
        }
    }

    [DataContract]
    public struct ZipCodeIndexSearchInput
    {
        [DataMember]
        public int SearchTerm { get; private set; }

        [DataMember]
        public int Take { get; private set; }

        [DataMember]
        public string ContinuationToken { get; private set; }

        public ZipCodeIndexSearchInput(int searchTerm)
        {
            SearchTerm = searchTerm;
            Take = -1;
            ContinuationToken = string.Empty;
        }

        public ZipCodeIndexSearchInput(int searchTerm, int take, string continuationToken)
        {
            SearchTerm = searchTerm;
            Take = take;
            ContinuationToken = continuationToken;
        }
    }

    [DataContract]
    public struct ZipCodeIndexSearchOutput
    {
        [DataMember]
        public IEnumerable<int> SearchResults { get; private set; }

        [DataMember]
        public string ContinuationToken { get; private set; }

        public ZipCodeIndexSearchOutput(IEnumerable<int> searchResults, string continuationToken)
        {
            SearchResults = searchResults;
            ContinuationToken = continuationToken;
        }
    }
}
