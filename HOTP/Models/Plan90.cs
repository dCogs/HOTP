using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOTP.Models
{
    public class Plan90ViewModel
    {
        public string EmployeeName { get; set; }
        public int EmployeeGoalID {get;set;}
        public int Weight { get; set; }
        public decimal? ItemScore { get; set; }
        public tblHOTP_Goals Goal { get; set; }
        public tblHOTP_Plan90 Plan { get; set; }
        public List<tblHOTP_ActionSteps> ActionSteps { get; set; }

    }
}