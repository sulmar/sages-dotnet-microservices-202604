using System.Net.Http.Json;

namespace BlazorApp.Services;

public interface IProductService
{
    Task<List<Model.Product>?> GetAllAsync();
}


public class ApiProductService(HttpClient _httpClient) : IProductService
{
    public async Task<List<Model.Product>?> GetAllAsync() => await _httpClient.GetFromJsonAsync<List<Model.Product>>("api/products");
}

public interface ICartService
{
    event Action? CartChanged;

    Task AddToCartAsync(Model.Product product);

    Task RemoveFromCartAsync(int productId);

    Task<List<Model.CartItem>?> GetCartItemsAsync();

    Task CheckoutAsync();
}

public class ApiCartService(HttpClient _httpClient) : ICartService
{
    public event Action? CartChanged;

    public async Task AddToCartAsync(Model.Product product)
    {
        var response = await _httpClient.PostAsJsonAsync("api/cart/items", product);
        response.EnsureSuccessStatusCode();
        CartChanged?.Invoke();
    }

    public async Task RemoveFromCartAsync(int productId)
    {
        var response = await _httpClient.DeleteAsync($"api/cart/items/{productId}");
        response.EnsureSuccessStatusCode();
        CartChanged?.Invoke();
    }

    public async Task<List<Model.CartItem>?> GetCartItemsAsync() =>
        await _httpClient.GetFromJsonAsync<List<Model.CartItem>>("api/cart/items");

    public async Task CheckoutAsync()
    {
        var response = await _httpClient.PostAsync("api/cart/checkout", null);
        response.EnsureSuccessStatusCode();
        CartChanged?.Invoke();
    }
}