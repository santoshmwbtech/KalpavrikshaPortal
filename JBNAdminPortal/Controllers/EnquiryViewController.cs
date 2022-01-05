using JBNAdminPortal.Models;
using JBNClassLibrary;
using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static JBNClassLibrary.DLEnquiries;

namespace JBNAdminPortal.Controllers
{
    public class EnquiryViewController : Controller
    {
        JBNClassLibrary.JBNDBClass DAL = new JBNClassLibrary.JBNDBClass();
        DLEnquiries EnquiriesDAL = new DLEnquiries();
        // GET: EnquiryView
        public ActionResult Index(string Route, string CustID, string EnquiryType)
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
                ViewBag.EnquiryStates = new SelectList(DAL.GetStateList(), "ID", "StateName");
                ViewBag.BusinessType = new SelectList(DAL.GetBusinessTypes(), "ID", "BusinessTypeName");
                ViewBag.CustomerList = new SelectList(DAL.GetCustomerList(context), "CustID", "FirmName");
                ViewBag.BusinessDemands = new SelectList(DAL.GetBusinessDemands(), "ID", "Demand");
                ViewBag.EnquiryCities = new SelectList(EnquiriesDAL.GetEnquiryCities(), "StateWithCityID", "VillageLocalityName");

                string MEnquiryType = Helper.Decrypt(EnquiryType, "sblw-3hn8-sqoy19");
                CustomerEnquiries enquiries = new CustomerEnquiries();
                enquiries.EnquiryType = MEnquiryType;
                return View(enquiries);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult EnquiryList(string Route, string CustID, string EnquiryType)
        {
            if (Session["UserID"] != null)
            {
                int? QueryID = Convert.ToInt32(Helper.Decrypt(Route, "sblw-3hn8-sqoy19"));
                int? MCustID = Convert.ToInt32(Helper.Decrypt(CustID, "sblw-3hn8-sqoy19"));
                string MEnquiryType = Helper.Decrypt(EnquiryType, "sblw-3hn8-sqoy19");
                CustomerEnquiries customerEnquiries = new CustomerEnquiries();
                customerEnquiries.QueryID = QueryID.Value;
                customerEnquiries.CustID = MCustID.Value;
                customerEnquiries.EnquiryType = MEnquiryType;
                List<CustomerEnquiries> enquiries = EnquiriesDAL.GetCustomerEnquiries(customerEnquiries);

                return PartialView(enquiries); 
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult SearchByDate(CustomerEnquiries searchEnquiry)
        {
            return PartialView("EnquiryList", EnquiriesDAL.GetCustomerEnquiries(searchEnquiry).ToList());
        }
    }
}