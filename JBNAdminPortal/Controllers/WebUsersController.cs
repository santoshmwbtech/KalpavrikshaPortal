using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JBNClassLibrary;

namespace JBNAdminPortal.Controllers
{
    public class WebUsersController : Controller
    {
        // GET: WebUsers
        AdminUserCreation adminUserCreation = new AdminUserCreation();

        // GET: UserCreations

        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                List<UserCreation> details = new List<UserCreation>();
                details = adminUserCreation.GetUsers().ToList();
                return View(details.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        // GET: UserCreations/Create
        public ActionResult CreateUser()
        {
            ViewBag.RolesList = new SelectList(adminUserCreation.GetRolesData(), "RoleID", "RoleName");
            return PartialView();
        }
        public ActionResult UserList()
        {
            List<UserCreation> details = new List<UserCreation>();
            details = adminUserCreation.GetUsers().ToList();
            return PartialView(details.ToList());
        }

        [HttpPost]
        public ActionResult Save(UserCreation userCreation)
        {
            if (Session["UserID"] == null)
            {
                return this.RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                if (adminUserCreation.AddWebUsers(userCreation, Session["UserID"].ToString()))
                {
                    ModelState.Clear();
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return PartialView("UserList", adminUserCreation.GetUsers().ToList());
        }

        // GET: UserCreations/Edit/5
        public ActionResult EditUser(int UserID)
        {
            if (Session["UserID"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (UserID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.RolesList = new SelectList(adminUserCreation.GetRolesData(), "RoleID", "RoleName");
            UserCreation userCreation = adminUserCreation.GetUsersById(UserID);

            if (userCreation == null)
            {
                return HttpNotFound();
            }

            return PartialView(userCreation);
        }
        [HttpPost]
        public ActionResult Update(UserCreation userCreation)
        {
            if (Session["UserID"] == null)
            {
                return this.RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                if (adminUserCreation.AddWebUsers(userCreation, Session["UserID"].ToString()))
                {
                    ModelState.Clear();
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                foreach (var error in ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors)) 
                {
                    ViewBag.Error = error.ErrorMessage;
                }
            }
            return PartialView("UserList", adminUserCreation.GetUsers().ToList());
        }
        [HttpPost]
        public JsonResult CheckUsernameAvailability(string userName)
        {
            //System.Threading.Thread.Sleep(200);
            return Json(adminUserCreation.CheckUsernameAvailability(userName), JsonRequestBehavior.AllowGet);
        }
    }
}