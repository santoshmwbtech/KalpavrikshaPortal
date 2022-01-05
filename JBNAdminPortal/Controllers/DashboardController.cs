using JBNAdminPortal.Models;
using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using static JBNClassLibrary.CityWiseDetails;

namespace JBNAdminPortal.Controllers
{
    public class DashboardController : Controller
    {
        DashBoardData dal = new DashBoardData();
        // GET: Dashboard
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                Dashboard dashboard = dal.GetCategoriesForApproval(Convert.ToInt32(Session["UserID"].ToString()));
                return View(dashboard);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public JsonResult GetDashboardData()
        {
            return Json(dal.GetDashboardData());
        }

        #region Not Using
        //[HttpPost]
        //public JsonResult GetStateChart()
        //{
        //    CityWiseDetails cityWiseDetails = new CityWiseDetails();
        //    CityWiseRpt obj = new CityWiseRpt();
        //    List<CityWiseRpt> cityWiseDetailsRpt = new List<CityWiseRpt>();
        //    List<StateChart> stateCharts = new List<StateChart>();
        //    List<object> chartData = new List<object>();

        //    cityWiseDetailsRpt = cityWiseDetails.CityWiseRegDealersList(obj, 1);
        //    foreach(var item in cityWiseDetailsRpt)
        //    {
        //        StateChart stateChart = new StateChart();
        //        stateChart.StateName = item.StateName;
        //        stateChart.CustomersCount = item.TtlRegDealers;
        //        stateCharts.Add(stateChart);
        //    }
        //    return Json(stateCharts);
        //}
        #endregion
    }
}