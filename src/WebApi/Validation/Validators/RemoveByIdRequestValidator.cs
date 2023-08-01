using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validation.Validators;

public class RemoveByIdRequestValidator : AbstractValidator<RemoveByIdRequest>
{
    public RemoveByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .Must(ObjectIdValidation.IsValidObjectId)
            .WithMessage("Id value is not valid ObjectID");
    }
}