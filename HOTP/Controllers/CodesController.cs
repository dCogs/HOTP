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
    public class CodesController : Controller
    {
        private HOTP_Entities db = new HOTP_Entities();

        // GET: Codes
        public ActionResult Index(int? SelectedKey)
        {
            string codeKey = "";
            if (SelectedKey == null && TempData["SelectedKey"] != null)
            {
                SelectedKey = Convert.ToInt16(TempData["SelectedKey"]);
            }
            if (SelectedKey != null)
            {
                if (SelectedKey == 0)
                {
                    TempData["SelectedKey"] = null;
                    SelectedKey = null;
                }
                else
                {
                    TempData["SelectedKey"] = SelectedKey;
                    tblHOTP_Codes selected = db.tblHOTP_Codes.Find(SelectedKey);
                    codeKey = selected.Code;
                }
            }
            var keys = db.tblHOTP_Codes.Where(w => w.CodeKey=="CodeKeys").OrderBy(q => q.Code).ToList();
            tblHOTP_Codes all = new tblHOTP_Codes();
            all.CodeId=0;
            all.Code="All";
            keys.Insert(0, all);
            ViewBag.SelectedKey = new SelectList(keys, "CodeID", "Code", SelectedKey);
            IQueryable<tblHOTP_Codes> tblHOTP_Codes = db.tblHOTP_Codes
                .Where(c => !SelectedKey.HasValue || c.CodeKey == codeKey)
                .OrderBy(d => d.Sequence);
            return View(tblHOTP_Codes.ToList());
        }


        // GET: Codes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHOTP_Codes tblHOTP_Codes = db.tblHOTP_Codes.Find(id);
            if (tblHOTP_Codes == null)
            {
                return HttpNotFound();
            }
            return View(tblHOTP_Codes);
        }

        // GET: Codes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Codes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CodeId,CodeKey,Code,Sequence")] tblHOTP_Codes tblHOTP_Codes)
        {
            if (ModelState.IsValid)
            {
                db.tblHOTP_Codes.Add(tblHOTP_Codes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblHOTP_Codes);
        }

        // GET: Codes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHOTP_Codes tblHOTP_Codes = db.tblHOTP_Codes.Find(id);
            if (tblHOTP_Codes == null)
            {
                return HttpNotFound();
            }
            return View(tblHOTP_Codes);
        }

        // POST: Codes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CodeId,CodeKey,Code,Sequence")] tblHOTP_Codes tblHOTP_Codes)
        {
            var keys = db.tblHOTP_Codes.Where(w => w.CodeKey == "CodeKeys").OrderBy(q => q.Code).ToList();
            int SelectedKey = Convert.ToInt16(TempData["SelectedKey"]);
            ViewBag.SelectedKey = new SelectList(keys, "CodeID", "Code", SelectedKey);
            if (ModelState.IsValid)
            {
                db.Entry(tblHOTP_Codes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblHOTP_Codes);
        }

        // GET: Codes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHOTP_Codes tblHOTP_Codes = db.tblHOTP_Codes.Find(id);
            if (tblHOTP_Codes == null)
            {
                return HttpNotFound();
            }
            return View(tblHOTP_Codes);
        }

        // POST: Codes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblHOTP_Codes tblHOTP_Codes = db.tblHOTP_Codes.Find(id);
            db.tblHOTP_Codes.Remove(tblHOTP_Codes);
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
    }
}
