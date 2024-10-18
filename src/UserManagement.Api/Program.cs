using ErrorOr;
using FlintSoft.Endpoints;
using System.Reflection;
using UserManagement.Api;
using UserManagement.Api.Common.Errors;
using UserManagement.Api.Features;
using UserManagement.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddPresentation();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddFeatures();

var app = builder.Build();

app.MapGet("/", () =>
{
    var errors = new List<Error>
     {
         Error.Failure("ABCD", "Fehler"),
         Error.Failure("EFGH", "Fehler2"),
         Error.Failure("IJKL", "Fehler3")
     };

    return errors.ToProblemDetails();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.UseExceptionHandler();

app.Run();
