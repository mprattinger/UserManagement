using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Features.Auth.Models;
using UserManagement.Api.Infrastructure.Data;
using UserManagement.Api.Infrastructure.Data.Services;

namespace UserManagement.Api.Features.Auth.Services;

public interface IUserService
{
    Task<ErrorOr<bool>> RegisterUser(User user);
    Task<ErrorOr<LoginResponse>> LoginUser(User user);
}

public class UserService(DataContext dataContext, IPasswordService passwordService, ITokenGenerator tokenGenerator) : IUserService
{
    public async Task<ErrorOr<LoginResponse>> LoginUser(User user)
    {
        if (!await userExists(user))
        {
            return Error.NotFound("AUTH.USEREXISTS", "User doesn't exists");
        }

        var dbuser = await dataContext.Users.FirstOrDefaultAsync(x => x.Username == x.Username);
        if (dbuser is null)
        {
            return Error.NotFound("AUTH.USEREXISTS", "User doesn't exists");
        }

        var pw = passwordService.VerifyPassword(dbuser.Password, user.Password);
        if (pw == PasswordVerificationResult.Failed)
        {
            return Error.Unauthorized();
        }


        var token = tokenGenerator.GenerateToken(dbuser);

        var resp = new LoginResponse()
        {
            Username = dbuser.Username,
            Token = token
        };

        return resp;
    }

    public async Task<ErrorOr<bool>> RegisterUser(User user)
    {
        if (await userExists(user))
        {
            return Error.Conflict("AUTH.USEREXISTS", "User already exists");
        }

        user.Id = Guid.NewGuid();
        user.Password = passwordService.HashPassword(user.Password);

        dataContext.Users.Add(user);
        await dataContext.SaveChangesAsync();

        return true;
    }

    private async Task<bool> userExists(User user)
    {
        return await dataContext.Users.AnyAsync(x => x.Username == user.Username);
    }
}
