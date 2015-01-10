using System;
using System.Linq;
using System.Web.Mvc;
using Interface.Models;
using Logic;

namespace Interface.Controllers
{
    public class LeaveController : Controller
    {
        private readonly HREntities hrEntities = new HREntities();

        public ActionResult Index()
        {
            return View();
        }

        [NeedsLogin]
        public ActionResult Book()
        {
            var userID = new Guid(Session["EmployeeID"].ToString());
            var bookLeaveModel = new BookLeaveModel
            {
                LeaveTypes = hrEntities.LeaveTypes,
                CurrentUser = hrEntities.Employees.SingleOrDefault(e => e.ID == userID)
            };
            return View(bookLeaveModel);
        }
    }
}