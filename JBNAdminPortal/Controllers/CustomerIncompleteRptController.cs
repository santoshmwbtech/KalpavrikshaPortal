using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JBNAdminPortal.Models;
using JBNClassLibrary;
using System.IO;
using System.Configuration;
using System.Net.Mail;

namespace JBNAdminPortal.Controllers
{
    public class CustomerIncompleteRptController : Controller
    {
      

        // GET: CustomerIncompleteRpt
        JBNDBClass jbndbClass = new JBNDBClass();
        DLCustomerIncompleteRpt DAL = new DLCustomerIncompleteRpt();        
        DLAllCategoryReport dlAllCatRpt = new DLAllCategoryReport();
        DLTemplates dLTemplates = new DLTemplates();
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {        
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
        public ActionResult SearchByItem(CustomerIncompleteRpt search)
        {
            if (Session["UserID"] != null)
            {
                PromoWithCustomerIncompleteRpt promoWithCustomerIncompleteRpt = new PromoWithCustomerIncompleteRpt();
                List<CustomerIncompleteRpt> customerIncompleteRpts = new List<CustomerIncompleteRpt>();
                customerIncompleteRpts = DAL.GetCustomerList(search);
                promoWithCustomerIncompleteRpt.customerIncompleteRpts = customerIncompleteRpts;
                promoWithCustomerIncompleteRpt.Result = search.SearchByOption;
                ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
                return PartialView("CustomerList", promoWithCustomerIncompleteRpt);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="CustID"></param>
        /// <param name="SearchByOption"></param>
        /// <returns></returns>
        public ActionResult BlockCustomer(int? CustID, string SearchByOption)
        {
            if (Session["UserID"] != null)
            {
                PromoWithCustomerIncompleteRpt promoWithCustomerIncompleteRpt = new PromoWithCustomerIncompleteRpt();
                List<CustomerIncompleteRpt> customerIncompleteRpts = new List<CustomerIncompleteRpt>();
                promoWithCustomerIncompleteRpt = DAL.BlockCustomer(CustID, Convert.ToInt32(Session["UserID"].ToString()));

                CustomerIncompleteRpt search = new CustomerIncompleteRpt();
                search.SearchByOption = SearchByOption;
                customerIncompleteRpts = DAL.GetCustomerList(search);

                promoWithCustomerIncompleteRpt.customerIncompleteRpts = customerIncompleteRpts;
                promoWithCustomerIncompleteRpt.Result = SearchByOption;
                return PartialView("CustomerList", promoWithCustomerIncompleteRpt);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="CustID"></param>
        /// <returns></returns>
        public ActionResult Edit(int? CustID)
        {
            
            if (Session["UserID"] != null)
            {
                CustomerIncompleteRpt customerIncompleteRpt = new CustomerIncompleteRpt();
                customerIncompleteRpt = DAL.GetCustomerDetails(CustID);
                ViewBag.StateList = new SelectList(jbndbClass.GetStateList(), "ID", "StateName");
                ViewBag.CityList = new SelectList(jbndbClass.GetAllCities(), "ID", "VillageLocalityName");
                ViewBag.BusinessType = new SelectList(jbndbClass.GetBusinessTypes(), "BusinessTypeID", "BusinessTypeName");
                ViewBag.SubCategory = new SelectList(DAL.GetAllSubCategories(), "ID", "SubCategoryName");
                return PartialView(customerIncompleteRpt);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="incompleteRpt"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(CustomerIncompleteRpt incompleteRpt)
        {
            if (Session["UserID"] != null)
            {
                if (DAL.UpdateCustomerDetailsKYC(incompleteRpt, Convert.ToInt32(Session["UserID"].ToString())))
                {
                    return Json("success");
                }
                else
                {
                    return Json("error");
                }
            }
            else
            {
                return Json("sessionexpired");
            }
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
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public string GetCurrentWebsiteRoot()
        {
            return HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
        }
        [HttpPost]
        public JsonResult Promo(PromoWithCustomerIncompleteRpt promotion, HttpPostedFileBase postedFile, HttpPostedFileBase notificationImage)
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
            return File(data, "application/txt", fileName);
        }
    }
}