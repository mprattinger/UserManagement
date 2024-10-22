using FlintSoft.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Kiota.Abstractions.Authentication;
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

        services.AddAuthentication()
            .AddJwtBearer("Local", options =>
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
                options.Authority = "https://localhost:7183/";
                options.Audience = jwtConf?.Audience;
            })
            .AddJwtBearer("EntraId", options =>
            {
                options.Authority = configuration.GetSection("EntraId").GetValue<string>("Authority");
                options.Audience = configuration.GetSection("EntraId").GetValue<string>("ClientId");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://login.microsoftonline.com/common/v2.0"
                };
            })
            ;
        //.AddMicrosoftIdentityWebApi(jwtOptions =>
        //{
        //    jwtOptions.MapInboundClaims = false;
        //    jwtOptions.Authority = configuration.GetSection("EntraId").GetValue<string>("Authority");

        //},
        //identityOptions =>
        //{
        //    configuration.GetSection("EntraId").Bind(identityOptions);
        //}, jwtBearerScheme: "EntraId");

        //services.AddAuthorization(options =>
        //{
        //    var authBuilder = new AuthorizationPolicyBuilder([
        //        JwtBearerDefaults.AuthenticationScheme,
        //        "EntraId"]);

        //    var authPolicy = authBuilder.RequireAuthenticatedUser();

        //    options.DefaultPolicy = authPolicy.Build();
        //});
        services.AddAuthorization(options =>
        {
            options.AddPolicy("MultiScheme", policy =>
            {
                policy.AddAuthenticationSchemes("EntraId", "Local");
                policy.RequireAuthenticatedUser();
            });
        });

        services.AddPermissions(configuration);

        services.AddScoped<IAccessTokenProvider, TokenProvider>();

        return services;
    }
}
