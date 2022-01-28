using JBNAdminPortal.Models;
using JBNClassLibrary;
using Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using static JBNClassLibrary.CityWiseDetails;
using static JBNClassLibrary.DLEnquiries;

namespace JBNAdminPortal.Controllers
{
    public class EnquiriesController : Controller
    {
        JBNClassLibrary.JBNDBClass DAL = new JBNClassLibrary.JBNDBClass();
        DLEnquiries EnquiriesDAL = new DLEnquiries();

        private readonly IEnquiryRepository _enquiryRepository;
        public EnquiriesController(IEnquiryRepository enquiryRepository)
        {
            _enquiryRepository = enquiryRepository;
        }
        // GET: Enquiries
        public async Task<ActionResult> Index()
        {
            if (Session["UserID"] != null)
            {
                CustomerDetails context = new CustomerDetails();
                State state = new State();
                City city = new City();
                state.StateID = 0;
                city.StateWithCityID = 0;
                context.state = state;
                context.city = city;
                ViewBag.StateList = new SelectList(DAL.GetStateList(), "ID", "StateName");
                ViewBag.BusinessType = new SelectList(DAL.GetBusinessTypes(), "BusinessTypeID", "BusinessTypeName");
                ViewBag.ItemCategories = new SelectList(await _enquiryRepository.GetItemCategories(), "ID", "ItemName");
                ViewBag.CustomerList = new SelectList(DAL.GetCustomerList(context), "CustID", "FirmName");
                ViewBag.BusinessDemand = new SelectList(DAL.GetBusinessDemands(), "ID", "BusinessDemand");
                ViewBag.EnquiryCities = new SelectList(EnquiriesDAL.GetEnquiryCities(), "StateWithCityID", "VillageLocalityName");
                ViewBag.BusinessDemands = new SelectList(DAL.GetBusinessDemands(), "ID", "Demand");
                ViewBag.EnquiryTypes = new SelectList(DAL.GetEnquiryTypes(), "EnquiryType", "EnquiryType");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult EnquiryList()
        {
            EnquiriesDL enquiries = new EnquiriesDL();
            enquiries.ProductID = 0;
            enquiries.StateID = 0;
            enquiries.CityID = 0;
            EnquiryListWithTotals enquiryListWithTotals = _enquiryRepository.GetEnquiries(enquiries); //EnquiriesDAL.GetEnquiries(enquiries);
            return PartialView("EnquiryList", enquiryListWithTotals);
        }

        [HttpPost]
        public ActionResult SearchByDate(Search search)
        {
            EnquiriesDL enquiries = new EnquiriesDL();
            //enquiries.ProductID = search.ItemID != null ? search.ItemID.Value : 0;
            //enquiries.StateID = search.StateID != null ? search.StateID.Value : 0;
            //enquiries.CityID = search.StatewithCityID != null ? search.StatewithCityID.Value : 0;

            enquiries.StateList = search.StateList;
            enquiries.CityList = search.CityList;
            enquiries.ItemCategoryList = search.ItemCategoryList;
            enquiries.BusinessTypeList = search.BusinessTypeList;
            enquiries.CustomerList = search.CustomerList;

            enquiries.BusinessDemandID = search.BusinessDemandID != null ? search.BusinessDemandID : 0;
            enquiries.BusinessTypeID = search.BusinessTypeID != null ? search.BusinessTypeID : 0;
            enquiries.PurposeBusiness = search.PurposeOfBusiness;
            enquiries.FromDate = search.FromDate;
            enquiries.ToDate = search.ToDate;
            enquiries.FromTime = search.FromTime;
            enquiries.ToTime = search.ToTime;
            enquiries.EnquiryType = search.EnquiryType;
            EnquiryListWithTotals enquiryListWithTotals = EnquiriesDAL.GetEnquiries(enquiries);
            return PartialView("EnquiryList", enquiryListWithTotals);
        }
    }
}