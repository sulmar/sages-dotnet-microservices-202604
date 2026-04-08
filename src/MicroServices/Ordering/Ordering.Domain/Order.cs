namespace Ordering.Domain;
public record Order(OrderItem[] Items)
{
    public string Id { get; set; }
}

public record OrderItem(int ProductId, int Quantity, decimal Price);