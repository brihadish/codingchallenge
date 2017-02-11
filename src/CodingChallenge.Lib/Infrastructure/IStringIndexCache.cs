using CodingChallenge.Lib.DataStructures;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.Infrastructure
{
    public interface IStringIndexCache
    {
        Task<Result<ITrie<string>>> GetTrieAsync(char headNodeLabel);

        Task<Result> AddTrieAsync(char headNodeLabel, ITrie<string> trie);
    }
}