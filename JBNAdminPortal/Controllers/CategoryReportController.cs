using JBNAdminPortal.Models;
using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class CategoryReportController : Controller
    {
        DLCategories DAL = new DLCategories();
        // GET: CategoryReport
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.CategoryList = new SelectList(DAL.GetAllCategories(), "CategoryProductID", "MainCategoryName");
                //ViewBag.SubCategoryList = new SelectList(DAL.GetAllSubCategories(), "SubCategoryID", "SubCategoryName");
                //ViewBag.ItemCategoryList = new SelectList(DAL.GetAllChildCategories(), "ItemID", "ItemName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult SearchByCategories(AllCategories allCategories)
        {
            if (Session["UserID"] != null)
            {
                List<AllCategories> allCategoriesList = new List<AllCategories>();

                if (allCategories.CategoryProductID == null)
                    allCategories.CategoryProductID = 0;
                if (allCategories.SubCategoryID == null)
                    allCategories.SubCategoryID = 0;
                if (allCategories.ChildCategoryID == null)
                    allCategories.ChildCategoryID = 0;
                if (allCategories.ItemID == null)
                    allCategories.ItemID = 0;

                allCategoriesList = DAL.GetAllCategories(allCategories);
                return PartialView("CategoryList", allCategoriesList);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult CategoryList()
        {
            //AllCategories allCategories = new AllCategories();
            //allCategories.CategoryProductID = 0;
            //allCategories.SubCategoryID = 0;
            //allCategories.ItemID = 0;
            List<AllCategories> allCategoriesList = new List<AllCategories>();
            //allCategoriesList = DAL.GetAllCategories(allCategories);
            return PartialView(allCategoriesList);
        }

        [HttpPost]
        public JsonResult GetSubCategories(int CategoryProductID)
        {
            //return Json(DAL.GetAllSubCategories(CategoryProductID).Select(s => new { SubCategoryID = s.SubCategoryID, SubCategoryName = s.SubCategoryName }).ToList(), JsonRequestBehavior.AllowGet);
            return Json(DAL.GetAllSubCategories(CategoryProductID).Select(s => new { SubCategoryID = s.ID, SubCategoryName = s.SubCategoryName }).ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetChildCategories(int SubCategoryID)
        {
            return Json(DAL.GetAllChildCategories(SubCategoryID).Select(s => new { ItemID = s.ID, ItemName = s.ItemName }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}