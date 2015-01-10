using System.Web.Mvc;

namespace Interface.Controllers
{
    public class DashboardController : Controller
    {
        [NeedsLogin]
        public ActionResult Index()
        {
            return View();
        }
    }
}