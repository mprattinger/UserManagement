﻿using FlintSoft.Permissions;
using UserManagement.Api.Features.Permissions.Service;

namespace UserManagement.Api.Features.Permissions;

public static class Extensions
{
    public static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IPermissionCheckService, PermissionService>();

        return services;
    }
}
