using CQRS.Read.Services;
using Events;
using MassTransit;

namespace CQRS.Read.Consumers;

public class WithdrewConsumer(IAccountService service) : IConsumer<Withdrew>
{
    public async Task Consume(ConsumeContext<Withdrew> context)
    {
        var message = context.Message;
        try
        {
            var account = await service.GetAccountById(message.AccountId.ToString())
                          ?? throw new Exception("Account does not exist");

            var destination = await service.GetAccountById(message.DestinationId.ToString())
                              ?? throw new Exception("Destination account does not exist");

            await service.UpdateBalance(account.Id, message.Amount, false);
            await service.UpdateBalance(destination.Id, message.Amount, true);
            await context.RespondAsync(new
            {
                Success = true,
                Message = "Withdraw successfully"
            });
        }
        catch (Exception e)
        {
            await context.RespondAsync(new
            {
                Success = false, e.Message
            });
        }
    }
}
