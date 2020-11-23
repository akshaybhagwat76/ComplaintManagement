using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class CompliantController : Controller
    {
        // GET: Compliant
        public ActionResult Compliant_one()
        {
            ViewBag.NavbarTitle = "Complaint Information";
            return View();
        }
        public ActionResult Compliant_two()
        {
            ViewBag.NavbarTitle = "BHU Approval";
            return View();
        }
        public ActionResult Compliant_three()
        {
            ViewBag.NavbarTitle = "Committee";
            return View();
        }

        // GET: Compliant/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Compliant/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Compliant/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Compliant/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Compliant/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Compliant/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Compliant/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
