using Colhetiva.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Colhetiva.Filters;

public sealed class RequireAdminAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        if (string.IsNullOrEmpty(session.GetString("UsuarioId")))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        var roleStr = session.GetString("UsuarioRole");
        if (string.IsNullOrEmpty(roleStr)
            || !Enum.TryParse<Role>(roleStr, ignoreCase: true, out var role)
            || role != Role.ADMIN)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
