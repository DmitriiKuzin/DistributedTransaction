using Market.DAL;
using Market.Mq;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Market.Storage.Service;

public class CancelStorageAllocationConsumer: IConsumer<CancelStorageAllocation>
{
    private readonly MarketDbContext _dbContext;

    public CancelStorageAllocationConsumer(MarketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CancelStorageAllocation> context)
    {
        var allocation = await _dbContext.Allocations.FirstAsync(x => x.OrderId == context.Message.OrderId);
        allocation.IsAllocated = false;
        await _dbContext.SaveChangesAsync();
    }
}