using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOTP.Models
{
    public class GoalFull
    {
        public string EmployeeName { get; set; }
        public tblHOTP_EmployeeGoals EmployeeGoal { get; set; }
        public tblHOTP_Goals Goal { get; set; }
        public List<Plan90Full> Plans { get; set; }
    }
}