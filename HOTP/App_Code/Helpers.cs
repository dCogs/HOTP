using HOTP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOTPHelpers.Helpers
{
    public static class MyHTMLHelpers
    {

        public static string GetWeight (int employeeID, int goalID)
        {
            HOTP_Entities db = new HOTP_Entities();
            string currWeight = "";
            try
            {
               int weight = (from eg in db.tblHOTP_EmployeeGoals
                              where eg.EmployeeID == employeeID && eg.GoalID == goalID
                              select eg.Weight).First();
               currWeight = weight.ToString();
            }
            catch {}
            //            orderby g.PillarGoalName
            //            select g;
            return currWeight.ToString();
        }

        public static bool AdminRole(string user)
        {
            HOTP_Entities db = new HOTP_Entities();
            bool adminUser = false;
            try
            {
                adminUser = (from e in db.tblHOTP_Employees
                              where e.Email == user
                              select e.Admin).First();
            }
            catch { }
            //            orderby g.PillarGoalName
            //            select g;
            return adminUser;
        }

    }
}