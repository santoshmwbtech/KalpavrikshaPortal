using JBNAdminPortal.Models;
using JBNClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using static JBNClassLibrary.CityWiseDetails;
using static JBNClassLibrary.DLPromo;

namespace JBNAdminPortal.Controllers
{
    public class CityWiseReportController : Controller
    {
        JBNClassLibrary.JBNDBClass DAL = new JBNClassLibrary.JBNDBClass();
        DLStateCityReport dL = new DLStateCityReport();
        DLTemplates dLTemplates = new DLTemplates();
        // GET: CityWiseReport
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.StateList = new SelectList(DAL.GetStateList(), "ID", "StateName");
                ViewBag.CityList = new SelectList(dL.GetAllCities(), "StateWithCityID", "VillageLocalityName");
                ViewBag.BusinessType = new SelectList(DAL.GetBusinessTypes(), "BusinessTypeID", "BusinessTypeName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult CityList()
        {
            StateCityRpt cityWiseRpt = new StateCityRpt();
            ConsolidatedStateCityReport consolidatedReport = dL.GetCityReport(cityWiseRpt);

            ViewBag.TotalStates = consolidatedReport.TotalStates;
            ViewBag.TotalCities = consolidatedReport.TotalCities;
            ViewBag.TotalCustomers = consolidatedReport.TotalCustomers;
            ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");

            return PartialView(consolidatedReport);
        }

        [HttpPost]
        public ActionResult SearchReport(Search search)
        {
            StateCityRpt cityWiseRpt = new StateCityRpt();
            cityWiseRpt.StateList = search.StateList;
            cityWiseRpt.CityList = search.CityList;
            cityWiseRpt.FromDate = search.FromDate;
            cityWiseRpt.ToDate = search.ToDate;
            ConsolidatedStateCityReport consolidatedReport = dL.GetCityReport(cityWiseRpt);

            ViewBag.TotalStates = consolidatedReport.TotalStates;
            ViewBag.TotalCities = consolidatedReport.TotalCities;
            ViewBag.TotalCustomers = consolidatedReport.TotalCustomers;
            ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");

            return PartialView("CityList", consolidatedReport);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Promotion(ConsolidatedStateCityReport consolidatedReport, HttpPostedFileBase postedFile, HttpPostedFileBase notificationImage)
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

                string Result = dL.Promotion(consolidatedReport, Convert.ToInt32(Session["UserID"].ToString()), 2, MailAttachments, ImageURL);

                // Generate a new unique identifier against which the file can be stored
                string handle = Guid.NewGuid().ToString();
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(Result);
                writer.Flush();
                stream.Position = 0;
                TempData[handle] = stream.ToArray();
                //using (MemoryStream memoryStream = new MemoryStream())
                //{
                //    workbook.SaveAs(memoryStream);
                //    memoryStream.Position = 0;
                //    TempData[handle] = memoryStream.ToArray();
                //}
                // Note we are returning a filename as well as the handle
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
            return File(data, "application/csv", fileName);
        }
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
        public string GetCurrentWebsiteRoot()
        {
            return HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
        }
    }
}