using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interface.Models
{
    public class GiveLeaveModel
    {
        public System.Data.Entity.DbSet<Logic.Employee> Employees { get; set; }

        public System.Data.Entity.DbSet<Logic.LeaveType> LeaveTypes { get; set; }
    }
}