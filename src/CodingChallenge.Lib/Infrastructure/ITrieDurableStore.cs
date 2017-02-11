using CodingChallenge.Lib.DataStructures;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.Infrastructure
{
    public interface ITrieDurableStore<T>
    {
        Task<Result<ITrie<T>>> GetTrieAsync(string trieName);

        Task<Result> SaveTrieAsync(string trieName, ITrie<T> trie);
    }
}
