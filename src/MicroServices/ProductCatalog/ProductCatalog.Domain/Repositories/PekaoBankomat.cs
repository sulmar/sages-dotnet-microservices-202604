namespace ProductCatalog.Domain.Repositories;

public class PekaoBankomat : IWithdraw
{   
    public void Withdraw(decimal amount)
    {
        Console.WriteLine($"wyplata {amount}");
    }
}