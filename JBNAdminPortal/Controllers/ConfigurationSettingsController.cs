using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class ConfigurationSettingsController : Controller
    {
        DLAdminSettings DAL = new DLAdminSettings();
        // GET: ConfigurationSettings
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                
                return View(DAL.GetAdminSettings());
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public JsonResult UpdateSettings(AdminSettings adminSettings)
        {
            if (Session["UserID"] != null)
            {
                var Result = DAL.UpdateAdminSettings(adminSettings);
                return Json(Result.DisplayMessage);
            }
            else
            {
                return Json("sessionexpired");
            }
        }
    }
}