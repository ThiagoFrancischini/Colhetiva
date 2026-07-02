using Colhetiva.Core.Entities;
using Colhetiva.Core.Enums;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ColhetivaDbContext _db;
    private Usuario? _usuarioCache;
    private bool _usuarioCarregado;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, ColhetivaDbContext db)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
    }

    public Guid? UsuarioId
    {
        get
        {
            var usuarioIdStr = _httpContextAccessor.HttpContext?.Session.GetString("UsuarioId");
            return Guid.TryParse(usuarioIdStr, out var id) ? id : null;
        }
    }

    public async Task<Usuario?> GetUsuarioAsync()
    {
        if (_usuarioCarregado)
            return _usuarioCache;

        _usuarioCarregado = true;
        var usuarioId = UsuarioId;
        if (usuarioId == null)
            return null;

        _usuarioCache = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == usuarioId.Value);
        return _usuarioCache;
    }

    public Task<bool> IsOrganizationAdminAsync()
    {
        var roleStr = _httpContextAccessor.HttpContext?.Session.GetString("UsuarioRole");
        var isAdmin = !string.IsNullOrEmpty(roleStr) &&
            Enum.TryParse<Role>(roleStr, ignoreCase: true, out var role) &&
            role == Role.ADMIN;
        return Task.FromResult(isAdmin);
    }

    public async Task<Guid?> GetOrganizationIdAsync()
    {
        var usuario = await GetUsuarioAsync();
        return usuario?.OrganizationId;
    }

    public async Task<bool> CanManageHortaAsync(Horta horta)
    {
        var usuario = await GetUsuarioAsync();
        if (usuario == null)
            return false;

        if (horta.UsuarioId == usuario.Id)
            return true;

        return usuario.OrganizationId.HasValue &&
            horta.OrganizationId.HasValue &&
            usuario.OrganizationId == horta.OrganizationId;
    }
}
