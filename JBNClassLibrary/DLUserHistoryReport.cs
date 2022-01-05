using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class UserHistory
    {
        [Required(ErrorMessage = "Please select the user")]
        public int UserID { get; set; }
        public int? CustID { get; set; }
        public string Name { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string UserName { get; set; }
        public string Comments { get; set; }
        public DateTime? CreationDate { get; set; }
        public string ActivityType { get; set; }
        public string ActivityPage { get; set; }
        public string ItemName { get; set; }
        public string ChildCategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string MainCategoryName { get; set; }
    }
    public class UserHistoryCounts
    {
        public int UserID { get; set; }
        public int RolesCount { get; set; }
        public int UsersCount { get; set; }
        public int AppUsersCount { get; set; }
        public int ItemsCount { get; set; }
        public int ChildCategoriesCount { get; set; }
        public int SubCategoriesCount { get; set; }
        public int MainCategoriesCount { get; set; }
        public string ActivityType { get; set; }
        public string ActivityPage { get; set; }
    }
    public class DLUserHistoryReport
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        public UserHistoryCounts GetUserHistoryCounts(UserHistory history)
        {
            UserHistoryCounts historyCounts = new UserHistoryCounts();
            IEnumerable<tblHistory> userHistories = dbContext.tblHistories;

            try
            {
                using(dbContext = new mwbtDealerEntities())
                {
                    if (!string.IsNullOrEmpty(history.FromDate) && !string.IsNullOrEmpty(history.ToDate))
                    {
                        DateTime FromDate = Convert.ToDateTime(history.FromDate);
                        DateTime ToDate = Convert.ToDateTime(history.ToDate);
                        //userHistories = userHistories.Where(h => h.CreationDate.Value.Date >= FromDate.Date && h.CreationDate.Value.Date <= ToDate.Date).ToList();
                        userHistories = userHistories.Where(h => h.CreatedDate.Date >= FromDate.Date && h.CreatedDate.Date <= ToDate.Date).ToList();
                    }

                    historyCounts = (from uh in userHistories
                                     join c in dbContext.tblUsers on uh.UserID equals c.UserID
                                     where c.UserID == history.UserID
                                     select new UserHistoryCounts
                                     {
                                         UserID = history.UserID,
                                         RolesCount = dbContext.tblHistories.Where(h => h.UserID == uh.UserID && h.ActivityPage == "roles").Count(),
                                         UsersCount = dbContext.tblHistories.Where(h => h.UserID == uh.UserID && h.ActivityPage == "users").Count(),
                                         AppUsersCount = dbContext.tblHistories.Where(h => h.UserID == uh.UserID && h.ActivityPage == "appusers").Count(),
                                         ItemsCount = dbContext.tblHistories.Where(h => h.UserID == uh.UserID && h.ActivityPage == "items").Count(),
                                         ChildCategoriesCount = dbContext.tblHistories.Where(h => h.UserID == uh.UserID && h.ActivityPage == "childcategories").Count(),
                                         SubCategoriesCount = dbContext.tblHistories.Where(h => h.UserID == uh.UserID && h.ActivityPage == "subcategories").Count(),
                                         MainCategoriesCount = dbContext.tblHistories.Where(h => h.UserID == uh.UserID && h.ActivityPage == "maincategories").Count(),
                                     }).FirstOrDefault();
                    return historyCounts;
                }
            }
            catch(Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<UserHistory> GetHistoryReport(UserHistory history)
        {
            IEnumerable<tblHistory> userHistories = dbContext.tblHistories;
            List<UserHistory> userHistoriesReport = new List<UserHistory>();

            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    if (history.ActivityPage == "roles")
                    {
                        userHistoriesReport = (from h in userHistories
                                               join u in dbContext.tblUsers on h.UserID equals u.UserID
                                               //join r in dbContext.tblSysRoles on h.ProductID equals r.RoleID
                                               join r in dbContext.tblSysRoles on h.ProductID equals r.ID
                                               where h.UserID == history.UserID && h.ActivityPage == history.ActivityPage
                                               select new UserHistory
                                               {
                                                   UserID = history.UserID,
                                                   Name = h.ProductCategory,
                                                   UserName = u.FullName,
                                                   CreationDate = h.CreatedDate,
                                                   ActivityType = h.ActivityType,
                                                   ActivityPage = h.ActivityPage,
                                                   Comments = h.Comments,
                                               }).ToList();
                    }
                    else if (history.ActivityPage == "webusers")
                    {
                        userHistoriesReport = (from h in userHistories
                                               join u in dbContext.tblUsers on h.UserID equals u.UserID
                                               where h.UserID == history.UserID && h.ActivityPage == history.ActivityPage
                                               select new UserHistory
                                               {
                                                   UserID = history.UserID,
                                                   Name = h.ProductCategory,
                                                   UserName = u.FullName,
                                                   CreationDate = h.CreatedDate,
                                                   ActivityType = h.ActivityType,
                                                   ActivityPage = h.ActivityPage,
                                                   Comments = h.Comments,
                                               }).ToList();
                    }
                    else if (history.ActivityPage == "appusers")
                    {
                        userHistoriesReport = (from h in userHistories
                                               join u in dbContext.tblUsers on h.UserID equals u.UserID
                                               join c in dbContext.tblCustomerDetails on h.CustID equals c.ID
                                               where h.UserID == history.UserID && h.ActivityPage == history.ActivityPage
                                               select new UserHistory
                                               {
                                                   UserID = history.UserID,
                                                   Name = h.ProductCategory,
                                                   UserName = u.FullName,
                                                   CreationDate = h.CreatedDate,
                                                   ActivityType = h.ActivityType,
                                                   ActivityPage = h.ActivityPage,
                                                   Comments = h.Comments,
                                               }).ToList();
                    }
                    else if (history.ActivityPage == "items")
                    {
                        userHistoriesReport = (from h in userHistories
                                               join u in dbContext.tblUsers on h.UserID equals u.UserID
                                               join cc in dbContext.tblChildCategories on h.ProductID equals cc.ID /// cc.ItemId
                                               join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID//Added
                                               join sc in dbContext.tblSubCategories on cc.SubCategoryId equals sc.SubCategoryId
                                               join mc in dbContext.tblCategoryProducts on sc.CategoryProductID equals mc.CategoryProductID
                                               where h.UserID == history.UserID && h.ActivityPage == history.ActivityPage
                                               select new UserHistory
                                               {
                                                   UserID = history.UserID,
                                                   Name = h.ProductCategory,
                                                   UserName = u.FullName,
                                                   CreationDate = h.CreatedDate,
                                                   ActivityType = h.ActivityType,
                                                   ActivityPage = h.ActivityPage,
                                                   ChildCategoryName = cc.ChildCategoryName,
                                                   SubCategoryName = sc.SubCategoryName,
                                                   MainCategoryName = mc.MainCategoryName,
                                                   Comments = h.Comments,
                                               }).ToList();
                    }
                    else if (history.ActivityPage == "childcategories")
                    {
                        userHistoriesReport = (from h in userHistories
                                               join u in dbContext.tblUsers on h.UserID equals u.UserID
                                               join cc in dbContext.tblChildCategories on h.ProductID equals cc.ID
                                               join sc in dbContext.tblSubCategories on cc.SubCategoryId equals sc.SubCategoryId
                                               join mc in dbContext.tblCategoryProducts on sc.CategoryProductID equals mc.CategoryProductID
                                               where h.UserID == history.UserID && h.ActivityPage == history.ActivityPage
                                               select new UserHistory
                                               {
                                                   UserID = history.UserID,
                                                   Name = h.ProductCategory,
                                                   UserName = u.FullName,
                                                   CreationDate = h.CreatedDate,
                                                   ActivityType = h.ActivityType,
                                                   ActivityPage = h.ActivityPage,
                                                   SubCategoryName = sc.SubCategoryName,
                                                   MainCategoryName = mc.MainCategoryName,
                                                   Comments = h.Comments,
                                               }).Distinct().ToList();
                    }
                    else if (history.ActivityPage == "subcategories")
                    {
                        userHistoriesReport = (from h in userHistories
                                               join u in dbContext.tblUsers on h.UserID equals u.UserID
                                               join sc in dbContext.tblSubCategories on h.ProductID equals sc.SubCategoryId
                                               join mc in dbContext.tblCategoryProducts on sc.CategoryProductID equals mc.CategoryProductID
                                               where h.UserID == history.UserID && h.ActivityPage == history.ActivityPage
                                               select new UserHistory
                                               {
                                                   UserID = history.UserID,
                                                   Name = h.ProductCategory,
                                                   UserName = u.FullName,
                                                   CreationDate = h.CreatedDate,
                                                   ActivityType = h.ActivityType,
                                                   ActivityPage = h.ActivityPage,
                                                   Comments = h.Comments,
                                               }).ToList();
                    }
                    else if (history.ActivityPage == "maincategories")
                    {
                        userHistoriesReport = (from h in userHistories
                                               join u in dbContext.tblUsers on h.UserID equals u.UserID
                                               join cp in dbContext.tblCategoryProducts on h.ProductID equals cp.CategoryProductID
                                               where h.UserID == history.UserID && h.ActivityPage == history.ActivityPage
                                               select new UserHistory
                                               {
                                                   UserID = history.UserID,
                                                   Name = h.ProductCategory,
                                                   UserName = u.FullName,
                                                   CreationDate = h.CreatedDate,
                                                   ActivityType = h.ActivityType,
                                                   ActivityPage = h.ActivityPage,
                                                   Comments = h.Comments,
                                               }).ToList();
                    }

                    if (!string.IsNullOrEmpty(history.ActivityPage))
                    {
                        userHistoriesReport = userHistoriesReport.Where(x => x.ActivityType == history.ActivityType).ToList();
                    }

                    return userHistoriesReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
            
        }
        public List<UserHistory> GetActivityPages()
        {
            List<UserHistory> ActivityTypes = new List<UserHistory>();
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    ActivityTypes = (from h in dbContext.tblHistories
                                     select new UserHistory
                                     {
                                         ActivityPage = h.ActivityPage
                                     }).Distinct().ToList();

                    return ActivityTypes;
                }
            }
            catch(Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
    }
}