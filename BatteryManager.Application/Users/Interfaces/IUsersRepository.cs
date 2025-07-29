using BatteryManager.Domain.Entities;

namespace BatteryManager.Application.Users.Interfaces;

public interface IUsersRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task CreateAsync(User user);
}