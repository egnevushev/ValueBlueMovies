using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validation.Validators;

public class DatePeriodRequestValidator : AbstractValidator<DatePeriodRequest>
{
    public DatePeriodRequestValidator()
    {
        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .When(x => x.End is not null);
        
        RuleFor(x => x.Count)
            .InclusiveBetween(ValidationConstants.MinOnPageCount, ValidationConstants.MaxOnPageCount);
        
        RuleFor(x => x.LastId)
            .Must(ObjectIdValidation.IsValidObjectId!)
            .When(x => x.LastId is not null)
            .WithMessage("LastId value is not valid ObjectID");
    }
}