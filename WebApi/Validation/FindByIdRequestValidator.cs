using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validation;

public class FindByIdRequestValidator: AbstractValidator<FindByIdRequest>
{
    public FindByIdRequestValidator()
    {
        RuleFor(x => x.Id).Must(ObjectIdValidator.IsValidObjectId);
    }
}