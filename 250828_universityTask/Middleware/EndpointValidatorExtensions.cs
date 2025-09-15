using FluentValidation;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace _250828_universityTask.Middleware
{
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
