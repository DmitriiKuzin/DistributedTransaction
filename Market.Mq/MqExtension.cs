using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Market.Mq;

public static class MqExtension
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection sc, Action<IBusRegistrationConfigurator> conf = null)
    {
     
        sc.AddMassTransit(x =>
        {
            conf?.Invoke(x);
            // elided...
            x.UsingRabbitMq((context,cfg) =>
            {
                cfg.Host("rabbitmq", "/", h => {
                    h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USER"));
                    h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD"));
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        return sc;
    }
}