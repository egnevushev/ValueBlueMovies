using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validation;

public class GetAllRequestValidator : AbstractValidator<GetAllRequest>
{
    public GetAllRequestValidator()
    {
        RuleFor(x => x.Count)
            .InclusiveBetween(1, 100);
        
        RuleFor(x => x.LastId)
            .Must(ObjectIdValidator.IsValidObjectId!)
            .When(x => x.LastId is not null);
    }
}