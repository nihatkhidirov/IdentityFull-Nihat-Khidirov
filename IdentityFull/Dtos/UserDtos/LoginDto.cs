using FluentValidation;

namespace FullIdentity.Dtos.UserDtos;

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(l => l.Email).EmailAddress();
        RuleFor(l => l.Password).MaximumLength(20).MinimumLength(5);
    }
}
