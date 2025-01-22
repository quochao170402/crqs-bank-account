using CQRS.Read.Services;
using Events;
using MassTransit;

namespace CQRS.Read.Consumers;

public class AccountCreatedConsumer(IAccountService service) : IConsumer<BankAccountCreated>
{
    public async Task Consume(ConsumeContext<BankAccountCreated> context)
    {
        var message = context.Message;
        try
        {
            await service.CreateAccount(message.AccountId.ToString(), message.Name, message.InitialBalance);
            await context.RespondAsync(new
            {
                Success = true,
                Message = "Account created successfully"
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
