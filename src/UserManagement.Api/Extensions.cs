using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using UserManagement.Api.Common.Errors;

namespace UserManagement.Api;

public static class Extensions
{
 public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        return services;
    }
}
