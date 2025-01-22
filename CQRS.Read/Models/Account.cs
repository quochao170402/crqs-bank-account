namespace CQRS.Read.Models;

public class Account
{
    public string Id { get; set; }

    public string AccountHolder { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public bool IsActive { get; set; }
}
