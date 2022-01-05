using JBNClassLibrary;
using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class AppUserDetailsController : Controller
    {
        JBNClassLibrary.JBNDBClass DAL = new JBNClassLibrary.JBNDBClass();
        // GET: AppUserDetails
        public ActionResult Index(string EncCustID)
        {
            if (Session["UserID"] != null && !string.IsNullOrEmpty(EncCustID))
            {
                string DecCustID = Helper.Decrypt(EncCustID, "sblw-3hn8-sqoy19");
                CustomerDetails customerDetails = new CustomerDetails();
                customerDetails = DAL.GetCustomerDetails(Convert.ToInt32(DecCustID));
                ViewBag.StateList = new SelectList(DAL.GetStateList(), "ID", "StateName");
                string markers = "[";
                markers += "{";
                markers += string.Format("'lat': '{0}',", customerDetails.Lattitude);
                markers += string.Format("'lng': '{0}',", customerDetails.Langitude);
                markers += "},";
                markers += "];";
                ViewBag.Markers = markers;
                return View(customerDetails);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public JsonResult Update(CustomerDetails customerDetails)
        {
            if (Session["UserID"] != null)
            {
                if (DAL.UpdateAppUser(customerDetails, Convert.ToInt32(Session["UserID"].ToString())))
                    return Json("success");
                else
                    return Json("db Error");

            }
            else
            {
                return Json("error");
            }
        }
    }
}