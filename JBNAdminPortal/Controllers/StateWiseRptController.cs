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
    public class StateWiseRptController : Controller
    {
        JBNClassLibrary.JBNDBClass DAL = new JBNClassLibrary.JBNDBClass();
        DLStateCityReport dL = new DLStateCityReport();
        DLTemplates dLTemplates = new DLTemplates();
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        // GET: StateWiseRpt
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.StateList = new SelectList(DAL.GetStateList(), "ID", "StateName");
                ViewBag.BusinessType = new SelectList(DAL.GetBusinessTypes(), "BusinessTypeID", "BusinessTypeName");
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
        public ActionResult StateList()
        {
            StateCityRpt stateCityRpt = new StateCityRpt();
            ConsolidatedStateCityReport consolidatedReport = dL.GetStateReport(stateCityRpt);
            ViewBag.TotalStates = consolidatedReport.TotalStates;
            ViewBag.TotalCities = consolidatedReport.TotalCities;
            ViewBag.TotalCustomers = consolidatedReport.TotalCustomers;
            ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
            return PartialView(consolidatedReport);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult SearchReport(Search search)
        {
            StateCityRpt stateCityRpt = new StateCityRpt();
            stateCityRpt.StateList = search.StateList;
            stateCityRpt.FromDate = search.FromDate;
            stateCityRpt.ToDate = search.ToDate;
            ConsolidatedStateCityReport consolidatedReport = dL.GetStateReport(stateCityRpt);
            if(consolidatedReport != null)
            {
                ViewBag.TotalStates = consolidatedReport.TotalStates;
                ViewBag.TotalCities = consolidatedReport.TotalCities;
                ViewBag.TotalCustomers = consolidatedReport.TotalCustomers;
                ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
            }
            return PartialView("StateList", consolidatedReport);
        }
        /// <summary>
        /// Done A        
        /// </summary>
        /// <param name="consolidatedReport"></param>
        /// <param name="postedFile"></param>
        /// <param name="notificationImage"></param>
        /// <returns></returns>

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

                string Result = dL.Promotion(consolidatedReport, Convert.ToInt32(Session["UserID"].ToString()), 1, MailAttachments, ImageURL);

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
            return File(data, "application/csv", fileName);
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