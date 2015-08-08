using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOTP.Models
{
    public class Plan90Full
    {
        public tblHOTP_Plan90 Plan { get; set; }
        public List<tblHOTP_ActionSteps> ActionSteps { get; set; }
    }
}