using FluentValidation;
using TSU360.DTOs.Auth;
using TSU360.Models.Enums;

namespace TSU360.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required");

            RuleFor(x => x.Birthday)
                .NotEmpty().WithMessage("Birthday is required")
                .Must(BeValidBirthday).WithMessage("You must be at least 13 years old");

            RuleFor(x => x.Year)
                .InclusiveBetween(1, 5).WithMessage("Year must be between 1 and 5");

            RuleFor(x => x.Faculty)
             .Must(f => Enum.GetNames(typeof(Faculty)).Contains(f))
             .WithMessage("Invalid faculty. Valid options: " +
                         string.Join(", ", Enum.GetNames(typeof(Faculty))));

            RuleFor(x => x.Degree)
                .Must(d => Enum.GetNames(typeof(Degree)).Contains(d))
                .WithMessage("Invalid degree. Valid options: " +
                            string.Join(", ", Enum.GetNames(typeof(Degree))));
        }

        private bool BeValidBirthday(DateTime birthday)
        {
            return birthday <= DateTime.Today.AddYears(-13);
        }
    }
}