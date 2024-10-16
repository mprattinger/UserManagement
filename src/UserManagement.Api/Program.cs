using ErrorOr;
using UserManagement.Api;
using UserManagement.Api.Common.Errors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddPresentation();

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

app.Run();
