using System.Net;
using System.Net.Http.Headers;
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
                cfg.Host(Environment.GetEnvironmentVariable("RABBITMQ_HOST"), "/", h => {
                    h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USER"));
                    h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD"));
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        return sc;
    }

    public static async Task WaitForRabbitReady()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("RABBITMQ_USER") + ":" +
                                                                      Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD"))));
        var url = $"http://{Environment.GetEnvironmentVariable("RABBITMQ_HOST")}:15672/api/health/checks/virtual-hosts";

        while (true)
        {
            try
            {
                Console.WriteLine("Checking rabbitmq");
                var res = await client.GetAsync(url);
                Console.WriteLine(await res.Content.ReadAsStringAsync());
                if (res.StatusCode == HttpStatusCode.OK) break;
            }
            catch
            {
                Console.WriteLine("Checking failed");
            }
            await Task.Delay(2000);
        }
    }
}