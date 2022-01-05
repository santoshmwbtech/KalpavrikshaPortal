using JBNClassLibrary;
using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class ViewHistoryController : Controller
    {
        DLUserHistoryReport dL = new DLUserHistoryReport();
        // GET: ViewHistory
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                AdminUserCreation userCreation = new AdminUserCreation();
                ViewBag.UserList = new SelectList(userCreation.GetUsers(), "UserID", "Username");
                ViewBag.ActivityPages = new SelectList(dL.GetActivityPages(), "ActivityPage", "ActivityPage");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult HistoryList(string encuserid, string encactivitypage)
        {
            if (Session["UserID"] != null && !string.IsNullOrEmpty(encuserid) && !string.IsNullOrEmpty(encactivitypage))
            {
                UserHistory history = new UserHistory();
                history.UserID = Convert.ToInt32(Helper.Decrypt(encuserid, "sblw-3hn8-sqoy19"));
                history.ActivityPage = Helper.Decrypt(encactivitypage, "sblw-3hn8-sqoy19");

                List<UserHistory> histories = dL.GetHistoryReport(history);
                return PartialView(histories);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Search(UserHistory history)
        {
            if (Session["UserID"] != null)
            {
                List<UserHistory> histories = dL.GetHistoryReport(history);
                return PartialView("HistoryList", histories);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}