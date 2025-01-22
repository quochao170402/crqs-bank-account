using CQRS.Write.Entities.Common;

namespace CQRS.Write.Entities;

public class BankAccount : Entity
{
    public string AccountHolder { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "VND";
    public bool IsActive { get; set; } = true;
}