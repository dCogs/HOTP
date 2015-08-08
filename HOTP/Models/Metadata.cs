using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace HOTP.Models
{
    public class EmployeesMetadata
    {
        [StringLength(50)]
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(100)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email{ get; set; }
    }

    public class EmployeeGoalsMetadata
    {
        [Display(Name = "Item Score")]
        [Required]
        public Nullable<decimal> ItemScore { get; set; }

    }



    public class GoalsMetadata
    {
        public int GoalID { get; set; }

        [Display(Name = "Year")]
        [Required]
        public string YearEnding { get; set; }

        [Display(Name = "Goal Type")]
        [Required]
        public string GoalType { get; set; }

        [Display(Name = "Pillar")]
        [Required]
        public string Pillar { get; set; }

        [Display(Name = "Goal Name")]
        [Required]
        public string GoalName { get; set; }

        [Display(Name = "Goal Template Name")]
        [Required]
        public string PillarGoalName { get; set; }

        [Display(Name = "Goal")]
        [DataType(DataType.MultilineText)]
        [Required]
        public string Goal { get; set; }

        [Display(Name = "Status")]
        [Required]
        [DefaultValue("Active")]
        public string Status { get; set; }

        [Display(Name = "Results Measured")]
        [Required]
        [DefaultValue("Whole Number")]
        public string ResultsMeasured { get; set; }

        [Display(Name = "Best Rating")]
        [Required]
        [DefaultValue("Higher is better")]
        public string BestRating { get; set; }

        [Display(Name = "1 is ")]
        [Required]
        public Nullable<float> Rating1 { get; set; }
        [Display(Name = "2 is")]
        [Required]
        public Nullable<float> Rating2 { get; set; }
        [Display(Name = "3 is")]
        [Required]
        public Nullable<float> Rating3 { get; set; }
        [Display(Name = "4 is")]
        [Required]
        public Nullable<float> Rating4 { get; set; }
        [Display(Name = "5 is")]
        [Required]
        public Nullable<float> Rating5 { get; set; }

        [Display(Name = "How are year-end results calculated?")]
        [Required]
        [DefaultValue("Average")]
        public string YearEndCalculation { get; set; }

        [Display(Name = "What units are results measured in?")]
        [Required]
        [DefaultValue("")]
        public string UnitsMeasuredIn { get; set; }

        [Display(Name = "Number of decimal places allowed")]
        [Required]
        [DefaultValue("0")]
        public Nullable<int> NumDecimals { get; set; }

        [Display(Name = "Results Entered")]
        [Required]
        [DefaultValue("Multiple Results")]
        public string ResultsEntered { get; set; }

        [Display(Name = "Select the person responsible for entering results")]
        public Nullable<int> EnteredBy { get; set; }

        [Display(Name = "Non-editable by leader")]
        public Nullable<bool> NonEditableByLeader { get; set; }

    }

    public class Plan90Metadata
    {
        [Display(Name = "90-Day Goal")]
        public string Goal { get; set; }
    }


}