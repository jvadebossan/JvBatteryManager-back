using BatteryManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatteryManager.Infrastructure.Repositories;

public class BatteryDbContext : DbContext
{
    public BatteryDbContext(DbContextOptions<BatteryDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
}
