using CQRS.Write.Data;
using Events;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Write.Features;

public record CloseAccount(Guid AccountId, string Reason) : IRequest<bool>;

public class CloseAccountHandler(ApplicationDbContext context, IPublishEndpoint publisher) : IRequestHandler<CloseAccount, bool>
{
    public async Task<bool> Handle(CloseAccount request, CancellationToken cancellationToken)
    {
        var account = await context.BankAccounts
                          .FirstOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken)
                      ?? throw new Exception("Account does not exist");

        if (account.Balance > 0) throw new Exception("Account balance must be zero before closing");

        account.IsActive = false;

        _ = Task.Run(() =>
                publisher.Publish(
                    new BankAccountClosed(account.Id,
                        request.Reason),
                    cancellationToken),
            cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
