namespace VerticalSliceArchitectureTemplate.Features.Book.GetBookById;

using Microsoft.EntityFrameworkCore;
using VerticalSliceArchitectureTemplate.Abstractions;
using VerticalSliceArchitectureTemplate.Database;
using VerticalSliceArchitectureTemplate.Features.Book;

public sealed record GetBookByIdRequest(Guid Id);
public sealed record GetBookByIdResponse(Guid Id, string Title, string Author, string ISBN, decimal Price, int PublishedYear);

public sealed class GetBookByIdHandler(ApplicationDbContext db) : IHandler<GetBookByIdRequest, Result<GetBookByIdResponse>>
{
    public async Task<Result<GetBookByIdResponse>> HandleAsync(GetBookByIdRequest command, CancellationToken cancellationToken)
    {
        var book = await db.Books
            .AsNoTracking()
            .Where(b => b.Id == command.Id)
            .Select(b => new GetBookByIdResponse(b.Id, b.Title, b.Author, b.ISBN, b.Price, b.PublishedYear))
            .FirstOrDefaultAsync(cancellationToken);

        if (book is null)
            return Result.Failure<GetBookByIdResponse>(BookErrors.NotFound(command.Id));

        return Result.Success(book);
    }
}
