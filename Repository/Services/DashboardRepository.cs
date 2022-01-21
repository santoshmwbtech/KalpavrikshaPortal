using JBNClassLibrary;
using JBNWebAPI.Logger;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Services
{
    public class DashboardRepository : IDashboardRepository
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public async Task<object> GetAllProductsData()
        {
            int MainCategories = 0, SubCategories = 0, ChildCategories = 0, ItemCategories = 0;
            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    MainCategories = await dbContext.tblCategoryProducts.Where(c => c.IsActive == true).CountAsync();
                    SubCategories = await dbContext.tblSubCategories.Where(c => c.IsActive == true).CountAsync();
                    ChildCategories = await dbContext.tblChildCategories.Where(c => c.IsActive == true).CountAsync();
                    ItemCategories = await dbContext.tblItemCategories.Where(c => c.IsActive == true).CountAsync();

                    var json = new
                    {
                        MainCategories = MainCategories,
                        SubCategories = SubCategories,
                        ChildCategories = ChildCategories,
                        ItemCategories = ItemCategories,
                    };

                    return json;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<Dashboard> GetCategoriesForApproval(int UserID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {

                    var user = await dbContext.tblUsers.Where(u => u.ID == UserID).FirstOrDefaultAsync();
                    int RoleID = user.RoleID.Value;
                    Dashboard dashboard = new Dashboard();
                    if (RoleID == 1)
                    {

                        dashboard.MainCategories = await (from c in dbContext.tblCategoryProducts
                                                          join u in dbContext.tblUsers on c.CreatedBy equals u.ID
                                                          where c.IsActive == false && c.IsRejected == false
                                                          select new MainCategory
                                                          {
                                                              CategoryProductID = c.ID,
                                                              MainCategoryName = c.MainCategoryName,
                                                              CreatedDate = c.CreatedDate,
                                                              CreatedByName = u.FullName
                                                          }
                                                   ).ToListAsync();

                        dashboard.SubCategories = await (from c in dbContext.tblSubCategories
                                                         join u in dbContext.tblUsers on c.CreatedBy equals u.ID
                                                         where c.IsActive == false && c.IsRejected == false
                                                         select new SubCat
                                                         {
                                                             ID = c.ID,
                                                             SubCategoryName = c.SubCategoryName,
                                                             CreatedDate = c.CreatedDate,
                                                             CreatedByName = u.FullName
                                                         }
                                                   ).ToListAsync();
                        dashboard.ChildCategories = await (from c in dbContext.tblChildCategories
                                                           join u in dbContext.tblUsers on c.CreatedBy equals u.ID
                                                           where c.IsActive == false && c.IsRejected == false
                                                           select new childcategory
                                                           {
                                                               ID = c.ID,
                                                               ChildCategoryName = c.ChildCategoryName,
                                                               CreatedDate = c.CreatedDate,
                                                               CreatedByName = u.FullName
                                                           }
                                                   ).Distinct().ToListAsync();
                        dashboard.ItemCategories = await (from c in dbContext.tblItemCategories
                                                          join u in dbContext.tblUsers on c.CreatedBy equals u.ID
                                                          where c.IsActive == false && c.IsRejected == false
                                                          select new ItemCategory
                                                          {
                                                              ID = c.ID,
                                                              ItemName = c.ItemName,
                                                              CreatedDate = c.CreatedDate,
                                                              CreatedByName = u.FullName
                                                          }
                                                   ).Distinct().ToListAsync();
                        dashboard.RoleID = RoleID;
                        return dashboard;
                    }
                    else
                    {
                        dashboard.MainCategories = null;
                        dashboard.SubCategories = null;
                        dashboard.ChildCategories = null;
                        dashboard.ItemCategories = null;
                        dashboard.RoleID = RoleID;
                        return dashboard;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public async Task<object> GetDashboardData()
        {
            int RegisteredCustomers = 0, ActiveCustomers = 0, InactiveCustomers = 0, BlockedCustomers = 0, CitiesCovered = 0, StatesCovered = 0, Enquiries = 0;
            int RegisteredToday = 0, ActiveToday = 0, InactiveToday = 0, BlockedToday = 0, CitiesToday = 0, StatesToday = 0, EnquiriesToday = 0;
            int MainCategories = 0, SubCategories = 0, ChildCategories = 0, ItemCategories = 0;
            int MainCatsToday = 0, SubCatsToday = 0, ChildCatsToday = 0, ItemCatsToday = 0;
            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    var Customers = dbContext.tblCustomerDetails;
                    RegisteredCustomers = Customers.Count();
                    RegisteredToday = Customers.ToList().Where(c => c.CreatedDate.Date == DateTimeNow.Date).Count();

                    ActivityReport activityReport = new ActivityReport();
                    activityReport.FromDate = string.Empty;
                    activityReport.ToDate = string.Empty;
                    activityReport.StateID = 0;
                    activityReport.CityID = 0;

                    activityReport.IsActive = 1;
                    DLActivityReport dLActivityReport = new DLActivityReport();
                    var activityReports = dLActivityReport.CustomerActivityReport(activityReport).activityReports;
                    ActiveCustomers = activityReports.Count();
                    ActiveToday = activityReports.Where(a => a.LastLoginDate.Date == DateTimeNow.Date).Count();

                    activityReport.IsActive = 2;
                    var inactivityReports = dLActivityReport.CustomerActivityReport(activityReport).activityReports;
                    InactiveCustomers = inactivityReports.Count();
                    InactiveToday = inactivityReports.Where(a => a.LastLoginDate.Date < DateTimeNow.Date).Count();

                    var CustomerTBL = await Customers.Where(c => c.ModifiedDate != null).ToListAsync();

                    CitiesCovered = CustomerTBL.Select(c => c.City).Distinct().Count();
                    CitiesToday = CustomerTBL.Where(ct => ct.CreatedDate.Date == DateTimeNow.Date).Select(c => c.City).Distinct().Count();

                    StatesCovered = CustomerTBL.Where(c => c.State != null).Select(c => c.State).Distinct().Count();
                    StatesToday = CustomerTBL.Where(ct => ct.CreatedDate.Date == DateTimeNow.Date && ct.State != null).Select(c => c.State).Distinct().Count();

                    BlockedCustomers = CustomerTBL.Where(c => c.IsActive == false).Count();
                    BlockedToday = CustomerTBL.Where(c => c.IsActive == false && c.UpdatedByDate.Value.Date == DateTimeNow.Date).Count();

                    var MainCatTBL = (from c in dbContext.tblCategoryProductWithCusts
                                      join cp in dbContext.tblCategoryProducts on c.CategoryProductID equals cp.ID
                                      select new
                                      {
                                          CategoryProductID = cp.ID,
                                          CreatedDate = c.CreatedDate
                                      }).Distinct().ToList();

                    MainCategories = MainCatTBL.Select(m => m.CategoryProductID).Distinct().Count();
                    var maincatsToday = MainCatTBL.Where(m => m.CreatedDate.Date == DateTimeNow.Date).ToList();
                    MainCatsToday = maincatsToday.Select(m => m.CategoryProductID).Distinct().Count();

                    var SubCatTBL = (from s in dbContext.tblSubCategoryProductWithCusts
                                     select s).ToList();

                    SubCategories = SubCatTBL.Select(s => s.ID).Distinct().Count();
                    SubCatsToday = SubCatTBL.Where(s => s.CreatedDate.Date == DateTimeNow.Date).Select(s => s.SubCategoryId).Distinct().Count();

                    var ChildCatTBL = (from sc in dbContext.tblSubCategoryProductWithCusts
                                       join c in dbContext.tblChildCategories on sc.SubCategoryId equals c.SubCategoryId
                                       join i in dbContext.tblItemCategories on c.ID equals i.ChildCategoryID
                                       select new ItemCategory
                                       {
                                           ChildCategoryId = c.ID,
                                           CreatedDate = sc.CreatedDate,
                                           ID = i.ID
                                       }).Distinct().AsNoTracking();

                    ChildCategories = ChildCatTBL.Select(c => c.ChildCategoryId).Distinct().Count();
                    ChildCatsToday = ChildCatTBL.Where(c => DbFunctions.TruncateTime(c.CreatedDate.Value) == DateTimeNow.Date).Select(c => c.ChildCategoryId).Distinct().Count();

                    ItemCategories = ChildCatTBL.Select(c => c.ID).Distinct().Count();
                    ItemCatsToday = ChildCatTBL.Where(c => DbFunctions.TruncateTime(c.CreatedDate.Value) == DateTimeNow.Date).Select(c => c.ID).Distinct().Count();

                    var EnquiryTBL = dbContext.tblselectedDealers;
                    Enquiries = EnquiryTBL.Count();
                    EnquiriesToday = EnquiryTBL.Where(e => DbFunctions.TruncateTime(e.CreatedDate) == DateTimeNow.Date).Count();

                    var json = new
                    {
                        RegisteredCustomers = RegisteredCustomers,
                        ActiveCustomers = ActiveCustomers,
                        InactiveCustomers = InactiveCustomers,
                        BlockedCustomers = BlockedCustomers,
                        CitiesCovered = CitiesCovered,
                        StatesCovered = StatesCovered,
                        MainCategories = MainCategories,
                        SubCategories = SubCategories,
                        ChildCategories = ChildCategories,
                        ItemCategories = ItemCategories,
                        Enquiries = Enquiries,
                        RegisteredToday = RegisteredToday,
                        ActiveToday = ActiveToday,
                        InactiveToday = InactiveToday,
                        CitiesToday = CitiesToday,
                        StatesToday = StatesToday,
                        BlockedToday = BlockedToday,
                        MainCatsToday = MainCatsToday,
                        SubCatsToday = SubCatsToday,
                        ChildCatsToday = ChildCatsToday,
                        ItemCatsToday = ItemCatsToday,
                        EnquiriesToday = EnquiriesToday
                    };

                    return json;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
