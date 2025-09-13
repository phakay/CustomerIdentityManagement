using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Extensions
{
    public static class ActionResultExtensions
    {
        public static ActionResult<T> ToActionResult<T>(this T value) => value;
    }
}
