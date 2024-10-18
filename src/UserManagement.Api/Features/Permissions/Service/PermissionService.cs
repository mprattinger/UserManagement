using ErrorOr;
using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Features.Permissions.Models;
using UserManagement.Api.Infrastructure.Data;

namespace UserManagement.Api.Features.Permissions.Service;

public interface IPermissionService
{
    Dictionary<string, HashSet<string>> GetPermissions();
    Task<ErrorOr<HashSet<string>>> GetUserPermission(Guid userid);
    Task<ErrorOr<Permission>> SetPermission(SetPermissionRequest request);
    Task<ErrorOr<List<Permission>>> SetPermissions(SetPermissionsRequest request);

    Task<ErrorOr<Permission>> HasPermission(Guid userId, string permission);
}

public class PermissionService(DataContext dataContext) : IPermissionService
{
    /*
     * 
     * BP
     *  -> BP.Read
     *  -> BP.Modify
     *  -> BP.Delete
     * 
     * Admin
     *  -> ADMIN.Modify
     */

    public Dictionary<string, HashSet<string>> GetPermissions()
    {
        var dict = new Dictionary<string, HashSet<string>>();

        var bp = new HashSet<string>();
        bp.Add("Read");
        bp.Add("Modify");
        bp.Add("Delete");
        dict.Add("BP", bp);

        return dict;
    }

    public async Task<ErrorOr<HashSet<string>>> GetUserPermission(Guid userid)
    {
        var myPerm = await dataContext
            .Permissions
            .Include(x => x.User)
            .Where(x => x.UserId == userid)
            .ToListAsync();

        if (myPerm is null)
        {
            return Error.NotFound("PERMISSION.GETMY", "Couldn't find any permissions for the current user");
        }

        return myPerm.Select(x => String.IsNullOrEmpty(x.PermissionName) ? x.Group : $"{x.Group}.{x.PermissionName}").ToHashSet();
    }

    public async Task<ErrorOr<Permission>> HasPermission(Guid userId, string permission)
    {
        if (string.IsNullOrEmpty(permission))
        {
            return Error.Conflict("PERMISSION.NOPERM", "No permission to check from");
        }

        string group = "";
        string perm = "";
        if (permission.Contains('.'))
        {
            var permissionItems = permission.Split('.');

            if (permissionItems.Length > 2)
            {
                return Error.Conflict("PERMISSION.NOTVALID", "The given permission is not valid");
            }

            group = permissionItems[0];
            perm = permissionItems[1];
        }
        else
        {
            group = permission;
        }

        //Prüfen ob der User rechte hat -> mal alle Rechte des Users holen
        var perms = await dataContext.Permissions.Where(x => x.UserId == userId).ToListAsync();
        if (perms is null)
        {
            return Error.NotFound("PERMISSION.GETMY", "Couldn't find any permissions for the current user");
        }

        //Admin hat immer rechte
        var admin = perms.FirstOrDefault(x => x.Group == "ADMIN");
        if (admin is not null)
        {
            return admin;
        }

        //Globale Rechte auf die Gruppe?
        var global = perms.FirstOrDefault(x => x.Group == group && x.PermissionName == "");
        if (global is not null)
        {
            return global;
        }

        //Spezifische Rechte
        var specific = perms.FirstOrDefault(x => x.Group == group && x.PermissionName == perm);
        if (specific is not null)
        {
            return specific;
        }

        return Error.NotFound("PERMISSION.NOPERM", "Feature is not allowed for the current user");
    }

    public async Task<ErrorOr<Permission>> SetPermission(SetPermissionRequest request)
    {
        if (request.Permission is null)
        {
            return Error.Validation("PERMISSION.PERMISSION_NULL", "Permission info is null");
        }
        if (string.IsNullOrEmpty(request.Permission.Group))
        {
            return Error.Validation("PERMISSION.GROUP_EMPTY", "Permission group is empty");
        }
        if (string.IsNullOrEmpty(request.Permission.Permission))
        {
            return Error.Validation("PERMISSION.PERM_EMPTY", "Permission name is empty");
        }

        var p = new Permission()
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Group = request.Permission!.Group,
            PermissionName = request.Permission!.Permission
        };

        dataContext.Permissions.Add(p);
        await dataContext.SaveChangesAsync();

        return p;
    }

    public async Task<ErrorOr<List<Permission>>> SetPermissions(SetPermissionsRequest request)
    {
        if (request.Permissions is null)
        {
            return Error.Validation("PERMISSION.PERMISSIONS_NULL", "Permissions list is null");
        }
        if (!request.Permissions.Any())
        {
            return Error.Validation("PERMISSION.PERMISSIONS_EMPTY", "Permissions list is empty");
        }
        if (request.Permissions.Any(x => string.IsNullOrEmpty(x.Group)))
        {
            return Error.Validation("PERMISSION.PERMISSIONS_GROUP_EMPTY", "A permission group in the permissions list is empty");
        }
        if (request.Permissions.Any(x => string.IsNullOrEmpty(x.Permission)))
        {
            return Error.Validation("PERMISSION.PERMISSIONS_PERM_EMPTY", "A permission name in the permissions list is empty");
        }

        var permList = new List<Permission>();

        foreach (var perm in request.Permissions)
        {
            var p = new Permission()
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Group = perm!.Group,
                PermissionName = perm!.Permission
            };

            permList.Add(p);
        }

        await dataContext.Permissions.AddRangeAsync(permList);
        await dataContext.SaveChangesAsync();

        return permList;
    }
}
