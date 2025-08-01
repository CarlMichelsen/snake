namespace Api.Extensions;

public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
    {
        return services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var environment = context.HttpContext.RequestServices
                    .GetRequiredService<IHostEnvironment>();
                if (!environment.IsDevelopment())
                {
                    context.ProblemDetails.Detail = "An error occurred while processing your request.";
                }
            };
        });
    }
}