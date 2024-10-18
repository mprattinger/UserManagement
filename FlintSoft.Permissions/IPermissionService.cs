namespace FlintSoft.Permissions;

public interface IPermissionCheckService<T>
{
    Task<T> HasPermission(Guid userId, string permission);
}
