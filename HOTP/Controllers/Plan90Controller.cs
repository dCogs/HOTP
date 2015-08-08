using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HOTP.Models;

namespace HOTP.Controllers
{
    public class Plan90Controller : Controller
    {
        private HOTP_Entities db = new HOTP_Entities();

        // GET: Plan90ViewModel
        public ActionResult Index(int? SelectedEmp, string YearEnding)
        {
            if (SelectedEmp == null && TempData["SelectedEmp"] != null)
            {
                SelectedEmp = Convert.ToInt16(TempData["SelectedEmp"]);
            }
            var emps =
              db.tblHOTP_Employees
                .Where(s => s.Evaluations)
                .OrderBy(s => s.LastName)
                .ToList()
                .Select(s => new
                {
                    EmployeeId = s.EmployeeID,
                    FullName = string.Format("{0}, {1}", s.LastName, s.FirstName)
                });
            ViewBag.SelectedEmp = new SelectList(emps, "EmployeeID", "FullName", SelectedEmp);

            if (YearEnding == null && TempData["YearEnding"] != null)
            {
                YearEnding = TempData["YearEnding"].ToString();
            }
            TempData["YearEnding"] = YearEnding;
            ViewBag.YearEnding = PopulateCodesDDL("YearEnding", YearEnding);

            if (SelectedEmp != null)
            {
                TempData["SelectedEmp"] = SelectedEmp;
                var goals = from eg in db.tblHOTP_EmployeeGoals
                            join g in db.tblHOTP_Goals on eg.GoalID equals g.GoalID into goal
                            from subGoal in goal.DefaultIfEmpty()
                            where eg.EmployeeID == SelectedEmp && subGoal.YearEnding == YearEnding
                            select new EvalViewModel
                            {
                                Weight = eg.Weight,
                                ItemScore = eg.ItemScore,
                                EmployeeID = eg.EmployeeID,
                                EmployeeGoalID = eg.EmployeeGoalID,
                                Goal = subGoal
                            };
                return View(goals.ToList());
            }
            else
            {
                var goals = from eg in db.tblHOTP_EmployeeGoals
                            where eg.EmployeeID == 4321
                            join g in db.tblHOTP_Goals on eg.GoalID equals g.GoalID into goal
                            from subGoal in goal.DefaultIfEmpty()
                            select new EvalViewModel { Weight = eg.Weight, EmployeeID = eg.EmployeeID, Goal = subGoal };
                return View(goals.ToList());
            }
        }


        // GET: Plan90ViewModel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHOTP_Plan90 tblHOTP_Plan90 = db.tblHOTP_Plan90.Find(id);
            if (tblHOTP_Plan90 == null)
            {
                return HttpNotFound();
            }
            return View(tblHOTP_Plan90);
        }


        // GET: Plan90ViewModel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PopulatePlan(id);
            tblHOTP_EmployeeGoals employeeGoal = db.tblHOTP_EmployeeGoals.Find(id);
            //var goal = db.tblHOTP_Goals.Find(employeeGoal.GoalID);
            tblHOTP_Employees employee = db.tblHOTP_Employees.Find(employeeGoal.EmployeeID);

            var plans = from p in db.tblHOTP_Plan90
                        where p.EmployeeGoalID == id
                        join eg in db.tblHOTP_EmployeeGoals on p.EmployeeGoalID equals eg.EmployeeGoalID into empGoal
                        from subEmployeeGoal in empGoal.DefaultIfEmpty()
                        join g in db.tblHOTP_Goals on employeeGoal.GoalID equals g.GoalID into goal
                        from subGoal in goal.DefaultIfEmpty()
                        select new Plan90ViewModel
                        {
                            EmployeeName = employee.FirstName + " " + employee.LastName,
                            EmployeeGoalID = employeeGoal.EmployeeGoalID,
                            Weight = employeeGoal.Weight,
                            ItemScore = employeeGoal.ItemScore,
                            Goal = subGoal,
                            Plan = p,
                            ActionSteps = db.tblHOTP_ActionSteps.Where(a => a.PlanID == p.PlanID).ToList()
                        };
            ViewBag.PlanStatus = PopulateCodesDDL("PlanStatus");
            return View(plans.ToList());
        }


        public void PopulatePlan(int? id)
        {
            var plans = from p in db.tblHOTP_Plan90
                        where p.EmployeeGoalID == id
                        select p.PlanID;
            if (plans.Count() > 0)
                return;
            for (int quarter = 1; quarter < 5; quarter++)
            {
                tblHOTP_Plan90 newPlan = new tblHOTP_Plan90();
                newPlan.EmployeeGoalID = Convert.ToInt16(id);
                newPlan.Quarter = quarter;
                newPlan.Goal = "";
                db.tblHOTP_Plan90.Add(newPlan);
                db.SaveChanges();
                int planID = newPlan.PlanID;
                tblHOTP_ActionSteps actionStep = new tblHOTP_ActionSteps();
                actionStep.PlanID = planID;
                actionStep.ActionStep = "";
                actionStep.Result = "";
                actionStep.Status = "";
                db.tblHOTP_ActionSteps.Add(actionStep);
                db.SaveChanges();
            }
        }



        // POST: Plan90ViewModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(List<Plan90ViewModel> MyPlans)
        //public ActionResult Edit([Bind(Include = "PlanID,EmployeeGoalID,Quarter,Goal")] tblHOTP_Plan90 tblHOTP_Plan90)
        {
            if (ModelState.IsValid)
            {
                foreach (Plan90ViewModel plan90 in MyPlans)
                {
                    tblHOTP_Plan90 existingPlan = db.tblHOTP_Plan90.Find(plan90.Plan.PlanID);
                    existingPlan.Goal = plan90.Plan.Goal;
                    foreach (tblHOTP_ActionSteps actionStep in plan90.ActionSteps)
                    {
                        if (actionStep.ActionID != 99999 && actionStep.Status == "Remove")
                        {
                            tblHOTP_ActionSteps existingAction = db.tblHOTP_ActionSteps.Find(actionStep.ActionID);
                            db.tblHOTP_ActionSteps.Remove(existingAction);
                        }
                        else
                            if (actionStep.ActionID == 99999 && actionStep.Status != "Remove")
                            {
                                tblHOTP_ActionSteps newActionStep = new tblHOTP_ActionSteps();
                                newActionStep.PlanID = plan90.Plan.PlanID;
                                newActionStep.ActionStep = actionStep.ActionStep;
                                newActionStep.Result = actionStep.Result;
                                newActionStep.Status = actionStep.Status;
                                db.tblHOTP_ActionSteps.Add(newActionStep);
                            }
                            else
                            {
                                if (actionStep.ActionID != 99999)
                                {
                                    tblHOTP_ActionSteps existingAction = db.tblHOTP_ActionSteps.Find(actionStep.ActionID);
                                    existingAction.Result = actionStep.Result;
                                    existingAction.ActionStep = actionStep.ActionStep;
                                    existingAction.Status = actionStep.Status;
                                }
                            }
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlanStatus = PopulateCodesDDL("PlanStatus");
            return View(MyPlans);
        }


        // GET: Plan90ViewModel/All/5
        public ActionResult All(int? SelectedEmp, string YearEnding)
        {
            if (SelectedEmp == null && TempData["SelectedEmp"] != null)
            {
                SelectedEmp = Convert.ToInt16(TempData["SelectedEmp"]);
            }
            var emps =
              db.tblHOTP_Employees
                .Where(s => s.Evaluations)
                .OrderBy(s => s.LastName)
                .ToList()
                .Select(s => new
                {
                    EmployeeId = s.EmployeeID,
                    FullName = string.Format("{0}, {1}", s.LastName, s.FirstName)
                });
            ViewBag.SelectedEmp = new SelectList(emps, "EmployeeID", "FullName", SelectedEmp);

            if (YearEnding == null && TempData["YearEnding"] != null)
            {
                YearEnding = TempData["YearEnding"].ToString();
            }
            TempData["YearEnding"] = YearEnding;
            ViewBag.YearEnding = PopulateCodesDDL("YearEnding", YearEnding);
            ViewBag.PlanStatus = PopulateCodesDDL("PlanStatus");

            List<GoalFull> AllGoals = new List<GoalFull>();

            if (SelectedEmp != null)
            {
                TempData["SelectedEmp"] = SelectedEmp;

                var emp = (from e in db.tblHOTP_Employees
                           where e.EmployeeID == SelectedEmp
                           select e).First();

                var empGoals = from eg in db.tblHOTP_EmployeeGoals
                               join g in db.tblHOTP_Goals on eg.GoalID equals g.GoalID into goal
                               from subGoal in goal.DefaultIfEmpty()
                               where eg.EmployeeID == SelectedEmp && subGoal.YearEnding == YearEnding
                               select eg;
                foreach (tblHOTP_EmployeeGoals eg in empGoals)
                {
                    PopulatePlan(eg.EmployeeGoalID);
                }

                foreach (tblHOTP_EmployeeGoals eg in empGoals)
                {
                    List<Plan90Full> gPlans = new List<Plan90Full>();
                    var goalPlans = from plan in db.tblHOTP_Plan90 where plan.EmployeeGoalID == eg.EmployeeGoalID select plan;
                    foreach (tblHOTP_Plan90 plan in goalPlans)
                    {
                        gPlans.Add(new Plan90Full()
                        {
                            Plan = plan,
                            ActionSteps = db.tblHOTP_ActionSteps.Where(a => a.PlanID == plan.PlanID).ToList()
                        });
                    }

                    AllGoals.Add(new GoalFull()
                    {
                        EmployeeName = emp.FirstName + " " + emp.LastName,
                        Goal = db.tblHOTP_Goals.Find(eg.GoalID),
                        EmployeeGoal = eg,
                        Plans = gPlans
                    });
                }
            }
            return View(AllGoals.ToList());
        }


        // POST: Plan90ViewModel/All/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult All(List<GoalFull> GoalFull)
        //public ActionResult Edit([Bind(Include = "PlanID,EmployeeGoalID,Quarter,Goal")] tblHOTP_Plan90 tblHOTP_Plan90)
        {
            if (ModelState.IsValid)
            {
                foreach (GoalFull goalFull in GoalFull)
                {
                    foreach (Plan90Full plan90 in goalFull.Plans)
                    {
                        tblHOTP_Plan90 existingPlan = db.tblHOTP_Plan90.Find(plan90.Plan.PlanID);
                        existingPlan.Goal = plan90.Plan.Goal;
                        foreach (tblHOTP_ActionSteps actionStep in plan90.ActionSteps)
                        {
                            if (actionStep.ActionID != 99999 && actionStep.Status == "Remove")
                            {
                                tblHOTP_ActionSteps existingAction = db.tblHOTP_ActionSteps.Find(actionStep.ActionID);
                                db.tblHOTP_ActionSteps.Remove(existingAction);
                            }
                            else
                                if (actionStep.ActionID == 99999 && actionStep.Status != "Remove")
                                {
                                    tblHOTP_ActionSteps newActionStep = new tblHOTP_ActionSteps();
                                    newActionStep.PlanID = plan90.Plan.PlanID;
                                    newActionStep.ActionStep = actionStep.ActionStep;
                                    newActionStep.Result = actionStep.Result;
                                    newActionStep.Status = actionStep.Status;
                                    db.tblHOTP_ActionSteps.Add(newActionStep);
                                }
                                else
                                {
                                    if (actionStep.ActionID != 99999)
                                    {
                                        tblHOTP_ActionSteps existingAction = db.tblHOTP_ActionSteps.Find(actionStep.ActionID);
                                        existingAction.Result = actionStep.Result;
                                        existingAction.ActionStep = actionStep.ActionStep;
                                        existingAction.Status = actionStep.Status;
                                    }
                                }
                        }
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlanStatus = PopulateCodesDDL("PlanStatus");
            return View(GoalFull);
        }




        //if (id == null)
        //{
        //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //}
        //PopulatePlan(id);
        //tblHOTP_EmployeeGoals employeeGoal = db.tblHOTP_EmployeeGoals.Find(id);
        //tblHOTP_Goals goal = db.tblHOTP_Goals.Find(employeeGoal.GoalID);
        //tblHOTP_Employees employee = db.tblHOTP_Employees.Find(employeeGoal.EmployeeID);

        //var plans = from p in db.tblHOTP_Plan90
        //            where p.EmployeeGoalID == id
        //            select new Plan90ViewModel
        //            {
        //                PillarGoalName = goal.PillarGoalName,
        //                EmployeeGoal = goal.Goal,
        //                EmployeeGoalID = employeeGoal.EmployeeGoalID,
        //                Weight = employeeGoal.Weight,
        //                ItemScore = employeeGoal.ItemScore,
        //                Plan = p,
        //                ActionSteps = db.tblHOTP_ActionSteps.Where(a => a.PlanID == p.PlanID).ToList()
        //            };
        //ViewBag.PlanStatus = PopulateCodesDDL("PlanStatus");
        //return View(plans.ToList());



        // GET: Plan90ViewModel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHOTP_Plan90 tblHOTP_Plan90 = db.tblHOTP_Plan90.Find(id);
            if (tblHOTP_Plan90 == null)
            {
                return HttpNotFound();
            }
            return View(tblHOTP_Plan90);
        }

        // POST: Plan90ViewModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblHOTP_Plan90 tblHOTP_Plan90 = db.tblHOTP_Plan90.Find(id);
            db.tblHOTP_Plan90.Remove(tblHOTP_Plan90);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        private SelectList PopulateCodesDDL(string codeKey, object selectedCode = null)
        {
            var codeQuery = from p in db.tblHOTP_Codes
                            where p.CodeKey == codeKey
                            orderby p.Sequence
                            select p;
            SelectList sl = new SelectList(codeQuery, "Code", "Code", selectedCode);
            return sl;
        }


    }

}
