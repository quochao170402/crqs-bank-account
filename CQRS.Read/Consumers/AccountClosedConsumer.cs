using CQRS.Read.Services;
using Events;
using MassTransit;

namespace CQRS.Read.Consumers;

public class AccountClosedConsumer(IAccountService service) : IConsumer<BankAccountClosed>
{
    public async Task Consume(ConsumeContext<BankAccountClosed> context)
    {
        var message = context.Message;
        try
        {
            var account = await service.GetAccountById(message.AccountId.ToString())
                          ?? throw new Exception("Account does not exist");

            await service.CloseAccount(account.Id, message.Reason);

            await context.RespondAsync(new
            {
                Success = true,
                Message = "Close account successfully"
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
