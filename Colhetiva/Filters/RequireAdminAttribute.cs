using Colhetiva.Core.Enums;
using Colhetiva.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Filters;

public sealed class RequireAdminAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var usuarioIdStr = session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        var roleStr = session.GetString("UsuarioRole");
        if (!string.IsNullOrEmpty(roleStr) &&
            Enum.TryParse<Role>(roleStr, ignoreCase: true, out var role) &&
            role == Role.ADMIN)
        {
            // usuário é ADMIN do sistema — autorizado
            return;
        }

        // Se não for ADMIN, tenta verificar se o usuário pertence a uma Organization
        // obtendo o DbContext via DI (RequestServices)
        try
        {
            var services = context.HttpContext.RequestServices;
            var db = services.GetService(typeof(ColhetivaDbContext)) as ColhetivaDbContext;
            if (db != null && Guid.TryParse(usuarioIdStr, out var usuarioId))
            {
                var usuario = db.Usuarios.AsNoTracking().FirstOrDefault(u => u.Id == usuarioId);
                if (usuario != null && usuario.OrganizationId.HasValue)
                {
                    // usuário pertence a uma organização — autorizado a gerenciar hortas da organização
                    return;
                }
            }
        }
        catch
        {
            // em caso de falha no acesso ao DB, não expor detalhes — tratar como não autorizado abaixo
        }

        // não é ADMIN nem gestor de organização -> redireciona para Home
        context.Result = new RedirectToActionResult("Index", "Home", null);
    }
}
