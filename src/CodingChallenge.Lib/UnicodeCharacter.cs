using Functional.Maybe;
using System;
using System.Linq;
using System.Text;

namespace CodingChallenge.Lib
{
    [Serializable]
    public sealed class UnicodeCharacter : ValueObject<UnicodeCharacter>
    {
        private readonly bool _isCaseSensitive;

        public char Value { get; }

        public UnicodeCharacter(char value, bool isCaseSensitive)
        {
            Value = value;
            _isCaseSensitive = isCaseSensitive;
        }

        public UnicodeCharacter(byte[] valueInBytes)
        {
            if (valueInBytes?.Length > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(valueInBytes));
            }
            try
            {
                Value = Encoding.Unicode.GetChars(valueInBytes).FirstOrDefault();
            }
            catch (DecoderFallbackException)
            {
                throw new ArgumentException(nameof(valueInBytes));
            }
        }

        protected override bool EqualsCore(UnicodeCharacter other)
        {
            if (_isCaseSensitive)
            {
                return string.Equals(
                    this.Value.ToString(), other.Value.ToString(), StringComparison.Ordinal);
            }
            else
            {
                return string.Equals(
                    this.Value.ToString(), other.Value.ToString(), StringComparison.OrdinalIgnoreCase);
            }
        }

        protected override int GetHashCodeCore()
        {
            if(_isCaseSensitive)
            {
                return Value.GetHashCode();
            }
            else
            {
                return Value.ToString().ToLowerInvariant().GetHashCode();
            }
        }
    }

    internal static class UnicodeCharacterExtensions
    {
        public static Maybe<UnicodeCharacter> FirstCharacter(this string value, bool isCaseSensitive)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Maybe<UnicodeCharacter>.Nothing;
            }
            return new UnicodeCharacter(value.First(), isCaseSensitive).ToMaybe();
        }
    }
}
