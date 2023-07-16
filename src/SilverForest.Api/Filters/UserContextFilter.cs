using Microsoft.AspNetCore.Mvc.Filters;

namespace SilverForest.Api.Filters
{
    public class UserContextFilter: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (int.TryParse(context.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value, out int id))
                context.RouteData.Values["UserId"] = id;
            else
                context.RouteData.Values["UserId"] = -1;
        }
    }
}
