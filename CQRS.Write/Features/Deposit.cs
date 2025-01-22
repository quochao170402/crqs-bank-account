using CQRS.Write.Data;
using Events;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Write.Features;

public record Deposit(Guid AccountId, decimal Amount) : IRequest<bool>;

public class DepositHandler(ApplicationDbContext context, IPublishEndpoint publisher) : IRequestHandler<Deposit, bool>
{
    public async Task<bool> Handle(Deposit request, CancellationToken cancellationToken)
    {
        var account = await context.BankAccounts
                          .FirstOrDefaultAsync(x => x.Id == request.AccountId && x.IsActive, cancellationToken)
                      ?? throw new Exception("Account does not exist");

        account.Balance += request.Amount;

        _ = Task.Run(() =>
                publisher.Publish(
                    new Deposited(account.Id,
                        request.Amount),
                    cancellationToken),
            cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
