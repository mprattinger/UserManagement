using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlintSoft.Permissions;

public static class Extensions
{
    public static IServiceCollection AddPermissions(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("FlintSoftPermissions").Get<PermissionSettings>();
        if (settings is null)
        {
            throw new Exception("Settings for FlintSoft.Permission library are not set in appsettings*.jsont");
        }

        services.AddSingleton(settings);

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
}
