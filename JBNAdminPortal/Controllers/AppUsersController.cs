using JBNAdminPortal.Models;
using JBNClassLibrary;
using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class AppUsersController : Controller
    {
        JBNClassLibrary.JBNDBClass DAL = new JBNClassLibrary.JBNDBClass();
        // GET: AppUsers
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                Search search = new Search();
                search.StatewithCityID = 0;
                search.StateID = 0;
                ViewBag.StateList = new SelectList(DAL.GetStateList(), "ID", "StateName");
                ViewBag.CityList = new SelectList(DAL.GetAllCities(), "ID", "VillageLocalityname");
                ViewBag.BusinessType = new SelectList(DAL.GetBusinessTypes(), "ID", "BusinessTypeName");
                ViewBag.SubCategory = new SelectList(DAL.GetSubCatagoryList(), "ID", "SubCategoryName");
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
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CustomerList(string id)
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
                List<CustomerDetails> custList = new List<CustomerDetails>();
                custList = DAL.GetCustomerList(context);
                ViewBag.RegisteredCustomers = custList.Count();
                if(id == "2")
                {
                    custList = custList.Where(c => c.IsActive == false).ToList();
                }

                ViewBag.TotalStates = custList.Where(c => c.state.StateID != null).Select(c => c.state.StateID).Distinct().Count();
                ViewBag.TotalCities = custList.Where(c => c.city.StateWithCityID != null).Select(c => c.city.StateWithCityID).Distinct().Count();
                
                List<CustomerDetails> BlockedList = custList.Where(c => c.IsActive == false).ToList();
                ViewBag.BlockedCustomers = BlockedList.Count();

                return PartialView(custList);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="CityID"></param>
        /// <param name="PID"></param>
        /// <param name="CType"></param>
        /// <returns></returns>
        public ActionResult CatCityWiseCustList(string CityID, string PID, string CType)
        {
            if (Session["UserID"] != null)
            {
                CatCityWiseCustListParameters context = new CatCityWiseCustListParameters();
                context.CityId = Convert.ToInt32(Helper.Decrypt(CityID, "sblw-3hn8-sqoy19"));
                context.ProductID = Convert.ToInt32(Helper.Decrypt(PID, "sblw-3hn8-sqoy19"));
                context.ProductType = Convert.ToInt32(Helper.Decrypt(CType, "sblw-3hn8-sqoy19"));
                List<CustomerDetails> custList = new List<CustomerDetails>();
                custList = DAL.CatCityWiseCustList(context);
                return PartialView("CustomerList", custList);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ActionResult GetCityStateWiseCustList(string ID, string Type)
        {
            if (Session["UserID"] != null)
            {
                CustomerDetails context = new CustomerDetails();
                State state = new State();
                City city = new City();
                
                
                List<CustomerDetails> custList = new List<CustomerDetails>();

                state.StateID = 0;
                city.StateWithCityID = 0;
                int SCType = Convert.ToInt32(Helper.Decrypt(Type, "sblw-3hn8-sqoy19"));
                int IDS = Convert.ToInt32(Helper.Decrypt(ID, "sblw-3hn8-sqoy19"));


                int[] statesList = null;
                int[] citiesList = null;

                if (SCType == 1)
                    statesList = new int[] { IDS };
                else if(SCType == 2)
                    citiesList = new int[] { IDS };

                context.StateList = statesList;
                context.CityList = citiesList;

                custList = DAL.GetCustomerList(context);

                return PartialView("CustomerList", custList);
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
        public ActionResult Search(Search search)
        {
            if (Session["UserID"] != null)
            {
                CustomerDetails context = new CustomerDetails();
                State state = new State();
                City city = new City();

                int[] statesList = search.StateList;
                int[] citiesList = search.CityList;

                if (search.StateID == null)
                    state.StateID = 0;
                else
                    state.StateID = search.StateID;

                if (search.StatewithCityID == null)
                    city.StateWithCityID = 0;
                else
                    city.StateWithCityID = search.StatewithCityID;

                context.MobileNumber = search.MobileNumber;
                if (search.CustID == null)
                    context.CustID = 0;
                else
                    context.CustID = search.CustID.Value;

                context.FromDate = search.FromDate;
                context.ToDate = search.ToDate;
                context.FromTime = search.FromTime;
                context.ToTime = search.ToTime;
                context.FirmName = search.FirmName;

                context.state = state;
                context.city = city;
                context.StateList = statesList;
                context.CityList = citiesList;
                List<CustomerDetails> custList = new List<CustomerDetails>();
                custList = DAL.GetCustomerList(context);

                ViewBag.TotalStates = custList.Select(c => c.state.StateID).Distinct().Count();
                ViewBag.TotalCities = custList.Select(c => c.city.StateWithCityID).Distinct().Count();
                ViewBag.RegisteredCustomers = custList.Count();
                List<CustomerDetails> BlockedList = custList.Where(c => c.IsActive == false).ToList();
                ViewBag.BlockedCustomers = BlockedList.Count();

                return PartialView("CustomerList", custList);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }


        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="StateList"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCities(string StateList)
        {
            JBNDBClass DAL = new JBNDBClass();
            string[] SplitStates = null;
            List<tblStateWithCity> CityList = new List<tblStateWithCity>();
            if (!string.IsNullOrEmpty(StateList))
            {
                if (StateList.Contains(','))
                {
                    SplitStates = StateList.Split(',');
                    foreach (var str in SplitStates)
                    {
                        int StateID = Convert.ToInt32(str);
                        CityList.AddRange(DAL.GetCitiesOfState(StateID));
                    }
                }
                else
                {
                    int StateID = Convert.ToInt32(StateList);
                    CityList.AddRange(DAL.GetCitiesOfState(StateID));
                }
            }
            else
            {
                CityList.AddRange(DAL.GetCitiesOfState(0));
            }
            SelectList selectListItems = new SelectList(DAL.GetAllCities(), "StatewithCityID", "VillageLocalityname");
            return Json(selectListItems, JsonRequestBehavior.AllowGet);
        }


        //not using
        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            if (Session["UserID"] != null)
            {
                List<tblStateWithCity> cityList = new List<tblStateWithCity>();
                cityList = DAL.GetAllCities(prefix);
                return Json(cityList.Select(C => new { C.VillageLocalityName, C.StatewithCityID }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", "Error");
            }
        }

        //Not Using
        [HttpPost]
        public ActionResult ShareAPK(int? CustID)
        {
            if (Session["UserID"] != null)
            {
                var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri)
                {
                    Path = Url.Content("~/apk/JBN_APK.apk"),
                    Query = null,
                };

                string APKURL = ConfigurationManager.AppSettings["APKURL"].ToString();

                string serverURL = "http://" + Request.Url.Authority + APKURL;

                Uri uri = urlBuilder.Uri;
                string url = urlBuilder.ToString();
                string RetVal = DAL.ShareAPK(CustID, url, serverURL);
                return Json(RetVal);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="CustID"></param>
        /// <param name="StatusType"></param>
        /// <returns></returns>
        public ActionResult GetCustID(int? CustID, int StatusType)
        {
            if (Session["UserID"] != null)
            {
                UserStatus status = new UserStatus();
                status.CustID = CustID.Value;
                status.StatusType = StatusType;
                return PartialView("UserStatus", status);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActivateDeactivateUser(UserStatus userStatus)
        {
            if (Session["UserID"] != null)
            {
                string Result = DAL.ActivateDeactivateUser(userStatus, Convert.ToInt32(Session["UserID"].ToString()));
                return Json(Result);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

    }
}