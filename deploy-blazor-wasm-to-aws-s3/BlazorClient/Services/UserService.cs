using BlazorClient.Models;
using System.Net.Http.Json;

namespace BlazorClient.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<User>?> GetAllAsync()
    {
        var users = await _httpClient.GetFromJsonAsync<List<User>>("users");
        return users;
    }
}
