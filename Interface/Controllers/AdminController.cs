using System;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Interface.Models;
using Logic;

namespace Interface.Controllers
{
    public class AdminController : Controller
    {
        private readonly HREntities hrEntities = new HREntities();

        [NeedsLogin]
        [NeedsAdmin]
        public ActionResult Users()
        {
            return View();
        }

        [NeedsLogin]
        [NeedsAdmin]
        public ActionResult AddUser()
        {
            return View(new AddUserModel {Supervisors = hrEntities.Employees});
        }

        [NeedsLogin]
        [NeedsAdmin]
        public ActionResult GiveLeave()
        {
            return View(new GiveLeaveModel {Employees = hrEntities.Employees, LeaveTypes = hrEntities.LeaveTypes});
        }

        [NeedsLogin]
        [NeedsAdmin]
        [HttpPost]
        public async Task<ActionResult> GiveLeave(Guid employeeID, int leaveDays, int leaveTypeID)
        {
            var userID = new Guid(Session["EmployeeID"].ToString());
            hrEntities.EmployeeLeaveManualAllotments.Add(new EmployeeLeaveManualAllotment
            {
                ID = Guid.NewGuid(),
                DateTime = DateTime.Now,
                Days = leaveDays,
                EmployeeID = employeeID,
                LeaveTypeID = leaveTypeID,
                GivenByEmployeeID = userID
            });
            await hrEntities.SaveChangesAsync();
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [NeedsLogin]
        [NeedsAdmin]
        public async Task<ActionResult> AddUser(Employee employee)
        {
            employee.ID = Guid.NewGuid();

            hrEntities.Employees.Add(employee);
            Task<int> saveUser = hrEntities.SaveChangesAsync();

            string passwordLink = Url.Action("Register", "User", new RouteValueDictionary {{"id", employee.ID}},
                Request.Url.Scheme);

            await
                AppSettings.SmtpClient.SendMailAsync(new MailMessage(AppSettings.DefaultMailAddress,
                    new MailAddress(employee.Email, employee.Name + employee.Surname))
                {
                    Subject = "Welcome to the " + AppSettings.CompanyName + " Company Portal",
                    IsBodyHtml = true,
                    Body =
                        "<h1>Hello " + employee.Name + "!</h1>You have been invited to join the " +
                        AppSettings.CompanyName +
                        " Company Portal. Please click this link to create a password: <a href='" + passwordLink + "'>" +
                        passwordLink + "</a>"
                });

            await saveUser;
            return RedirectToAction("Index", "Dashboard");
        }
    }
}