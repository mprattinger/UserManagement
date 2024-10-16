using System;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UserManagement.Api.Common.Errors;

public static class CustomProblemDetailResult
{
    public static IResult ToProblemDetails(this Error error)
    {
        var err = new List<Error> { error };
        return err.ToProblemDetails();
    }

    public static IResult ToProblemDetails(this List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Results.Problem();
        }

        if (errors.All(err => err.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        var firstError = errors[0];
        var statusCode = translateStatusCode(firstError);

        var allTheErrors = new Dictionary<string, object?>();
        foreach (var error in errors)
        {
            allTheErrors.Add(error.Code, new[] { error.Description });
        }

        return Results.Problem(statusCode: statusCode,
        title: firstError.Description,
        extensions: allTheErrors);

    }

    public static IResult Problem(Error error)
    {
        var statusCode = translateStatusCode(error);

        return Results.Problem(statusCode: statusCode, title: error.Description);
    }

    static int translateStatusCode(Error error)
    {
        return error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    public static IResult ValidationProblem(List<Error> errors)
    {
        var modelStateDictionary = new Dictionary<string, string[]>();

        foreach (var error in errors)
        {
            modelStateDictionary.Add(error.Code, [error.Description]);
        }

        return Results.ValidationProblem(modelStateDictionary);
    }
}
