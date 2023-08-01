using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validation.Validators;

public class GetStatisticsPerDayRequestValidator : AbstractValidator<GetStatisticsPerDayRequest>
{
    public GetStatisticsPerDayRequestValidator()
    {
        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .When(x => x.Start is not null && x.End is not null);
    }
}