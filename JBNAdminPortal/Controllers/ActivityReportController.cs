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

namespace JBNAdminPortal.Controllers
{
   
    public class ActivityReportController : Controller
    {
        DLActivityReport DAL = new DLActivityReport();
        JBNDBClass jbndbClass = new JBNDBClass();
        DLTemplates dLTemplates = new DLTemplates();
        // GET: ActivityReport
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

                ViewBag.StateList = new SelectList(jbndbClass.GetStateList(), "ID", "StateName");
                ViewBag.CityList = new SelectList(jbndbClass.GetAllCities(), "ID", "VillageLocalityname");
                ViewBag.CustomerList = new SelectList(jbndbClass.GetCustomerList(context), "CustID", "FirmName");
                ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
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
        public ActionResult CustomerList(string id = null)
        {
            ActivityReport activityReport = new ActivityReport();
            PromoWithActivityReport promoWithActivityReport = new PromoWithActivityReport();
            promoWithActivityReport = DAL.CustomerActivityReport(activityReport);

            if(id != null)
            {
                if(id == "1")
                {
                    promoWithActivityReport.activityReports = promoWithActivityReport.activityReports.Where(a => a.Status.ToLower() == "active").ToList();
                }
                else
                {
                    promoWithActivityReport.activityReports = promoWithActivityReport.activityReports.Where(a => a.Status.ToLower() == "inactive").ToList();
                }
            }
            ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
            return PartialView(promoWithActivityReport);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchByDate(Search search)
        {
            ActivityReport activityReport = new ActivityReport();
            activityReport.FromDate = search.FromDate;
            activityReport.ToDate = search.ToDate;
            if (search.StateID != null)
                activityReport.StateID = search.StateID.Value;
            else
                activityReport.StateID = 0;

            if (search.StatewithCityID != null)
                activityReport.CityID = search.StatewithCityID.Value;
            else
                activityReport.CityID = 0;

            activityReport.StateList = search.StateList;
            activityReport.CityList = search.CityList;

            activityReport.IsActive = search.IsActive == null ? 0 : search.IsActive;

               activityReport.NumberofDays = search.NumberofDays == null ? 0 : search.NumberofDays;
            ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
            PromoWithActivityReport promoWithActivityReport = new PromoWithActivityReport();
            promoWithActivityReport = DAL.CustomerActivityReport(activityReport);
            return PartialView("CustomerList", promoWithActivityReport);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="promotion"></param>
        /// <param name="postedFile"></param>
        /// <param name="notificationImage"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Promo(PromoWithActivityReport promotion, HttpPostedFileBase postedFile, HttpPostedFileBase notificationImage)
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
                if(Result != "Success")
                {
                    RootData myDeserializedClass = JsonConvert.DeserializeObject<RootData>(Result);
                }

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