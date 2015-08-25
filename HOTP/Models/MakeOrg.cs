using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOTP.Models
{
    public class MakeOrg
    {
        public EvalViewModel Eval { get; set; }
        public SelectList OrgGoals { get; set; }
    }
}