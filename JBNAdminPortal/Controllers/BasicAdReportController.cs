using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Net.Mail;
using System.IO;
using System.Configuration;

namespace JBNAdminPortal.Controllers
{
    public class BasicAdReportController : Controller
    {
        DLAdsReport dLAdsReport = new DLAdsReport();
        DLAdvertisements dLAdvertisements = new DLAdvertisements();
        DLTemplates dLTemplates = new DLTemplates();

        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        // GET: BasicAdReport
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.AdTypes = new SelectList(dLAdvertisements.GetAdvertisementTypes(), "Type", "Type");
                ViewBag.AdAreas = new SelectList(dLAdvertisements.GetAdvertisementAreas(), "AdvertisementAreaName", "AdvertisementAreaName");
                ViewBag.Products = new SelectList(dLAdvertisements.GetAdProducts(), "ItemName", "ItemName");
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
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadData(AdSearchOptions search)
        {
            AdSearchOptions searchOptions = new AdSearchOptions();
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //find search columns info
            searchOptions.FirmName = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            searchOptions.ProductName = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            searchOptions.FromDate = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            searchOptions.ToDate = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            searchOptions.AdvertisementType = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            searchOptions.AdvertisementArea = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt16(start) : 0;            
            searchOptions.pageSize = pageSize;
            searchOptions.skip = skip;
            searchOptions.sortColumn = sortColumn;
            searchOptions.sortColumnDir = sortColumnDir;

            var adMainReport = dLAdsReport.GetAdvertisementReport(searchOptions);
            
            return Json(new { draw = draw, recordsFiltered = adMainReport.recordsTotal, recordsTotal = adMainReport.recordsTotal, data = adMainReport.AdsList },
                JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="Prefix"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Autocomplete(string Prefix)
        {
            List<CustomerInfo> customerList = new List<CustomerInfo>();
            customerList = dLAdsReport.GetCustomers(Prefix);
            return Json(customerList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="searchOptions"></param>
        /// <param name="postedFile"></param>
        /// <param name="notificationImage"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Promotion(AdSearchOptions searchOptions, HttpPostedFileBase postedFile, HttpPostedFileBase notificationImage)
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

                string Result = dLAdsReport.Promotion(searchOptions, MailAttachments, ImageURL, Convert.ToInt32(Session["UserID"].ToString()));

                //Generate a new unique identifier against which the file can be stored
                string handle = Guid.NewGuid().ToString();
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(Result);
                writer.Flush();
                stream.Position = 0;
                TempData[handle] = stream.ToArray();
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