using UserManagement.Api.Features.Auth;
using UserManagement.Api.Features.Permissions;

namespace UserManagement.Api.Features;

public static class Extensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddAuth();
        services.AddPermissions();

        return services;
    }
}
