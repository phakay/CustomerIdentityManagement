using System.Numerics;

namespace IdentityManagement.API.Common
{
    public class Result<TValue>
    {
        private Result(TValue value)
        {
            IsSuccessful = true;
            Value = value;
        }

        private Result(string errorMessage)
        {
            IsSuccessful = false;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccessful { get; }
        public string ErrorMessage { get; }
        public TValue Value { get; }

        public static Result<TValue> Success(TValue value) => new Result<TValue>(value);

        public static Result<TValue> Failure(string errorMessage) => new Result<TValue>(errorMessage);

        public static implicit operator Result<TValue>(TValue value)
        {
            return Success(value);
        }
    }
}
