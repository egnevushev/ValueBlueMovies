using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validation.Validators;

public class FindByIdRequestValidator : AbstractValidator<FindByIdRequest>
{
    public FindByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .Must(ObjectIdValidation.IsValidObjectId)
            .WithMessage("Id value is not valid ObjectID");
    }
}