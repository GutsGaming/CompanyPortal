using System.Collections.Generic;
using System.Data.Entity;
using Logic;
using System;

namespace Interface.Models
{
    public class BookLeaveModel
    {
        public DbSet<LeaveType> LeaveTypes { get; set; }

        public Employee CurrentUser { get; set; }

        public IEnumerable<LeaveTypeAmount> RemainingLeave { get; set; }
    }

    public class BookLeaveRequestDate
    {
        public DateTime Date { get; set; }
        public int IsFullDay { get; set; }
    }

    public class BookLeaveRequest
    {

        public int LeaveType { get; set; }
        public List<BookLeaveRequestDate> Dates { get; set; } 
    }
}