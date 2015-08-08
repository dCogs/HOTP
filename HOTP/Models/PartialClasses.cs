using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HOTP.Models
{
    [MetadataType(typeof(EmployeesMetadata))]
    public partial class tblHOTP_Employees
    {
    }

    [MetadataType(typeof(GoalsMetadata))]
    public partial class tblHOTP_Goals
    {
    }

    [MetadataType(typeof(EmployeeGoalsMetadata))]
    public partial class tblHOTP_EmployeeGoals
    {
    }

    [MetadataType(typeof(Plan90Metadata))]
    public partial class tblHOTP_Plan90
    {
    }


}
