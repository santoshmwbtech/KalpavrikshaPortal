using JBNAdminPortal.Models;
using JBNClassLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static JBNClassLibrary.CityWiseDetails;
using static JBNClassLibrary.DLPromo;

namespace JBNAdminPortal.Controllers
{
    public class CitywiseRptController : Controller
    {
        JBNClassLibrary.JBNDBClass DAL = new JBNClassLibrary.JBNDBClass();
        CityWiseDetails CityWise = new CityWiseDetails();
        // GET: CitywiseRpt
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                CustomerDetails context = new CustomerDetails();
                State state = new State();
                City city = new City();
                state.StateID = 0;
                city.StateWithCityID = 0;
                context.state = state;
                context.city = city;
                ViewBag.StateList = new SelectList(DAL.GetStateList(), "ID", "StateName");
                ViewBag.BusinessType = new SelectList(DAL.GetBusinessTypes(), "BusinessTypeID", "BusinessTypeName");
                ViewBag.SubCategory = new SelectList(DAL.GetSubCatagoryList(), "ID", "SubCategoryName");
                ViewBag.CustomerList = new SelectList(DAL.GetCustomerList(context), "CustID", "FirmName");

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CityList()
        {
            
            CityWiseRpt cityWiseRpt = new CityWiseRpt();
            List<CityWiseRpt> cityWiseRpts = CityWise.CityWiseRegDealersList(cityWiseRpt, 1).ToList();
            PromoWithList promoWithList = new PromoWithList();
            promoWithList.cityWiseRpts = cityWiseRpts;
            promoWithList.IsSMS = false;
            promoWithList.IsEmail = false;
            promoWithList.IsWhatsApp = false;
            return PartialView("StateList", promoWithList);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Promo()
        {
            return PartialView();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="promoWithList"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Promo(PromoWithList promoWithList)
        {
            //string Result = DAL.Promotion(promoWithList);
            return Json("Success");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchByDate(Search search)
        {
            CityWiseRpt cityWiseRpt = new CityWiseRpt();
            cityWiseRpt.FromDate = search.FromDate;
            cityWiseRpt.ToDate = search.ToDate;

            cityWiseRpt.StateID = search.StateID != null ? search.StateID.Value : 0;

            cityWiseRpt.CityID = search.StatewithCityID != null ? search.StatewithCityID.Value : 0;

            cityWiseRpt.BusinessTypeID = search.BusinessTypeID != null ? search.BusinessTypeID.Value : 0;
            cityWiseRpt.CategoryID = search.SubCategoryID != null ? search.SubCategoryID.Value : 0;

            List<CityWiseRpt> cityWiseRpts = CityWise.CityWiseRegDealersList(cityWiseRpt).ToList();

            PromoWithList promoWithList = new PromoWithList();
            promoWithList.cityWiseRpts = cityWiseRpts;

            return PartialView("CityList", promoWithList);
        }
    }
}