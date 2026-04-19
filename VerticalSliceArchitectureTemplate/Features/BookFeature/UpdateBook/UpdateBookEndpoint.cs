using VerticalSliceArchitectureTemplate.Abstractions;
using VerticalSliceArchitectureTemplate.Constants;
using VerticalSliceArchitectureTemplate.Extensions;
namespace VerticalSliceArchitectureTemplate.Features.BookFeature.UpdateBook;

internal sealed class UpdateBookEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("books/{id:guid}", async (
            Guid id,
            UpdateBookRequest request,
            IHandler<UpdateBookRequest, Result<UpdateBookResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(request with { Id = id }, cancellationToken);
            return result.ToHttpResult(Results.Ok);
        })
        .WithTags(ApiTags.Books)
        .Produces<UpdateBookResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status422UnprocessableEntity);
    }
}
