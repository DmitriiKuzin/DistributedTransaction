using Market.Mq;
using Market.Payment.Service;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddRabbitMq(x =>
    {
        x.AddRequestClient<CheckPayment>();
        x.AddConsumer<CheckPaymentConsumer>();
    }); })
    .Build();

host.Run();
