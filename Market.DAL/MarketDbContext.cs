using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Market.DAL;

public class MarketDbContext: DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var a = new NpgsqlConnectionStringBuilder();
        a.Host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
        a.Database = "market";
        a.Username = "postgres";
        a.Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        optionsBuilder.UseNpgsql(a.ToString());
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Allocation> Allocations { get; set; }
    public DbSet<DeliveryAllocation> DeliveryAllocations { get; set; }
}