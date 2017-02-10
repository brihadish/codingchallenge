using CodingChallenge.Lib.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.Domain
{
    public interface IZipCodeIndex
    {
        Result AddZipCode(int zipCode);

        Result<ZipCodeIndexSearchOutput> Search(ZipCodeIndexSearchInput input);
    }

    public sealed class ZipCodeIndex : IZipCodeIndex
    {
        private readonly ITrie<string> _stringTrie;

        public ZipCodeIndex(ITrie<string> stringTrie)
        {
            _stringTrie = stringTrie;
        }

        public Result AddZipCode(int zipCode)
        {
            if(zipCode <= 0)
            {
                return Result.Failure(Error.CreateFromEnum(ZipCodeErrorType.InvalidZipCode));
            }
            var zipCodeStr = zipCode.ToString();
            _stringTrie.AddIndex(zipCodeStr);
            return Result.Ok();
        }

        public Result<ZipCodeIndexSearchOutput> Search(ZipCodeIndexSearchInput input)
        {
            if(input.SearchTerm < 0)
            {
                return Result<ZipCodeIndexSearchOutput>.Failure(Error.CreateFromEnum(ZipCodeErrorType.InvalidZipCode));
            }
            var trieSearchInput = TrieSearchInput<string>.Create(input.SearchTerm.ToString(), input.Take, input.ContinuationToken);
            var trieSearchResult = _stringTrie.Search(trieSearchInput);
            if(trieSearchResult.IsSuccess)
            {
                var searchResult = new ZipCodeIndexSearchOutput(
                    trieSearchResult.ResultValue.SearchResults.Select(t => int.Parse(t)), 
                    trieSearchResult.ResultValue.ContinuationToken);
                return Result<ZipCodeIndexSearchOutput>.Ok(searchResult);
            }
            return Result<ZipCodeIndexSearchOutput>.Failure(trieSearchResult.Error);
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
