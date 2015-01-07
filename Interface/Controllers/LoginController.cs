using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logic;

namespace Interface.Controllers
{
    public class LoginController : Controller
    {
        private HREntities hrEntities = new HREntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string email, string password)
        {
            var employee = hrEntities.Employees.SingleOrDefault(e => e.Email.Equals(email));
            if (employee != null)
            {
                var triedPassword = Security.Hash(password, employee.Salt);
                var userPassword = employee.Password;

                if (employee.Password.SequenceEqual(triedPassword))
                {
                    Session.Add("EmployeeID", employee.ID);
                    return RedirectToAction("Index", "Dashboard");                    
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");                
            }
        }
    }
}