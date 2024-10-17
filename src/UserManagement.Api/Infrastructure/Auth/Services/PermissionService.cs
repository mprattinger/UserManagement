using System;

namespace UserManagement.Api.Infrastructure.Auth.Services;

public interface IPermissionService
{
    Task<HashSet<string>> GetPermissionsAsync(Guid userId);
}

public class PermissionService : IPermissionService
{
    public async Task<HashSet<string>> GetPermissionsAsync(Guid userId)
    {
        return ["BP.Read", "BP.Modify"];
    }
}
