using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class AdvertisementsController : Controller
    {
        DLAdvertisements dLAdvertisements = new DLAdvertisements();
        // GET: Advertisements
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.AdTypes = new SelectList(dLAdvertisements.GetAdvertisementTypes(), "ID", "Type");
                ViewBag.StateList = new SelectList(dLAdvertisements.GetAdStates(), "StateID", "StateName");
                ViewBag.CityList = new SelectList(dLAdvertisements.GetAdCities(), "StateWithCityID", "VillageLocalityName");
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
        /// <param name="searchOptions"></param>
        /// <returns></returns>
        public ActionResult AdvertisementList(SearchOptions searchOptions)
        {
            if (Session["UserID"] != null)
            {
                List<AdvertisementMain> advertisements = dLAdvertisements.GetAdvertisementsForPortal(searchOptions);
                return PartialView(advertisements);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done
        /// </summary>
        /// <param name="searchOptions"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchAds(SearchOptions searchOptions)
        {
            if (Session["UserID"] != null)
            {
                List<AdvertisementMain> advertisements = dLAdvertisements.GetAdvertisementsForPortal(searchOptions);
                return PartialView("AdvertisementList", advertisements);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}