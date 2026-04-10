using VerticalSliceArchitectureTemplate.Abstractions;

namespace VerticalSliceArchitectureTemplate.Features.BookFeature.CreateBook;

public sealed record BookCreatedEvent(Guid BookId, string Title, string Author) : IDomainEvent;
