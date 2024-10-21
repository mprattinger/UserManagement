﻿using FlintSoft.Permissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
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
                options.Authority = "https://localhost:7183/";
                options.Audience = jwtConf?.Audience;
            })
            .AddMicrosoftIdentityWebApi(jwtOptions =>
            {
                jwtOptions.MapInboundClaims = false;
                jwtOptions.Authority = configuration.GetSection("EntraId").GetValue<string>("Authority");

            },
            identityOptions =>
            {
                configuration.GetSection("EntraId").Bind(identityOptions);
            }, jwtBearerScheme: "EntraId");

        services.AddAuthorization(options =>
        {
            var authBuilder = new AuthorizationPolicyBuilder([
                JwtBearerDefaults.AuthenticationScheme,
                "EntraId"]);

            var authPolicy = authBuilder.RequireAuthenticatedUser();

            options.DefaultPolicy = authPolicy.Build();
        });

        services.AddPermissions(configuration);

        services.AddScoped<IAccessTokenProvider, TokenProvider>();

        return services;
    }
}
