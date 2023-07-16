using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilverForest.Api.Abstraction;
using SilverForest.Common.Models;
using SilverForest.Common.Requests;
using SilverForest.Infrastructure.Redis.Abstraction;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace SilverForest.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class ForestryController : SilverForestApiControllerBase
{
    private readonly ILogger<ForestryController> _logger;
    private readonly IRedisJobCache _cache;

    public ForestryController(ILogger<ForestryController> logger, IRedisJobCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    [HttpPost]
    public async Task<IActionResult> Woodcut([FromBody]ForestryWoodcutRequest req)
    {
        if (req == null)
        {
            return BadRequest();
        }

        var j = new SkillJob()
        {
            PlayerId = currentUserId(),
            Guid = Guid.NewGuid(),
            Skill = new Skill()
            {
                Name = "Forestry"
            },
            ExpireTime = TimeSpan.FromSeconds(5)
        };

        var jobJson = JsonSerializer.Serialize(j);

        await _cache.AddJob(jobJson, currentUserId());
        return Ok();
    }
}
