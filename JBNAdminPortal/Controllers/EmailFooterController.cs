using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class EmailFooterController : Controller
    {
        DLEmailFooter DL = new DLEmailFooter();
        // GET: EmailFooter
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                return View(DL.GetEmailFooters().ToList());
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
        public ActionResult Save(EmailFooter emailFooter)
        {
            if (Session["UserID"] == null)
            {
                return this.RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                if (DL.SaveEmailFooter(emailFooter))
                {
                    ModelState.Clear();
                }
                else
                {
                    ModelState.Clear();
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return PartialView("EmailFooterList", DL.GetEmailFooters().ToList());
        }
        public ActionResult EmailFooterList()
        {
            List<EmailFooter> result = new List<EmailFooter>();
            try
            {
                result = DL.GetEmailFooters();
            }
            catch (Exception ex)
            {

            }
            return PartialView(result);
        }

        public ActionResult Edit(int ID)
        {
            if (Session["UserID"] == null)
            {
                return this.RedirectToAction("Index", "Login");
            }
            if (ID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EmailFooter obj = DL.GetEmailFooter(ID);

            if (obj == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", obj);
        }
        [HttpPost]
        public ActionResult Update(EmailFooter emailFooter)
        {
            if (Session["UserID"] == null)
            {
                return this.RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                if (DL.SaveEmailFooter(emailFooter))
                {
                    ModelState.Clear();
                    return PartialView("EmailFooterList", DL.GetEmailFooters().ToList());
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
    }
}