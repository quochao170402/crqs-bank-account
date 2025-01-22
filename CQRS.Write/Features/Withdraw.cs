using CQRS.Write.Data;
using Events;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Write.Features;

public record Withdraw(Guid AccountId, Guid DestinationId, decimal Amount) : IRequest<bool>;

public class WithdrawHandler(ApplicationDbContext context, IPublishEndpoint publisher) : IRequestHandler<Withdraw, bool>
{
    public async Task<bool> Handle(Withdraw request, CancellationToken cancellationToken)
    {
        var account = await context.BankAccounts
                          .FirstOrDefaultAsync(x => x.Id == request.AccountId && x.IsActive, cancellationToken)
                      ?? throw new Exception("Account does not exist");

        if (account.Balance < request.Amount) throw new Exception("Insufficient balance");

        var destination = await context.BankAccounts
                              .FirstOrDefaultAsync(x => x.Id == request.DestinationId && x.IsActive, cancellationToken)
                          ?? throw new Exception("Destination account does not exist");

        account.Balance -= request.Amount;
        destination.Balance += request.Amount;

        _ = Task.Run(() =>
                publisher.Publish(
                    new Withdrew(account.Id,
                        destination.Id,
                        request.Amount),
                    cancellationToken),
            cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
