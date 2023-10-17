using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<OrderService>();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.MapPost("order", ([FromBody] Order order, OrderService orderService) => orderService.PlaceOrder(order));
app.MapGet("orders", (OrderService orderService) => orderService.GetOrders());
app.MapGet("getNewOrderId", (OrderService orderService) => orderService.GetNewOrderId());

app.Run();

class OrderService
{
    private List<Order> _orders = new();
    private HashSet<Guid> _existingOrderIds = new();

    public void PlaceOrder(Order order)
    {
        if (_existingOrderIds.Contains(order.Id)) return;
        _orders.Add(order);
        _existingOrderIds.Add(order.Id);
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

class Order
{
    public Guid Id { get; set; }
    public List<ItemEntry> ItemEntries { get; set; }
}

class ItemEntry
{
    public Guid ItemId { get; set; }
    public int Count { get; set; }
}

class Item
{
    public Guid Id { get; set; }
}