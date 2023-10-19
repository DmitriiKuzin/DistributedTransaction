using Market.DAL;
using Market.Mq;
using MassTransit;

namespace Market.Delivery.Service;

public class AllocateDeliveryTimeslotConsumer: IConsumer<AllocateDeliveryTimeslot>
{
    private readonly MarketDbContext _dbContext;

    public AllocateDeliveryTimeslotConsumer(MarketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<AllocateDeliveryTimeslot> context)
    {
        if (context.Message.OrderId.ToString().StartsWith("3"))
        {
            await context.RespondAsync(new AllocationDeliveryTimeslotFailed(context.Message.OrderId));
            return;
        }

        _dbContext.DeliveryAllocations.Add(new DeliveryAllocation
        {
            OrderId = context.Message.OrderId,
            IsAllocated = true
        });
        await _dbContext.SaveChangesAsync();
        await context.RespondAsync(new AllocationDeliveryTimeslotSucceed(context.Message.OrderId));
    }
}