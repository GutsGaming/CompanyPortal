using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
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
            Employee currentUser = hrEntities.Employees.SingleOrDefault(e => e.ID == userID);

            var bookLeaveModel = new BookLeaveModel
            {
                LeaveTypes = hrEntities.LeaveTypes,
                CurrentUser = currentUser,
                RemainingLeave = hrEntities.GetRemainingLeave(currentUser)
            };
            return View(bookLeaveModel);
        }

        [NeedsLogin]
        [HttpPost]
        public async Task<JsonResult> Book(BookLeaveRequest bookLeaveRequest)
        {
            var userID = new Guid(Session["EmployeeID"].ToString());
            Employee currentUser = hrEntities.Employees.SingleOrDefault(e => e.ID == userID);
            Employee supervisor = currentUser.Supervisor;

            var leaveRequest = new EmployeeLeaveRequest
            {
                ID = Guid.NewGuid(),
                DateTime = DateTime.Now,
                LeaveTypeID = bookLeaveRequest.LeaveType,
            };

            foreach (BookLeaveRequestDate bookLeaveRequestDate in bookLeaveRequest.Dates)
            {
                leaveRequest.EmployeeLeaveRequestDates.Add(new EmployeeLeaveRequestDate
                {
                    Day = bookLeaveRequestDate.Date,
                    IsFullDay = bookLeaveRequestDate.IsFullDay == 1 ? true : false
                });
            }

            currentUser.EmployeeLeaveRequests.Add(leaveRequest);

            var tasks = new List<Task>();

            tasks.Add(hrEntities.SaveChangesAsync());

            if (supervisor != null)
            {
                string approvalURL = @Url.Action("Approve", "Supervisor",
                    new RouteValueDictionary {{"ID", leaveRequest.ID}}, Request.Url.Scheme);

                var supervisorMailMessage = new MailMessage(AppSettings.DefaultMailAddress,
                    supervisor.GetMailAddress())
                {
                    IsBodyHtml = true,
                    Subject = "New Leave Application from: " + currentUser.Name + " " + currentUser.Surname,
                    Body =
                        "You have a new leave application to approve. You can visit <a href='" + approvalURL + "'>" +
                        approvalURL + "</a> to go directly to this application"
                };

                tasks.Add(AppSettings.SmtpClient.SendMailAsync(supervisorMailMessage));
            }


            await Task.WhenAll(tasks);
            return Json(true);
        }
    }
}