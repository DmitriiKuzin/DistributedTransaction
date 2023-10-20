using Market.DAL;
using Market.Mq;
using Market.Storage.Service;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<MarketDbContext>();
        services.AddRabbitMq(x =>
    {
        x.AddRequestClient<AllocateItemInStorage>();
        x.AddConsumer<AllocateItemInStorageConsumer>();
        x.AddConsumer<CancelStorageAllocationConsumer>();
    }); })
    .Build();

await MqExtension.WaitForRabbitReady();
host.Run();
