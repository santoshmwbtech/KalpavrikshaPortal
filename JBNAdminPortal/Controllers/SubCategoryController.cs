using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JBNClassLibrary;

namespace JBNAdminPortal.Controllers
{
    public class SubCategoryController : Controller
    {
        SubCategory DLSubCategory = new SubCategory();

        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                List<SubCat> category = new List<SubCat>();
                category = DLSubCategory.GetSubCatList().ToList();
                return View(category.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Details()
        {
            List<SubCat> category = DLSubCategory.GetSubCatList().ToList();

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

            return PartialView(category);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            CategoryProduct pro = new CategoryProduct();
            ViewBag.CategoryProducts = new SelectList(pro.GetCategoryProductList(), "ID", "MainCategoryName");
            return PartialView();
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="subCat"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SubCat subCat)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (DLSubCategory.SaveSubCategory(subCat, Convert.ToInt32(Session["UserID"].ToString())))
                    {
                        List<SubCat> SubCategoryList = DLSubCategory.GetSubCatList().ToList();
                        CategoryProduct pro = new CategoryProduct();
                        ViewBag.CategoryProducts = new SelectList(pro.GetCategoryProductList(), "ID", "MainCategoryName");
                        return PartialView("Create");
                    }
                    else
                    {
                        return HttpNotFound("Your request did not find.");
                    }
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
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Edit(int ID)
        {
            if (ID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            SubCat subCat = DLSubCategory.GetSubCategoryDetail(ID);
            if (subCat == null)
            {

                return Content("Not Found");
            }
            CategoryProduct pro = new CategoryProduct();
            ViewBag.CategoryProducts = new SelectList(pro.GetCategoryProductList(), "ID", "MainCategoryName");
            return PartialView(subCat);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="subCat"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int ID, [Bind] SubCat subCat)
        {
            if (Session["UserID"] != null)
            {
                if (ID != subCat.ID)
                {
                    return HttpNotFound();
                }
                if (ModelState.IsValid)
                {

                }
                DLSubCategory.SaveSubCategory(subCat, Convert.ToInt32(Session["UserID"].ToString()));
                List<SubCat> SubCategoryList = DLSubCategory.GetSubCatList().ToList();
                return PartialView("Details", SubCategoryList);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="SubCategoryName"></param>
        /// <returns></returns>
        public JsonResult CheckDuplicateName(string SubCategoryName)
        {
            bool Result = DLSubCategory.CheckDuplicateName(SubCategoryName);
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
