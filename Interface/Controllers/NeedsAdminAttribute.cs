using System;
using System.Web.Mvc;

namespace Interface.Controllers
{
    public class NeedsAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session != null)
            {
                if (filterContext.HttpContext.Session["IsAdmin"] != null)
                {
                    bool isAdmin = Convert.ToBoolean(filterContext.HttpContext.Session["IsAdmin"]);
                    if (isAdmin)
                    {
                        base.OnActionExecuting(filterContext);
                        return;
                    }
                }
            }

            filterContext.Result = new HttpNotFoundResult();
        }
    }
}