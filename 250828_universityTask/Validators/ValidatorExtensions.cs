using FluentValidation;

namespace _250828_universityTask.Validators
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> ApplyStudentNameRules<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotEmpty().WithMessage("Student name is required.")
                .MaximumLength(50).WithMessage("Student name must not exceed 50 characters.");
        }
    }
}
