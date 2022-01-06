using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class UpdateUserController : Controller
    {
        DLCustomerIncompleteRpt dLCustomerIncompleteRpt = new DLCustomerIncompleteRpt();
        JBNDBClass dLMain = new JBNDBClass();
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
            var customerList = dLCustomerIncompleteRpt.GetUsers();
            return PartialView(customerList);
        }
        public ActionResult Edit(int? CustID)
        {
            ViewBag.BusinessType = new SelectList(dLMain.GetBusinessTypes(), "BusinessTypeID", "BusinessTypeName");
            ViewBag.SubCategory = new SelectList(dLCustomerIncompleteRpt.GetAllSubCategories(), "ID", "SubCategoryName");
            var customerDetails = dLCustomerIncompleteRpt.GetCustomerDetails(CustID);
            return PartialView(customerDetails);
        }
    }
}