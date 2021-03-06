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
    public class MainCatRptController : Controller
    {
        DLAllCategoryReport DAL = new DLAllCategoryReport();
        JBNDBClass DLMain = new JBNDBClass();
        CategoryProduct DLCategoryProduct = new CategoryProduct();
        DLTemplates dLTemplates = new DLTemplates();
        // GET: MainCatRpt
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.StateList = new SelectList(DLMain.GetStateList(), "ID", "StateName");
                ViewBag.CategoryProducts = new SelectList(DAL.GetMainCategoryList(), "CategoryProductID", "MainCategoryName");
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
        public ActionResult MainCatRptList()
        {
            if (Session["UserID"] != null)
            {
                CategoryRpt main = new CategoryRpt();
                ConsolidatedReport consolidated = DAL.MainCatReport(main);

                ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
                ViewBag.TotalCategories = consolidated.TotalCategories;
                ViewBag.TotalStates = consolidated.TotalStates;
                ViewBag.TotalCities = consolidated.TotalCities;
                ViewBag.TotalCustomers = consolidated.TotalCustomers;

                return PartialView(consolidated);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchReport(Search search)
        {
            if (Session["UserID"] != null)
            {
                CategoryRpt main = new CategoryRpt();
                main.StateList = search.StateList;
                main.CategoryList = search.MainCategoryList;
                main.FromDate = search.FromDate;
                main.ToDate = search.ToDate;
                ConsolidatedReport consolidated = DAL.MainCatReport(main);

                ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
                ViewBag.TotalCategories = consolidated.TotalCategories;
                ViewBag.TotalStates = consolidated.TotalStates;
                ViewBag.TotalCities = consolidated.TotalCities;
                ViewBag.TotalCustomers = consolidated.TotalCustomers;

                return PartialView("MainCatRptList", consolidated);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consolidatedReport"></param>
        /// <param name="postedFile"></param>
        /// <param name="notificationImage"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Promotion(ConsolidatedReport consolidatedReport, HttpPostedFileBase postedFile, HttpPostedFileBase notificationImage)
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

                string Result = DAL.Promotion(consolidatedReport, Convert.ToInt32(Session["UserID"].ToString()), 1, MailAttachments, ImageURL);
                
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
        /// 
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
        /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCurrentWebsiteRoot()
        {
            return HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
        }
    }
}