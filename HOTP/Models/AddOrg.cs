using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOTP.Models
{
    public class AddOrg
    {
        public int GoalID { get; set; }
        public int Weight { get; set; }
        public int EmployeeID { get; set; }
        public string EmpName { get; set; }
    }

}