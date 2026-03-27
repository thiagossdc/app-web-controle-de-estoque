using System.Net.Http.Headers;
using System.Net.Http.Json;
using Estoque.Api.Contracts;
using Estoque.Api.Tests.Infrastructure;

namespace Estoque.Api.Tests.Integration;

public class AuthAndProductsTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public AuthAndProductsTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithSeedAdmin_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin@estoque.com", "Admin@123"));
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload!.AccessToken));
        Assert.Equal("Admin", payload.Role);
    }

    [Fact]
    public async Task Products_WithAuthAndPagination_ReturnsPagedData()
    {
        var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin@estoque.com", "Admin@123"));
        login.EnsureSuccessStatusCode();
        var token = (await login.Content.ReadFromJsonAsync<LoginResponse>())!.AccessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/products?page=1&pageSize=1");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<PagedResponse<object>>();
        Assert.NotNull(content);
        Assert.Equal(1, content!.Page);
        Assert.Equal(1, content.PageSize);
        Assert.True(content.TotalItems >= 1);
        Assert.Single(content.Items);
    }
}
