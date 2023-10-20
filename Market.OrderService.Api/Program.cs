using Market.DAL;
using Market.Mq;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OrderService>();
builder.Services.AddDbContext<MarketDbContext>();
builder.Services.AddRabbitMq();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.MapPost("order", ([FromBody] Order order, OrderService orderService) => orderService.PlaceOrder(order));
app.MapGet("orders", (OrderService orderService) => orderService.GetOrders());
app.MapGet("getNewOrderId", (OrderService orderService) => orderService.GetNewOrderId());
app.MapGet("/getStorageAllocationInfo/{orderId:guid}",
    async (Guid orderId, MarketDbContext dbContext) =>
        await dbContext.Allocations.AsNoTracking().Where(x=>  x.OrderId == orderId).Select(x => x.IsAllocated).FirstOrDefaultAsync());
app.MapGet("/getDeliveryAllocationInfo/{orderId:guid}",
    async (Guid orderId, MarketDbContext dbContext) =>
        await dbContext.DeliveryAllocations.AsNoTracking().Where(x=>  x.OrderId == orderId).Select(x => x.IsAllocated).FirstOrDefaultAsync());

await MqExtension.WaitForRabbitReady();
var dbContext = app.Services.CreateScope().ServiceProvider.GetService<MarketDbContext>();
dbContext.Database.Migrate();
app.Run();



class OrderService
{
    private List<Order> _orders = new();
    private readonly MarketDbContext _context;
    private readonly IBus _bus;
    readonly IRequestClient<CheckPayment> _paymentCheckingClient;
    readonly IRequestClient<AllocateItemInStorage> _storageAllocatingClient;
    readonly IRequestClient<AllocateDeliveryTimeslot> _deliverySlotAllocatingClient;


    public OrderService(MarketDbContext context, IBus bus, IRequestClient<CheckPayment> paymentCheckingClient, IRequestClient<AllocateItemInStorage> storageAllocatingClient, IRequestClient<AllocateDeliveryTimeslot> deliverySlotAllocatingClient)
    {
        _context = context;
        _bus = bus;
        _paymentCheckingClient = paymentCheckingClient;
        _storageAllocatingClient = storageAllocatingClient;
        _deliverySlotAllocatingClient = deliverySlotAllocatingClient;
    }

    public async Task PlaceOrder(Order order)
    {
        var existingOrderIds =
            (await _context.Orders.Select(x => x.Id).ToListAsync()).ToHashSet();
        
        if (existingOrderIds.Contains(order.Id)) return;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        var (paymentSucceed, paymentFailed) = await _paymentCheckingClient.GetResponse<PaymentSucceed, PaymentFailed>(new CheckPayment(order.Id));
        if (!paymentSucceed.IsCompletedSuccessfully)
            return;
        var sas = _storageAllocatingClient.GetResponse<AllocationSucceed, AllocationFailed>(new AllocateItemInStorage(order.Id));
        var (allocationSucceed, allocationFailed) = await sas;
        if (!allocationSucceed.IsCompletedSuccessfully)
        {
            return;
        }
        var (allocationDeliverySucceed, allocationDeliveryFailed) = await _deliverySlotAllocatingClient.GetResponse<AllocationDeliveryTimeslotSucceed, AllocationDeliveryTimeslotFailed>(new AllocateDeliveryTimeslot(order.Id));
        if (!allocationDeliverySucceed.IsCompletedSuccessfully)
        {
            await _bus.Publish(new CancelStorageAllocation(order.Id));
            return;
        }
    }

    public List<Order> GetOrders()
    {
        return _orders;
    }

    public Guid GetNewOrderId()
    {
        return Guid.NewGuid();
    }
}