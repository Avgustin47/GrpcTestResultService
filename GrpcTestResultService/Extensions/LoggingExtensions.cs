using Serilog;
using Serilog.Events;

namespace GrpcTestResultService.Extensions
{
    public static class LoggingExtensions
    {
        public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Mircosoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.LifeTime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Application", "GrpcTestResultService")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("logs/logs-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();
            builder.Host.UseSerilog();
            return builder;
        }
    }
}
