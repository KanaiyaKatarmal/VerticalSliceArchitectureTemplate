namespace VerticalSliceArchitectureTemplate.Features.BookFeature.DeleteBook;

using Microsoft.EntityFrameworkCore;
using VerticalSliceArchitectureTemplate.Abstractions;
using VerticalSliceArchitectureTemplate.Database;
using VerticalSliceArchitectureTemplate.Features.BookFeature;

public sealed record DeleteBookRequest(Guid Id);
public sealed record DeleteBookResponse(Guid Id);

public sealed class DeleteBookHandler(ApplicationDbContext db) : IHandler<DeleteBookRequest, Result<DeleteBookResponse>>
{
    public async Task<Result<DeleteBookResponse>> HandleAsync(DeleteBookRequest command, CancellationToken cancellationToken)
    {
        var book = await db.Books.FindAsync([command.Id], cancellationToken);

        if (book is null)
            return Result.Failure<DeleteBookResponse>(BookErrors.NotFound(command.Id));

        db.Books.Remove(book);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteBookResponse(book.Id));
    }
}
