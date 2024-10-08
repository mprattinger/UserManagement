using ErrorOr;
using UserManagement.Api;
using UserManagement.Api.Common.Errors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddPresentation();

var app = builder.Build();

app.MapGet("/", (HttpContext ctx) => {
    
var errors = new List<Error>();
errors.Add(Error.Failure("ABCD", "Fehler"));
errors.Add(Error.Failure("EFGH", "Fehler2"));
errors.Add(Error.Failure("IJKL", "Fehler3"));

    return CustomProblemDetailResult.Problem(errors,ctx);
     });

app.Run();
