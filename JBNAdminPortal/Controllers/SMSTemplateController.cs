using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class SMSTemplateController : Controller
    {
        DLTemplates dLTemplates = new DLTemplates();
        // GET: SMSTemplate
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
        public ActionResult Save(Templates templates)
        {
            if (Session["UserID"] == null)
            {
                return this.RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                Templates Result = dLTemplates.SaveSMSTemplate(templates, Session["UserID"].ToString());
                return Json(Result.DisplayMessage);
            }
            else
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return Json(new { allErrors });
            }
        }
        public ActionResult TemplateList()
        {
            if (Session["UserID"] == null)
            {
                return this.RedirectToAction("Index", "Login");
            }

            List<Templates> result = new List<Templates>();
            try
            {
                result = dLTemplates.GetSMSTemplates();
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
                return Json(new { url = Url.Action("Index", "Login") });
            }
            if (ID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Templates templates = dLTemplates.GetSMSTemplateDetails(ID);

            if (templates == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", templates);
        }
        public ActionResult Delete(int ID)
        {
            if (Session["UserID"] == null)
            {
                return Json(new { url = Url.Action("Index", "Login") });
            }
            if (ID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Templates Result = dLTemplates.DeleteSMSTemplate(ID, Session["OrgID"].ToString(), Session["UserID"].ToString());
            return Json(Result.DisplayMessage, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Templates templates)
        {
            if (Session["UserID"] == null)
            {
                return Json(new { url = Url.Action("Index", "Login") });
            }
            if (ModelState.IsValid)
            {
                Templates Result = dLTemplates.SaveSMSTemplate(templates, Session["UserID"].ToString());
                return Json(Result.DisplayMessage);
            }
            else
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return Json(allErrors.ToList());
            }
        }
    }
}