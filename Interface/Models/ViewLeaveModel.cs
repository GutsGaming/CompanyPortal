using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interface.Models
{
    public class ViewLeaveModel
    {
        public Logic.Employee User { get; set; }

        public IEnumerable<Logic.LeaveTypeAmount> RemainingLeave { get; set; }
    }
}