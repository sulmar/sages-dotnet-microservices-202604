namespace ShoppingCart.Domain;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }    
}


public class CartItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }

}

