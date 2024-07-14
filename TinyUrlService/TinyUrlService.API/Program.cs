using Microsoft.Extensions.Logging.Console;

namespace TinyUrlService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddOpenTelemetry(options =>
                    {
                        options.IncludeScopes = true;
                    });
                    logging.AddConsole(options =>
                    {
                        options.FormatterName = ConsoleFormatterNames.Systemd;
                        options.IncludeScopes = true; // Include scopes when true
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
