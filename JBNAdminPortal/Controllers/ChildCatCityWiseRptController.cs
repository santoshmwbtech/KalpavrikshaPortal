using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JBNClassLibrary;
using JBNAdminPortal.Models;
using System.Net.Mail;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;

namespace JBNAdminPortal.Controllers
{
    public class ChildCatCityWiseRptController : Controller
    {
        // GET: ChildCatCityWiseRpt
        JBNDBClass DAL = new JBNDBClass();
        DLAllCategoryReport dLChildCatRpt = new DLAllCategoryReport();
        DLTemplates dLTemplates = new DLTemplates();
        public ActionResult Index(string EncStateID)
        {
            if (Session["UserID"] != null && !string.IsNullOrEmpty(EncStateID))
            {
                ViewBag.StateList = new SelectList(DAL.GetStateList(), "ID", "StateName");
                ViewBag.ChildCategoryList = new SelectList(dLChildCatRpt.GetChildCategoryList(), "ID", "ChildCategoryName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult CustomerList(string id, string Pid)
        {
            if (Session["UserID"] != null && !string.IsNullOrEmpty(id))
            {
                CategoryRpt child = new CategoryRpt();
                string DecStateID = Helper.Decrypt(id, "sblw-3hn8-sqoy19");
                child.StateID = Convert.ToInt32(DecStateID);
                string DecPID = Helper.Decrypt(Pid, "sblw-3hn8-sqoy19");
                int[] CategoryArray = new int[] { Convert.ToInt32(DecPID) };
                child.CategoryList = CategoryArray;
                int[] StateArray = new int[] { Convert.ToInt32(DecStateID) };
                child.StateList = StateArray;
                ConsolidatedReport consolidated = dLChildCatRpt.ChildCatCityWiseReport(child);
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

        [HttpPost]
        public ActionResult SearchReport(Search search)
        {
            if (Session["UserID"] != null)
            {
                CategoryRpt child = new CategoryRpt();
                child.StateList = search.StateList;
                child.CategoryList = search.ChildCategoryList;
                child.FromDate = search.FromDate;
                child.ToDate = search.ToDate;
                ConsolidatedReport consolidated = dLChildCatRpt.ChildCatCityWiseReport(child);
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


                string Result = dLChildCatRpt.CityWisePromotion(consolidatedReport, Convert.ToInt32(Session["UserID"].ToString()), 3, MailAttachments, ImageURL);

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