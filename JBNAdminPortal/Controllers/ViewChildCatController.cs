using JBNAdminPortal.Models;
using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class ViewChildCatController : Controller
    {
        DLItemCategory DAL = new DLItemCategory();
        // GET: ViewChildCat
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
        public ActionResult Edit(int ItemID)
        {
            if (ItemID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemCategory itemCategory = DAL.GetItemCategoryDetail(ItemID);
            if (itemCategory == null)
            {
                return HttpNotFound();
            }
            DLChildCategory dLChildCategory = new DLChildCategory();
            ViewBag.ChildCategory = new SelectList(dLChildCategory.GetChildCatList(), "ID", "ChildCategoryName");

            return PartialView(itemCategory);
        }
        public ActionResult ItemCategoryList()
        {
            if (Session["UserID"] != null)
            {
                List<ItemCategory> itemCategories = new List<ItemCategory>();
                itemCategories = DAL.GetItemCatList();
                return PartialView(itemCategories);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public JsonResult GetParentCategories(int? ChildCategoryID)
        {
            ItemCategory itemCategory = new ItemCategory();
            itemCategory = DAL.GetParentCategories(ChildCategoryID);
            return Json(itemCategory, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ItemCategory itemCategory)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    DAL.SaveItemCategory(itemCategory, Convert.ToInt32(Session["UserID"].ToString()));
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
        [HttpPost]
        public JsonResult ItemCategoryListJson()
        {
            if (Session["UserID"] != null)
            {
                //Server Side Parameter
                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                List<ItemCategory> itemCategories = new List<ItemCategory>();
                itemCategories = DAL.GetItemCatList();

                int totalrows = itemCategories.Count;
                if (!string.IsNullOrEmpty(searchValue))//filter
                {
                    itemCategories = itemCategories.
                        Where(x => x.ItemName.ToLower().Contains(searchValue.ToLower()) || x.ItemApprovedBy.ToLower().Contains(searchValue.ToLower()) || x.ItemRefferedByOrReason.ToLower().Contains(searchValue.ToLower()) || x.ChildCategoryName.ToString().Contains(searchValue.ToLower())).ToList<ItemCategory>();
                }
                int totalrowsafterfiltering = itemCategories.Count;
                //sorting
                //itemCategories = itemCategories.OrderBy(sortColumnName + " " + sortDirection).ToList<ItemCategory>();

                //paging
                itemCategories = itemCategories.Skip(start).Take(length).ToList<ItemCategory>();


                return Json(new { data = itemCategories, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0");
            }
        }
    }
}