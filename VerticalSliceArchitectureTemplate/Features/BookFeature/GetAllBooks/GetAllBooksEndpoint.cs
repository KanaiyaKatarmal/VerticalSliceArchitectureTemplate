using VerticalSliceArchitectureTemplate.Abstractions;
using VerticalSliceArchitectureTemplate.Constants;
using VerticalSliceArchitectureTemplate.Extensions;
namespace VerticalSliceArchitectureTemplate.Features.BookFeature.GetAllBooks;

internal sealed class GetAllBooksEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("books", async (
            IHandler<GetAllBooksRequest, Result<GetAllBooksResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetAllBooksRequest(), cancellationToken);
            return result.ToHttpResult(Results.Ok);
        })
        .WithTags(ApiTags.Books)
        .Produces<GetAllBooksResponse>(StatusCodes.Status200OK);
    }
}
