using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class AdServiceController : Controller
    {
        // GET: AdService
        public ActionResult Index()
        {
            DLAdvertisements dLAdvertisements = new DLAdvertisements();
            dLAdvertisements.SendNotifications();
            return View();
        }
    }
}