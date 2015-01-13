using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Hosting;
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

            return View(new ViewLeaveModel
            {
                User = currentUser,
                RemainingLeave = hrEntities.GetRemainingLeave(currentUser)
            });
        }

        [NeedsLogin]
        public ActionResult Pending()
        {
            var userID = new Guid(Session["EmployeeID"].ToString());
            Employee currentUser = hrEntities.Employees.SingleOrDefault(e => e.ID == userID);

            var approvals =
                currentUser.Subordinates.SelectMany(so => so.EmployeeLeaveRequests)
                    .Where(elr => elr.EmployeeLeaveRequestStatusChanges.Count() == 0);
            if (approvals.Count() == 1)
                return RedirectToAction("Application", new RouteValueDictionary { { "ID", approvals.FirstOrDefault().ID } });
            else
                return View(approvals);
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
        public async Task<ActionResult> Application(Guid id, int statusID, string reason)
        {
            var userID = new Guid(Session["EmployeeID"].ToString());
            Employee currentUser = hrEntities.Employees.SingleOrDefault(e => e.ID == userID);

            EmployeeLeaveRequest employeeLeaveRequest =
                hrEntities.EmployeeLeaveRequests.SingleOrDefault(
                    elr => elr.ID == id &&
                           ((elr.Employee.Supervisor != null && elr.Employee.Supervisor.ID == currentUser.ID) ||
                            currentUser.IsAdmin));

            var newStatus = hrEntities.LeaveStatuses.SingleOrDefault(ls => ls.ID == statusID);
            var employee = employeeLeaveRequest.Employee;
            employeeLeaveRequest.EmployeeLeaveRequestStatusChanges.Add(new EmployeeLeaveRequestStatusChanx
            {
                DateTime = DateTime.Now,
                ChangedByEmployeeID = userID,
                LeaveStatus = newStatus,
                Reason = reason
            });

            var saveToDB = hrEntities.SaveChangesAsync();


            string applicationURL = @Url.Action("Application", "Leave",
                    new RouteValueDictionary { { "ID", id } }, Request.Url.Scheme);

            var notificationMessage = new MailMessage(AppSettings.DefaultMailAddress,
                employee.GetMailAddress())
            {
                IsBodyHtml = true,
                Subject = "Your leave application has been " + newStatus.Status.ToLower(),
                Body =
                    "<h1>" + newStatus.Status + "</h1>Your leave application has been " + newStatus.Status.ToLower() + " by " + currentUser.Name + " " + currentUser.Surname + ". You can <a href='" + applicationURL + "'>go directly to this application</a> for more details."
            };

            HostingEnvironment.QueueBackgroundWorkItem(
                ct => AppSettings.SmtpClient.SendMailAsync(notificationMessage));

            await saveToDB;

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

            var saveToDB = hrEntities.SaveChangesAsync();

            if (supervisor != null)
            {
                string approvalURL = @Url.Action("Application", "Leave",
                    new RouteValueDictionary { { "ID", leaveRequest.ID } }, Request.Url.Scheme);

                var supervisorMailMessage = new MailMessage(AppSettings.DefaultMailAddress,
                    supervisor.GetMailAddress())
                {
                    IsBodyHtml = true,
                    Subject = "New Leave Application from " + currentUser.Name + " " + currentUser.Surname,
                    Body =
                        "You have a new leave application to approve. You can <a href='" + approvalURL + "'>go directly to this application</a> to approve or reject it."
                };

                HostingEnvironment.QueueBackgroundWorkItem(
                    ct => AppSettings.SmtpClient.SendMailAsync(supervisorMailMessage));
            }

            await saveToDB;
            return Json(leaveRequest.ID);
        }
    }
}