using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using UserManagement.Api.Features.Permissions.Service;

namespace UserManagement.Api.Infrastructure.Auth;

public class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
: AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   PermissionRequirement requirement)
    {
        string? userId = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub || x.Type == "UMUID")?.Value;

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IPermissionService permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        var res = await permissionService.HasPermission(parsedUserId, requirement.Permission);
        if (res.IsError)
        {
            return;
        }

        context.Succeed(requirement);
    }
}
