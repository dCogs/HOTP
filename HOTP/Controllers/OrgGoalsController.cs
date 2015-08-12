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
    public class OrgGoalsController : Controller
    {
        private HOTP_Entities db = new HOTP_Entities();

        // GET: Goals
        public ActionResult Index(int? SelectedKey)
        {
            string year = "";
            if (SelectedKey == null)
            {
                if (Request.QueryString["year"] == null)
                {
                    var currentYear = (db.tblHOTP_Codes.Where(w => w.CodeKey == "CurrentYear").Select(w => w.Code)).First();
                    year = currentYear.ToString();
                }
                else year = Request.QueryString["year"]; 
                try
                {
                    SelectedKey = (db.tblHOTP_Codes.Where(c => c.CodeKey == "YearEnding" && c.Code == year).Select(c => c.CodeId)).First();
                }
                catch { }
            }
            if (SelectedKey != null)
            {
                tblHOTP_Codes selected = db.tblHOTP_Codes.Find(SelectedKey);
                year = selected.Code;
                var keys = db.tblHOTP_Codes.Where(w => w.CodeKey == "YearEnding").OrderBy(q => q.Code).ToList();
                ViewBag.SelectedKey = new SelectList(keys, "CodeID", "Code", SelectedKey);
                IQueryable<tblHOTP_Goals> tblHOTP_Goals = db.tblHOTP_Goals
                    .Where(g => g.YearEnding == year && g.GoalType == "Organizational");
                //.OrderBy(d => d.Sequence);
                //var sql = tblHOTP_Goals.ToString();
                return View(tblHOTP_Goals.ToList());
            }
            else
            {
                var keys = db.tblHOTP_Codes.Where(w => w.CodeKey == "YearEnding").OrderBy(q => q.Code).ToList();
                ViewBag.SelectedKey = new SelectList(keys, "CodeID", "Code", SelectedKey);
                IQueryable<tblHOTP_Goals> tblHOTP_Goals = db.tblHOTP_Goals
                    .Where(g => g.YearEnding == "2999");
                //.OrderBy(d => d.Sequence);
                //var sql = tblHOTP_Goals.ToString();
                return View(tblHOTP_Goals.ToList());
            }

            //return View(db.tblHOTP_Goals.ToList());
        }

        // GET: Goals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHOTP_Goals tblHOTP_Goals = db.tblHOTP_Goals.Find(id);
            if (tblHOTP_Goals == null)
            {
                return HttpNotFound();
            }
            return View(tblHOTP_Goals); 
        }

        // GET: Goals/Create
        public ActionResult Create()
        {
            tblHOTP_Goals goal = new tblHOTP_Goals();
            goal.Rating1 = 0;
            goal.Rating2 = 0;
            goal.Rating3 = 0;
            goal.Rating4 = 0;
            goal.Rating5 = 0;
            goal.NumDecimals = 0;
            goal.Status = "Active";
            goal.GoalType = "Organizational";
            ViewBag.Pillar = PopulateCodesDDL("Pillar");
            ViewBag.YearEnding = PopulateCodesDDL("YearEnding");
            ViewBag.ResultsMeasured = PopulateCodesDDL("ResultsMeasured");
            ViewBag.BestRating = PopulateCodesDDL("BestRating");
            ViewBag.YearEndCalculation = PopulateCodesDDL("YearEndCalculation");
            ViewBag.UnitsMeasuredIn = PopulateCodesDDL("UnitsMeasuredIn");
            ViewBag.ResultsEntered = PopulateCodesDDL("ResultsEntered");
            return View(goal);
        }

        // POST: Goals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GoalID,YearEnding,GoalType,Pillar,GoalName,PillarGoalName,Goal,Status,ResultsMeasured,BestRating,Rating1,Rating2,Rating3,Rating4,Rating5,Rating1Suffix,Rating5Suffix,Rating2End,Rating3End,Rating4End,YearEndCalculation,UnitsMeasuredIn,NumDecimals,ResultsEntered,EnteredBy,NonEditableByLeader,Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec")] tblHOTP_Goals tblHOTP_Goals)
        {
            if (ModelState.IsValid)
            {
                db.tblHOTP_Goals.Add(tblHOTP_Goals);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Pillar = PopulateCodesDDL("Pillar");
            ViewBag.YearEnding = PopulateCodesDDL("YearEnding");
            ViewBag.ResultsMeasured = PopulateCodesDDL("ResultsMeasured");
            ViewBag.BestRating = PopulateCodesDDL("BestRating");
            ViewBag.YearEndCalculation = PopulateCodesDDL("YearEndCalculation");
            ViewBag.UnitsMeasuredIn = PopulateCodesDDL("UnitsMeasuredIn");
            ViewBag.ResultsEntered = PopulateCodesDDL("ResultsEntered");
            return View(tblHOTP_Goals);
        }

        // GET: Goals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHOTP_Goals tblHOTP_Goals = db.tblHOTP_Goals.Find(id);
            if (tblHOTP_Goals == null)
            {
                return HttpNotFound();
            }
            ViewBag.Pillar = PopulateCodesDDL("Pillar", tblHOTP_Goals.Pillar);
            ViewBag.YearEnding = PopulateCodesDDL("YearEnding", tblHOTP_Goals.YearEnding);
            ViewBag.ResultsMeasured = PopulateCodesDDL("ResultsMeasured", tblHOTP_Goals.ResultsMeasured);
            ViewBag.BestRating = PopulateCodesDDL("BestRating", tblHOTP_Goals.BestRating);
            ViewBag.YearEndCalculation = PopulateCodesDDL("YearEndCalculation", tblHOTP_Goals.YearEndCalculation);
            ViewBag.UnitsMeasuredIn = PopulateCodesDDL("UnitsMeasuredIn", tblHOTP_Goals.UnitsMeasuredIn);
            ViewBag.ResultsEntered = PopulateCodesDDL("ResultsEntered", tblHOTP_Goals.ResultsEntered);
            return View(tblHOTP_Goals);
        }

        // POST: Goals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GoalID,YearEnding,GoalType,Pillar,GoalName,PillarGoalName,Goal,Status,ResultsMeasured,BestRating,Rating1,Rating2,Rating3,Rating4,Rating5,Rating1Suffix,Rating5Suffix,Rating2End,Rating3End,Rating4End,YearEndCalculation,UnitsMeasuredIn,NumDecimals,ResultsEntered,EnteredBy,NonEditableByLeader,Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec")] tblHOTP_Goals tblHOTP_Goals)
        {
            var goal = db.tblHOTP_Goals.Find(tblHOTP_Goals.GoalID);
            if (goal != null)
                TryUpdateModel(goal);
            else
                return HttpNotFound();
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Index", new { year = tblHOTP_Goals.YearEnding });
            }
            //if (ModelState.IsValid)
            //{
            //    db.Entry(tblHOTP_Goals).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index", new { year = tblHOTP_Goals.YearEnding });
            //}
            ViewBag.Pillar = PopulateCodesDDL("Pillar", tblHOTP_Goals.Pillar);
            ViewBag.YearEnding = PopulateCodesDDL("YearEnding", tblHOTP_Goals.YearEnding);
            ViewBag.ResultsMeasured = PopulateCodesDDL("ResultsMeasured", tblHOTP_Goals.ResultsMeasured);
            ViewBag.BestRating = PopulateCodesDDL("BestRating", tblHOTP_Goals.BestRating);
            ViewBag.YearEndCalculation = PopulateCodesDDL("YearEndCalculation", tblHOTP_Goals.YearEndCalculation);
            ViewBag.UnitsMeasuredIn = PopulateCodesDDL("UnitsMeasuredIn", tblHOTP_Goals.UnitsMeasuredIn);
            ViewBag.ResultsEntered = PopulateCodesDDL("ResultsEntered", tblHOTP_Goals.ResultsEntered);

            return View(tblHOTP_Goals);
        }

        // GET: Goals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHOTP_Goals tblHOTP_Goals = db.tblHOTP_Goals.Find(id);
            if (tblHOTP_Goals == null)
            {
                return HttpNotFound();
            }
            return View(tblHOTP_Goals);
        }

        // POST: Goals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblHOTP_Goals tblHOTP_Goals = db.tblHOTP_Goals.Find(id);
            db.tblHOTP_Goals.Remove(tblHOTP_Goals);
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

        // GET: Goals/Cascade
        public ActionResult Cascade()
        {
            var cascadeGoals = new CascadeGoals();
            cascadeGoals.Employees = (from e in db.tblHOTP_Employees where e.Evaluations orderby e.LastName select e).ToList();
            cascadeGoals.Goals = (from g in db.tblHOTP_Goals where g.GoalType=="Organizational"
                                  join c in db.tblHOTP_Codes on g.Pillar equals c.Code
                                  orderby c.Sequence
                                  select g).ToList();
            return View(cascadeGoals);
        }

        // POST: Goals/Cascade
        [HttpPost]
        public ActionResult Cascade(List<tblHOTP_Employees> MyEmps)
        {
            var Employees = (from e in db.tblHOTP_Employees where e.Evaluations orderby e.LastName select e); //).ToList();

            var Goals = (from g in db.tblHOTP_Goals
                         join c in db.tblHOTP_Codes on g.Pillar equals c.Code
                         select g);
            string year = "";

            foreach (tblHOTP_Employees emp in Employees)
            {
                foreach (tblHOTP_Goals goal in Goals)
                { 
                    year = goal.YearEnding;
                    try
                    {
                        string inputField = "MyGoals_" + emp.EmployeeID + "_" + goal.GoalID + "_Weight";
                        string myString = Request.Form[inputField];

                        if (db.tblHOTP_EmployeeGoals.Any(eg => eg.EmployeeID == emp.EmployeeID && eg.GoalID == goal.GoalID))
                        // tblHOTP_EmployeeGoals already exists
                        {
                            if (myString.CompareTo("") == 0)
                            {
                                // DON'T remove it. They could have added it separately.
                                //var itemToRemove = db.tblHOTP_EmployeeGoals.SingleOrDefault(eg => eg.EmployeeID == emp.EmployeeID && eg.GoalID == goal.GoalID);
                                //if (itemToRemove != null)
                                //{
                                //    db.tblHOTP_EmployeeGoals.Remove(itemToRemove);
                                //}
                            }
                            else
                            {
                                var itemToUpdate = db.tblHOTP_EmployeeGoals.SingleOrDefault(eg => eg.EmployeeID == emp.EmployeeID && eg.GoalID == goal.GoalID);
                                if (itemToUpdate != null)
                                {
                                    itemToUpdate.Weight = Convert.ToInt16(myString);
                                }
                            }
                        }
                        else
                        //Doesn't already exist
                        {
                            if (myString.CompareTo("") == 0)
                            {
                                // do nothing
                            }
                            else
                            {
                                var newEmployeeGoal = new tblHOTP_EmployeeGoals();
                                newEmployeeGoal.EmployeeID = emp.EmployeeID;
                                newEmployeeGoal.GoalID = goal.GoalID;
                                newEmployeeGoal.Weight = Convert.ToInt16(myString);
                                db.tblHOTP_EmployeeGoals.Add(newEmployeeGoal);
                            }
                        }

                    }
                    catch { }
                }
            }
            db.SaveChanges();

            //string SelectedYear = MV.SelectedVendor;
            return RedirectToAction("Index", new { year = year });
            //return View(employees.ToList());
        }


        // GET: Goals
        public ActionResult Scores()
        {
            string year = "";
            year = Request.QueryString["year"];
            //if (SelectedKey != null)
            //{
            //    tblHOTP_Codes selected = db.tblHOTP_Codes.Find(SelectedKey);
            //    year = selected.Code;
            //    var keys = db.tblHOTP_Codes.Where(w => w.CodeKey == "YearEnding").OrderBy(q => q.Code).ToList();
            //    ViewBag.SelectedKey = new SelectList(keys, "CodeID", "Code", SelectedKey);
                IQueryable<tblHOTP_Goals> tblHOTP_Goals = db.tblHOTP_Goals
                    .Where(g => g.YearEnding == year && g.GoalType == "Organizational");
                //.OrderBy(d => d.Sequence);
                //var sql = tblHOTP_Goals.ToString();
                return View(tblHOTP_Goals.ToList());
            //}
            //else
            //{
            //    var keys = db.tblHOTP_Codes.Where(w => w.CodeKey == "YearEnding").OrderBy(q => q.Code).ToList();
            //    ViewBag.SelectedKey = new SelectList(keys, "CodeID", "Code", SelectedKey);
            //    IQueryable<tblHOTP_Goals> tblHOTP_Goals = db.tblHOTP_Goals
            //        .Where(g => g.YearEnding == "2999");
            //    //.OrderBy(d => d.Sequence);
            //    //var sql = tblHOTP_Goals.ToString();
            //    return View(tblHOTP_Goals.ToList());
            //}

            //return View(db.tblHOTP_Goals.ToList());
        }



        // POST: Goals/Scores
        [HttpPost]
        public ActionResult Scores(List<tblHOTP_Goals> MyGoals)
        {
            string year = "";
            foreach (tblHOTP_Goals goal in MyGoals)
            {
                tblHOTP_Goals existingGoal = db.tblHOTP_Goals.Find(goal.GoalID);
                existingGoal.JanScore = goal.JanScore;
                existingGoal.FebScore = goal.FebScore;
                existingGoal.MarScore = goal.MarScore;
                existingGoal.AprScore = goal.AprScore;
                existingGoal.MayScore = goal.MayScore;
                existingGoal.JunScore = goal.JunScore;
                existingGoal.JulScore = goal.JulScore;
                existingGoal.AugScore = goal.AugScore;
                existingGoal.SepScore = goal.SepScore;
                existingGoal.OctScore = goal.OctScore;
                existingGoal.NovScore = goal.NovScore;
                existingGoal.DecScore = goal.DecScore;
                existingGoal.Result = goal.Result;
                existingGoal.Score = goal.Score;

                var EmpGoals = (from eg in db.tblHOTP_EmployeeGoals
                                where eg.GoalID==goal.GoalID
                                select eg);
                foreach(tblHOTP_EmployeeGoals empGoal in EmpGoals)
                {
                    decimal rounded = decimal.Round(Convert.ToDecimal(empGoal.Weight) * Convert.ToDecimal(goal.Score) / 100, 2);
                    empGoal.ItemScore = rounded;
                }
                year = goal.YearEnding;
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { year = year });
        }
        
    }
}
