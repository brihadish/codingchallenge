using Functional.Maybe;
using System;
using System.Runtime.Serialization;

namespace CodingChallenge.Lib
{
    [DataContract]
    public struct CommonResult
    {
        [DataMember]
        private readonly Error? _error;

        public bool IsFailure => _error.ToMaybe().IsSomething();

        public bool IsSuccess => !IsFailure;

        public Error Error
        {
            get
            {
                if (IsSuccess)
                {
                    throw new InvalidOperationException("There is no error for success scenario.");
                }
                return _error.GetValueOrDefault();
            }
        }

        public CommonResult(Maybe<Error> error)
        {
            if (error.IsSomething())
            {
                _error = error.Value;
            }
            else
            {
                _error = null;
            }
        }
    }

    [DataContract]
    public struct Result
    {
        [DataMember]
        private readonly CommonResult _common;

        public bool IsFailure => _common.IsFailure;

        public bool IsSuccess => _common.IsSuccess;

        public Error Error => _common.Error;

        private Result(Maybe<Error> error)
        {
            _common = new CommonResult(error);
        }

        public static Result Ok()
        {
            return new Result(Maybe<Error>.Nothing);
        }

        public static Result Failure(Error error)
        {
            return new Result(error.ToMaybe());
        }
    }

    [DataContract]
    public struct Result<T>
    {
        [DataMember]
        private readonly CommonResult _common;

        [DataMember]
        private readonly T _resultValue;

        public bool IsFailure => _common.IsFailure;

        public bool IsSuccess => _common.IsSuccess;

        public Error Error => _common.Error;

        public T ResultValue
        {
            get
            {
                if (IsFailure)
                {
                    throw new InvalidOperationException("There is no result value for failure scenario");
                }
                return _resultValue;
            }
        }

        private Result(Error error)
        {
            _common = new CommonResult(error.ToMaybe());
            _resultValue = default(T);
        }

        private Result(T resultValue)
        {
            _common = new CommonResult(Maybe<Error>.Nothing);
            _resultValue = resultValue;
        }

        public static Result<T> Ok(T resultValue)
        {
            return new Result<T>(resultValue);

        }

        public static Result<T> Failure(Error error)
        {
            return new Result<T>(error);
        }
    }
}
