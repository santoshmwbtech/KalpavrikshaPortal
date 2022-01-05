using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class YearlyBillingRptController : Controller
    {
        DLAdsReport dLAdsReport = new DLAdsReport();
        DLAdvertisements dLAdvertisements = new DLAdvertisements();
        // GET: YearlyBillingRpt


        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.AdTypes = new SelectList(dLAdvertisements.GetAdvertisementTypes(), "Type", "Type");
                ViewBag.AdAreas = new SelectList(dLAdvertisements.GetAdvertisementAreas(), "AdvertisementAreaName", "AdvertisementAreaName");
                ViewBag.Products = new SelectList(dLAdvertisements.GetAdProducts(), "ItemName", "ItemName");
                ViewBag.Years = new SelectList(dLAdsReport.GetYears(), "AdYear", "AdYear");
                ViewBag.Months = new SelectList(dLAdsReport.Months, "Value", "Text");
                ViewBag.StateList = new SelectList(dLAdvertisements.GetAdStates(), "ID", "StateName");
                ViewBag.DistrictList = new SelectList(dLAdvertisements.GetAdDistricts(), "DistrictID", "DistrictName");
                ViewBag.CityList = new SelectList(dLAdvertisements.GetAdCities(), "StatewithCityID", "VillageLocalityName");
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
        public ActionResult Search(SearchVM search)
        {
            if (Session["UserID"] != null)
            {
                if(!string.IsNullOrEmpty(search.Month))
                {
                    var billingRptVM = dLAdsReport.GetAdvertisementRptOfMonth(search);
                    return PartialView("DetailedRpt", billingRptVM);
                }
                else
                {
                    var billingRptVM = dLAdsReport.GetMonthlyReport(search);
                    return PartialView("MonthlyRpt", billingRptVM);
                }
                
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }            
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult GetYearlyReport()
        {
            var billingRptVM = dLAdsReport.GetYearlyReport();
            return PartialView("YearlyRpt", billingRptVM);
        }
    }
}