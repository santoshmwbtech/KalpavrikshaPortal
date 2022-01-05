using JBNAdminPortal.Models;
using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using static JBNClassLibrary.DLEnquiries;

namespace JBNAdminPortal.Controllers
{
    public class LaunchpadReportController : Controller
    {
        JBNClassLibrary.JBNDBClass DAL = new JBNClassLibrary.JBNDBClass();
        DLEnquiries EnquiriesDAL = new DLEnquiries();
        DLTemplates dLTemplates = new DLTemplates();
        // GET: LaunchpadReport
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
                ViewBag.BusinessType = new SelectList(DAL.GetBusinessTypes(), "BusinessTypeID", "BusinessTypeName");
                ViewBag.ItemCategories = new SelectList(EnquiriesDAL.GetItemCategories(), "ID", "ItemName");
                ViewBag.CustomerList = new SelectList(DAL.GetCustomerList(context), "CustID", "FirmName");
                ViewBag.BusinessDemand = new SelectList(DAL.GetBusinessDemands(), "ID", "BusinessDemand");
                ViewBag.EnquiryCities = new SelectList(EnquiriesDAL.GetEnquiryCities(), "StateWithCityID", "VillageLocalityName");
                ViewBag.BusinessDemands = new SelectList(DAL.GetBusinessDemands(), "ID", "Demand");

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
        /// <returns></returns>
        public ActionResult EnquiryList()
        {
            EnquiriesDL enquiries = new EnquiriesDL();
            enquiries.ProductID = 0;
            enquiries.StateID = 0;
            enquiries.CityID = 0;
            ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
            EnquiryListWithTotals enquiryListWithTotals = EnquiriesDAL.GetLaunchpadReport(enquiries);
            return PartialView("EnquiryList", enquiryListWithTotals);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(Search search)
        {
            EnquiriesDL enquiries = new EnquiriesDL();
            //enquiries.ProductID = search.ItemID != null ? search.ItemID.Value : 0;
            //enquiries.StateID = search.StateID != null ? search.StateID.Value : 0;
            //enquiries.CityID = search.StatewithCityID != null ? search.StatewithCityID.Value : 0;

            enquiries.StateList = search.StateList;
            enquiries.CityList = search.CityList;
            enquiries.ItemCategoryList = search.ItemCategoryList;
            enquiries.BusinessTypeList = search.BusinessTypeList;
            enquiries.BusinessDemandID = search.BusinessDemandID != null ? search.BusinessDemandID : 0;
            enquiries.BusinessTypeID = search.BusinessTypeID != null ? search.BusinessTypeID : 0;
            enquiries.PurposeBusiness = search.PurposeOfBusiness;
            enquiries.FromDate = search.FromDate;
            enquiries.ToDate = search.ToDate;
            enquiries.FromTime = search.FromTime;
            enquiries.ToTime = search.ToTime;
            enquiries.InterstCountry = search.InterstCountry;
            enquiries.InterstState = search.InterstState;
            enquiries.InterstCity = search.InterstCity;
            EnquiryListWithTotals enquiryListWithTotals = EnquiriesDAL.GetLaunchpadReport(enquiries);
            ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
            return PartialView("EnquiryList", enquiryListWithTotals);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="enquiryListWithTotals"></param>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Promotion(EnquiryListWithTotals enquiryListWithTotals, HttpPostedFileBase postedFile, HttpPostedFileBase notificationImage)
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
                string Result = EnquiriesDAL.Promotion(enquiryListWithTotals, MailAttachments);
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
        [HttpGet]
        public virtual ActionResult DownloadResponseData(string fileGuid, string fileName)
        {
            byte[] data = TempData[fileGuid] as byte[];
            return File(data, "application/txt", fileName);
        }
    }
}