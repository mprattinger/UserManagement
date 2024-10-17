using UserManagement.Api.Features.Auth.Services;

namespace UserManagement.Api.Features.Auth;

public static class Extensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
