using Microsoft.AspNetCore.Mvc;

namespace SilverForest.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UsersController : ControllerBase
{
    public async Task<IActionResult> Index()
    {
        return Ok();
    }
}
