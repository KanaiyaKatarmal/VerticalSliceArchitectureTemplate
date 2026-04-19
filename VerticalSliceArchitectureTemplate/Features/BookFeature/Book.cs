namespace VerticalSliceArchitectureTemplate.Features.BookFeature;

public sealed class Book
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string ISBN { get; set; }
    public decimal Price { get; set; }
    public int PublishedYear { get; set; }
}
