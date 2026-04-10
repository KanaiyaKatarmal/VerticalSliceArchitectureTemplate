namespace VerticalSliceArchitectureTemplate.Features.BookFeature.CreateBook;

using Microsoft.EntityFrameworkCore;
using VerticalSliceArchitectureTemplate.Abstractions;
using VerticalSliceArchitectureTemplate.Database;

public sealed record CreateBookRequest(string Title, string Author, string ISBN, decimal Price, int PublishedYear);
public sealed record CreateBookResponse(Guid Id, string Title, string Author, string ISBN, decimal Price, int PublishedYear);

public sealed class CreateBookHandler(
    ApplicationDbContext db,
    IEventDispatcher events) : IHandler<CreateBookRequest, Result<CreateBookResponse>>
{
    public async Task<Result<CreateBookResponse>> HandleAsync(CreateBookRequest command, CancellationToken cancellationToken)
    {
        var book = new Book
        {
            Id = Guid.CreateVersion7(),
            Title = command.Title,
            Author = command.Author,
            ISBN = command.ISBN,
            Price = command.Price,
            PublishedYear = command.PublishedYear
        };

        db.Books.Add(book);
        await db.SaveChangesAsync(cancellationToken);

        await events.DispatchAsync(new BookCreatedEvent(book.Id, book.Title, book.Author), cancellationToken);

        return Result.Success(new CreateBookResponse(book.Id, book.Title, book.Author, book.ISBN, book.Price, book.PublishedYear));
    }
}
