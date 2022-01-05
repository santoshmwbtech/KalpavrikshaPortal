using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using JBNAdminPortal.Models;
using JBNClassLibrary;
using Newtonsoft.Json;

namespace JBNAdminPortal.Controllers
{
    public class BusinessTypeWiseRptController : Controller
    {
        JBNClassLibrary.JBNDBClass DAL = new JBNClassLibrary.JBNDBClass();
        BusinessTypeWiseDetails DALBusinessTypeWiseDetails = new BusinessTypeWiseDetails();
        DLAllCategoryReport dlAllCatRpt = new DLAllCategoryReport();
        DLTemplates dLTemplates = new DLTemplates();
        // GET: BusinessTypeWiseRpt
        /// <summary>
        /// Done A
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
                ViewBag.CityList = new SelectList(DAL.GetAllCities(), "ID", "VillageLocalityname");
                ViewBag.BusinessType = new SelectList(DAL.GetBusinessTypes(), "BusinessTypeID", "BusinessTypeName");
                ViewBag.MainCategory = new SelectList(dlAllCatRpt.GetMainCategoryList(), "CategoryProductId", "MainCategoryName");
                ViewBag.SubCategory = new SelectList(dlAllCatRpt.GetSubCategoryList(), "ID", "SubCategoryName");
                ViewBag.ChildCategory = new SelectList(dlAllCatRpt.GetChildCategoryList(), "ID", "ChildCategoryName");
                ViewBag.ItemCategory = new SelectList(dlAllCatRpt.GetItemCategoryList(), "ID", "ItemName");
                ViewBag.CustomerList = new SelectList(DAL.GetCustomerList(context), "CustID", "FirmName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A --Check Santosh
        /// </summary>
        /// <param name="businessRpt"></param>
        /// <returns></returns>
        public ActionResult CustomerList(BusinessTypeWiseRpt businessRpt)
        {
            BusinessTypeWiseRpt businessTypeWiseRpt = new BusinessTypeWiseRpt();
            PromoWithBusinessTypeRptList promoWithBusinessTypeRptList = new PromoWithBusinessTypeRptList();
            promoWithBusinessTypeRptList = DALBusinessTypeWiseDetails.BusinessList(businessTypeWiseRpt);
            ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
            return PartialView(promoWithBusinessTypeRptList);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchByDate(Search search)
        {
            BusinessTypeWiseRpt businessTypeWiseRpt = new BusinessTypeWiseRpt();
            businessTypeWiseRpt.FromDate = search.FromDate;
            businessTypeWiseRpt.ToDate = search.ToDate;
            businessTypeWiseRpt.DealerName = search.FirmName;

            if (search.StateID != null)
                businessTypeWiseRpt.StateID = search.StateID.Value;
            else
                businessTypeWiseRpt.StateID = 0;

            if (search.StatewithCityID != null)
                businessTypeWiseRpt.CityID = search.StatewithCityID.Value;
            else
                businessTypeWiseRpt.CityID = 0;

            businessTypeWiseRpt.BusinessTypeID = search.BusinessTypeID != null ? search.BusinessTypeID.Value : 0;
            businessTypeWiseRpt.CategoryID = search.SubCategoryID != null ? search.SubCategoryID.Value : 0;

            businessTypeWiseRpt.StateList = search.StateList;
            businessTypeWiseRpt.CityList = search.CityList;
            businessTypeWiseRpt.SubCategoryList = search.SubCategoryList;
            businessTypeWiseRpt.BusinessTypeList = search.BusinessTypeIDStrList;
            businessTypeWiseRpt.MainCategoryList = search.MainCategoryList;
            businessTypeWiseRpt.ChildCategoryList = search.ChildCategoryList;
            businessTypeWiseRpt.ItemCategoryList = search.ItemCategoryList;


            PromoWithBusinessTypeRptList promoWithBusinessTypeRptList = new PromoWithBusinessTypeRptList();
            promoWithBusinessTypeRptList = DALBusinessTypeWiseDetails.BusinessList(businessTypeWiseRpt);
            ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
            return PartialView("CustomerList", promoWithBusinessTypeRptList);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="StateID"></param>
        /// <param name="CityName"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCitiesofState(int StateID, string CityName = "")
        {
            List<CityView> cityList = new List<CityView>();
            JBNDBClass jBNDBClass = new JBNDBClass();
            cityList = jBNDBClass.GetCities(StateID, CityName);
            return Json(cityList.Select(C => new { C.ID, C.VillageLocalityName }).ToList(), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="promotion"></param>
        /// <param name="postedFile"></param>
        /// <param name="notificationImage"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Promo(PromoWithBusinessTypeRptList promotion, HttpPostedFileBase postedFile, HttpPostedFileBase notificationImage)
        {
            if (Session["UserID"] != null)
            {
                List<Attachment> MailAttachments = new List<Attachment>();
                string ImageURL = string.Empty;

                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    try
                    {
                        string fileName = Path.GetFileName(postedFile.FileName);
                        var attachment = new Attachment(postedFile.InputStream, fileName); 
                        MailAttachments.Add(attachment);
                    }
                    catch (Exception) { }
                }
                if (notificationImage != null && notificationImage.ContentLength > 0)
                {
                    try
                    {
                        string fileName = Path.GetFileName(notificationImage.FileName);
                        string path = Server.MapPath("~/notificationImages/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        notificationImage.SaveAs(path + Path.GetFileName(notificationImage.FileName));
                        string URL = ConfigurationManager.AppSettings["PortalURL"].ToString();
                        ImageURL = URL + "/notificationImages/" + fileName;
                    }
                    catch (Exception e) { }
                }

                string Result = DAL.Promotion(promotion, MailAttachments, ImageURL, Convert.ToInt32(Session["UserID"].ToString()));

                // Generate a new unique identifier against which the file can be stored
                string handle = Guid.NewGuid().ToString();
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(Result);
                writer.Flush();
                stream.Position = 0;
                TempData[handle] = stream.ToArray();
                
                return new JsonResult()
                {
                    Data = new { FileGuid = handle, FileName = "PromoResult.txt" }
                };
            }
            else
            {
                return Json("sessionexpired");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="fileGuid"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult DownloadResponseData(string fileGuid, string fileName)
        {
            byte[] data = TempData[fileGuid] as byte[];
            return File(data, "application/txt", fileName);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSMSTemplate(int? TemplateID)
        {
            if (Session["UserID"] != null)
            {
                var SMSTemplate = dLTemplates.GetSMSTemplateDetails(TemplateID.Value);
                return Json(SMSTemplate, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("sessionexpired");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public string GetCurrentWebsiteRoot()
        {
            return HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
        }
    }
}