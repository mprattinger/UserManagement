using UserManagement.Api.Features.Auth;

namespace UserManagement.Api.Features;

public static class Extensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddAuth();

        return services;
    }
}
