using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOTP.Controllers
{
    public class EncryptController : Controller
    {
        // GET: Encrypt
        public ActionResult Index()
        {
            return View();
        }

        // POST: Encrypt/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
