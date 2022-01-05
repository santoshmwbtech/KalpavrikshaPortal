using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JBNClassLibrary;
using static JBNClassLibrary.DLEnquiries;

namespace JBNAdminPortal.Controllers
{
    public class CustomerConversationsController : Controller
    {
        DLEnquiries DAL = new DLEnquiries();
        // GET: CustomerConversations
        public ActionResult Index(string QueryID, string CustID, string SenderID, string EnquiryType)
        {
            if (Session["UserID"] != null)
            {
                int MQueryID = Convert.ToInt32(Helper.Decrypt(QueryID, "sblw-3hn8-sqoy19"));
                int MCustID = Convert.ToInt32(Helper.Decrypt(CustID, "sblw-3hn8-sqoy19"));
                int MSenderID = Convert.ToInt32(Helper.Decrypt(SenderID, "sblw-3hn8-sqoy19"));
                string MEnquiryType = Helper.Decrypt(EnquiryType, "sblw-3hn8-sqoy19");
                CustomerConversations conversations = new CustomerConversations();
                conversations = DAL.GetConversations(MCustID, MQueryID, MSenderID, MEnquiryType);
                return View(conversations);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult Conversations(string QueryID, string CustID, string SenderID, string EnquiryType)
        {
            if (Session["UserID"] != null)
            {
                int MQueryID = Convert.ToInt32(Helper.Decrypt(QueryID, "sblw-3hn8-sqoy19"));
                int MCustID = Convert.ToInt32(Helper.Decrypt(CustID, "sblw-3hn8-sqoy19"));
                int MSenderID = Convert.ToInt32(Helper.Decrypt(SenderID, "sblw-3hn8-sqoy19"));
                string MEnquiryType = Helper.Decrypt(EnquiryType, "sblw-3hn8-sqoy19");
                CustomerConversations conversations = new CustomerConversations();
                conversations = DAL.GetConversations(MCustID, MQueryID, MSenderID, MEnquiryType);

                return PartialView(conversations.MessageList);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}