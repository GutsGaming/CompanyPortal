using System;
using System.Linq;
using System.Web.Mvc;
using Logic;

namespace Interface.Controllers
{
    public class LoginController : Controller
    {
        private readonly HREntities hrEntities = new HREntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string email, string password, string redirectURL)
        {
            Employee employee = hrEntities.Employees.SingleOrDefault(e => !e.IsDisabled && e.Email.Equals(email));
            if (employee != null)
            {
                byte[] triedPassword = Security.Hash(password, employee.Salt);
                byte[] userPassword = employee.Password;

                if (employee.Password.SequenceEqual(triedPassword))
                {
                    Session.Add("EmployeeID", employee.ID);
                    Session.Add("IsAdmin", employee.IsAdmin);
                    Session.Add("Name", employee.Name);

                    if (String.IsNullOrEmpty(redirectURL))
                        return RedirectToAction("Index", "Dashboard");
                    return Redirect(redirectURL);
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}