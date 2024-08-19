using FluentValidation;

namespace FullIdentity.Dtos.UserDtos;

public class SignUpDto
{
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class SignUpDtoValidator : AbstractValidator<SignUpDto>
{
    public SignUpDtoValidator()
    {
        RuleFor(s => s.Fullname).NotEmpty().MaximumLength(30);
        RuleFor(s => s.Email).NotEmpty().EmailAddress();
        RuleFor(s => s.Password).MaximumLength(20).MinimumLength(5);
        RuleFor(m => m.ConfirmPassword).Must((model, field) => field == model.Password).WithMessage("Passwords do not match");
    }
}