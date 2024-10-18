namespace FlintSoft.Permissions;

public interface IPermissionCheckService
{
    Task<bool> HasPermission(Guid userId, string permission);
}
