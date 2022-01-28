using JBNClassLibrary;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class PromotionsController : Controller
    {
        // GET: Promotions
        private readonly IPromotionRepository _promotionRepository;
        DLTemplates dLTemplates = new DLTemplates();
        public PromotionsController(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.NotificationType = new SelectList(
                                    new List<SelectListItem>
                                    {
                                        new SelectListItem { Text = "KYC Completed", Value = "KYC Completed" },
                                        new SelectListItem { Text = "KYC Not Completed", Value = "KYC Not Completed" },
                                        new SelectListItem { Text = "All", Value = "All" },
                                    }, "Value", "Text");
                ViewBag.SMSTemplates = new SelectList(dLTemplates.GetSMSTemplates(), "ID", "TemplateName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public async Task<JsonResult> Promote(PromotionsDTO promotionsDTO, HttpPostedFileBase postedFile, HttpPostedFileBase notificationImage)
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

            var Result = await _promotionRepository.Promotion(promotionsDTO, MailAttachments, ImageURL, Convert.ToInt32(Session["UserID"].ToString()));
            return Json(Result);
        }
    }
}