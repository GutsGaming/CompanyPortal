using System;
using System.Linq;
using System.Web.Mvc;
using Interface.Models;
using Logic;
using System.Threading.Tasks;

namespace Interface.Controllers
{
    public class AdminController : Controller
    {
        private readonly HREntities hrEntities = new HREntities();

        public ActionResult Users()
        {
            return View();
        }

        public ActionResult AddUser()
        {
            return View(new AddUserModel {Supervisors = hrEntities.Employees});
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(Employee employee)
        {
            employee.ID = Guid.NewGuid();

            hrEntities.Employees.Add(employee);
            await hrEntities.SaveChangesAsync();
            return RedirectToAction("Users");
        }
    }
}