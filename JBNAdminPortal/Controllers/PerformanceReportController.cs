using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class PerformanceReportController : Controller
    {
        // GET: PerformanceReport
        DLPerformanceReport DAL = new DLPerformanceReport();
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(PerformanceReport search)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            PerformanceReport performanceReport = new PerformanceReport();
            performanceReport = DAL.GetPerformanceReport(search);

            if(!string.IsNullOrEmpty(search.CompareFromDate) && !string.IsNullOrEmpty(search.CompareToDate))
                return PartialView("CompareReport", performanceReport);
            else
                return PartialView("PerformanceReport", performanceReport);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Compare(PerformanceReport search)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                PerformanceReport performanceReport = new PerformanceReport();
                search.FromDate = search.CompareFromDate;
                search.ToDate = search.CompareToDate;
                performanceReport = DAL.GetPerformanceReport(search);
                return PartialView("CompareReport", performanceReport);;
            }
            else
            {
                return Content("Error while getting the data.. Please try later");
            }
        }
    }
}