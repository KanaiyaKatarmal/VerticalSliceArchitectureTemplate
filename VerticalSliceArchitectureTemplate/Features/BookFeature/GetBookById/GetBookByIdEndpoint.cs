using VerticalSliceArchitectureTemplate.Abstractions;
using VerticalSliceArchitectureTemplate.Constants;
using VerticalSliceArchitectureTemplate.Extensions;
namespace VerticalSliceArchitectureTemplate.Features.BookFeature.GetBookById;

internal sealed class GetBookByIdEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("books/{id:guid}", async (
            Guid id,
            IHandler<GetBookByIdRequest, Result<GetBookByIdResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetBookByIdRequest(id), cancellationToken);
            return result.ToHttpResult(Results.Ok);
        })
        .WithTags(ApiTags.Books)
        .Produces<GetBookByIdResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
