using Microsoft.AspNetCore.Authorization;

namespace FlintSoft.Permissions;

public class PermissionRequirement
: IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
