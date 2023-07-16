using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SilverForest.Api.Filters;

namespace SilverForest.Api.Abstraction
{
    [UserContextFilter]
    public class SilverForestApiControllerBase : ControllerBase
    {
        internal int currentUserId()
        {
            return int.Parse(RouteData.Values["UserId"].ToString());
        }

        internal SilverForestApiControllerBase()
        {
            //    
        }
    }
}
