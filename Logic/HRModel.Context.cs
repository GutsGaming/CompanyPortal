﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Logic
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HREntities : DbContext
    {
        public HREntities()
            : base("name=HREntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<EmployeeLeaveManualAllotment> EmployeeLeaveManualAllotments { get; set; }
        public virtual DbSet<EmployeeLeaveRequestDate> EmployeeLeaveRequestDates { get; set; }
        public virtual DbSet<EmployeeLeaveRequest> EmployeeLeaveRequests { get; set; }
        public virtual DbSet<EmployeeLeaveRequestStatusChanx> EmployeeLeaveRequestStatusChanges { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<LeaveStatus> LeaveStatuses { get; set; }
        public virtual DbSet<LeaveType> LeaveTypes { get; set; }
    }
}