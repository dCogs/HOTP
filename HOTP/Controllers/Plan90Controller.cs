using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HOTP.Models;
using System.IO;
//using RazorPDF;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Xml;
using System.Text;


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

                bool editCapability = false;
                int supervisorID = (from e in db.tblHOTP_Employees
                                    where e.EmployeeID == SelectedEmp
                                    select e.SupervisorID).First();
                var me = (from e in db.tblHOTP_Employees
                          where e.Email == User.Identity.Name
                          select e).First();
                if (me.Admin || me.EmployeeID == SelectedEmp || me.EmployeeID == supervisorID) editCapability = true;

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
                                Goal = subGoal,
                                CanEdit = editCapability,
                                Admin = me.Admin
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


        public List<GoalFull> GetAllGoals(int? SelectedEmp, string YearEnding)
        {
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
                               join c in db.tblHOTP_Codes on subGoal.Pillar equals c.Code into code
                               from subCode in code.DefaultIfEmpty()
                               where eg.EmployeeID == SelectedEmp && subGoal.YearEnding == YearEnding
                               orderby subCode.Sequence, subGoal.PillarGoalName
                               select eg;
                foreach (tblHOTP_EmployeeGoals eg in empGoals.ToList())
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
            return AllGoals.ToList();
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
            return View(GetAllGoals(SelectedEmp, YearEnding));
        }


        // GET: Plan90ViewModel/Report/5
        public ActionResult Report(int? SelectedEmp, string YearEnding)
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
            TempData["SelectedEmp"] = SelectedEmp;
            return View(GetAllGoals(SelectedEmp, YearEnding));
        }

        // GET: Plan90ViewModel/Report/5
        public ActionResult Report2(int? SelectedEmp, string YearEnding)
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
            //ViewBag.YearEnding = PopulateCodesDDL("YearEnding", YearEnding);
            //ViewBag.PlanStatus = PopulateCodesDDL("PlanStatus");
            //return View(GetAllGoals(SelectedEmp, YearEnding));
            return new RazorPDF.PdfResult(GetAllGoals(SelectedEmp, YearEnding), "Report");
        }

        //protected string RenderActionResultToString(ActionResult result)
        //{
        //    // Create memory writer.
        //    var sb = new StringBuilder();
        //    var memWriter = new StringWriter(sb);

        //    // Create fake http context to render the view.
        //    var fakeResponse = new HttpResponse(memWriter);
        //    var fakeContext = new HttpContext(System.Web.HttpContext.Current.Request,
        //        fakeResponse);
        //    var fakeControllerContext = new ControllerContext(
        //        new HttpContextWrapper(fakeContext),
        //        this.ControllerContext.RouteData,
        //        this.ControllerContext.Controller);
        //    var oldContext = System.Web.HttpContext.Current;
        //    System.Web.HttpContext.Current = fakeContext;

        //    // Render the view.
        //    result.ExecuteResult(fakeControllerContext);

        //    // Restore old context.
        //    System.Web.HttpContext.Current = oldContext;

        //    // Flush memory and return output.
        //    memWriter.Flush();
        //    return sb.ToString();
        //}

        //protected ActionResult ViewPdf(object model)
        //{
        //    // Create the iTextSharp document.
        //    iTextSharp.text.Document doc = new Document();
        //    // Set the document to write to memory.
        //    MemoryStream memStream = new MemoryStream();
        //    PdfWriter writer = PdfWriter.GetInstance(doc, memStream);
        //    writer.CloseStream = false;
        //    doc.Open();

        //    // Render the view xml to a string, then parse that string into an XML dom.
        //    string xmltext = this.RenderActionResultToString(this.View(model));
        //    XmlDocument xmldoc = new XmlDocument();
        //    xmldoc.InnerXml = xmltext.Trim();

        //    // Parse the XML into the iTextSharp document.
        //    iTextSharp.tool.xml.XMLWorker textHandler = new iTextSharp.tool.xml.XMLWorker(doc);

        //     textHandler = new ITextHandler(doc);
        //    textHandler.Parse(xmldoc);

        //    // Close and get the resulted binary data.
        //    doc.Close();
        //    byte[] buf = new byte[memStream.Position];
        //    memStream.Position = 0;
        //    memStream.Read(buf, 0, buf.Length);

        //    // Send the binary data to the browser.
        //    return new BinaryContentResult(buf, "application/pdf");
        //}



        public ActionResult ReportQuarter()
        {
            int SelectedEmp = 1;
            string YearEnding = "2005";
            return View(GetAllGoals(SelectedEmp, YearEnding));
        }

        //        public ActionResult DownloadViewPDF()
        //{
        //var model = new GeneratePDFModel();
        ////Code to get content
        //return new Rotativa.ViewAsPdf("GeneratePDF", model){FileName = "TestViewAsPdf.pdf"}
        //}

        //public ActionResult DownloadActionAsPDF()
        //{
        //    var model = new GeneratePDFModel();
        //    //Code to get content
        //    return new Rotativa.ActionAsPdf("GeneratePDF", model) { FileName = "TestActionAsPdf.pdf" };
        //}

        //public ActionResult GeneratePDF()
        //{
        //    var model = new GeneratePDFModel();
        //    //get content
        //    return View(model);
        //}

        //        public ActionResult DownloadPartialViewPDF()
        //        {
        //            var model = new GeneratePDFModel();
        //            //Code to get content
        //            return new Rotativa.PartialViewAsPdf("_PartialViewTest", model) { FileName = "TestPartialViewAsPdf.pdf" };
        //        }

        public ActionResult UrlAsPDF()
        {
            return new Rotativa.UrlAsPdf("/Report") { FileName = "UrlTest.pdf" };
        }

        public ActionResult GeneratePDF()
        {
            return new Rotativa.ViewAsPdf("ReportQuarter", GetAllGoals(1, "2015"));
        }


        // POST: Plan90ViewModel/All
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult All(List<GoalFull> GoalFull)
        {
            if (ModelState.IsValid)
            {
                foreach (GoalFull goalFull in GoalFull)
                {
                    foreach (Plan90Full plan90 in goalFull.Plans)
                    {
                        tblHOTP_Plan90 existingPlan = db.tblHOTP_Plan90.Find(plan90.Plan.PlanID);
                        existingPlan.Goal = plan90.Plan.Goal;
                        if (plan90.ActionSteps != null)
                        {
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
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlanStatus = PopulateCodesDDL("PlanStatus");
            return View(GoalFull);
        }


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



        // POST: Plan90ViewModel/Copy
        [HttpPost, ActionName("Copy")]
        [ValidateAntiForgeryToken]
        public ActionResult Copy(FormCollection formCollection)
        {
            foreach (string _formData in formCollection)
            {
                ViewData[_formData] = formCollection[_formData];
            }
            string copyFrom = Request["copyFrom"].ToString();
            string copyTo = Request["copyTo"].ToString();
            int copyFromInt = Convert.ToInt16(copyFrom);
            int copyToInt = Convert.ToInt16(copyTo);

            for (int g = 0; g < 5; g++)
            {
                string empGoalID = null;
                try
                {
                    empGoalID = Request["copyGoal[" + g + "]"].ToString();
                }
                catch { }
                if (empGoalID != null)
                {
                    try
                    {
                        int employeeGoalID = Convert.ToInt16(empGoalID);
                        tblHOTP_Plan90 fromPlan = db.tblHOTP_Plan90.Where(p => p.EmployeeGoalID == employeeGoalID && p.Quarter == copyFromInt).First();
                        tblHOTP_Plan90 toPlan = db.tblHOTP_Plan90.Where(p => p.EmployeeGoalID == employeeGoalID && p.Quarter == copyToInt).First();
                        toPlan.Goal = fromPlan.Goal;

                        var toSteps = from step in db.tblHOTP_ActionSteps where step.PlanID == toPlan.PlanID select step;
                        foreach (tblHOTP_ActionSteps step in toSteps)
                        {
                            db.tblHOTP_ActionSteps.Remove(step);
                        }

                        var fromSteps = from step in db.tblHOTP_ActionSteps where step.PlanID == fromPlan.PlanID select step;
                        foreach (tblHOTP_ActionSteps step in fromSteps)
                        {
                            tblHOTP_ActionSteps newActionStep = new tblHOTP_ActionSteps();
                            newActionStep.PlanID = toPlan.PlanID;
                            newActionStep.ActionStep = step.ActionStep;
                            newActionStep.Result = step.Result;
                            newActionStep.Status = step.Status;
                            db.tblHOTP_ActionSteps.Add(newActionStep);
                        }
                    }
                    catch { }
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Plan90");
        }


        // POST: Plan90ViewModel/Copy1Quarter
        [HttpPost, ActionName("Copy1Quarter")]
        [ValidateAntiForgeryToken]
        public ActionResult Copy1Quarter()
        {
            //foreach (string _formData in formCollection)
            //{
            //    ViewData[_formData] = formCollection[_formData];
            //}
            string EmployeeGoalID = Request["EmployeeGoalID"].ToString();
            string copyFrom = Request["copyFrom"].ToString();
            string copyTo = Request["copyTo"].ToString();
            int copyFromInt = Convert.ToInt16(copyFrom);
            int copyToInt = Convert.ToInt16(copyTo);
            int employeeGoalID = Convert.ToInt16(EmployeeGoalID);

            for (int g = 0; g < 5; g++)
            {
                tblHOTP_Plan90 fromPlan = db.tblHOTP_Plan90.Where(p => p.EmployeeGoalID == employeeGoalID && p.Quarter == copyFromInt).First();
                tblHOTP_Plan90 toPlan = db.tblHOTP_Plan90.Where(p => p.EmployeeGoalID == employeeGoalID && p.Quarter == copyToInt).First();
                toPlan.Goal = fromPlan.Goal;

                var toSteps = from step in db.tblHOTP_ActionSteps where step.PlanID == toPlan.PlanID select step;
                foreach (tblHOTP_ActionSteps step in toSteps)
                {
                    db.tblHOTP_ActionSteps.Remove(step);
                }

                var fromSteps = from step in db.tblHOTP_ActionSteps where step.PlanID == fromPlan.PlanID select step;
                foreach (tblHOTP_ActionSteps step in fromSteps)
                {
                    tblHOTP_ActionSteps newActionStep = new tblHOTP_ActionSteps();
                    newActionStep.PlanID = toPlan.PlanID;
                    newActionStep.ActionStep = step.ActionStep;
                    newActionStep.Result = step.Result;
                    newActionStep.Status = step.Status;
                    db.tblHOTP_ActionSteps.Add(newActionStep);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Plan90");
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

    //public class BinaryContentResult : ActionResult
    //{
    //    private string ContentType;
    //    private byte[] ContentBytes;

    //    public BinaryContentResult(byte[] contentBytes, string contentType)
    //    {
    //        this.ContentBytes = contentBytes;
    //        this.ContentType = contentType;
    //    }

    //    public override void ExecuteResult(ControllerContext context)
    //    {
    //        var response = context.HttpContext.Response;
    //        response.Clear();
    //        response.Cache.SetCacheability(HttpCacheability.NoCache);
    //        response.ContentType = this.ContentType;

    //        var stream = new MemoryStream(this.ContentBytes);
    //        stream.WriteTo(response.OutputStream);
    //        stream.Dispose();
    //    }
    //}


}
