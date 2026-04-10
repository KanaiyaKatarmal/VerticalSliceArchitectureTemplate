using Microsoft.AspNetCore.Http;
using VerticalSliceArchitectureTemplate.Abstractions;
using VerticalSliceArchitectureTemplate.Abstractions.Errors;

namespace VerticalSliceArchitectureTemplate.Extensions;

public static class ResultExtensions
{
    public static TOut Match<TOut>(this Result result, Func<TOut> onSuccess, Func<Error, TOut> onFailure)
        => result.IsSuccess ? onSuccess() : onFailure(result.Error);

    public static TOut Match<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> onSuccess, Func<Error, TOut> onFailure)
        => result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);

    public static IResult ToHttpResult<TValue>(this Result<TValue> result, Func<TValue, IResult> onSuccess)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : result.Error.ToHttpResult();
    }

    public static IResult ToHttpResult(this Result result, Func<IResult> onSuccess)
    {
        return result.IsSuccess ? onSuccess() : result.Error.ToHttpResult();
    }

    public static IResult ToHttpResult(this Error error) => error.Type switch
    {
        ErrorType.NotFound     => Results.NotFound(new { error.Code, error.Description }),
        ErrorType.Validation   => error is ValidationError ve
                                    ? Results.ValidationProblem(ve.Errors
                                        .GroupBy(e => e.Code)
                                        .ToDictionary(g => g.Key, g => g.Select(e => e.Description ?? string.Empty).ToArray()))
                                    : Results.UnprocessableEntity(new { error.Code, error.Description }),
        ErrorType.Conflict     => Results.Conflict(new { error.Code, error.Description }),
        ErrorType.Unauthorized => Results.Unauthorized(),
        ErrorType.Forbidden    => Results.Forbid(),
        _                      => Results.Problem(error.Description, statusCode: StatusCodes.Status500InternalServerError)
    };
}
