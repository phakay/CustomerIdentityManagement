using IdentityManagement.API.Common;

namespace IdentityManagement.API.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> ToResult<T>(this T value) => value;
    }
}
