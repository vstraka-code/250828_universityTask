using _250828_universityTask.Models.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _250828_universityTask.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // runs on POST and PUT req - if AUTH passes it gets handed here to validate if the body is valid
            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            {
                var endpoint = context.GetEndpoint(); // where it is trying to go
                var validators = endpoint?.Metadata.GetOrderedValidators(); // looks inside the metadata and pulls out all validators (like FluentValidation)

                if (validators != null && validators.Any())
                {
                    var bodyType = endpoint?.Metadata.GetBodyType(); // type expected (like addstudentrequest)

                    if (bodyType != null)
                    {
                        // reads JSON req body + converts it into expected type
                        var body = await context.Request.ReadFromJsonAsync(bodyType);

                        if (body == null)
                        {
                            throw new ArgumentNullException();
                        }

                        foreach (var validator in validators)
                        {
                            var result = await validator.ValidateAsync(new ValidationContext<object>(body)); // each validator is getting checked against its rules
                            if (!result.IsValid)
                            {
                                var errors = result.Errors
                                    .GroupBy(e => e.PropertyName)
                                    .ToDictionary(
                                        g => g.Key,
                                        g => g.Select(e => e.ErrorMessage).ToArray()
                                    );

                                throw new Exceptions.ValidationException(errors);
                            }
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
