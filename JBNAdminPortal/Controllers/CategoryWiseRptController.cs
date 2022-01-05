using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JBNClassLibrary;

namespace JBNAdminPortal.Controllers
{
    public class CategoryWiseRptController : Controller
    {
        GetCategoryandCustomerDetails details = new GetCategoryandCustomerDetails();
        // GET: CategoryWiseRpt
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.Display = new SelectList(details.GetCategory(), "CategoryProductID", "MainCategoryName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult Display(CategoryWiseRpt categoryWiseRpt)
        {
            if (Session["UserID"] != null)
            {
                List<CategoryWiseRpt> users = new List<CategoryWiseRpt>();
                if (categoryWiseRpt.CategoryProductID != null)
                {
                    users = details.GetDetails(categoryWiseRpt);
                    ModelState.Clear();
                }
                else
                {
                    users = details.GetDetails(categoryWiseRpt);
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