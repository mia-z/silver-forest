using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SilverForest.Api.Abstraction;
using SilverForest.Common.Models;
using SilverForest.Infrastructure.Postgres.Abstraction;

namespace SilverForest.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUsersRepository _users;
    private readonly IJwtService _jwt;

    public UsersController(IUsersRepository users, ILogger<UsersController> logger, IJwtService jwt)
    {
        _logger = logger;
        _users = users;
        _jwt = jwt;
    }

    [HttpGet]
    public async Task<IActionResult> Me()
    {
        var currentUserId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Id");

        var user = await _users.GetUserById(Convert.ToInt32(currentUserId?.Value));

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser(User user)
    {
        var newUserId = await _users.CreateUser(user);

        if (newUserId > 0)
        {
            var token =_jwt.GenerateJsonWebToken(newUserId);

            return Created($"/users/{newUserId}", token);
        } else
        {
            return BadRequest("Couldnt create account");
        }
    }
}
