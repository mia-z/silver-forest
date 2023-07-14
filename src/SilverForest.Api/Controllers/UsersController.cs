using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SilverForest.Api.Services;
using SilverForest.Common.Models;
using SilverForest.Infrastructure.Postgres.Services;

namespace SilverForest.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly SilverForestDbContext _context;
    private readonly JwtService _jwt;

    public UsersController(SilverForestDbContext context, ILogger<UsersController> logger, JwtService jwt)
    {
        _logger = logger;
        _context = context;
        _jwt = jwt;
    }

    [HttpGet]
    public async Task<IActionResult> Me()
    {
        var currentUserId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Id");

        var user = await _context.Users.FindAsync(Convert.ToInt32(currentUserId?.Value));

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser(User user)
    {
        var newUser = await _context.Users.AddAsync(user);

        var rows = await _context.SaveChangesAsync();

        if (rows > 0)
        {
            var token =_jwt.GenerateJsonWebToken(newUser.Entity.Id);

            return Created($"/users/{newUser.Entity.Id}", token);
        } else
        {
            return BadRequest("Couldnt create account");
        }
    }
}
