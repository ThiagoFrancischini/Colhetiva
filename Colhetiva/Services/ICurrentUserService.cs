using Colhetiva.Core.Entities;

namespace Colhetiva.Services;

public interface ICurrentUserService
{
    Guid? UsuarioId { get; }
    Task<Usuario?> GetUsuarioAsync();
    Task<bool> IsOrganizationAdminAsync();
    Task<Guid?> GetOrganizationIdAsync();
    Task<bool> CanManageHortaAsync(Horta horta);
}
