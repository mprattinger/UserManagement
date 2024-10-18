using UserManagement.Api.Features.Auth.Models;

namespace UserManagement.Api.Features.Permissions.Models;

public class Permission
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Group { get; set; } = "";

    public string PermissionName { get; set; } = "";

    public virtual User? User { get; set; }
}
