﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class CompetencyController : Controller
    {
        // GET: Competency
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageCompetencyMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageCompetencyMaster");
        }
    }
}