using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FlintSoft.Permissions;

public class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
: AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   PermissionRequirement requirement)
    {
        string? userId = context.User.Claims.FirstOrDefault(x => x.Type == "SUB" || x.Type == "UMUID")?.Value;

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IPermissionCheckService permissionService = scope.ServiceProvider.GetRequiredService<IPermissionCheckService>();

        var res = await permissionService.HasPermission(parsedUserId, requirement.Permission);
        if (res.IsError)
        {
            return;
        }

        context.Succeed(requirement);
    }
}
