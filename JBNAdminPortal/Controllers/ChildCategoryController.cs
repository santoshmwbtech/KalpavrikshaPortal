using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JBNClassLibrary;
using System.Net;
using System.Data;
using System.Data.Entity;
using Newtonsoft.Json;

namespace JBNAdminPortal.Controllers
{
    public class ChildCategoryController : Controller
    {
        DLChildCategory DAL = new DLChildCategory();
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                List<childcategory> category = new List<childcategory>();
                return View(category.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult Details()
        {

            SubCategory subCategory = new SubCategory();
            ViewBag.SubCategory = new SelectList(subCategory.GetSubCatList(), "ID", "SubCategoryName");
            CategoryProduct pro = new CategoryProduct();
            ViewBag.CategoryProduct = new SelectList(pro.GetCategoryProductList(), "ID", "MainCategoryName");

            return PartialView();
        }
        public ActionResult Create()
        {
            SubCategory subCategory = new SubCategory();
            ViewBag.SubCategory = new SelectList(subCategory.GetSubCatList(), "ID", "SubCategoryName");
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind] childcategory category)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    DAL.SaveChildCategory(category, Convert.ToInt32(Session["UserID"].ToString()));
                    ModelState.Clear();
                    SubCategory subCategory = new SubCategory();
                    ViewBag.SubCategory = new SelectList(subCategory.GetSubCatList(), "ID", "SubCategoryName");
                    CategoryProduct pro = new CategoryProduct();
                    ViewBag.CategoryProduct = new SelectList(pro.GetCategoryProductList(), "ID", "MainCategoryName");
                    return PartialView();
                }
                else
                {
                    List<string> Errors = new List<string>();
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            Errors.Add(error.ErrorMessage);
                        }
                    }
                    ModelState.AddModelError("", Errors.ToString());

                    return HttpNotFound("Your request did not find.");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            
        }
        public ActionResult Edit(int ChildCategoryId)
        {
            if (ChildCategoryId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            childcategory category = DAL.GetChildCategoryDetail(ChildCategoryId);
            if (category == null)
            {
                return HttpNotFound();
            }
            SubCategory subCategory = new SubCategory();
            ViewBag.SubCategory = new SelectList(subCategory.GetSubCatList(), "ID", "SubCategoryName");
            return PartialView(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(childcategory childcategory)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    DAL.SaveChildCategory(childcategory, Convert.ToInt32(Session["UserID"].ToString()));
                    return PartialView("ChildCategoryList");
                }
                else
                {
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            ModelState.AddModelError("", error.ToString());
                        }
                    }
                    return HttpNotFound("Your request did not find.");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult GetMainCategory(int selectedValue)
        {
            DLChildCategory childCat = new DLChildCategory();

            string result = childCat.GetMainCategoryName(selectedValue);

            return Json(result);
        }
        public ActionResult ChildCategoryList()
        {
            if (Session["UserID"] != null)
            {
                List<childcategory> childList = new List<childcategory>();
                childList = DAL.GetChildCatList();

                DashBoardData DashboardDAL = new DashBoardData();
                object CategoriesData = DashboardDAL.GetAllProductsData();

                System.Reflection.PropertyInfo Mpi = CategoriesData.GetType().GetProperty("MainCategories");
                ViewBag.MainCategories = (int)(Mpi.GetValue(CategoriesData, null));

                System.Reflection.PropertyInfo Spi = CategoriesData.GetType().GetProperty("SubCategories");
                ViewBag.SubCategories = (int)(Spi.GetValue(CategoriesData, null));

                System.Reflection.PropertyInfo Cpi = CategoriesData.GetType().GetProperty("ChildCategories");
                ViewBag.ChildCategories = (int)(Cpi.GetValue(CategoriesData, null));

                System.Reflection.PropertyInfo Ipi = CategoriesData.GetType().GetProperty("ItemCategories");
                ViewBag.ItemCategories = (int)(Ipi.GetValue(CategoriesData, null));

                return PartialView(childList);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
       
        public ActionResult Search(childcategory obj)
        {
            if (Session["UserID"] != null)
            {
                if (obj.CategoryProductID == null)
                    obj.CategoryProductID = 0;
                if (obj.SubCategoryId == null)
                    obj.SubCategoryId = 0;
                obj.MainCategoryName = this.HttpContext.Request.Form["MainCategoryName"];
                List<childcategory> childList = new List<childcategory>();
                childList = DAL.GetChildCatList(obj);
                return PartialView("CategoryList", childList);
                
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public JsonResult GetMainCategoryName(string prefix)
        {
            List<childcategory> childList = new List<childcategory>();
            childcategory search = new childcategory();
            search.MainCategoryName = string.Empty;
            search.CategoryProductID = 0;
            search.SubCategoryId = 0;
            search.SubCategoryName = string.Empty;
            search.MainCategoryName = prefix;
            childList = DAL.GetChildCatList(search);
            return Json(childList.Select(C => new { C.CategoryProductID, C.MainCategoryName }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckDuplicateName(string ChildCategoryName)
        {
            bool Result = DAL.CheckDuplicateName(ChildCategoryName);
            if (Result == true)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
    }
}
