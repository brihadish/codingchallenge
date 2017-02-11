namespace CodingChallenge.Lib.Domain
{
    public enum StringIndexerErrorType
    {
        EmptyIndexValue,
        EmptySearchValue,
        IndexMismatch,
        IndexNotFound,
        IndexDataNotReadable,
        UnableToCreateNewIndex,
        UnableToAddValueToIndex,
        UnableToAcquireLockOnIndex
    }
}
