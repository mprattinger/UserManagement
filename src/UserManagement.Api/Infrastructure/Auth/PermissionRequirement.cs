using System;
using Microsoft.AspNetCore.Authorization;

namespace UserManagement.Api.Infrastructure.Auth;

public class PermissionRequirement
: IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
