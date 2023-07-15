using Microsoft.AspNetCore.Mvc;

namespace SilverForest.Api.Abstraction
{
    public class SilverForestApiControllerBase : ControllerBase
    {
        internal readonly int currentUserId;
        internal SilverForestApiControllerBase()
        {
            if (int.TryParse(HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value, out int id))
                currentUserId = id;
            else
                currentUserId = -1;
        }
    }
}
