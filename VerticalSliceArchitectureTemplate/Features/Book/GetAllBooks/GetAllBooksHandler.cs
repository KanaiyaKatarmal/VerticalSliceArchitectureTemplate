namespace VerticalSliceArchitectureTemplate.Features.Book.GetAllBooks;

using Microsoft.EntityFrameworkCore;
using VerticalSliceArchitectureTemplate.Abstractions;
using VerticalSliceArchitectureTemplate.Database;

public sealed record GetAllBooksRequest;
public sealed record GetAllBooksResponse(IReadOnlyList<BookSummary> Books);
public sealed record BookSummary(Guid Id, string Title, string Author, decimal Price, int PublishedYear);

public sealed class GetAllBooksHandler(ApplicationDbContext db) : IHandler<GetAllBooksRequest, Result<GetAllBooksResponse>>
{
    public async Task<Result<GetAllBooksResponse>> HandleAsync(GetAllBooksRequest command, CancellationToken cancellationToken)
    {
        var books = await db.Books
            .AsNoTracking()
            .Select(b => new BookSummary(b.Id, b.Title, b.Author, b.Price, b.PublishedYear))
            .ToListAsync(cancellationToken);

        return Result.Success(new GetAllBooksResponse(books));
    }
}
