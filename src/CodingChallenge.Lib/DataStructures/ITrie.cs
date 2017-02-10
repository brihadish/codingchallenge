using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.DataStructures
{
    public interface ITrie<T>
    {
        long ApproximateSizeInBytes { get; }

        Result<bool> AddIndex(T indexValue);

        Result<TrieSearchOutput<T>> Search(TrieSearchInput<T> searchInput);
    }
}
