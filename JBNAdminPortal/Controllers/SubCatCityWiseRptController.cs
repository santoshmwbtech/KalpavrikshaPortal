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
using System.Configuration;

namespace JBNAdminPortal.Controllers
{
    public class SubCatCityWiseRptController : Controller
    {
        JBNDBClass DAL = new JBNDBClass();
        DLAllCategoryReport dLSubCatRpt = new DLAllCategoryReport();
        DLTemplates dLTemplates = new DLTemplates();
        // GET: SubCatCityWiseRpt
        public ActionResult Index(string EncStateID)
        {
            if (Session["UserID"] != null && !string.IsNullOrEmpty(EncStateID))
            {
                ViewBag.StateList = new SelectList(DAL.GetStateList(), "ID", "StateName");
                ViewBag.SubCategoryList = new SelectList(dLSubCatRpt.GetSubCategoryList(), "ID", "SubCategoryName");
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
                CategoryRpt sub = new CategoryRpt();
                string DecStateID = Helper.Decrypt(id, "sblw-3hn8-sqoy19");
                sub.StateID = Convert.ToInt32(DecStateID);
                string DecPID = Helper.Decrypt(Pid, "sblw-3hn8-sqoy19");
                int[] CategoryArray = new int[] { Convert.ToInt32(DecPID) };
                sub.CategoryList = CategoryArray;
                int[] StateArray = new int[] { Convert.ToInt32(DecStateID) };
                sub.StateList = StateArray;
                ConsolidatedReport consolidated = dLSubCatRpt.SubCatCityWiseReport(sub);
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
                CategoryRpt sub = new CategoryRpt();
                sub.StateList = search.StateList;
                sub.CategoryList = search.SubCategoryList;
                sub.FromDate = search.FromDate;
                sub.ToDate = search.ToDate;
                ConsolidatedReport consolidated = dLSubCatRpt.SubCatCityWiseReport(sub);
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

                string Result = dLSubCatRpt.CityWisePromotion(consolidatedReport, Convert.ToInt32(Session["UserID"].ToString()), 2, MailAttachments, ImageURL);
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