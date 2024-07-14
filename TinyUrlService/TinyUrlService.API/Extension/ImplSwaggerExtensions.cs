using Microsoft.OpenApi.Models;

namespace TinyUrlService.API.Extension;

public static class ImplSwaggerExtensions
{
    public static IServiceCollection UseImplSwagger(this IServiceCollection services)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "TinyURL API", Version = "v1" });
        });

        return services;
    }

    public static IApplicationBuilder UseImplSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));
        _ = env ?? throw new ArgumentNullException(nameof(env));

        if (env.IsProduction())
        {
            return app;
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "TinyURL API V1");
            c.RoutePrefix = string.Empty;
        });

        return app;
    }
}