using ErrorOr;
using FlintSoft.Endpoints;
using UserManagement.Api.Common.Errors;
using UserManagement.Api.Features.Permissions.Models;
using UserManagement.Api.Features.Permissions.Service;

namespace UserManagement.Api.Features.Permissions.Endpoints;

public class PermissionEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/perm", (IPermissionService permissionService) =>
        {
            var data = permissionService.GetPermissions();

            return Results.Ok(data);
        });

        app.MapGet("/perm/my", async (IPermissionService permissionService, HttpContext context) =>
        {
            var user = context.User;
            var uid = context.User.Claims.FirstOrDefault(x => x.Type == "UMUID")?.Value;

            if (!Guid.TryParse(uid, out var userId))
            {
                return Error.Conflict("PERMISSION.NOUID", "No userid found").ToProblemDetails();
            }

            var perm = await permissionService.GetUserPermission(userId);
            if (perm.IsError)
            {
                perm.Errors.ToProblemDetails();
            }

            return Results.Ok(perm.Value);
        })
            .RequireAuthorization();

        app.MapPost("/perm", async (SetPermissionRequest request, IPermissionService permissionService) =>
        {
            var p = await permissionService.SetPermission(request);

            if (p.IsError)
            {
                return p.Errors.ToProblemDetails();
            }

            return Results.Ok(p.Value);
        })
            .RequireAuthorization("ADMIN");

        app.MapPost("/perm/multi", async (SetPermissionsRequest request, IPermissionService permissionService) =>
        {
            var p = await permissionService.SetPermissions(request);

            if (p.IsError)
            {
                return p.Errors.ToProblemDetails();
            }

            return Results.Ok(p.Value);
        })
            .RequireAuthorization("ADMIN");
    }
}
