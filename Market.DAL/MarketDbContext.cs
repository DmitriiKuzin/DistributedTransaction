using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Market.DAL;

public class MarketDbContext: DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var a = new NpgsqlConnectionStringBuilder();
        a.Host = "localhost";
        a.Database = "market";
        optionsBuilder.UseNpgsql(a.ToString());
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Order> Orders { get; set; }
}