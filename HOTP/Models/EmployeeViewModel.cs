using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOTP.Models
{
    public class EmployeeViewModel
    {
        public tblHOTP_Employees Employee { get; set; }
        public string SupervisorName { get; set; }
        //public SelectList Supervisors { get; set; }
    }
}