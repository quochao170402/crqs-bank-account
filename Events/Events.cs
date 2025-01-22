namespace Events;

public abstract record Event(Guid StreamId);

public record BankAccountCreated(Guid AccountId, string Name, decimal InitialBalance, string Currency = "VND")
    : Event(AccountId);

public record Deposited(Guid AccountId, decimal Amount) : Event(AccountId);

public record Withdrew(Guid AccountId, Guid DestinationId, decimal Amount) : Event(AccountId);

public record BankAccountClosed(Guid AccountId, string Reason) : Event(AccountId);

public record BankAccountReopened(Guid AccountId) : Event(AccountId);

public record RetryDeposit(Guid AccountId, decimal Amount) : Event(AccountId);

public record RetryWithdraw(Guid AccountId, decimal Amount) : Event(AccountId);
