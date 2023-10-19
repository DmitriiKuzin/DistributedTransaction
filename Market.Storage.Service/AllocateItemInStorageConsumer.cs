using Market.DAL;
using Market.Mq;
using MassTransit;

namespace Market.Storage.Service;

public class AllocateItemInStorageConsumer: IConsumer<AllocateItemInStorage>
{
    private readonly MarketDbContext _dbContext;

    public AllocateItemInStorageConsumer(MarketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<AllocateItemInStorage> context)
    {
        if (context.Message.OrderId.ToString().StartsWith("2"))
        {
            await context.RespondAsync(new AllocationFailed(context.Message.OrderId));
            return;
        }

        _dbContext.Allocations.Add(new Allocation
        {
            OrderId = context.Message.OrderId,
            IsAllocated = true
        });
        await _dbContext.SaveChangesAsync();
        await context.RespondAsync(new AllocationSucceed(context.Message.OrderId));
    }
}