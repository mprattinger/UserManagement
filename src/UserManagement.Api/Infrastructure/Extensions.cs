using FlintSoft.Permissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagement.Api.Features.Auth.Models;
using UserManagement.Api.Infrastructure.Auth.Services;
using UserManagement.Api.Infrastructure.Data;

namespace UserManagement.Api.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();

        services.AddDbContext<DataContext>(opt =>
            opt.UseSqlite("Data Source=userman.db")
        );

        var jwtConf = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if (jwtConf is not null)
        {
            services.AddSingleton(jwtConf);
        }

        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConf?.Issuer,
                    ValidAudience = jwtConf?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtConf?.Secret!)),

                };
                options.MapInboundClaims = false;
            });

        services.AddAuthorization();
        services.AddPermissions(configuration);

        return services;
    }
}
