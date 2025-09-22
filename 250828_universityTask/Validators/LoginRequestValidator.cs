using _250828_universityTask.Models.Requests;
using FluentValidation;

namespace _250828_universityTask.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Id).ApplyIdRules();
            RuleFor(x => x.Password).ApplyPasswordRules();
            RuleFor(x => x.Role).ApplyRoleRules();
        }
    }
}
