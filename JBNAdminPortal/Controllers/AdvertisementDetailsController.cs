using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JBNClassLibrary;
using JBNWebAPI.Logger;

namespace JBNAdminPortal.Controllers
{
    public class AdvertisementDetailsController : Controller
    {
        DLAdvertisements dLAdvertisements = new DLAdvertisements();
        // GET: AdvertisementDetails
        public ActionResult Index(string Route)
        {
            if (Session["UserID"] != null && !string.IsNullOrEmpty(Route))
            {
                int AdvertisementMainID = Convert.ToInt32(Helper.Decrypt(Route, "sblw-3hn8-sqoy19"));
                AdvertisementMain advertisementMain = new AdvertisementMain();
                advertisementMain = dLAdvertisements.GetAdvertisementDetailsForPortal(AdvertisementMainID);

                string URL = ConfigurationManager.AppSettings["WebsiteURL"].ToString();
                if (!string.IsNullOrEmpty(advertisementMain.AdImageURL))
                {
                    advertisementMain.AdImageURL = URL + "/MWBImages/" + advertisementMain.AdImageURL;
                }

                if (advertisementMain.IsApproved == true && advertisementMain.IsPaymentPaid == true)
                {
                    ViewBag.StatusTypes = new SelectList(
                                    new List<SelectListItem>
                                    {
                                        new SelectListItem { Text = "Stop Advertisement", Value = "Stop Advertisement"},
                                    }, "Value", "Text");
                }
                else if (advertisementMain.IsApproved == false && advertisementMain.IsRejected == true)
                {
                    ViewBag.StatusTypes = new SelectList(
                                    new List<SelectListItem>
                                    {
                                        new SelectListItem { Text = "Content Approved", Value = "Content Approved" },
                                    }, "Value", "Text");
                }
                else if (advertisementMain.IsApproved == false && advertisementMain.IsRejected == false)
                {
                    ViewBag.StatusTypes = new SelectList(
                                    new List<SelectListItem>
                                    {
                                        new SelectListItem { Text = "Content Approved", Value = "Content Approved" },
                                        new SelectListItem { Text = "Content Rejected", Value = "Content Rejected"},
                                    }, "Value", "Text");
                }
                else if (advertisementMain.IsApproved == false && advertisementMain.IsPaymentPaid == false && advertisementMain.IsRejected == true)
                {
                    ViewBag.StatusTypes = new SelectList(
                                    new List<SelectListItem>
                                    {
                                        new SelectListItem { Text = "Content Approved", Value = "Content Approved" },
                                        new SelectListItem { Text = "Payment Approved", Value = "Payment Approved"},
                                        //new SelectListItem { Text = "Content Rejected", Value = "Content Rejected"},
                                        //new SelectListItem { Text = "Payment Rejected", Value = "Payment Rejected"},
                                    }, "Value", "Text");
                }
                else if (advertisementMain.IsApproved == true && advertisementMain.IsRejected == false && advertisementMain.IsPaymentPaid == false)
                {
                    if(advertisementMain.paymentDetails == null || advertisementMain.paymentDetails.Count() <= 0)
                    {
                        ViewBag.StatusTypes = new SelectList(
                                       new List<SelectListItem>
                                       {
                                            new SelectListItem { Text = "Content Rejected", Value = "Content Rejected"},
                                            new SelectListItem { Text = "Payment Rejected", Value = "Payment Rejected"},
                                       }, "Value", "Text");
                    }
                    else
                    {
                        ViewBag.StatusTypes = new SelectList(
                                       new List<SelectListItem>
                                       {
                                            new SelectListItem { Text = "Content Rejected", Value = "Content Rejected"},
                                            new SelectListItem { Text = "Payment Approved", Value = "Payment Approved"},
                                            new SelectListItem { Text = "Payment Rejected", Value = "Payment Rejected"},
                                       }, "Value", "Text");
                    }
                    
                }
                else
                {
                    ViewBag.StatusTypes = new SelectList(
                                       new List<SelectListItem>
                                       {
                                           new SelectListItem { Text = "Content Approved", Value = "Content Approved" },
                                           new SelectListItem { Text = "Payment Approved", Value = "Payment Approved"},
                                           new SelectListItem { Text = "Content Rejected", Value = "Content Rejected"},
                                           new SelectListItem { Text = "Payment Rejected", Value = "Payment Rejected"},
                                       }, "Value", "Text");

                }

                return View(advertisementMain);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public string GetBaseUrl()
        {
            var request = HttpContext.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }
        public JsonResult SaveAdvertisement(AdvertisementMain advertisementMain)
        {
            if (Session["UserID"] != null)
            {
                string Result = dLAdvertisements.ApproveAd(advertisementMain, Convert.ToInt32(Session["UserID"].ToString()));
                return Json(Result);
            }
            else
            {
                return Json("sessionexpired");
            }
        }
        [HttpPost]
        public JsonResult RejectAdvertisement(int? AdvertisementMainID, string Reason, int? CustID)
        {
            if (Session["UserID"] != null)
            {
                AdvertisementMain advertisementMain = new AdvertisementMain();
                advertisementMain.AdvertisementMainID = AdvertisementMainID.Value;
                advertisementMain.CustID = CustID.Value;
                advertisementMain.Remarks = Reason;
                advertisementMain.IsRejected = true;
                string Result = dLAdvertisements.ApproveAd(advertisementMain, Convert.ToInt32(Session["UserID"].ToString()));
                return Json(Result);
            }
            else
            {
                return Json("sessionexpired");
            }
        }
    }
}