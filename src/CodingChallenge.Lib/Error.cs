using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib
{
    public struct Error
    {
        public string ErrorType { get; }

        private Error(string errorType)
        {
            ErrorType = errorType;
        }
        
        public static Error CreateFromEnum<T>(T errorType)
        {
            try
            {
                return new Error(Enum.GetName(typeof(T), errorType));
            }
            catch(ArgumentException)
            {
                throw new ArgumentException(nameof(errorType));
            }
        }

        public static Error Create(string errorType)
        {
            return new Error(errorType);
        }
    }
}