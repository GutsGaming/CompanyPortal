using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Logic
{
    public class LeaveTypeAmount
    {
        public LeaveType LeaveType { get; set; }

        public decimal Days { get; set; }
    }

    public partial class HREntities : DbContext
    {
        public IEnumerable<LeaveTypeAmount> GetRemainingLeave(Employee e)
        {
            List<LeaveTypeAmount> startingLeave = LeaveTypes.Select(lt => new LeaveTypeAmount
            {
                LeaveType = lt,
                Days = (lt.IsInheritable
                    ? ((DateTime.Now.Year - AppSettings.StartingYear) + 1)*lt.YearlyLeave
                    : lt.YearlyLeave)
            }).ToList();

            List<LeaveTypeAmount> manualAllotments =
                e.EmployeeLeaveManualAllotments.GroupBy(ea => ea.LeaveType).Select(ea => new LeaveTypeAmount
                {
                    LeaveType = ea.Key,
                    Days =
                        ea.Sum(
                            l => ea.Key.IsInheritable ? l.Days : (l.DateTime.Year == DateTime.Now.Year ? l.Days : 0))
                }).ToList();

            foreach (LeaveTypeAmount leaveTypeAmount in startingLeave)
            {
                IEnumerable<EmployeeLeaveRequest> leaveRequests =
                    e.EmployeeLeaveRequests.Where(elr => elr.LeaveTypeID == leaveTypeAmount.LeaveType.ID);

                if (!leaveTypeAmount.LeaveType.IsInheritable)
                    leaveRequests = leaveRequests.Where(lr => lr.DateTime.Year == DateTime.Now.Year);

                LeaveTypeAmount manualAllotment =
                    manualAllotments.SingleOrDefault(ma => ma.LeaveType == leaveTypeAmount.LeaveType);

                if (manualAllotment != null)
                    leaveTypeAmount.Days += manualAllotment.Days;

                leaveTypeAmount.Days -=
                    leaveRequests.Sum(
                        lr =>
                            lr.EmployeeLeaveRequestDates.Count(d => d.IsFullDay) +
                            (lr.EmployeeLeaveRequestDates.Count(d => !d.IsFullDay)/2));
            }

            return startingLeave;
        }
    }
}