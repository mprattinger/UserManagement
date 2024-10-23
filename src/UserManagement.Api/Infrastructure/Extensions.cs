using FlintSoft.Permissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Text;
using System.Text.Json;
using UserManagement.Api.Features.Auth.Models;
using UserManagement.Api.Infrastructure.Auth;
using UserManagement.Api.Infrastructure.Auth.Services;
using UserManagement.Api.Infrastructure.Data;

namespace UserManagement.Api.Infrastructure;

public static class Extensions
{
    public static async Task<IServiceCollection> AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
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

        var url = configuration.GetSection("EntraId").GetValue<string>("OpenIDConfUrl");
        var httpClient = new HttpClient();
        var res = await httpClient.GetStringAsync(url);
        var conf = JsonSerializer.Deserialize<OpenIdConfigurationResponse>(res);
        if (conf is null)
        {
            throw new Exception("Cannot load openid configuration from endpoint");
        }

        var jwksResp = await httpClient.GetStringAsync(conf.jwks_uri);
        var jwks = JsonSerializer.Deserialize<JsonWebKeySet>(jwksResp);
        if (jwks is null)
        {
            throw new Exception("Cannot load signing keys from jwks endpoint");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer("Local", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
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
                    ValidIssuer = conf.issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = jwks.Keys,
                    ValidateAudience = true,
                    ValidAudience = configuration.GetSection("EntraId").GetValue<string>("ClientId"),
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async (ctx) =>
                    {
                        Console.WriteLine(ctx.Exception.Message);
                    }
                };
            })
            ;

        services.AddAuthorization();

        services.AddPermissions(configuration);

        services.AddScoped<IAccessTokenProvider, TokenProvider>();

        return services;
    }
}
