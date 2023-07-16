using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SilverForest.Api.Abstraction;
using SilverForest.Api.Abstraction.Interfaces;
using SilverForest.Common.Models;
using SilverForest.Infrastructure.Postgres.Abstraction;
using SilverForest.Infrastructure.Redis.Abstraction;

namespace SilverForest.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController : SilverForestApiControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUsersRepository _users;
    private readonly IJwtService _jwt;
    private readonly IRedisJobCache _cache;

    public UsersController(IUsersRepository users, ILogger<UsersController> logger, IJwtService jwt, IRedisJobCache cache)
    {
        _logger = logger;
        _users = users;
        _jwt = jwt;
        _cache = cache;
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> Me()
    {
        var user = await _users.GetUserById(currentUserId());

        return Ok(user);
    }

    [HttpGet]
    [Route("me/[action]")]
    public async Task<IActionResult> Jobs()
    {
        var jobs = await _cache.GetJobsById(currentUserId());

        return Ok(jobs);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("create")]
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
