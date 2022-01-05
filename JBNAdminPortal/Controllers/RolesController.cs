using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class RolesController : Controller
    {
        DLGetRoleCreation dLGetRoleCreation = new DLGetRoleCreation();
        // GET: Roles
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                return View(dLGetRoleCreation.GetData());
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(DLRoleCreation dLRoleCreation)
        {
            if (Session["UserID"] == null)
            {
                return this.RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                if (dLGetRoleCreation.SaveData(dLRoleCreation, Session["UserID"].ToString()))
                {
                    ModelState.Clear();
                }
                else
                {
                    ModelState.Clear();
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return PartialView("RoleList", dLGetRoleCreation.GetData().ToList());
        }
        public ActionResult RoleList()
        {
            List<DLRoleCreation> result = new List<DLRoleCreation>();
            try
            {
                result = dLGetRoleCreation.GetData();
            }
            catch (Exception ex)
            {

            }
            return PartialView(result);
        }

        public ActionResult Edit(int RoleID)
        {
            if (Session["UserID"] == null)
            {
                return this.RedirectToAction("Index", "Login");
            }
            if (RoleID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DLRoleCreation obj = dLGetRoleCreation.GetRoleDetailsByID(RoleID);

            if (obj == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", obj);
        }
        [HttpPost]
        public ActionResult Update(DLRoleCreation dLRole)
        {
            if (Session["UserID"] == null)
            {
                return this.RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                if (dLGetRoleCreation.SaveData(dLRole, Session["UserID"].ToString()))
                {
                    ModelState.Clear();
                    return PartialView("RoleList", dLGetRoleCreation.GetData().ToList());
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        public JsonResult CheckDuplicateRoleName(string rolename)
        {

            bool Result = dLGetRoleCreation.CheckDuplicateRoleName(rolename);
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