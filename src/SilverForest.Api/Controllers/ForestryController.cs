using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilverForest.Api.Abstraction;
using SilverForest.Common.Requests;
using SilverForest.Infrastructure.Redis.Abstraction;

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

        await _cache.AddJob(Guid.NewGuid().ToString(), req.Id);
        return Ok();
    }
}
