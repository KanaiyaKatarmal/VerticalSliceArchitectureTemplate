using VerticalSliceArchitectureTemplate.Abstractions;
using VerticalSliceArchitectureTemplate.Constants;
using VerticalSliceArchitectureTemplate.Extensions;
namespace VerticalSliceArchitectureTemplate.Features.Book.DeleteBook;

internal sealed class DeleteBookEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("books/{id:guid}", async (
            Guid id,
            IHandler<DeleteBookRequest, Result<DeleteBookResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new DeleteBookRequest(id), cancellationToken);
            return result.ToHttpResult(_ => Results.NoContent());
        })
        .WithTags(ApiTags.Books)
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
