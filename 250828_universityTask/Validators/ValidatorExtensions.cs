using FluentValidation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace _250828_universityTask.Validators
{
    public static class ValidatorExtensions
    {
        // IRuleBuilder = FluentValidation interface, allow chaining validation rules
        public static IRuleBuilderOptions<T, string> ApplyNameRules<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }

        public static IRuleBuilderOptions<T, int?> ApplyIdRules<T>(this IRuleBuilder<T, int?> rule)
        {
            return rule
                .NotEmpty().WithMessage("Id must have a value.")
                .NotNull().WithMessage("Id must have a value.")
                .GreaterThan(0).WithMessage("Id must be bigger than 0.");
        }

        public static IRuleBuilderOptions<T, string> ApplyPasswordRules<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(30).WithMessage("Password must not exceed 30 characters.");
        }

        public static IRuleBuilderOptions<T, string> ApplyRoleRules<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotEmpty().WithMessage("Role is required.")
                .MaximumLength(30).WithMessage("Student name must not exceed 30 characters.");
        }
    }
}
