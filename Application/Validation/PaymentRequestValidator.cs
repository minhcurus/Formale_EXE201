using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using FluentValidation;

namespace Application.Validation
{
    public class PaymentRequestValidator : AbstractValidator<PaymentDTO>
    {
        public PaymentRequestValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.ReturnUrl)
                .NotEmpty().WithMessage("Return URL is required.")
                .Must(BeAValidUrl).WithMessage("Return URL must be a valid URL.");

            RuleFor(x => x.BuyerName)
                .NotEmpty().WithMessage("Buyer name is required.");

            RuleFor(x => x.BuyerEmail)
                .NotEmpty().WithMessage("Buyer email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.BuyerPhone)
                .NotEmpty().WithMessage("Buyer phone is required.")
                .Matches(@"^\d{9,12}$").WithMessage("Phone number must be between 9 and 12 digits.");

            RuleFor(x => x.BuyerAddress)
                .NotEmpty().WithMessage("Buyer address is required.");

            RuleFor(x => x.Method)
                .IsInEnum().WithMessage("Invalid payment method.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
