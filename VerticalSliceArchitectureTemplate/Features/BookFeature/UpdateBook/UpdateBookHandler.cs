namespace VerticalSliceArchitectureTemplate.Features.BookFeature.UpdateBook;

using Microsoft.EntityFrameworkCore;
using VerticalSliceArchitectureTemplate.Abstractions;
using VerticalSliceArchitectureTemplate.Database;
using VerticalSliceArchitectureTemplate.Features.BookFeature;

public sealed record UpdateBookRequest(Guid Id, string? Title, string? Author, string? ISBN, decimal? Price, int? PublishedYear);
public sealed record UpdateBookResponse(Guid Id, string Title, string Author, string ISBN, decimal Price, int PublishedYear);

public sealed class UpdateBookHandler(ApplicationDbContext db) : IHandler<UpdateBookRequest, Result<UpdateBookResponse>>
{
    public async Task<Result<UpdateBookResponse>> HandleAsync(UpdateBookRequest command, CancellationToken cancellationToken)
    {
        var book = await db.Books.FindAsync([command.Id], cancellationToken);

        if (book is null)
            return Result.Failure<UpdateBookResponse>(BookErrors.NotFound(command.Id));

        if (command.Title is not null) book.Title = command.Title;
        if (command.Author is not null) book.Author = command.Author;
        if (command.ISBN is not null) book.ISBN = command.ISBN;
        if (command.Price is not null) book.Price = command.Price.Value;
        if (command.PublishedYear is not null) book.PublishedYear = command.PublishedYear.Value;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateBookResponse(book.Id, book.Title, book.Author, book.ISBN, book.Price, book.PublishedYear));
    }
}
