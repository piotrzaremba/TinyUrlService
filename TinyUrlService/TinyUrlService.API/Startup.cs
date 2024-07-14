using System.Reflection;
using TinyUrlService.API.Infrastructure;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using FluentValidation;
using Microsoft.OpenApi.Models;
using MediatR;
using TinyUrlService.Domain.Services.Commands;
using TinyUrlService.Domain.Services.Handlers;
using TinyUrlService.Domain.Services;
using TinyUrlService.Domain.Services.Queries;
using TinyUrlService.Domain.Entities;
using Google.Protobuf.WellKnownTypes;
using TinyUrlService.API.Extension;

namespace TinyUrlService.API
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
            });

            services.UseImplSwagger();

            services.AddOpenTelemetry()
                    .ConfigureResource(resource => resource.AddService("TinyUrlService"))
                    .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation().AddConsoleExporter())
                    .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation());

            // Required for sessions.
            services.AddDistributedMemoryCache();
            services.AddCors(options =>
            {
                options.AddPolicy("ReactPolicy",
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = ".TinyUrlService.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // No SSL certs required.
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.None; // We are not sending any sensative data.
                options.Cookie.SecurePolicy = CookieSecurePolicy.None; // I don't want to create a self signed cert, otherwise changes this to Always.
            });

            DependencyInjection(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseImplSwagger(env);
            }
            else
            {
                app.UseMiddleware<ExceptionMiddleware>();
            }

            app.UseRouting();

            app.UseCors("ReactPolicy");

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void DependencyInjection(IServiceCollection services)
        {
            services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()); });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddSingleton<IUrlService, UrlService>();

            services.AddScoped(typeof(IRequestHandler<CreateShortUrlCommand, string>), typeof(CreateShortUrlHandler));
            services.AddScoped(typeof(IRequestHandler<DeleteShortUrlCommand, bool>), typeof(DeleteShortUrlHandler));
            services.AddScoped(typeof(IRequestHandler<GetLongUrlQuery, UrlMapping>), typeof(GetLongUrlHandler));
            services.AddScoped(typeof(IRequestHandler<GetStatisticsQuery, Dictionary<string, UrlMapping>>), typeof(GetStatisticsHandler));

            services.AddScoped<IValidator<CreateShortUrlCommand>, CreateShortUrlValidator>();
            services.AddScoped<IValidator<DeleteShortUrlCommand>, DeleteShortUrlValidator>();
            services.AddScoped<IValidator<GetLongUrlQuery>, GetLongUrlValidator>();
            services.AddScoped<IValidator<GetStatisticsQuery>, GetStatisticsValidator>();

            services.AddTransient(typeof(IPipelineBehavior<CreateShortUrlCommand, string>), typeof(ValidationBehavior<CreateShortUrlCommand, string>));
            services.AddTransient(typeof(IPipelineBehavior<DeleteShortUrlCommand, bool>), typeof(ValidationBehavior<DeleteShortUrlCommand, bool>));
            services.AddTransient(typeof(IPipelineBehavior<GetLongUrlQuery, UrlMapping>), typeof(ValidationBehavior<GetLongUrlQuery, UrlMapping>));
            services.AddTransient(typeof(IPipelineBehavior<GetStatisticsQuery, Dictionary<string, UrlMapping>>), typeof(ValidationBehavior<GetStatisticsQuery, Dictionary<string, UrlMapping>>));
        }
    }
}
