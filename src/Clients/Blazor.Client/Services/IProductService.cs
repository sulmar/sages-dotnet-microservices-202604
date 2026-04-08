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
    Task AddToCartAsync(Model.Product product);        
}

public class ApiCartService(HttpClient _httpClient) : ICartService
{
    public async Task AddToCartAsync(Model.Product product) => await _httpClient.PostAsJsonAsync("api/cart/items", product);
}