using VerticalSliceArchitectureTemplate.Abstractions;

namespace VerticalSliceArchitectureTemplate.Features.Book.CreateBook;

public sealed record BookCreatedEvent(Guid BookId, string Title, string Author) : IDomainEvent;
