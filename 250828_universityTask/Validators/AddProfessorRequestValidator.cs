using _250828_universityTask.Helpers;
using _250828_universityTask.Models.Requests;
using FluentValidation;

namespace _250828_universityTask.Validators
{
    public class AddProfessorRequestValidator : AbstractValidator<RegistrationRequest>
    {
        public AddProfessorRequestValidator()
        {
            RuleFor(x => x.Name).ApplyNameRules();
            RuleFor(x => x.UniId).ApplyIdRules();
        }
    }
}
