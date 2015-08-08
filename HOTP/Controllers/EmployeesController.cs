using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HOTP.Models;
using PagedList;

namespace HOTP.Controllers
{
    public class EmployeesController : Controller
    {
        private HOTP_Entities db = new HOTP_Entities();

        // GET: Employees
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "first_name" ? "first_name_desc" : "first_name";
            ViewBag.SupervisorSortParm = sortOrder == "super" ? "super_desc" : "super";
            ViewBag.TitleSortParm = sortOrder == "title" ? "title_desc" : "title";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var emps = from e in db.tblHOTP_Employees
                       where String.IsNullOrEmpty(searchString) ||
                       (e.FirstName.Contains(searchString) || e.LastName.Contains(searchString))
                       join s in db.tblHOTP_Employees on e.SupervisorID equals s.EmployeeID into gj
                       from subsup in gj.DefaultIfEmpty()
                       select new EmployeeViewModel { Employee = e, SupervisorName = (subsup == null ? String.Empty : subsup.LastName + ", " + subsup.FirstName) };
            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    emps = emps.Where(Employee.LastName.Contains(searchString)
            //                           || e.FirstMidName.Contains(searchString));
            //}
            switch (sortOrder)
            {
                case "last_name_desc":
                    emps = emps.OrderByDescending(e => e.Employee.LastName);
                    break;
                case "first_name_desc":
                    emps = emps.OrderByDescending(e => e.Employee.FirstName);
                    break;
                case "first_name":
                    emps = emps.OrderBy(e => e.Employee.FirstName);
                    break;
                case "super_desc":
                    emps = emps.OrderByDescending(e => e.SupervisorName);
                    break;
                case "super":
                    emps = emps.OrderBy(e => e.SupervisorName);
                    break;
                case "title_desc":
                    emps = emps.OrderByDescending(e => e.Employee.Title);
                    break;
                case "title":
                    emps = emps.OrderBy(e => e.Employee.Title);
                    break;
                default:  // Name ascending 
                    emps = emps.OrderBy(e => e.Employee.LastName);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(emps.ToPagedList(pageNumber, pageSize));
        }


        // GET: Employees
        public ActionResult All()
        {
            //var emps = from e in db.tblHOTP_Employees
            //           join s in db.tblHOTP_Employees on e.SupervisorID equals s.EmployeeID into gj
            //           from subsup in gj.DefaultIfEmpty()
            //           select new EmployeeViewModel { Employee = e, SupervisorName = (subsup == null ? String.Empty : subsup.LastName + ", " + subsup.FirstName) };
            //return View(emps.ToList());
            var employees = from e in db.tblHOTP_Employees
                           orderby e.LastName
                           select e;
            return View(employees.ToList());
        }

        // GET: Employees/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHOTP_Employees tblHOTP_Employees = db.tblHOTP_Employees.Find(id);
            if (tblHOTP_Employees == null)
            {
                return HttpNotFound();
            }
            return View(tblHOTP_Employees);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            tblHOTP_Employees tblHOTP_Employees = new tblHOTP_Employees();
            PopulateSupervisorsDropDownList(tblHOTP_Employees.SupervisorID);
            return View(tblHOTP_Employees);
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,EmpStatus,FirstName,LastName,SupervisorID,Title,Department,Division,Email,Evaluations,Admin")] tblHOTP_Employees tblHOTP_Employees)
        {
            if (ModelState.IsValid)
            {
                db.tblHOTP_Employees.Add(tblHOTP_Employees);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblHOTP_Employees);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHOTP_Employees tblHOTP_Employees = db.tblHOTP_Employees.Find(id);
            if (tblHOTP_Employees == null)
            {
                return HttpNotFound();
            }
            PopulateSupervisorsDropDownList(tblHOTP_Employees.SupervisorID);
            return View(tblHOTP_Employees);
        }

        private void PopulateSupervisorsDropDownList(object selectedSupervisor = null)
        {
            //var supervisorsQuery = from s in db.tblHOTP_Employees
            //                       orderby s.LastName
            //                       select s;
            //ViewBag.SupervisorID = new SelectList(supervisorsQuery, "EmployeeID",  ("LastName, FirstName"), selectedSupervisor);
            var supers =
              db.tblHOTP_Employees
                .Where(s => s.Evaluations)
                .OrderBy(s => s.LastName)
                .ToList()
                .Select(s => new
                {
                    EmployeeId = s.EmployeeID,
                    FullName = string.Format("{0}, {1}", s.LastName, s.FirstName)
                });
            ViewBag.SupervisorID = new SelectList(supers, "EmployeeID", "FullName", selectedSupervisor);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,EmpStatus,FirstName,LastName,SupervisorID,Title,Department,Division,Email,Evaluations,Admin")] tblHOTP_Employees tblHOTP_Employees)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblHOTP_Employees).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblHOTP_Employees);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHOTP_Employees tblHOTP_Employees = db.tblHOTP_Employees.Find(id);
            if (tblHOTP_Employees == null)
            {
                return HttpNotFound();
            }
            return View(tblHOTP_Employees);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblHOTP_Employees tblHOTP_Employees = db.tblHOTP_Employees.Find(id);
            db.tblHOTP_Employees.Remove(tblHOTP_Employees);
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

        // POST: Employees/All
        [HttpPost]
        public ActionResult All(List<tblHOTP_Employees> MyEmps)
        {
            foreach (tblHOTP_Employees Emp in MyEmps)
            {
                tblHOTP_Employees ExistingEmp = db.tblHOTP_Employees.Find(Emp.EmployeeID);
                ExistingEmp.LastName = Emp.LastName;
                ExistingEmp.FirstName = Emp.FirstName;
                ExistingEmp.Title = Emp.Title;
            }
            db.SaveChanges();
            var employees = from e in db.tblHOTP_Employees
                            orderby e.LastName
                            select e;
            return View(employees.ToList());
        }


    }
}
