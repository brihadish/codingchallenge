using Functional.Maybe;
using System;
using System.Linq;
using System.Text;

namespace CodingChallenge.Lib
{
    [Serializable]
    public sealed class UnicodeCharacter : ValueObject<UnicodeCharacter>
    {
        public char Value { get; }

        public static UnicodeCharacter Empty()
        {
            return new UnicodeCharacter(' ');
        }

        public UnicodeCharacter(char value)
        {
            Value = value;
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
            return string.Equals(
                this.Value.ToString(), other.Value.ToString(), StringComparison.Ordinal);
        }

        public bool EqualsCharacter(char value)
        {
            return this.EqualsCore(new UnicodeCharacter(value));
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }
    }

    internal static class UnicodeCharacterExtensions
    {
        public static Maybe<UnicodeCharacter> FirstCharacter(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Maybe<UnicodeCharacter>.Nothing;
            }
            return new UnicodeCharacter(value.First()).ToMaybe();
        }
    }
}
