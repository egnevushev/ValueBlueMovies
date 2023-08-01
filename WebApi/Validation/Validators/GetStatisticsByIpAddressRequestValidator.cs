using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validation.Validators;

public class GetStatisticsByIpAddressRequestValidator : AbstractValidator<GetStatisticsByIpAddressRequest>
{
    public GetStatisticsByIpAddressRequestValidator()
    {
        RuleFor(x => x.IpAddress).NotNull().NotEmpty();
    }
}