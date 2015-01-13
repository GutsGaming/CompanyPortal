using System.Web.Mvc;
using System.Web.Routing;

namespace Interface.Controllers
{
    public class NeedsLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session != null)
            {
                if (filterContext.HttpContext.Session["EmployeeID"] != null)
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }
            }

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                {"controller", "Login"},
                {"redirectURL", filterContext.HttpContext.Request.Url.PathAndQuery.TrimEnd('/')}
            });
        }
    }
}