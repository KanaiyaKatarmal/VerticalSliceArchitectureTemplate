using FluentValidation;

namespace VerticalSliceArchitectureTemplate.Features.Book.GetBookById;

public class GetBookByIdValidator : AbstractValidator<GetBookByIdRequest>
{
    public GetBookByIdValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("Book Id is required");
    }
}
