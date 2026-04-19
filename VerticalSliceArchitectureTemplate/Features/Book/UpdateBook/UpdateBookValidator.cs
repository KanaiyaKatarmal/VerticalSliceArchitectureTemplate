using FluentValidation;

namespace VerticalSliceArchitectureTemplate.Features.Book.UpdateBook;

public class UpdateBookValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("Book Id is required");

        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(c => c.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(100).WithMessage("Author must not exceed 100 characters");

        RuleFor(c => c.ISBN)
            .NotEmpty().WithMessage("ISBN is required");
            
        RuleFor(c => c.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(c => c.PublishedYear)
            .GreaterThan(1000).WithMessage("Published year must be a valid year")
            .LessThanOrEqualTo(System.DateTime.UtcNow.Year).WithMessage("Published year cannot be in the future");
    }
}
