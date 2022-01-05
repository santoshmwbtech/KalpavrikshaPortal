using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class FormPermissionsController : Controller
    {
        DLFormPermission dL = new DLFormPermission();
        // GET: FormPermissions
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.Roles = new SelectList(dL.GetSysRoles(), "ID", "RoleName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult FormPermissionItemList()
        {
            if (Session["UserID"] != null)
            {
                List<FormPermissionItem> itemList = new List<FormPermissionItem>();
                itemList = dL.GetItems();
                return PartialView(itemList.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult LoadFormPermissionItems(int? RoleID)
        {
            if (Session["UserID"] != null)
            {
                ViewBag.Roles = new SelectList(dL.GetSysRoles(), "ID", "RoleName");
                List<FormPermissionItem> itemList = new List<FormPermissionItem>();
                itemList = dL.LoadFormPermissionItems(RoleID);
                return PartialView("FormPermissionItemList", itemList.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Save(FormPermission formPermission, List<FormPermissionItem> ItemList)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    formPermission.Items = ItemList;
                    FormPermission Result = dL.SaveFormPermission(formPermission, Convert.ToInt32(Session["UserID"].ToString()));
                    ModelState.Clear();
                    ViewBag.Roles = new SelectList(dL.GetSysRoles(), "ID", "RoleName");
                    List<FormPermissionItem> itemList = new List<FormPermissionItem>();
                    itemList = dL.GetItems();
                    return PartialView("FormPermissionItemList", itemList.ToList());
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
    }
}