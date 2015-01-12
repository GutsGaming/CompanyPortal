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

        [NeedsLogin]
        public ActionResult Index()
        {
            var userID = new Guid(Session["EmployeeID"].ToString());
            Employee currentUser = hrEntities.Employees.SingleOrDefault(e => e.ID == userID);

            return View(currentUser);
        }

        [NeedsLogin]
        public ActionResult Pending()
        {
            var userID = new Guid(Session["EmployeeID"].ToString());
            Employee currentUser = hrEntities.Employees.SingleOrDefault(e => e.ID == userID);

            return View(currentUser.Subordinates.SelectMany(so=>so.EmployeeLeaveRequests).Where(elr=>elr.EmployeeLeaveRequestStatusChanges.Count()==0));
        }

        [NeedsLogin]
        public ActionResult Application(Guid id)
        {
            var userID = new Guid(Session["EmployeeID"].ToString());
            Employee currentUser = hrEntities.Employees.SingleOrDefault(e => e.ID == userID);
            ViewBag.CurrentUser = currentUser;

            EmployeeLeaveRequest employeeLeaveRequest =
                hrEntities.EmployeeLeaveRequests.SingleOrDefault(
                    elr => elr.ID == id &&
                           (elr.EmployeeID == currentUser.ID ||
                            (elr.Employee.Supervisor != null && elr.Employee.Supervisor.ID == currentUser.ID) ||
                            currentUser.IsAdmin));

            if (employeeLeaveRequest == null)
                return HttpNotFound();

            return View(employeeLeaveRequest);
        }

        [NeedsLogin]
        [HttpPost]
        public ActionResult Application(Guid id, int statusID, string reason)
        {
            var userID = new Guid(Session["EmployeeID"].ToString());
            Employee currentUser = hrEntities.Employees.SingleOrDefault(e => e.ID == userID);

            EmployeeLeaveRequest employeeLeaveRequest =
                hrEntities.EmployeeLeaveRequests.SingleOrDefault(
                    elr => elr.ID == id &&
                           ((elr.Employee.Supervisor != null && elr.Employee.Supervisor.ID == currentUser.ID) ||
                            currentUser.IsAdmin));

            employeeLeaveRequest.EmployeeLeaveRequestStatusChanges.Add(new EmployeeLeaveRequestStatusChanx
            {
                DateTime = DateTime.Now,
                ChangedByEmployeeID = userID,
                LeaveStatusID = statusID,
                Reason = reason
            });

            hrEntities.SaveChanges();

            return RedirectToAction("Application", new RouteValueDictionary { { "ID", id } });
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

            if (supervisor == null)
            {
                leaveRequest.EmployeeLeaveRequestStatusChanges.Add(new EmployeeLeaveRequestStatusChanx
                {
                    ChangedByEmployee = currentUser,
                    DateTime = DateTime.Now,
                    LeaveStatusID = 2,
                    Reason = "Auto-Approved"
                });
            }

            currentUser.EmployeeLeaveRequests.Add(leaveRequest);

            var tasks = new List<Task>();

            tasks.Add(hrEntities.SaveChangesAsync());

            if (supervisor != null)
            {
                string approvalURL = @Url.Action("Application", "Leave",
                    new RouteValueDictionary { { "ID", leaveRequest.ID } }, Request.Url.Scheme);

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
            return Json(leaveRequest.ID);
        }
    }
}