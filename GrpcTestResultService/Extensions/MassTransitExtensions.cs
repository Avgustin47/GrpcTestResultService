using MassTransit;

namespace GrpcTestResultService.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(
            this IServiceCollection services, 
            Action<IBusRegistrationConfigurator>? configConsumer = null)
        {
            services.AddMassTransit(busConfig =>
            {
                busConfig.SetKebabCaseEndpointNameFormatter();
                configConsumer?.Invoke(busConfig);
                busConfig.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", 5672, "/", h =>
                    {
                        h.Username("admin");
                        h.Password("admin123");
                    });
                    cfg.UseMessageRetry(retry =>
                    {
                        retry.Interval(3, TimeSpan.FromSeconds(5));
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}
