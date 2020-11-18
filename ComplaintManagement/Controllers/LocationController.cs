﻿using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class LocationController : Controller
    {
        // GET: Location
        public ActionResult Index()
        {
            ViewBag.lstLocation = GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("Location", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public List<LocationMasterVM> GetAll(string range = "")
        {
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                return new LocationMastersRepository().GetAll().Where(x => x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();
            }
            else
            {
                return new LocationMastersRepository().GetAll();
            }
        }

        [HttpGet]
        public ActionResult GetLocations(string range)
        {
            ViewBag.lstLocation = GetAll(range);
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("Categories", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View("Index");
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            bool retval = true;
            try
            {
                retval = new LocationMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Location"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateLocation(LocationMasterVM LocationVM)
        {
            try
            {
                var Location = new LocationMastersRepository().AddOrUpdate(LocationVM);
                return new ReplyFormat().Success(Messages.SUCCESS, Location);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            LocationMasterVM LocationMasterVM = new LocationMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageLocationMaster", LocationMasterVM);
            
        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                LocationMasterVM LocationVM = new LocationMastersRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                return View("ManageLocationMaster", LocationVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
           
        }
    }
}