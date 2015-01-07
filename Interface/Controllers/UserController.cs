using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Logic;

namespace Interface.Controllers
{
    public class UserController : Controller
    {
        private readonly HREntities hrEntities = new HREntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register(Guid id)
        {
            return
                View(
                    hrEntities.Employees.SingleOrDefault(
                        e => e.ID == id && e.Password == null));
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<ActionResult> Register(Guid id, string password)
        {
            var employee = hrEntities.Employees.SingleOrDefault(
                e => e.ID == id && e.Password == null);

            employee.Salt = Security.GenerateSalt();
            employee.Password = Security.Hash(password, employee.Salt);

            await hrEntities.SaveChangesAsync();

            return
                RedirectToAction("Index", "Login");
        }
    }
}