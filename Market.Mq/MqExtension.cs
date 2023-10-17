using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Market.Mq;

public static class MqExtension
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection sc)
    {
     
        sc.AddMassTransit(x =>
        {
            x.AddSaga<OrderSaga>();
            x.SetInMemorySagaRepositoryProvider();
            // elided...
            x.UsingRabbitMq((context,cfg) =>
            {
                cfg.ReceiveEndpoint("manually-configured", e =>
                {
                    // configure any required middleware components next
                    e.UseMessageRetry(r => r.Interval(5, 1000));
            
                    // configure the saga last
                    e.ConfigureSaga<OrderSaga>(context);
                });
                cfg.Host("localhost", "/", h => {
                    h.Username("user");
                    h.Password("rZx9EArHfiBRRDPy");
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        return sc;
    }
}