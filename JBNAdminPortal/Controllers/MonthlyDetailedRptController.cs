using JBNClassLibrary;
using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class MonthlyDetailedRptController : Controller
    {
        // GET: MonthlyDetailedRpt
        DLAdsReport dLAdsReport = new DLAdsReport();
        DLAdvertisements dLAdvertisements = new DLAdvertisements();
        public ActionResult Index(string Route)
        {
            if (Session["UserID"] != null && !string.IsNullOrEmpty(Route))
            {
                string MonthYear = Helper.Decrypt(Route, "sblw-3hn8-sqoy19");
                ViewBag.AdTypes = new SelectList(dLAdvertisements.GetAdvertisementTypes(), "Type", "AdvertisementType");
                ViewBag.AdAreas = new SelectList(dLAdvertisements.GetAdvertisementAreas(), "AdvertisementAreaName", "AdvertisementAreaName");
                ViewBag.Products = new SelectList(dLAdvertisements.GetAdProducts(), "ItemName", "ItemName");
                ViewBag.Years = new SelectList(dLAdsReport.GetYears(), "AdYear", "AdYear");
                ViewBag.Months = new SelectList(dLAdsReport.Months, "Value", "Text");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Search(SearchVM search)
        {
            if (Session["UserID"] != null)
            {
                var billingRptVM = dLAdsReport.GetMonthlyReport(search);

                return PartialView("MonthlyRpt", billingRptVM);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        public ActionResult GetYearlyReport()
        {
            var billingRptVM = dLAdsReport.GetYearlyReport();
            return PartialView("YearlyRpt", billingRptVM);
        }
    }
}