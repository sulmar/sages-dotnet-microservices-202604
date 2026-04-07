namespace ProductCatalog.Domain.Repositories;

public class PkoBpBankomat : IBankomat
{
    public void Deposit(decimal amount)
    {
        Console.WriteLine($"wplata {amount}");
    }

    public void Withdraw(decimal amount)
    {
        Console.WriteLine($"wyplata {amount}");
    }
}
