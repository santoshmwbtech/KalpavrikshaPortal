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
    public class CategoryProductsController : Controller
    {
        
        CategoryProduct DALMainCategory = new CategoryProduct();


        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                List<MainCategory> product = new List<MainCategory>();
                product = DALMainCategory.GetCategoryProductList().ToList();
                return View(product.ToList());
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
        [HttpGet]
        public ActionResult Create_CategoryProduct()
        {
            return PartialView();
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="ProductDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_CategoryProduct([Bind] MainCategory ProductDetails)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    ProductDetails.CreatedID = Convert.ToInt32(Session["UserID"].ToString());
                    var Result = DALMainCategory.SaveCategoryProduct(ProductDetails, Convert.ToInt32(Session["UserID"].ToString()));
                    ModelState.Clear();
                    return Json(Result.DisplayMsg, JsonRequestBehavior.AllowGet);

                }
                return View(DALMainCategory);
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
        public ActionResult Edit_CategoryProduct(int ID)
        {
            if (ID == 0)
            {
                return HttpNotFound();
            }
            MainCategory product = DALMainCategory.GetCategoryProductDetail(ID);

            if (product == null)
            {
                return HttpNotFound();
            }
            return PartialView(product);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="categoryProduct"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_CategoryProduct(MainCategory categoryProduct)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    categoryProduct.CreatedID = Convert.ToInt32(Session["UserID"].ToString());
                    var Result = DALMainCategory.SaveCategoryProduct(categoryProduct, Convert.ToInt32(Session["UserID"].ToString()));
                    return Json(Result.DisplayMsg, JsonRequestBehavior.AllowGet);
                }
                List<MainCategory> product = new List<MainCategory>();

                DashBoardData DashboardDAL = new DashBoardData();
                object CategoriesData = DashboardDAL.GetDashboardData();

                System.Reflection.PropertyInfo Mpi = CategoriesData.GetType().GetProperty("MainCategories");
                ViewBag.MainCategories = (int)(Mpi.GetValue(CategoriesData, null));

                System.Reflection.PropertyInfo Spi = CategoriesData.GetType().GetProperty("SubCategories");
                ViewBag.SubCategories = (int)(Spi.GetValue(CategoriesData, null));

                System.Reflection.PropertyInfo Cpi = CategoriesData.GetType().GetProperty("ChildCategories");
                ViewBag.ChildCategories = (int)(Cpi.GetValue(CategoriesData, null));

                System.Reflection.PropertyInfo Ipi = CategoriesData.GetType().GetProperty("ItemCategories");
                ViewBag.ItemCategories = (int)(Ipi.GetValue(CategoriesData, null));

                product = DALMainCategory.GetCategoryProductList().ToList();
                return PartialView("Product_Details", product);
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
        [HttpGet]
        public ActionResult Product_Details()
        {
            List<MainCategory> product = DALMainCategory.GetCategoryProductList().ToList();
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

            return PartialView(product);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="MainCategoryName"></param>
        /// <returns></returns>
        public JsonResult CheckDuplicateName(string MainCategoryName)
        {
            bool Result = DALMainCategory.CheckDuplicateName(MainCategoryName);
            if (Result == true)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        //    [HttpGet]
        //    public ActionResult Delete_CategoryProduct(int ID)
        //    {
        //        if (ID == 0)
        //        {
        //            return HttpNotFound();
        //        }
        //        CatPro product = pro.GetCategoryProductDetail(ID);

        //        if (product == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(product);
        //    }

        //    [HttpPost, ActionName("Delete_CategoryProduct")]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult DeleteConfirmed(int ID)
        //    {
        //        pro.DeleteProduct((int)ID);
        //        return RedirectToAction("Index");
        //    }
    }
}    