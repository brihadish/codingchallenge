using CodingChallenge.Lib.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.Domain
{
    public interface ILocationIndex
    {
        Result AddLocation(string location);

        Result<LocationIndexSearchOutput> Search(LocationIndexSearchInput input);
    }

    public sealed class LocationIndex
    {
        private readonly ITrie<string> _stringTrie;

        public LocationIndex(ITrie<string> stringTrie)
        {
            _stringTrie = stringTrie;
        }

        public Result AddLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return Result.Failure(Error.CreateFromEnum(LocationErrorType.InvalidLocation));
            }
            _stringTrie.AddIndex(location);
            return Result.Ok();
        }

        public Result<LocationIndexSearchOutput> Search(LocationIndexSearchInput input)
        {
            if (string.IsNullOrWhiteSpace(input.SearchTerm))
            {
                return Result<LocationIndexSearchOutput>.Failure(Error.CreateFromEnum(LocationErrorType.InvalidLocation));
            }
            var trieSearchInput = TrieSearchInput<string>.Create(input.SearchTerm.ToString(), input.Take, input.ContinuationToken);
            var trieSearchResult = _stringTrie.Search(trieSearchInput);
            if (trieSearchResult.IsSuccess)
            {
                var searchResult = new LocationIndexSearchOutput(
                    trieSearchResult.ResultValue.SearchResults,
                    trieSearchResult.ResultValue.ContinuationToken);
                return Result<LocationIndexSearchOutput>.Ok(searchResult);
            }
            return Result<LocationIndexSearchOutput>.Failure(trieSearchResult.Error);
        }
    }

    [DataContract]
    public class LocationIndexSearchInput
    {
        [DataMember]
        public string SearchTerm { get; private set; }

        [DataMember]
        public int Take { get; private set; }

        [DataMember]
        public string ContinuationToken { get; private set; }

        public LocationIndexSearchInput(string searchTerm, int take, string continuationToken)
        {
            SearchTerm = searchTerm;
            Take = take;
            ContinuationToken = continuationToken;
        }
    }

    [DataContract]
    public class LocationIndexSearchOutput
    {
        [DataMember]
        public IEnumerable<string> SearchResults { get; private set; }

        [DataMember]
        public string ContinuationToken { get; private set; }

        public LocationIndexSearchOutput(IEnumerable<string> searchResults, string continuationToken)
        {
            SearchResults = searchResults;
            ContinuationToken = continuationToken;
        }
    }
}
