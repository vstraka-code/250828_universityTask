using _250828_universityTask.Helpers;
using _250828_universityTask.Models.Requests;
using FluentValidation;

namespace _250828_universityTask.Validators
{
    public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
    {
        public UpdateStudentRequestValidator()
        {
            RuleFor(x => x.Name).ApplyNameRules();

        }
    }
}
