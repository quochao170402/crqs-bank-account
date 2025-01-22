using CQRS.Write.Data;
using CQRS.Write.Entities;
using Events;
using MassTransit;
using MediatR;

namespace CQRS.Write.Features;

public record CreateAccountRequest(string Name, decimal InitBalance, string Currency = "VND") : IRequest<BankAccount>;

public class CreateAccountHandler(
    ApplicationDbContext context,
    IPublishEndpoint publisher)
    : IRequestHandler<CreateAccountRequest, BankAccount>
{
    public async Task<BankAccount> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var account = new BankAccount
        {
            AccountHolder = request.Name,
            Balance = request.InitBalance,
            Currency = request.Currency,
            IsActive = true,
        };

        await context.BankAccounts.AddAsync(account, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        _ = Task.Run(() =>
                publisher.Publish(
                    new BankAccountCreated(account.Id,
                        account.AccountHolder,
                        account.Balance,
                        account.Currency),
                    cancellationToken),
            cancellationToken);

        return account;
    }
}
