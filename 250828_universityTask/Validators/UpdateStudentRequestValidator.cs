using _250828_universityTask.Models.Requests;
using FluentValidation;

namespace _250828_universityTask.Validators
{
    public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
    {
        public UpdateStudentRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Student name is required.")
                .MaximumLength(50).WithMessage("Student name must not exceed 50 characters.");
        }
    }
}
