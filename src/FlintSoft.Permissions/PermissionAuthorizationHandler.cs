using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FlintSoft.Permissions;

public class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
: AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   PermissionRequirement requirement)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();

        var settings = scope.ServiceProvider.GetRequiredService<PermissionSettings>();

        var type = settings.ClaimType;
        if (string.IsNullOrEmpty(type))
        {
            type = "sub";
        }

        string? userId = context.User.Claims.FirstOrDefault(x => x.Type == type)?.Value;

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return;
        }

        IPermissionCheckService permissionService = scope.ServiceProvider.GetRequiredService<IPermissionCheckService>();

        var res = await permissionService.HasPermission(parsedUserId, requirement.Permission);
        if (res)
        {
            context.Succeed(requirement);
        }

        return;
    }
}
