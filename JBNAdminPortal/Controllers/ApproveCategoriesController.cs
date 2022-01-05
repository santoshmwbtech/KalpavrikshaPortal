using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class ApproveCategoriesController : Controller
    {
        // GET: ApproveCategories
        DLApproveCategories DAL = new DLApproveCategories();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApproveMainCategory(int id)
        {
            if (Session["UserID"] != null)
            {
                CategoryProduct DLCP = new CategoryProduct();
                MainCategory mainCategory = DLCP.GetCategoryProductDetail(id);
                return View(mainCategory);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult ApproveSubCategory(int id)
        {
            if (Session["UserID"] != null)
            {
                CategoryProduct pro = new CategoryProduct();
                ViewBag.CategoryProducts = new SelectList(pro.GetCategoryProductList(), "ID", "MainCategoryName");
                SubCategory DLSC = new SubCategory();
                SubCat subCat = DLSC.GetSubCategoryDetailForApprove(id);
                return View(subCat);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult ApproveChildCategory(int ChildCatID)
        {
            if (Session["UserID"] != null)
            {
                SubCategory subCategory = new SubCategory();
                ViewBag.SubCategory = new SelectList(subCategory.GetSubCatList(), "ID", "SubCategoryName");
                DLChildCategory DLCC = new DLChildCategory();
                childcategory childCategory = DLCC.GetChildCategoryDetail(ChildCatID);
                return View(childCategory);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult ApproveItemCategory(int ItemCat)
        {
            if (Session["UserID"] != null)
            {
                DLChildCategory dLChildCategory = new DLChildCategory();
                ViewBag.ChildCategory = new SelectList(dLChildCategory.GetChildCatList(), "ID", "ChildCategoryName");
                DLItemCategory DLIC = new DLItemCategory();
                ItemCategory item = DLIC.GetItemCategoryDetail(ItemCat);
                return View(item);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public JsonResult ApproveMainCategory(MainCategory mainCategory)
        {
            if (Session["UserID"] != null)
            {
                mainCategory.IsActive = true;
                string Message = DAL.ApproveMainCategory(mainCategory);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("SessionExpired");
            }
        }
        //reject main category
        [HttpPost]
        public JsonResult RejectMainCategory(int? CategoryID)
        {
            if (Session["UserID"] != null)
            {
                MainCategory mainCategory = new MainCategory();
                mainCategory.CategoryProductID = CategoryID.Value;
                mainCategory.IsActive = false;
                string Message = DAL.RejectMainCategory(mainCategory);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("SessionExpired");
            }
        }

        [HttpPost]
        public JsonResult ApproveSubCategory(SubCat subCat)
        {
            if (Session["UserID"] != null)
            {
                string Message = DAL.ApproveSubCategory(subCat);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("SessionExpired");
            }
        }

        //reject sub category
        [HttpPost]
        public JsonResult RejectSubCategory(int? CategoryID)
        {
            if (Session["UserID"] != null)
            {
                SubCat subCat = new SubCat();
                subCat.ID = CategoryID.Value;
                subCat.IsActive = false;
                string Message = DAL.RejectSubCategory(subCat);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("SessionExpired");
            }
        }

        [HttpPost]
        public JsonResult ApproveChildCategory(childcategory childcategory)
        {
            if (Session["UserID"] != null)
            {
                string Message = DAL.ApproveChildCategory(childcategory);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("SessionExpired");
            }
        }

        //reject child category
        [HttpPost]
        public JsonResult RejectChildCategory(int? CategoryID)
        {
            if (Session["UserID"] != null)
            {
                childcategory childcategory = new childcategory();
                childcategory.ID = CategoryID.Value;
                childcategory.IsActive = false;
                string Message = DAL.RejectChildCategory(childcategory);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("SessionExpired");
            }
        }

        [HttpPost]
        public JsonResult ApproveItemCategory(ItemCategory item)
        {
            if (Session["UserID"] != null)
            {
                string Message = DAL.ApproveItemCategory(item);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("SessionExpired");
            }
        }

        //reject sub category
        [HttpPost]
        public JsonResult RejectItemCategory(int? CategoryID)
        {
            if (Session["UserID"] != null)
            {
                ItemCategory item = new ItemCategory();
                item.ID = CategoryID.Value;
                item.IsActive = false;
                string Message = DAL.RejectItemCategory(item);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("SessionExpired");
            }
        }
    }
}