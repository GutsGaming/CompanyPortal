using System.Web.Mvc;

namespace Interface.Controllers
{
    public class LeaveController : Controller
    {
        // GET: Leave
        public ActionResult Index()
        {
            return View();
        }

        [NeedsLogin]
        public ActionResult Book()
        {
            return View();
        }
    }
}