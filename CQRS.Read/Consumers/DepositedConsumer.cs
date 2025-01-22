using CQRS.Read.Services;
using Events;
using MassTransit;

namespace CQRS.Read.Consumers;

public class DepositedConsumer(IAccountService service) : IConsumer<Deposited>
{
    public async Task Consume(ConsumeContext<Deposited> context)
    {
        var message = context.Message;
        try
        {
            var account = await service.GetAccountById(message.AccountId.ToString())
                          ?? throw new Exception("Account does not exist");

            await service.UpdateBalance(account.Id, message.Amount, true);

            await context.RespondAsync(new EventResponse((Guid)context.MessageId!, true, "Deposit successfully"));
        }
        catch (Exception e)
        {
            await context.RespondAsync(new EventResponse((Guid)context.MessageId!, false, e.Message));
        }
    }
}
