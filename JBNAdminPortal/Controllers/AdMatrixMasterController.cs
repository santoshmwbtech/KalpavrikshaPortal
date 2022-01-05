using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class AdMatrixMasterController : Controller
    {
        // GET: AdMatrixMaster
        DLMatrixMaster dLMatrixMaster = new DLMatrixMaster(); 
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

        public ActionResult GetMatrixTypeDetails(string matrixType)
        {
            if(matrixType.ToLower() == "advertisement type")
            {
                return PartialView("AdvertisementTypes", dLMatrixMaster.GetAdvertisementType());
            }
            else if(matrixType.ToLower() == "advertisement area")
            {
                return PartialView("AdvertisementArea", dLMatrixMaster.GetAdvertisementArea());
            }
            else
            {
                return PartialView("AdvertisementTimeSlot", dLMatrixMaster.GetAdvertisementTimeSlot());
            }   
        }

        public JsonResult UpdateAdType(List<AdvertisementType> advertisementTypes)
        {
            if (Session["UserID"] == null)
            {
                return Json("sessionexpired");
            }
            var Result = dLMatrixMaster.UpdateAdType(advertisementTypes);
            return Json(Result);
        }
        public JsonResult UpdateAdArea(List<AdvertisementArea> advertisementAreas)
        {
            if (Session["UserID"] == null)
            {
                return Json("sessionexpired");
            }
            var Result = dLMatrixMaster.UpdateAdArea(advertisementAreas);
            return Json(Result);
        }
        public JsonResult UpdateTimeSlots(List<AdvertisementTimeSlot> advertisementTimeSlots)
        {
            if (Session["UserID"] == null)
            {
                return Json("sessionexpired");
            }
            var Result = dLMatrixMaster.UpdateTimeSlots(advertisementTimeSlots);
            return Json(Result);
        }
    }
}