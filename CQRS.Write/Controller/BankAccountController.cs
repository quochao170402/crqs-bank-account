using CQRS.Write.Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CQRS.Write.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
public class BankAccountController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Deposit([FromBody] Deposit request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Withdraw([FromBody] Withdraw request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> CloseAccount([FromBody] CloseAccount request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
}
