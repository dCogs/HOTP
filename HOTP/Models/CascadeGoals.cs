using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOTP.Models
{
    public class CascadeGoals
    {
        //public int EmployeeID { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public int GoalID { get; set; }
        //public string PillarGoalName { get; set; }
        //public int Weight { get; set; }
        public List<tblHOTP_Employees> Employees { get; set; }
        public List<tblHOTP_Goals> Goals { get; set; }

        //public int CurrentWeight(int employeeID, int goalID)
        //{
        //    get 
        //    {
        //        HOTP_Entities db = new HOTP_Entities();
        //        var currWeight = (from eg in db.tblHOTP_EmployeeGoals
        //                         where eg.EmployeeID == employeeID && eg.GoalID == goalID
        //                             select eg.Weight).First();
        //        //            orderby g.PillarGoalName
        //        //            select g;
        //        return (int)currWeight;
        //    }
        //}

    }
}