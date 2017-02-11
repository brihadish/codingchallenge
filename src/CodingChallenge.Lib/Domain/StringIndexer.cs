using CodingChallenge.Lib.DataStructures;
using CodingChallenge.Lib.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.Domain
{
    public sealed class StringIndexer : IStringIndexer
    {
        private readonly IStringIndexCache _cache;

        public StringIndexer(IStringIndexCache cache)
        {
            _cache = cache;
        }

        public async Task<Result> AddIndexAsync(string indexValue)
        {
            if (string.IsNullOrWhiteSpace(indexValue))
            {
                return Result.Failure(Error.CreateFromEnum(StringIndexerErrorType.EmptyIndexValue));
            }
            var firstCharInIndex = indexValue.FirstOrDefault();
            var trieResult = await _cache.GetTrieAsync(firstCharInIndex);
            var addToCache = false;
            ITrie<string> trie;
            if (trieResult.IsFailure)
            {
                SimpleStringIndexCacheErrorType errorType;
                if (Enum.TryParse(trieResult.Error.ErrorType, out errorType))
                {
                    if (errorType == SimpleStringIndexCacheErrorType.UnableToParseTrie)
                    {
                        return Result.Failure(Error.CreateFromEnum(StringIndexerErrorType.IndexDataNotReadable));
                    }
                    else if (errorType == SimpleStringIndexCacheErrorType.TrieNotFound)
                    {
                        var newTrieResult = NonEmptyRootStringTrie.CreateNew(indexValue);
                        if (newTrieResult.IsFailure)
                        {
                            return Result.Failure(Error.CreateFromEnum(StringIndexerErrorType.UnableToCreateNewIndex));
                        }
                        trie = newTrieResult.ResultValue;
                        addToCache = true;
                    }
                    else
                    {
                        return Result.Failure(Error.Create($"SimpleStringIndexCacheErrorType.{trieResult.Error.ErrorType}"));
                    }
                }
                else
                {
                    return Result.Failure(trieResult.Error);
                }
            }
            else
            {
                trie = trieResult.ResultValue;
            }
            var result = trie.AddIndex(indexValue);
            if (result.IsFailure)
            {
                // Should not happen unless there is a bug.
                return Result.Failure(Error.CreateFromEnum(StringIndexerErrorType.UnableToAddValueToIndex));
            }
            if(addToCache)
            {
                var cacheResult = await _cache.AddTrieAsync(firstCharInIndex, trie);
                if(cacheResult.IsFailure)
                {
                    return Result.Failure(Error.CreateFromEnum(StringIndexerErrorType.UnableToCreateNewIndex));
                }
            }
            return Result.Ok();
        }

        public async Task<Result<StringIndexerSearchOutput>> SearchAsync(StringIndexerSearchInput searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput.SearchTerm))
            {
                return Result<StringIndexerSearchOutput>.Failure(Error.CreateFromEnum(StringIndexerErrorType.EmptyIndexValue));
            }
            var searchValue = searchInput.SearchTerm;
            var firstCharInIndex = searchValue.First();
            var trieResult = await _cache.GetTrieAsync(firstCharInIndex);
            if (trieResult.IsFailure)
            {
                SimpleStringIndexCacheErrorType errorType;
                if (Enum.TryParse(trieResult.Error.ErrorType, out errorType))
                {
                    if (errorType == SimpleStringIndexCacheErrorType.UnableToParseTrie)
                    {
                        return Result<StringIndexerSearchOutput>.Failure(Error.CreateFromEnum(StringIndexerErrorType.IndexDataNotReadable));
                    }
                    else if (errorType == SimpleStringIndexCacheErrorType.TrieNotFound)
                    {
                        return Result<StringIndexerSearchOutput>.Failure(Error.CreateFromEnum(StringIndexerErrorType.IndexNotFound));
                    }
                    else
                    {
                        return Result<StringIndexerSearchOutput>.Failure(
                            Error.Create($"SimpleStringIndexCacheErrorType.{trieResult.Error.ErrorType}"));
                    }
                }
                else
                {
                    return Result<StringIndexerSearchOutput>.Failure(
                        Error.Create($"SimpleStringIndexCacheErrorType.{trieResult.Error.ErrorType}"));
                }
            }
            var trie = trieResult.ResultValue;
            var searchResult = trie.Search(TrieSearchInput<string>.Create(
                searchValue, searchInput.Take, searchInput.ContinuationToken));
            if (searchResult.IsFailure)
            {
                return Result<StringIndexerSearchOutput>.Failure(
                    Error.Create($"StrihngTrieErrorType.{trieResult.Error.ErrorType}"));
            }
            var output = new StringIndexerSearchOutput(
                searchResult.ResultValue.SearchResults, searchResult.ResultValue.ContinuationToken);
            return Result<StringIndexerSearchOutput>.Ok(output);
        }
    }
}
