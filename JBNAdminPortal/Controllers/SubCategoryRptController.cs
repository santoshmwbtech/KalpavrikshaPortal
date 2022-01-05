using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JBNClassLibrary;

namespace JBNAdminPortal.Controllers
{
    public class SubCategoryRptController : Controller
    {
        GetSubCategoryandCustomerDetails details = new GetSubCategoryandCustomerDetails();
        // GET: CategoryWiseRpt
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.Display = new SelectList(details.GetSubCategory(), "SubCategoryId", "SubCategoryName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult Display(SubCategoryWiseRpt subCategoryWiseRpt)
        {
            if (Session["UserID"] != null)
            {
                List<SubCategoryWiseRpt> users = new List<SubCategoryWiseRpt>();
                if (subCategoryWiseRpt.SubCategoryId != null)
                {
                    users = details.GetDetails(subCategoryWiseRpt);
                    ModelState.Clear();
                }
                else
                {
                    users = details.GetDetails(subCategoryWiseRpt);
                    ModelState.Clear();
                }

                if (users == null)
                {
                    return HttpNotFound();
                }
                return PartialView(users);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
    }
}