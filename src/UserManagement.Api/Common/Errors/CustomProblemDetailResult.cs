using System;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UserManagement.Api.Common.Errors;

public static class CustomProblemDetailResult
{
    public static IResult Problem(List<Error> errors, HttpContext httpContext) {
         if (errors.Count == 0)
            {
                return Results.Problem();
            }

            if (errors.All(err => err.Type == ErrorType.Validation))
            {
                return ValidationProblem(errors);
            }

            httpContext.Items[HttpContextItemKeys.Errors] = errors;

            var firstError = errors[0];
            return Problem(firstError);
    }

    public static IResult Problem(Error error) {
        var statusCode = error.Type switch {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(statusCode: statusCode, title: error.Description);
    }

    public static IResult ValidationProblem(List<Error> errors) {
        var modelStateDictionary = new Dictionary<string, string[]>();

            foreach (var error in errors)
            {
                modelStateDictionary.Add(error.Code, [error.Description]);
            }

            return Results.ValidationProblem(modelStateDictionary);
    }
}
