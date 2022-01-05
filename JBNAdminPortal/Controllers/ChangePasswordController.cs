using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JBNClassLibrary;

namespace JBNAdminPortal.Controllers
{
    public class ChangePasswordController : Controller
    {
        // GET: ChangePassword
        DLChangePassword dLChangePassword = new DLChangePassword();
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                AdminUserCreation DAL = new AdminUserCreation();
                UserCreation userCreation = new UserCreation();
                userCreation = DAL.GetUsersById(Convert.ToInt32(Session["UserID"].ToString()));
                ViewBag.UserName = userCreation.Username;
                ChangePassword changePassword = new ChangePassword();
                changePassword.UserID = Convert.ToInt32(Session["UserID"].ToString());
                return View(changePassword);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdatePassword(ChangePassword changePassword)
        {
            string Result = string.Empty;

            if (Session["UserID"] == null)
            {
                return Json("0");
            }
            if (ModelState.IsValid)
            {
                Result = dLChangePassword.UpdatePassword(changePassword).ReturnMessage;
                ModelState.Clear();
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ValidatePassword(string oldPassword)
        {
            if (Session["UserID"] != null)
            {
                int Result = dLChangePassword.ValidatePassword(oldPassword, Convert.ToInt32(Session["UserID"].ToString()));
                return Json(Result);
            }
            else
            {
                return Json("sessionexpired");
            }
            
        }
    }
}