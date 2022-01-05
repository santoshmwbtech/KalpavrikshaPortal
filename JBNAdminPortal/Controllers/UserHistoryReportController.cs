using JBNAdminPortal.Models;
using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class UserHistoryReportController : Controller
    {
        DLUserHistoryReport dL = new DLUserHistoryReport();
        // GET: UserHistoryReport
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                AdminUserCreation userCreation = new AdminUserCreation();
                ViewBag.UserList = new SelectList(userCreation.GetUsers(), "UserID", "Username");
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
        /// <returns></returns>
        public ActionResult HistoryList()
        {
            return PartialView();
        }

        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public ActionResult Search(UserHistory history)
        {
            if (Session["UserID"] != null)
            {
                UserHistoryCounts userHistory = dL.GetUserHistoryCounts(history);
                return PartialView("HistoryList", userHistory);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}