using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class PerformanceReport
    {
        [Required(ErrorMessage = "Please select from date")]
        public string FromDate { get; set; }
        [Required(ErrorMessage = "Please select to date")]
        public string ToDate { get; set; }
        [Required(ErrorMessage = "Please select from date")]
        public string CompareFromDate { get; set; }
        [Required(ErrorMessage = "Please select to date")]
        public string CompareToDate { get; set; }
        public int RegisteredUser { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int BlockedUsers { get; set; }
        public int MainCategories { get; set; }
        public int SubCategories { get; set; }
        public int ChildCategories { get; set; }
        public int ItemCategories { get; set; }
        public int TotalCities { get; set; }
        public int TotalStates { get; set; }
        public int TotalEnquiries { get; set; }
        public int CRegisteredUser { get; set; }
        public int CActiveUsers { get; set; }
        public int CInactiveUsers { get; set; }
        public int CBlockedUsers { get; set; }
        public int CMainCategories { get; set; }
        public int CSubCategories { get; set; }
        public int CChildCategories { get; set; }
        public int CItemCategories { get; set; }
        public int CTotalCities { get; set; }
        public int CTotalStates { get; set; }
        public int CTotalEnquiries { get; set; }
        public string CRegisteredStatus { get; set; }
        public string CActiveUsersStatus { get; set; }
        public string CInactiveUsersStatus { get; set; }
        public string CBlockedUsersStatus { get; set; }
        public string CMainCategoriesStatus { get; set; }
        public string CSubCategoriesStatus { get; set; }
        public string CChildCategoriesStatus { get; set; }
        public string CItemCategoriesStatus { get; set; }
        public string CTotalCitiesStatus { get; set; }
        public string CTotalStatesStatus { get; set; }
        public string CTotalEnquiriesStatus { get; set; }
        public float RegisteredPer { get; set; }
        public float ActivePer { get; set; }
        public float InactivePer { get; set; }
        public float BlockedPer { get; set; }
        public float MainCatPer { get; set; }
        public float SubCatPer { get; set; }
        public float ChildCatPer { get; set; }
        public float ItemCatPer { get; set; }
        public float CitiesPer { get; set; }
        public float StatesPer { get; set; }
        public float EnquiriesPer { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class DLPerformanceReport
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public PerformanceReport GetPerformanceReport(PerformanceReport search)
        {
            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    PerformanceReport performanceReports = new PerformanceReport();
                    if ((!string.IsNullOrEmpty(search.FromDate) && !string.IsNullOrEmpty(search.ToDate)) && (string.IsNullOrEmpty(search.CompareFromDate) && string.IsNullOrEmpty(search.CompareToDate)))
                    {
                        DateTime FromDate = DateTime.ParseExact(search.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime ToDate = DateTime.ParseExact(search.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        var CustomerTBL = (from c in dbContext.tblCustomerDetails
                                           select c).ToList();

                        //performanceReports.RegisteredUser = CustomerTBL.Where(c => c.CreatedDate.Value.Date >= FromDate.Date && c.CreatedDate.Value.Date <= ToDate.Date).Select(ct => ct.CustID).Count();
                        performanceReports.RegisteredUser = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(ct => ct.ID).Count();//Select(ct => ct.CustID).Count();

                        ActivityReport activityReport = new ActivityReport();
                        activityReport.FromDate = search.FromDate;
                        activityReport.ToDate = search.ToDate;
                        activityReport.StateID = 0;
                        activityReport.CityID = 0;

                        activityReport.IsActive = 1;
                        DLActivityReport dLActivityReport = new DLActivityReport();
                        performanceReports.ActiveUsers = dLActivityReport.CustomerActivityReport(activityReport).activityReports.Count();

                        activityReport.IsActive = 2;
                        performanceReports.InactiveUsers = dLActivityReport.CustomerActivityReport(activityReport).activityReports.Count();                        

                        performanceReports.TotalCities = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(c => c.City).Distinct().Count();

                        performanceReports.TotalStates = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(c => c.State).Distinct().Count();

                        performanceReports.BlockedUsers = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Where(c => c.IsActive == false).Count();

                        var MainCatTBL = (from c in dbContext.tblCategoryProductWithCusts
                                          select c).ToList();

                        performanceReports.MainCategories = MainCatTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(m => m.CategoryProductID).Distinct().Count();

                        var SubCatTBL = (from s in dbContext.tblSubCategoryProductWithCusts
                                         select s).ToList();

                        performanceReports.SubCategories = SubCatTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(s => s.SubCategoryId).Distinct().Count();

                        var ChildCatTBL = (from sc in dbContext.tblSubCategoryProductWithCusts
                                          join c in dbContext.tblChildCategories on sc.SubCategoryId equals c.SubCategoryId
                                          join ic in dbContext.tblItemCategories on c.ID equals ic.ID
                                          

                                           select new ItemCategory
                                           {
                                               //ChildCategoryId = c.ChildCategoryId,
                                               ChildCategoryId = c.ID,
                                               CreatedDate = sc.CreatedDate,
                                               ID = ic.ID
                                           }).ToList();

                        performanceReports.ChildCategories = ChildCatTBL.Where(c => c.CreatedDate.Value.Date >= FromDate.Date && c.CreatedDate.Value.Date <= ToDate.Date).Select(c => c.ChildCategoryId).Distinct().Count();

                        performanceReports.ItemCategories = ChildCatTBL.Where(c => c.CreatedDate.Value.Date >= FromDate.Date && c.CreatedDate.Value.Date <= ToDate.Date).Select(c => c.ID).Distinct().Count();//.Select(c => c.ItemId).Distinct

                        var EnquiryTBL = dbContext.tblselectedDealers.ToList();
                        performanceReports.TotalEnquiries = EnquiryTBL.Where(sd => sd.CreatedDate.Date >= FromDate.Date && sd.CreatedDate.Date <= ToDate.Date).Count();
                        performanceReports.ErrorMessage = "success";
                        performanceReports.FromDate = search.FromDate;
                        performanceReports.ToDate = search.ToDate;
                        return performanceReports;
                    }
                    else if ((!string.IsNullOrEmpty(search.FromDate) && !string.IsNullOrEmpty(search.ToDate)) && (!string.IsNullOrEmpty(search.CompareFromDate) && !string.IsNullOrEmpty(search.CompareToDate)))
                    {
                        DateTime FromDate = DateTime.ParseExact(search.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime ToDate = DateTime.ParseExact(search.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime CompareFromDate = DateTime.ParseExact(search.CompareFromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime CompareToDate = DateTime.ParseExact(search.CompareToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        var CustomerTBL = (from c in dbContext.tblCustomerDetails
                                           select c).ToList();

                        performanceReports.RegisteredUser = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(ct => ct.ID).Count();//Select(ct => ct.CustID).Count();
                        performanceReports.CRegisteredUser = CustomerTBL.Where(c => c.CreatedDate.Date >= CompareFromDate.Date && c.CreatedDate.Date <= CompareToDate.Date).Select(ct => ct.ID).Count();//.Select(ct => ct.CustID).Count

                        performanceReports.RegisteredPer = GetPercentage(performanceReports.CRegisteredUser, performanceReports.RegisteredUser);
                        performanceReports.CRegisteredStatus = performanceReports.RegisteredPer < 0 ? "Fall" : "Growth";
                        #region activity report
                        ActivityReport activityReport = new ActivityReport();
                        activityReport.FromDate = search.FromDate;
                        activityReport.ToDate = search.ToDate;
                        activityReport.StateID = 0;
                        activityReport.CityID = 0;

                        activityReport.IsActive = 1;
                        DLActivityReport dLActivityReport = new DLActivityReport();
                        performanceReports.ActiveUsers = dLActivityReport.CustomerActivityReport(activityReport).activityReports.Count();

                        activityReport.IsActive = 2;
                        performanceReports.InactiveUsers = dLActivityReport.CustomerActivityReport(activityReport).activityReports.Count();
                        

                        activityReport.FromDate = search.CompareFromDate;
                        activityReport.ToDate = search.CompareToDate;

                        performanceReports.CActiveUsers = dLActivityReport.CustomerActivityReport(activityReport).activityReports.Count();

                        activityReport.IsActive = 2;
                        performanceReports.CInactiveUsers = dLActivityReport.CustomerActivityReport(activityReport).activityReports.Count();

                        performanceReports.ActivePer = GetPercentage(performanceReports.CActiveUsers, performanceReports.ActiveUsers);
                        performanceReports.CActiveUsersStatus = performanceReports.ActivePer < 0 ? "Fall" : "Growth";

                        performanceReports.InactivePer = GetPercentage(performanceReports.CInactiveUsers, performanceReports.InactiveUsers);
                        performanceReports.CInactiveUsersStatus = performanceReports.InactivePer < 0 ? "Fall" : "Growth";
                        #endregion

                        #region cities count
                        performanceReports.TotalCities = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(c => c.City).Distinct().Count();
                        performanceReports.CTotalCities = CustomerTBL.Where(c => c.CreatedDate.Date >= CompareFromDate.Date && c.CreatedDate.Date <= CompareToDate.Date).Select(c => c.City).Distinct().Count();
                        
                        performanceReports.CitiesPer = GetPercentage(performanceReports.CTotalCities, performanceReports.TotalCities);
                        performanceReports.CTotalCitiesStatus = performanceReports.CitiesPer < 0 ? "Fall" : "Growth";
                        #endregion


                        #region States Count
                        performanceReports.TotalStates = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(c => c.State).Distinct().Count();
                        performanceReports.CTotalStates = CustomerTBL.Where(c => c.CreatedDate.Date >= CompareFromDate.Date && c.CreatedDate.Date <= CompareToDate.Date).Select(c => c.State).Distinct().Count();

                        performanceReports.StatesPer = GetPercentage(performanceReports.CTotalStates, performanceReports.TotalStates);
                        performanceReports.CTotalStatesStatus = performanceReports.StatesPer < 0 ? "Fall" : "Growth";
                        #endregion

                        #region Blocked Users Count
                        performanceReports.BlockedUsers = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Where(c => c.IsActive == false).Count();
                        performanceReports.CBlockedUsers = CustomerTBL.Where(c => c.CreatedDate.Date >= CompareFromDate.Date && c.CreatedDate.Date <= CompareToDate.Date).Where(c => c.IsActive == false).Count();
                        performanceReports.BlockedPer = GetPercentage(performanceReports.CBlockedUsers, performanceReports.BlockedUsers);
                        performanceReports.CBlockedUsersStatus = performanceReports.BlockedPer < 0 ? "Fall" : "Growth";
                        #endregion

                        #region Main Categories
                        var MainCatTBL = (from c in dbContext.tblCategoryProductWithCusts
                                          select c).ToList();

                        performanceReports.MainCategories = MainCatTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(m => m.CategoryProductID).Distinct().Count();
                        performanceReports.CMainCategories = MainCatTBL.Where(c => c.CreatedDate.Date >= CompareFromDate.Date && c.CreatedDate.Date <= CompareToDate.Date).Select(m => m.CategoryProductID).Distinct().Count();
                        performanceReports.MainCatPer = GetPercentage(performanceReports.CMainCategories, performanceReports.MainCategories);
                        performanceReports.CMainCategoriesStatus = performanceReports.MainCatPer < 0 ? "Fall" : "Growth";
                        #endregion

                        #region Subcategories
                        var SubCatTBL = (from s in dbContext.tblSubCategoryProductWithCusts
                                         select s).ToList();

                        performanceReports.SubCategories = SubCatTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(s => s.SubCategoryId).Distinct().Count();
                        performanceReports.CSubCategories = SubCatTBL.Where(c => c.CreatedDate.Date >= CompareFromDate.Date && c.CreatedDate.Date <= CompareToDate.Date).Select(s => s.SubCategoryId).Distinct().Count();
                        performanceReports.SubCatPer = GetPercentage(performanceReports.CSubCategories, performanceReports.SubCategories);
                        performanceReports.CSubCategoriesStatus = performanceReports.SubCatPer < 0 ? "Fall" : "Growth";
                        #endregion

                        var ChildCatTBL = (from sc in dbContext.tblSubCategoryProductWithCusts
                                           join c in dbContext.tblChildCategories on sc.SubCategoryId equals c.SubCategoryId
                                           join ic in dbContext.tblItemCategories on c.SubCategoryId equals ic.ChildCategoryID

                                           select new ItemCategory
                                           {
                                               //ChildCategoryId = c.ChildCategoryId,
                                               ChildCategoryId = c.ID,
                                               CreatedDate = sc.CreatedDate,
                                             //  ItemId = c.ItemId
                                             ID = ic.ID
                                           }).ToList();

                        performanceReports.ChildCategories = ChildCatTBL.Where(c => c.CreatedDate.Value.Date >= FromDate.Date && c.CreatedDate.Value.Date <= ToDate.Date).Select(c => c.ChildCategoryId).Distinct().Count();
                        performanceReports.CChildCategories = ChildCatTBL.Where(c => c.CreatedDate.Value.Date >= CompareFromDate.Date && c.CreatedDate.Value.Date <= CompareToDate.Date).Select(c => c.ChildCategoryId).Distinct().Count();
                        performanceReports.ChildCatPer = GetPercentage(performanceReports.CChildCategories, performanceReports.ChildCategories);
                        performanceReports.CChildCategoriesStatus = performanceReports.ChildCatPer < 0 ? "Fall" : "Growth";

                        performanceReports.ItemCategories = ChildCatTBL.Where(c => c.CreatedDate.Value.Date >= FromDate.Date && c.CreatedDate.Value.Date <= ToDate.Date).Select(c => c.ID).Distinct().Count();//Select(c => c.ItemId).Distinct().Count();
                        performanceReports.CItemCategories = ChildCatTBL.Where(c => c.CreatedDate.Value.Date >= CompareFromDate.Date && c.CreatedDate.Value.Date <= CompareToDate.Date).Select(c => c.ID).Distinct().Count();//Select(c => c.ItemId).Distinct().Count();
                        performanceReports.ItemCatPer = GetPercentage(performanceReports.CItemCategories, performanceReports.ItemCategories);
                        performanceReports.CItemCategoriesStatus = performanceReports.ItemCatPer < 0 ? "Fall" : "Growth";

                        var EnquiryTBL = dbContext.tblselectedDealers.ToList();
                        performanceReports.TotalEnquiries = EnquiryTBL.Where(sd => sd.CreatedDate.Date >= FromDate.Date && sd.CreatedDate.Date <= ToDate.Date).Count();
                        performanceReports.CTotalEnquiries = EnquiryTBL.Where(sd => sd.CreatedDate.Date >= CompareFromDate.Date && sd.CreatedDate.Date <= CompareToDate.Date).Count();
                        performanceReports.EnquiriesPer = GetPercentage(performanceReports.CTotalEnquiries, performanceReports.TotalEnquiries);
                        performanceReports.CTotalEnquiriesStatus = performanceReports.EnquiriesPer < 0 ? "Fall" : "Growth";

                        performanceReports.ErrorMessage = "success";
                        performanceReports.FromDate = search.FromDate;
                        performanceReports.ToDate = search.ToDate;
                        performanceReports.CompareFromDate = search.CompareFromDate;
                        performanceReports.CompareToDate = search.CompareToDate;
                        return performanceReports;
                    }
                    else
                    {
                        PerformanceReport errorReport = new PerformanceReport();
                        errorReport.ErrorMessage = "Please select fromdate and todate";
                        return errorReport;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                PerformanceReport errorReport = new PerformanceReport();
                errorReport.ErrorMessage = "error";
                return errorReport;
                
            }
        }

        public PerformanceReport GetComparison(PerformanceReport search)
        {
            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    PerformanceReport performanceReports = new PerformanceReport();
                    if (!string.IsNullOrEmpty(search.CompareFromDate) && !string.IsNullOrEmpty(search.CompareToDate))
                    {
                        DateTime FromDate = DateTime.ParseExact(search.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime ToDate = DateTime.ParseExact(search.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        var CustomerTBL = (from c in dbContext.tblCustomerDetails
                                           select c).ToList();

                        performanceReports.RegisteredUser = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(ct => ct.ID).Count();
                        //Select(ct => ct.CustID).Count();

                        ActivityReport activityReport = new ActivityReport();
                        activityReport.FromDate = search.FromDate;
                        activityReport.ToDate = search.ToDate;
                        activityReport.StateID = 0;
                        activityReport.CityID = 0;

                        activityReport.IsActive = 1;
                        DLActivityReport dLActivityReport = new DLActivityReport();
                        performanceReports.ActiveUsers = dLActivityReport.CustomerActivityReport(activityReport).activityReports.Count();

                        activityReport.IsActive = 2;
                        performanceReports.InactiveUsers = dLActivityReport.CustomerActivityReport(activityReport).activityReports.Count();

                        performanceReports.TotalCities = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(c => c.City).Distinct().Count();

                        performanceReports.TotalStates = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(c => c.State).Distinct().Count();

                        performanceReports.BlockedUsers = CustomerTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Where(c => c.IsActive == false).Count();

                        var MainCatTBL = (from c in dbContext.tblCategoryProductWithCusts
                                          select c).ToList();

                        performanceReports.MainCategories = MainCatTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(m => m.CategoryProductID).Distinct().Count();

                        var SubCatTBL = (from s in dbContext.tblSubCategoryProductWithCusts
                                         select s).ToList();

                        performanceReports.SubCategories = SubCatTBL.Where(c => c.CreatedDate.Date >= FromDate.Date && c.CreatedDate.Date <= ToDate.Date).Select(s => s.SubCategoryId).Distinct().Count();

                        var ChildCatTBL = (from sc in dbContext.tblSubCategoryProductWithCusts
                                           join c in dbContext.tblChildCategories on sc.SubCategoryId equals c.SubCategoryId
                                           join ic in dbContext.tblItemCategories on c.SubCategoryId equals ic.ChildCategoryID
                                           select new ItemCategory
                                           {
                                               //ChildCategoryId = c.ChildCategoryId,
                                               ChildCategoryId = c.ID,
                                               CreatedDate = sc.CreatedDate,
                                               //  ItemId = c.ItemId
                                               ID = ic.ID,
                                           }).ToList();

                        performanceReports.ChildCategories = ChildCatTBL.Where(c => c.CreatedDate.Value.Date >= FromDate.Date && c.CreatedDate.Value.Date <= ToDate.Date).Select(c => c.ChildCategoryId).Distinct().Count();

                        performanceReports.ItemCategories = ChildCatTBL.Where(c => c.CreatedDate.Value.Date >= FromDate.Date && c.CreatedDate.Value.Date <= ToDate.Date).Select(c => c.ID).Distinct().Count();//.Select(c => c.ItemId).Distinct().Count();

                        var EnquiryTBL = dbContext.tblselectedDealers.ToList();
                        performanceReports.TotalEnquiries = EnquiryTBL.Where(sd => sd.CreatedDate.Date >= FromDate.Date && sd.CreatedDate.Date <= ToDate.Date).Count();
                        performanceReports.ErrorMessage = "success";
                        performanceReports.FromDate = search.FromDate;
                        performanceReports.ToDate = search.ToDate;
                        performanceReports.CompareFromDate = search.CompareFromDate;
                        performanceReports.CompareToDate = search.CompareToDate;
                        return performanceReports;
                    }
                    else
                    {
                        PerformanceReport errorReport = new PerformanceReport();
                        errorReport.ErrorMessage = "Please select fromdate and todate";
                        return errorReport;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                PerformanceReport errorReport = new PerformanceReport();
                errorReport.ErrorMessage = "error";
                return errorReport;

            }
        }

        public float GetPercentage(float OldValue, float  NewValue)
        {
            float DiffValue = (OldValue - NewValue);
            float NewVal = (NewValue == 0 ? 1 : NewValue);
            float Percentage = (float)Math.Round(((DiffValue / NewVal) * 100), 2);
            return Percentage;
        }
    }
}
