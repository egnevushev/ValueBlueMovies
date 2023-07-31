using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validation.Validators;

public class GetAllRequestValidator : AbstractValidator<GetAllRequest>
{
    public GetAllRequestValidator()
    {
        RuleFor(x => x.Count)
            .InclusiveBetween(ValidationConstants.MinOnPageCount, ValidationConstants.MaxOnPageCount);
        
        RuleFor(x => x.LastId)
            .Must(ObjectIdValidation.IsValidObjectId!)
            .When(x => x.LastId is not null)
            .WithMessage("LastId value is not valid ObjectID");
    }
}