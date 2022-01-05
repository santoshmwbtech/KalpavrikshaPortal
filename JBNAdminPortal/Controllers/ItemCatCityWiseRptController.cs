using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JBNClassLibrary;
using JBNAdminPortal.Models;
using System.IO;
using System.Net.Mail;
using Newtonsoft.Json;
using System.Configuration;

namespace JBNAdminPortal.Controllers
{
    public class ItemCatCityWiseRptController : Controller
    {
        // GET: ItemCatCityWiseRpt
        JBNDBClass DAL = new JBNDBClass();
        DLAllCategoryReport dLItemCatRpt = new DLAllCategoryReport();
        DLTemplates dLTemplates = new DLTemplates();

        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="EncStateID"></param>
        /// <returns></returns>
        public ActionResult Index(string EncStateID)
        {
            if (Session["UserID"] != null && !string.IsNullOrEmpty(EncStateID))
            {
                ViewBag.StateList = new SelectList(DAL.GetStateList(), "ID", "StateName");
                ViewBag.ItemCategories = new SelectList(dLItemCatRpt.GetItemCategoryList(), "ID", "ItemName");
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
        /// <param name="Pid"></param>
        /// <returns></returns>
        public ActionResult CustomerList(string id, string Pid)
        {
            if (Session["UserID"] != null && !string.IsNullOrEmpty(id))
            {
                CategoryRpt item = new CategoryRpt();
                string DecStateID = Helper.Decrypt(id, "sblw-3hn8-sqoy19");
                item.StateID = Convert.ToInt32(DecStateID);
                string DecPID = Helper.Decrypt(Pid, "sblw-3hn8-sqoy19");
                int[] CategoryArray = new int[] { Convert.ToInt32(DecPID) };
                item.CategoryList = CategoryArray;
                int[] StateArray = new int[] { Convert.ToInt32(DecStateID) };
                item.StateList = StateArray;
                ConsolidatedReport consolidated = dLItemCatRpt.ItemCatCityWiseReport(item);

                ViewBag.TotalCategories = consolidated.TotalCategories;
                ViewBag.TotalStates = consolidated.TotalStates;
                ViewBag.TotalCities = consolidated.TotalCities;
                ViewBag.TotalCustomers = consolidated.TotalCustomers;
                ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");

                return PartialView(consolidated);
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
        public ActionResult SearchReport(Search search)
        {
            if (Session["UserID"] != null)
            {
                CategoryRpt item = new CategoryRpt();
                item.StateList = search.StateList;
                item.CategoryList = search.ItemCategoryList;
                item.FromDate = search.FromDate;
                item.ToDate = search.ToDate;
                ConsolidatedReport consolidated = dLItemCatRpt.ItemCatCityWiseReport(item);

                ViewBag.TotalCategories = consolidated.TotalCategories;
                ViewBag.TotalStates = consolidated.TotalStates;
                ViewBag.TotalCities = consolidated.TotalCities;
                ViewBag.TotalCustomers = consolidated.TotalCustomers;
                ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
                return PartialView("CustomerList", consolidated);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
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

                string Result = dLItemCatRpt.CityWisePromotion(consolidatedReport, Convert.ToInt32(Session["UserID"].ToString()), 4, MailAttachments, ImageURL);
                
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