using BlazorClient.Models;

namespace BlazorClient.Services;

public interface IUserService
{
    Task<List<User>?> GetAllAsync();
}
