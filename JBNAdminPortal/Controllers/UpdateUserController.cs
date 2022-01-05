using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class UpdateUserController : Controller
    {
        // GET: UpdateUser
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult UserList()
        {
            return PartialView();
        }
    }
}