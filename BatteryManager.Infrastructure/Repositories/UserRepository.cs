using BatteryManager.Application.Users.Interfaces;
using BatteryManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatteryManager.Infrastructure.Repositories;

public class UserRepository : IUsersRepository
{
    private readonly BatteryDbContext _context;

    public UserRepository(BatteryDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByIdAsync(Guid id)
        => await _context.Users.FindAsync(id);

    public async Task CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
