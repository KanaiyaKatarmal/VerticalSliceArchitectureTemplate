using VerticalSliceArchitectureTemplate.Abstractions;
using VerticalSliceArchitectureTemplate.Constants;
using VerticalSliceArchitectureTemplate.Extensions;
namespace VerticalSliceArchitectureTemplate.Features.BookFeature.CreateBook;

internal sealed class CreateBookEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("books", async (
            IHandler<CreateBookRequest, Result<CreateBookResponse>> handler,
            CreateBookRequest command,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(command, cancellationToken);
            return result.ToHttpResult(book => Results.Created($"/books/{book.Id}", book));
        })
        .WithTags(ApiTags.Books)
        .Produces<CreateBookResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status422UnprocessableEntity);
    }
}
