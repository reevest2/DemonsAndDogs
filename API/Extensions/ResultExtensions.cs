using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Extensions;

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return ToErrorResult<T>(result.Error!);
    }

    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new NoContentResult();

        return ToErrorResult(result.Error!);
    }

    public static ActionResult<TOut> ToActionResult<TIn, TOut>(this Result<TIn> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return ToErrorResult<TOut>(result.Error!);
    }

    private static ActionResult<T> ToErrorResult<T>(ServiceError error)
    {
        var problemDetails = new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Message,
            Status = MapStatusCode(error.Code)
        };

        return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
    }

    private static IActionResult ToErrorResult(ServiceError error)
    {
        var problemDetails = new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Message,
            Status = MapStatusCode(error.Code)
        };

        return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
    }

    private static int MapStatusCode(string errorCode) => errorCode switch
    {
        ErrorCodes.NotFound => StatusCodes.Status404NotFound,
        ErrorCodes.InvalidInput => StatusCodes.Status400BadRequest,
        ErrorCodes.Unsupported => StatusCodes.Status422UnprocessableEntity,
        ErrorCodes.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
