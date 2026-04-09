namespace Ordering.Domain;
public record Order(OrderItem[] Items)
{
    public string Id { get; set; }

    public decimal TotalAmount => Items.Sum(item => item.Amount);
}

public record OrderItem(int ProductId, int Quantity, decimal Price)
{
    public decimal Amount => Price * Quantity;
}