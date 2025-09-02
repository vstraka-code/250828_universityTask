using FluentValidation;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace _250828_universityTask.Middleware
{
    // runs on POST and PUT req - if AUTH passes it gets handed here to validate if the body is valid
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            {
                var endpoint = context.GetEndpoint(); // where it is trying to go
                var validators = endpoint?.Metadata.GetOrderedValidators(); // looks inside the metadata and pulls out all validators

                if (validators != null && validators.Any())
                {
                    var bodyType = endpoint.Metadata.GetBodyType(); // type expected (like addstudentrequest)

                    if (bodyType != null)
                    {
                        var body = await context.Request.ReadFromJsonAsync(bodyType);
                        foreach (var validator in validators)
                        {
                            var result = await validator.ValidateAsync(new ValidationContext<object>(body!)); // each validator is getting checked against its rules
                            if (!result.IsValid)
                            {
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                await context.Response.WriteAsJsonAsync(result.Errors.Select(e => e.ErrorMessage));
                                return;
                            }
                        }
                    }
                }
            }

            await _next(context);
        }
    }

    // Helper to extract validators from endpoint metadata
    public static class EndpointValidatorExtensions
    {
        // looks through all metadata attached to endpoint + extracts everything that implements IValidator
        public static IEnumerable<IValidator> GetOrderedValidators(this EndpointMetadataCollection metadata)
        {
            return metadata.OfType<IValidator>();
        }

        // returns the type
        public static Type? GetBodyType(this EndpointMetadataCollection metadata)
        {
            var controllerActionDescriptor = metadata
                .OfType<ControllerActionDescriptor>()
                .FirstOrDefault();

            return controllerActionDescriptor?.Parameters
                .FirstOrDefault(p => p.BindingInfo?.BindingSource?.Id == "Body")
                ?.ParameterType;
        }
    }
}
