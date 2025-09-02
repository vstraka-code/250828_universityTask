namespace _250828_universityTask.Middleware
{
    // app.UseValidationMiddleware();
    public static class ValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidationMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ValidationMiddleware>();
        }
    }
}
