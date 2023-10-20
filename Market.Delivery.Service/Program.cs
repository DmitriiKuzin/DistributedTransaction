using Market.DAL;
using Market.Delivery.Service;
using Market.Mq;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<MarketDbContext>();
        services.AddRabbitMq(x =>
        {
            x.AddRequestClient<AllocateDeliveryTimeslot>();
            x.AddConsumer<AllocateDeliveryTimeslotConsumer>();
        });
    })
    .Build();

await MqExtension.WaitForRabbitReady();
host.Run();