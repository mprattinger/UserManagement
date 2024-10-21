using ErrorOr;
using FlintSoft.Endpoints;
using Microsoft.Kiota.Abstractions.Authentication;
using UserManagement.Api.Common.Errors;
using UserManagement.Api.Features.Auth.Models;
using UserManagement.Api.Features.Auth.Services;

namespace UserManagement.Api.Features.Auth.Endpoints;

public class AuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (User? user, IUserService userService) =>
        {
            if (user is null)
            {
                return Error.Conflict("AUTH.REGISTER", "You must provide a user").ToProblemDetails();
            }

            var res = await userService.RegisterUser(user);
            if (res.IsError)
            {
                return res.Errors.ToProblemDetails();
            }

            return Results.Ok();
        });

        app.MapGet("/auth/register2", async (IAccessTokenProvider accessProvider, HttpContext context) =>
        {

            return Results.Ok();
        })
            .RequireAuthorization();

        app.MapPost("/auth/login", async (User? user, IUserService userService) =>
        {
            if (user is null)
            {
                return Error.Conflict("AUTH.LOGIN", "You must provide login information").ToProblemDetails();
            }

            var res = await userService.LoginUser(user);
            if (res.IsError)
            {
                return res.Errors.ToProblemDetails();
            }

            return Results.Ok(res.Value);
        });
    }
}
