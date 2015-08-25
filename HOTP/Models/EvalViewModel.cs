using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOTP.Models
{
    public class EvalViewModel
    {
        public tblHOTP_Goals Goal { get; set; }
        public int EmployeeGoalID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int Weight { get; set; }
        [Display(Name = "Item Score")]
        public decimal? ItemScore { get; set; }
        public bool CanEdit { get; set; }
        public bool Admin { get; set; }
        public bool LockCurrentFY { get; set; }
    }
}