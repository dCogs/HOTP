﻿using System;
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
    public class EmployeeGoalsController : Controller
    {
        private HOTP_Entities db = new HOTP_Entities();

        // GET: EmployeeGoals
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
                            //where subGoal.GoalType=="Individual"
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


        // GET: EmployeeGoals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvalViewModel evalViewModel = GetEvalViewModel(id);
            if (evalViewModel == null)
            {
                return HttpNotFound();
            }
            return View(evalViewModel);

            //var empGoal = from eg in db.tblHOTP_EmployeeGoals
            //         where eg.EmployeeGoalID == id
            //         join g in db.tblHOTP_Goals on eg.GoalID equals g.GoalID into goal
            //         from subgoal in goal.DefaultIfEmpty()
            //         join e in db.tblHOTP_Employees on eg.EmployeeID equals e.EmployeeID into emp
            //         from subemp in emp.DefaultIfEmpty()
            //         select new EvalViewModel { Goal=subgoal, EmployeeGoalID=eg.EmployeeGoalID, EmployeeID=eg.EmployeeID,
            //             EmployeeName=subemp.LastName, ItemScore=eg.ItemScore, Weight=eg.Weight };
            //if (empGoal == null)
            //{
            //    return HttpNotFound();
            //}

        }


        //// GET: EmployeeGoals/Create
        //public ActionResult Create()
        //{
        //    ViewBag.EmployeeID = new SelectList(db.tblHOTP_Employees, "EmployeeID", "FirstName");
        //    ViewBag.GoalID = new SelectList(db.tblHOTP_Goals, "GoalID", "YearEnding");
        //    return View();
        //}

        //// POST: EmployeeGoals/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "EmployeeGoalID,EmployeeID,GoalID,Weight,Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec,Notes")] tblHOTP_EmployeeGoals tblHOTP_EmployeeGoals)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.tblHOTP_EmployeeGoals.Add(tblHOTP_EmployeeGoals);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.EmployeeID = new SelectList(db.tblHOTP_Employees, "EmployeeID", "FirstName", tblHOTP_EmployeeGoals.EmployeeID);
        //    ViewBag.GoalID = new SelectList(db.tblHOTP_Goals, "GoalID", "YearEnding", tblHOTP_EmployeeGoals.GoalID);
        //    return View(tblHOTP_EmployeeGoals);
        //}

        // GET: EmployeeGoals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EvalViewModel evalViewModel = GetEvalViewModel(id);

            //tblHOTP_EmployeeGoals tblHOTP_EmployeeGoals = db.tblHOTP_EmployeeGoals.Find(id);
            //tblHOTP_Goals tblHOTP_Goals = db.tblHOTP_Goals.Find(tblHOTP_EmployeeGoals.GoalID);
            //tblHOTP_Employees tblHOTP_Employees = db.tblHOTP_Employees.Find(tblHOTP_EmployeeGoals.EmployeeID);
            //ViewBag.Pillar = PopulateCodesDDL("Pillar", tblHOTP_Goals.Pillar);
            //ViewBag.YearEnding = PopulateCodesDDL("YearEnding", tblHOTP_Goals.YearEnding);
            //ViewBag.ResultsMeasured = PopulateCodesDDL("ResultsMeasured", tblHOTP_Goals.ResultsMeasured);
            //ViewBag.BestRating = PopulateCodesDDL("BestRating", tblHOTP_Goals.BestRating);
            //ViewBag.YearEndCalculation = PopulateCodesDDL("YearEndCalculation", tblHOTP_Goals.YearEndCalculation);
            //ViewBag.UnitsMeasuredIn = PopulateCodesDDL("UnitsMeasuredIn", tblHOTP_Goals.UnitsMeasuredIn);
            //ViewBag.ResultsEntered = PopulateCodesDDL("ResultsEntered", tblHOTP_Goals.ResultsEntered);

            //EvalViewModel evalViewModel = new EvalViewModel
            //{
            //    Goal = tblHOTP_Goals,
            //    EmployeeGoalID = tblHOTP_EmployeeGoals.EmployeeGoalID,
            //    EmployeeID = tblHOTP_EmployeeGoals.EmployeeID,
            //    EmployeeName = tblHOTP_Employees.LastName,
            //    ItemScore = tblHOTP_EmployeeGoals.ItemScore,
            //    Weight = tblHOTP_EmployeeGoals.Weight
            //};
            if (evalViewModel == null)
            {
                return HttpNotFound();
            }
            return View(evalViewModel);
        }


        // POST: EmployeeGoals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EvalViewModel returnedEvalViewModel)
        {
            if (ModelState.IsValid)
            {
                var goal = db.tblHOTP_Goals.Find(returnedEvalViewModel.Goal.GoalID);
                goal.YearEnding = returnedEvalViewModel.Goal.YearEnding;
                goal.GoalType = returnedEvalViewModel.Goal.GoalType;
                goal.Pillar = returnedEvalViewModel.Goal.Pillar;
                goal.GoalName = returnedEvalViewModel.Goal.GoalName;
                goal.PillarGoalName = returnedEvalViewModel.Goal.PillarGoalName;
                goal.Goal = returnedEvalViewModel.Goal.Goal;
                goal.Status = returnedEvalViewModel.Goal.Status;
                goal.ResultsMeasured = returnedEvalViewModel.Goal.ResultsMeasured;
                goal.BestRating = returnedEvalViewModel.Goal.BestRating;
                goal.YearEndCalculation = returnedEvalViewModel.Goal.YearEndCalculation;
                goal.UnitsMeasuredIn = returnedEvalViewModel.Goal.UnitsMeasuredIn;
                goal.NumDecimals = returnedEvalViewModel.Goal.NumDecimals;
                goal.ResultsEntered = returnedEvalViewModel.Goal.ResultsEntered;
                goal.EnteredBy = returnedEvalViewModel.Goal.EnteredBy;
                goal.NonEditableByLeader = returnedEvalViewModel.Goal.NonEditableByLeader;
                goal.Rating1 = returnedEvalViewModel.Goal.Rating1;
                goal.Rating2 = returnedEvalViewModel.Goal.Rating2;
                goal.Rating3 = returnedEvalViewModel.Goal.Rating3;
                goal.Rating4 = returnedEvalViewModel.Goal.Rating4;
                goal.Rating5 = returnedEvalViewModel.Goal.Rating5;
                goal.Rating1Suffix = returnedEvalViewModel.Goal.Rating1Suffix;
                goal.Rating2End = returnedEvalViewModel.Goal.Rating2End;
                goal.Rating3End = returnedEvalViewModel.Goal.Rating3End;
                goal.Rating4End = returnedEvalViewModel.Goal.Rating4End;
                goal.Rating5Suffix = returnedEvalViewModel.Goal.Rating5Suffix;
                goal.Jan = returnedEvalViewModel.Goal.Jan;
                goal.Feb = returnedEvalViewModel.Goal.Feb;
                goal.Mar = returnedEvalViewModel.Goal.Mar;
                goal.Apr = returnedEvalViewModel.Goal.Apr;
                goal.May = returnedEvalViewModel.Goal.May;
                goal.Jun = returnedEvalViewModel.Goal.Jun;
                goal.Jul = returnedEvalViewModel.Goal.Jul;
                goal.Aug = returnedEvalViewModel.Goal.Aug;
                goal.Sep = returnedEvalViewModel.Goal.Sep;
                goal.Oct = returnedEvalViewModel.Goal.Oct;
                goal.Nov = returnedEvalViewModel.Goal.Nov;
                goal.Dec = returnedEvalViewModel.Goal.Dec;
                tblHOTP_EmployeeGoals existing_EmployeeGoals = db.tblHOTP_EmployeeGoals.Find(returnedEvalViewModel.EmployeeGoalID);
                existing_EmployeeGoals.Weight = returnedEvalViewModel.Weight;
                //existing_EmployeeGoals.ItemScore = returnedEvalViewModel.ItemScore;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            tblHOTP_EmployeeGoals tblHOTP_EmployeeGoals = db.tblHOTP_EmployeeGoals.Find(returnedEvalViewModel.EmployeeGoalID);
            tblHOTP_Goals tblHOTP_Goals2 = db.tblHOTP_Goals.Find(tblHOTP_EmployeeGoals.GoalID);
            tblHOTP_Employees tblHOTP_Employees = db.tblHOTP_Employees.Find(tblHOTP_EmployeeGoals.EmployeeID);
            EvalViewModel evalViewModel = new EvalViewModel
            {
                Goal = tblHOTP_Goals2,
                EmployeeGoalID = tblHOTP_EmployeeGoals.EmployeeGoalID,
                EmployeeID = tblHOTP_EmployeeGoals.EmployeeID,
                EmployeeName = tblHOTP_Employees.LastName,
                ItemScore = tblHOTP_EmployeeGoals.ItemScore,
                Weight = tblHOTP_EmployeeGoals.Weight
            };
            ViewBag.Pillar = PopulateCodesDDL("Pillar", tblHOTP_Goals2.Pillar);
            ViewBag.YearEnding = PopulateCodesDDL("YearEnding", tblHOTP_Goals2.YearEnding);
            ViewBag.ResultsMeasured = PopulateCodesDDL("ResultsMeasured", tblHOTP_Goals2.ResultsMeasured);
            ViewBag.BestRating = PopulateCodesDDL("BestRating", tblHOTP_Goals2.BestRating);
            ViewBag.YearEndCalculation = PopulateCodesDDL("YearEndCalculation", tblHOTP_Goals2.YearEndCalculation);
            ViewBag.UnitsMeasuredIn = PopulateCodesDDL("UnitsMeasuredIn", tblHOTP_Goals2.UnitsMeasuredIn);
            ViewBag.ResultsEntered = PopulateCodesDDL("ResultsEntered", tblHOTP_Goals2.ResultsEntered);
            return View(evalViewModel);
        }


        // GET: EmployeeGoals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvalViewModel evalViewModel = GetEvalViewModel(id);
            if (evalViewModel == null)
            {
                return HttpNotFound();
            }
            return View(evalViewModel);

            //tblHOTP_EmployeeGoals tblHOTP_EmployeeGoals = db.tblHOTP_EmployeeGoals.Find(id);
            //if (tblHOTP_EmployeeGoals == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(tblHOTP_EmployeeGoals);
        }


        // POST: EmployeeGoals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblHOTP_EmployeeGoals tblHOTP_EmployeeGoals = db.tblHOTP_EmployeeGoals.Find(id);
            db.tblHOTP_EmployeeGoals.Remove(tblHOTP_EmployeeGoals);
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


        // GET: Goals/Create
        public ActionResult Create(int? SelectedEmp)
        {
            if (SelectedEmp == null && TempData["SelectedEmp"] != null)
            {
                SelectedEmp = Convert.ToInt16(TempData["SelectedEmp"]);
            }
            string empName = "";
            try
            {
                //string employeeID = Request.QueryString["EmployeeID"];
                //selectedEmp = Convert.ToInt16(employeeID);
                empName = (db.tblHOTP_Employees.Where(e => e.EmployeeID == SelectedEmp).Select(e => e.FirstName + " " + e.LastName)).First();
                //(db.tblHOTP_Codes.Where(c => c.CodeKey == "YearEnding" && c.Code == year).Select(c => c.CodeId)).First();
            }
            catch { }
            if (SelectedEmp != null)
            {
                tblHOTP_Goals goal = new tblHOTP_Goals();
                goal.Rating1 = 0;
                goal.Rating2 = 0;
                goal.Rating3 = 0;
                goal.Rating4 = 0;
                goal.Rating5 = 0;
                goal.NumDecimals = 0;
                goal.Status = "Active";
                goal.GoalType = "Individual";
                TempData["SelectedEmp"] = SelectedEmp;
                ViewBag.Pillar = PopulateCodesDDL("Pillar");
                ViewBag.YearEnding = PopulateCodesDDL("YearEnding");
                ViewBag.ResultsMeasured = PopulateCodesDDL("ResultsMeasured");
                ViewBag.BestRating = PopulateCodesDDL("BestRating");
                ViewBag.YearEndCalculation = PopulateCodesDDL("YearEndCalculation");
                ViewBag.UnitsMeasuredIn = PopulateCodesDDL("UnitsMeasuredIn");
                ViewBag.ResultsEntered = PopulateCodesDDL("ResultsEntered");

                EvalViewModel evalViewModel = new EvalViewModel
                {
                    EmployeeID = (int)SelectedEmp,
                    EmployeeName = empName,
                    Goal = goal
                };
                return View(evalViewModel);
            }
            return View("index");
            //ViewBag.Pillar = PopulateCodesDDL("Pillar");
            //ViewBag.YearEnding = PopulateCodesDDL("YearEnding");
            //ViewBag.ResultsMeasured = PopulateCodesDDL("ResultsMeasured");
            //ViewBag.BestRating = PopulateCodesDDL("BestRating");
            //ViewBag.YearEndCalculation = PopulateCodesDDL("YearEndCalculation");
            //ViewBag.UnitsMeasuredIn = PopulateCodesDDL("UnitsMeasuredIn");
            //ViewBag.ResultsEntered = PopulateCodesDDL("ResultsEntered");
            //ViewBag.EmployeeID = selectedEmp;
            //ViewBag.EmployeeName = empName;
            //return View(evalViewModel);
        }


        // POST: Goals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EvalViewModel returnedEvalViewModel)
        //public ActionResult Create([Bind(Include = "GoalID,YearEnding,GoalType,Pillar,GoalName,PillarGoalName,Goal,Status,ResultsMeasured,BestRating,Rating1,Rating2,Rating3,Rating4,Rating5,YearEndCalculation,UnitsMeasuredIn,NumDecimals,ResultsEntered,EnteredBy,NonEditableByLeader,Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec")] tblHOTP_Goals tblHOTP_Goals)
        {
            if (ModelState.IsValid)
            {
                tblHOTP_Goals newGoal = new tblHOTP_Goals();
                //var goal = db.tblHOTP_Goals.Find(returnedEvalViewModel.Goal.GoalID);
                newGoal.YearEnding = returnedEvalViewModel.Goal.YearEnding;
                newGoal.GoalType = returnedEvalViewModel.Goal.GoalType;
                newGoal.Pillar = returnedEvalViewModel.Goal.Pillar;
                newGoal.GoalName = returnedEvalViewModel.Goal.GoalName;
                newGoal.PillarGoalName = returnedEvalViewModel.Goal.PillarGoalName;
                newGoal.Goal = returnedEvalViewModel.Goal.Goal;
                newGoal.Status = returnedEvalViewModel.Goal.Status;
                newGoal.ResultsMeasured = returnedEvalViewModel.Goal.ResultsMeasured;
                newGoal.BestRating = returnedEvalViewModel.Goal.BestRating;
                newGoal.YearEndCalculation = returnedEvalViewModel.Goal.YearEndCalculation;
                newGoal.UnitsMeasuredIn = returnedEvalViewModel.Goal.UnitsMeasuredIn;
                newGoal.NumDecimals = returnedEvalViewModel.Goal.NumDecimals;
                newGoal.ResultsEntered = returnedEvalViewModel.Goal.ResultsEntered;
                newGoal.EnteredBy = returnedEvalViewModel.Goal.EnteredBy;
                newGoal.NonEditableByLeader = returnedEvalViewModel.Goal.NonEditableByLeader;

                newGoal.Rating1 = returnedEvalViewModel.Goal.Rating1;
                newGoal.Rating2 = returnedEvalViewModel.Goal.Rating2;
                newGoal.Rating3 = returnedEvalViewModel.Goal.Rating3;
                newGoal.Rating4 = returnedEvalViewModel.Goal.Rating4;
                newGoal.Rating5 = returnedEvalViewModel.Goal.Rating5;
                newGoal.Rating1Suffix = returnedEvalViewModel.Goal.Rating1Suffix;
                newGoal.Rating2End = returnedEvalViewModel.Goal.Rating2End;
                newGoal.Rating3End = returnedEvalViewModel.Goal.Rating3End;
                newGoal.Rating4End = returnedEvalViewModel.Goal.Rating4End;
                newGoal.Rating5Suffix = returnedEvalViewModel.Goal.Rating5Suffix;

                newGoal.Jan = returnedEvalViewModel.Goal.Jan;
                newGoal.Feb = returnedEvalViewModel.Goal.Feb;
                newGoal.Mar = returnedEvalViewModel.Goal.Mar;
                newGoal.Apr = returnedEvalViewModel.Goal.Apr;
                newGoal.May = returnedEvalViewModel.Goal.May;
                newGoal.Jun = returnedEvalViewModel.Goal.Jun;
                newGoal.Jul = returnedEvalViewModel.Goal.Jul;
                newGoal.Aug = returnedEvalViewModel.Goal.Aug;
                newGoal.Sep = returnedEvalViewModel.Goal.Sep;
                newGoal.Oct = returnedEvalViewModel.Goal.Oct;
                newGoal.Nov = returnedEvalViewModel.Goal.Nov;
                newGoal.Dec = returnedEvalViewModel.Goal.Dec;
                db.tblHOTP_Goals.Add(newGoal);
                db.SaveChanges();
                int goalID = newGoal.GoalID;

                tblHOTP_EmployeeGoals newEmployeeGoal = new tblHOTP_EmployeeGoals(); // db.tblHOTP_EmployeeGoals.Find(returnedEvalViewModel.EmployeeGoalID);
                newEmployeeGoal.GoalID = goalID;
                newEmployeeGoal.EmployeeID = returnedEvalViewModel.EmployeeID;
                newEmployeeGoal.Weight = returnedEvalViewModel.Weight;
                newEmployeeGoal.ItemScore = 0; // returnedEvalViewModel.ItemScore;
                db.tblHOTP_EmployeeGoals.Add(newEmployeeGoal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //if (ModelState.IsValid)
            //{
            //    db.tblHOTP_Goals.Add(tblHOTP_Goals);
            //    db.SaveChanges();
            //    int goalID = tblHOTP_Goals.GoalID;
            //    var newEmployeeGoal = new tblHOTP_EmployeeGoals();
            //    string employeeID = Request["EmployeeID"];
            //    newEmployeeGoal.GoalID = goalID;
            //    newEmployeeGoal.EmployeeID = Convert.ToInt16(employeeID);
            //    newEmployeeGoal.Weight = Convert.ToInt16(Request["Weight"]); // Convert.ToInt16(myString);
            //    db.tblHOTP_EmployeeGoals.Add(newEmployeeGoal);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            ViewBag.Pillar = PopulateCodesDDL("Pillar");
            ViewBag.YearEnding = PopulateCodesDDL("YearEnding");
            ViewBag.ResultsMeasured = PopulateCodesDDL("ResultsMeasured");
            ViewBag.BestRating = PopulateCodesDDL("BestRating");
            ViewBag.YearEndCalculation = PopulateCodesDDL("YearEndCalculation");
            ViewBag.UnitsMeasuredIn = PopulateCodesDDL("UnitsMeasuredIn");
            ViewBag.ResultsEntered = PopulateCodesDDL("ResultsEntered");
            return View(returnedEvalViewModel);
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



        // GET: EmployeeGoals/Eval
        public ActionResult Eval(int? SelectedEmp)
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
            if (SelectedEmp != null)
            {
                TempData["SelectedEmp"] = SelectedEmp;
                var goals = from eg in db.tblHOTP_EmployeeGoals
                            where eg.EmployeeID == SelectedEmp
                            join g in db.tblHOTP_Goals on eg.GoalID equals g.GoalID into goal
                            from subGoal in goal.DefaultIfEmpty()
                            join c in db.tblHOTP_Codes on subGoal.Pillar equals c.Code into code
                            from subCode in code.DefaultIfEmpty()
                            orderby subCode.Sequence, subGoal.PillarGoalName
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


        // POST: Goals/Eval
        [HttpPost]
        public ActionResult Eval(List<EvalViewModel> MyGoals)
        {
            string year = "";
            foreach (EvalViewModel eval in MyGoals)
            {
                tblHOTP_Goals existingGoal = db.tblHOTP_Goals.Find(eval.Goal.GoalID);
                if (existingGoal.GoalType == "Individual")
                {
                    existingGoal.JanScore = eval.Goal.JanScore;
                    existingGoal.FebScore = eval.Goal.FebScore;
                    existingGoal.MarScore = eval.Goal.MarScore;
                    existingGoal.AprScore = eval.Goal.AprScore;
                    existingGoal.MayScore = eval.Goal.MayScore;
                    existingGoal.JunScore = eval.Goal.JunScore;
                    existingGoal.JulScore = eval.Goal.JulScore;
                    existingGoal.AugScore = eval.Goal.AugScore;
                    existingGoal.SepScore = eval.Goal.SepScore;
                    existingGoal.OctScore = eval.Goal.OctScore;
                    existingGoal.NovScore = eval.Goal.NovScore;
                    existingGoal.DecScore = eval.Goal.DecScore;
                    existingGoal.Result = eval.Goal.Result;
                    existingGoal.Score = eval.Goal.Score;
                    year = eval.Goal.YearEnding;
                    tblHOTP_EmployeeGoals existingEmployeeGoal = db.tblHOTP_EmployeeGoals.Find(eval.EmployeeGoalID); // Where(eg => eg.EmployeeID == eval.EmployeeID && eg.GoalID == eval.Goal.GoalID).First();//.Select();
                    if (existingGoal.GoalType == "Individual")
                    {
                        existingEmployeeGoal.ItemScore = eval.ItemScore;
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: EmployeeGoals/ReportCard
        public ActionResult ReportCard(int? SelectedEmp, string YearEnding)
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
                            join c in db.tblHOTP_Codes on subGoal.Pillar equals c.Code into code
                            from subCode in code.DefaultIfEmpty()
                            where eg.EmployeeID == SelectedEmp && subGoal.YearEnding==YearEnding
                            orderby subCode.Sequence, subGoal.PillarGoalName
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


        // POST: Goals/ReportCard
        [HttpPost]
        public ActionResult ReportCard(List<EvalViewModel> MyGoals)
        {
            string year = "";
            foreach (EvalViewModel eval in MyGoals)
            {
                tblHOTP_Goals existingGoal = db.tblHOTP_Goals.Find(eval.Goal.GoalID);
                if (existingGoal.GoalType == "Individual")
                {
                    existingGoal.JanScore = eval.Goal.JanScore;
                    existingGoal.FebScore = eval.Goal.FebScore;
                    existingGoal.MarScore = eval.Goal.MarScore;
                    existingGoal.AprScore = eval.Goal.AprScore;
                    existingGoal.MayScore = eval.Goal.MayScore;
                    existingGoal.JunScore = eval.Goal.JunScore;
                    existingGoal.JulScore = eval.Goal.JulScore;
                    existingGoal.AugScore = eval.Goal.AugScore;
                    existingGoal.SepScore = eval.Goal.SepScore;
                    existingGoal.OctScore = eval.Goal.OctScore;
                    existingGoal.NovScore = eval.Goal.NovScore;
                    existingGoal.DecScore = eval.Goal.DecScore;
                    existingGoal.Result = eval.Goal.Result;
                    existingGoal.Score = eval.Goal.Score;
                    year = eval.Goal.YearEnding;
                    tblHOTP_EmployeeGoals existingEmployeeGoal = db.tblHOTP_EmployeeGoals.Find(eval.EmployeeGoalID); // Where(eg => eg.EmployeeID == eval.EmployeeID && eg.GoalID == eval.Goal.GoalID).First();//.Select();
                    if (existingGoal.GoalType == "Individual")
                    {
                        existingEmployeeGoal.ItemScore = eval.ItemScore;
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public EvalViewModel GetEvalViewModel(int? id)
        {
            tblHOTP_EmployeeGoals tblHOTP_EmployeeGoals = db.tblHOTP_EmployeeGoals.Find(id);
            tblHOTP_Goals tblHOTP_Goals = db.tblHOTP_Goals.Find(tblHOTP_EmployeeGoals.GoalID);
            tblHOTP_Employees tblHOTP_Employees = db.tblHOTP_Employees.Find(tblHOTP_EmployeeGoals.EmployeeID);
            ViewBag.Pillar = PopulateCodesDDL("Pillar", tblHOTP_Goals.Pillar);
            ViewBag.YearEnding = PopulateCodesDDL("YearEnding", tblHOTP_Goals.YearEnding);
            ViewBag.ResultsMeasured = PopulateCodesDDL("ResultsMeasured", tblHOTP_Goals.ResultsMeasured);
            ViewBag.BestRating = PopulateCodesDDL("BestRating", tblHOTP_Goals.BestRating);
            ViewBag.YearEndCalculation = PopulateCodesDDL("YearEndCalculation", tblHOTP_Goals.YearEndCalculation);
            ViewBag.UnitsMeasuredIn = PopulateCodesDDL("UnitsMeasuredIn", tblHOTP_Goals.UnitsMeasuredIn);
            ViewBag.ResultsEntered = PopulateCodesDDL("ResultsEntered", tblHOTP_Goals.ResultsEntered);

            EvalViewModel evalViewModel = new EvalViewModel
            {
                Goal = tblHOTP_Goals,
                EmployeeGoalID = tblHOTP_EmployeeGoals.EmployeeGoalID,
                EmployeeID = tblHOTP_EmployeeGoals.EmployeeID,
                EmployeeName = tblHOTP_Employees.FirstName + " " + tblHOTP_Employees.LastName,
                ItemScore = tblHOTP_EmployeeGoals.ItemScore,
                Weight = tblHOTP_EmployeeGoals.Weight
            };
            if (evalViewModel == null)
            {
                return (evalViewModel);
            }
            return (evalViewModel);
        }





        // GET: Goals/AddOrg
        public ActionResult AddOrg(int? SelectedEmp)
        {
            if (SelectedEmp == null && TempData["SelectedEmp"] != null)
            {
                SelectedEmp = Convert.ToInt16(TempData["SelectedEmp"]);
            }
            string empName = (db.tblHOTP_Employees.Where(e => e.EmployeeID == SelectedEmp).Select(e => e.FirstName + " " + e.LastName)).First();
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
            if (SelectedEmp != null)
            {
                TempData["SelectedEmp"] = SelectedEmp;
                AddOrg addOrg = new AddOrg();
                addOrg.EmpName = empName;
                addOrg.Weight = 0;
                addOrg.EmployeeID = Convert.ToInt16(SelectedEmp);
                var goalQuery = db.tblHOTP_Goals
                                .Where(g => g.GoalType == "Organizational")
                                .OrderBy(g => g.YearEnding)
                                .Select(g => new
                                {
                                    GoalId = g.GoalID,
                                    GoalDisplayText = g.YearEnding + " " + g.PillarGoalName
                                });
                ViewBag.GoalID = new SelectList(goalQuery, "GoalID", "GoalDisplayText");
                return View(addOrg);
            }
            return View("index");
        }


        // POST: Goals/AddOrg
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrg(AddOrg addOrg)
        {
            if (ModelState.IsValid)
            {
                tblHOTP_EmployeeGoals newEmployeeGoals = new tblHOTP_EmployeeGoals();
                newEmployeeGoals.GoalID = addOrg.GoalID;
                newEmployeeGoals.EmployeeID = addOrg.EmployeeID;
                newEmployeeGoals.Weight = addOrg.Weight;
                int? score = (db.tblHOTP_Goals.Where(g => g.GoalID == addOrg.GoalID).Select(g => g.Score)).First();
                decimal rounded = decimal.Round(Convert.ToDecimal(newEmployeeGoals.Weight) * Convert.ToDecimal(score) / 100, 2);
                newEmployeeGoals.ItemScore = rounded;
                db.tblHOTP_EmployeeGoals.Add(newEmployeeGoals);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(addOrg);
        }




    }
}
