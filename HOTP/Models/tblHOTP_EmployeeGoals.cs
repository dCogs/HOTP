//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HOTP.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblHOTP_EmployeeGoals
    {
        public tblHOTP_EmployeeGoals()
        {
            this.tblHOTP_Plan90 = new HashSet<tblHOTP_Plan90>();
        }
    
        public int EmployeeGoalID { get; set; }
        public int EmployeeID { get; set; }
        public int GoalID { get; set; }
        public int Weight { get; set; }
        public string IndNotes { get; set; }
        public Nullable<decimal> ItemScore { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
    
        public virtual tblHOTP_Employees tblHOTP_Employees { get; set; }
        public virtual tblHOTP_Goals tblHOTP_Goals { get; set; }
        public virtual ICollection<tblHOTP_Plan90> tblHOTP_Plan90 { get; set; }
    }
}
