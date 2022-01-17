using JBNClassLibrary;
using JBNWebAPI.Logger;
using System.Collections.Generic;
using System.Web.Mvc;


namespace JBNAdminPortal.Controllers
{
    public class LoginController : Controller
    {
        LoginValidation Dal = new LoginValidation();
        // GET: Login
        public ActionResult Index()
        {
            DLAdvertisements dLAdvertisements = new DLAdvertisements();
            dLAdvertisements.SendNotifications();
            return View();
        }
        [HttpPost]
        public ActionResult Index(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Login login = (Login)Dal.Login(model);
            if (login == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            else
            {
                Session["UserID"] = login.UserID;
                Session["FullName"] = login.FullName;
                Session["RoleID"] = login.RoleID;
                DLFormPermission dL = new DLFormPermission();
                List<FormPermissionItem> MenuList = dL.LoadFormPermissionItems(login.RoleID);
                Session["MenuMaster"] = MenuList;
                return RedirectToAction("Index", "Dashboard");
            }
        }
    }
}