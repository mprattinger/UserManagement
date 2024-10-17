using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using UserManagement.Api.Infrastructure.Auth.Services;

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

        var permissions = await permissionService.GetPermissionsAsync(parsedUserId);

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
