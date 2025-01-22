using CQRS.Read.Services;
using Microsoft.AspNetCore.Mvc;

namespace CQRS.Read.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController(IAccountService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccountById([FromRoute] string id)
    {
        var result = await service.GetAccountById(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts([FromQuery] int pageSize = 20, [FromQuery] int pageIndex = 1)
    {
        var result = await service.GetAccounts(pageSize, pageIndex);
        return Ok(new
        {
            Accounts = result.accounts,
            Total = result.total
        });
    }
}
