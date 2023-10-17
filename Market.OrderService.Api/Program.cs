using Market.DAL;
using Market.Mq;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

var appTask = app.RunAsync();

var bus = app.Services.CreateScope().ServiceProvider.GetService<IBus>();
await bus.Publish(new SubmitOrder
(
    Guid.NewGuid(), "", Guid.NewGuid()
));

await appTask;

class OrderService
{
    private List<Order> _orders = new();
    private readonly MarketDbContext _context;

    public OrderService(MarketDbContext context)
    {
        _context = context;
    }

    public async Task PlaceOrder(Order order)
    {
        var existingOrderIds =
            (await _context.Orders.Select(x => x.Id).ToListAsync()).ToHashSet();
        
        if (existingOrderIds.Contains(order.Id)) return;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
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