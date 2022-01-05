using JBNClassLibrary;
using System.Linq;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class CityWiseDetailedController : Controller
    {
        CityWiseDetails CityWise = new CityWiseDetails();
        // GET: CityWiseDetailed
        public ActionResult Index()
        {
            //ViewBag.DebtorsList = new SelectList(dLDebtorsCreation.GetDebtorsList(), "ID", "DebtorName");
            //return View();
            if (Session["UserID"] != null)
            {
                ViewBag.CityList = new SelectList(CityWise.GetCities(string.Empty), "ID", "VillageLocalityName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult CityDetailedList(CityWiseDetailedRpt cityWiseDetail)
        {
            return PartialView(CityWise.CityDetailedList(cityWiseDetail).ToList());
        }
    }
}