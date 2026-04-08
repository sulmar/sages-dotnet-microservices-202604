using ShoppingCart.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace ShoppingCart.Domain;

public interface ICartService
{
    Task Checkout(string sessionId);
}


public class CartService(ICartItemRepository repository, IHttpClientFactory factory) : ICartService
{
    public async Task Checkout(string sessionId)
    {
        // 1. Pobierz zawartość koszyka dla danego sessionId
        var cartItems = await repository.GetItemsAsync(sessionId);

        // 2. Utwórz zamówienie na podstawie zawartości koszyka
        var order = new 
        {            
            Items = cartItems.Select(ci => new 
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,

                // TODO: pobrac cene ze snapshota produktu, a nie z koszyka
                Price = 4.99m,
            }).ToList(),            
        };

        // https://json-schema.org/learn/getting-started-step-by-step

        var http = factory.CreateClient("OrderingApi");

        // 3. Wyślij zamówienie do systemu zamówień (np. poprzez RabbitMQ)
        var response = await http.PostAsJsonAsync("/api/orders", order);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Blad skladania zamowienia");
        }

        

    }
}