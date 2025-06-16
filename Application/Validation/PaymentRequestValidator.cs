using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using FluentValidation;

namespace Application.Validation
{
    public class PaymentRequestValidator : AbstractValidator<PaymentRequestDTO>
    {
        public PaymentRequestValidator()
        {

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.ReturnUrl)
                .NotEmpty().WithMessage("Return URL is required.")
                .Must(BeAValidUrl).WithMessage("Return URL must be a valid URL.");

        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
