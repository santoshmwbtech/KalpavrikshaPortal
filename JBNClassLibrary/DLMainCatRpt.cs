using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JBNClassLibrary
{
    public class ConsolidatedReport
    {
        public int TotalCategories { get; set; }
        public int TotalStates { get; set; }
        public int TotalCities { get; set; }
        public int TotalCustomers { get; set; }
        public List<CategoryRpt> categoryRpts { get; set; }
        public bool IsEmail { get; set; }
        public bool IsSMS { get; set; }
        public bool IsWhatsApp { get; set; }
        [Required(ErrorMessage = "Enter SMS body")]
        public string SMSBody { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Enter your message")]
        public string MailBody { get; set; }
        [Required(ErrorMessage = "Enter Mail Subject")]
        public string MailSubject { get; set; }
        public bool IsNotification { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public HttpPostedFileBase[] files { get; set; }
        public int SMSTemplateID { get; set; }
    }
    public class CategoryRpt
    {
        public int StateID { get; set; }
        public int CategoryID { get; set; }
        public int ChildCategories { get; set; }
        public string CategoryName { get; set; }
        public string StateName { get; set; }
        public int CitiesCount { get; set; }
        public int CustomersCount { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int[] StateList { get; set; }
        public int[] CategoryList { get; set; }
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int CustID { get; set; }
        public int TotalCategories { get; set; }
        public int TotalStates { get; set; }
        public int TotalCities { get; set; }
        public int TotalCustomers { get; set; }
        public bool IsChecked { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDir { get; set; }
        public string recordsTotal { get; set; }
        public int pageSize { get; set; }
        public int skip { get; set; }
    }
    public class DLAllCategoryReport
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //Start StateWise Report
        public ConsolidatedReport MainCatReport(CategoryRpt mainCatRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime? FromDate = null, ToDate = null;
                    if (!string.IsNullOrEmpty(mainCatRpt.FromDate) && !string.IsNullOrEmpty(mainCatRpt.ToDate))
                    {
                        //FromDate = Convert.ToDateTime(Convert.ToDateTime(mainCatRpt.FromDate).ToString("yyyy-MM-dd"));
                        //ToDate = Convert.ToDateTime(Convert.ToDateTime(mainCatRpt.ToDate).ToString("yyyy-MM-dd"));
                        FromDate = Convert.ToDateTime(mainCatRpt.FromDate);
                        ToDate = Convert.ToDateTime(mainCatRpt.ToDate);
                    }
                    ConsolidatedReport consolidatedReport = new ConsolidatedReport();
                    List<CategoryRpt> mainCatRpts = new List<CategoryRpt>();
                    mainCatRpts = (from u in dbContext.USP_MainCatRpt(FromDate, ToDate)
                                   where u.StateID != null
                                   select new CategoryRpt
                                   {
                                       StateID = u.StateID.Value,
                                       StateName = u.StateName,
                                       CategoryID = u.CategoryProductID.Value,
                                       CategoryName = u.MainCategoryName,
                                       CustomersCount = u.CustomerCount.Value,
                                       CitiesCount = u.CityCount.Value,
                                       ChildCategories = u.SubCategoryCount.Value
                                   }).ToList();

                    var stateList = mainCatRpt.StateList == null ? Enumerable.Empty<int>() : mainCatRpt.StateList.AsEnumerable();
                    var categoryList = mainCatRpt.CategoryList == null ? Enumerable.Empty<int>() : mainCatRpt.CategoryList.AsEnumerable();

                    var CatList = (from cpc in dbContext.tblCategoryProductWithCusts
                                       //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                   join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                   where c.IsActive == true && c.State != null && c.City != null
                                   && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                   && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                   select new
                                   {
                                       //CustID = c.CustID,
                                       CustID = c.ID,
                                       CreatedDate = c.CreatedDate,
                                       CategoryProductID = cpc.CategoryProductID,
                                       StateID = c.State,
                                       CityID = c.City
                                   }).AsQueryable();

                    if (mainCatRpt.StateList != null && mainCatRpt.StateList.Count() > 0)
                    {
                        mainCatRpts = mainCatRpts.Where(m => mainCatRpt.StateList.Contains(m.StateID)).ToList();
                    }

                    if (mainCatRpt.CategoryList != null && mainCatRpt.CategoryList.Count() > 0)
                    {
                        mainCatRpts = mainCatRpts.Where(m => mainCatRpt.CategoryList.Contains(m.CategoryID)).ToList();
                    }

                    consolidatedReport.TotalCategories = (from c in CatList
                                                          where !categoryList.Any() || categoryList.Contains(c.CategoryProductID.Value)
                                                          && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                          select c.CategoryProductID
                                                          ).Distinct().Count();

                    consolidatedReport.TotalStates = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.CategoryProductID.Value)
                                                          && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                      select c.StateID).Distinct().Count();
                    consolidatedReport.TotalCities = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.CategoryProductID.Value)
                                                          && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                      select c.CityID).Distinct().Count();
                    consolidatedReport.TotalCustomers = (from c in CatList
                                                         where !categoryList.Any() || categoryList.Contains(c.CategoryProductID.Value)
                                                          && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                         select c.CustID).Distinct().Count();

                    consolidatedReport.categoryRpts = mainCatRpts;

                    return consolidatedReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public ConsolidatedReport SubCatReport(CategoryRpt subCatRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime? FromDate = null, ToDate = null;
                    if (!string.IsNullOrEmpty(subCatRpt.FromDate) && !string.IsNullOrEmpty(subCatRpt.ToDate))
                    {
                        FromDate = Convert.ToDateTime(subCatRpt.FromDate);
                        ToDate = Convert.ToDateTime(subCatRpt.ToDate);
                    }

                    ConsolidatedReport consolidatedReport = new ConsolidatedReport();
                    List<CategoryRpt> subCatRpts = new List<CategoryRpt>();
                    subCatRpts = (from sub in dbContext.USP_SubCatRpt(FromDate, ToDate)
                                  where sub.StateID != null
                                  select new CategoryRpt
                                  {
                                      StateID = sub.StateID.Value,
                                      StateName = sub.StateName,
                                      CategoryID = sub.subcategoryid.Value,
                                      CategoryName = sub.subcategoryname,
                                      CustomersCount = sub.CustomerCount.Value,
                                      CitiesCount = sub.CityCount.Value,
                                      ChildCategories = sub.ChildCategoryCount.Value
                                  }).ToList();

                    if (subCatRpt.StateList != null && subCatRpt.StateList.Count() > 0)
                    {
                        subCatRpts = subCatRpts.Where(m => subCatRpt.StateList.Contains(m.StateID)).ToList();
                    }

                    if (subCatRpt.CategoryList != null && subCatRpt.CategoryList.Count() > 0)
                    {
                        subCatRpts = subCatRpts.Where(m => subCatRpt.CategoryList.Contains(m.CategoryID)).ToList();
                    }

                    var CatList = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                       //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                   join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                   where c.IsActive == true && c.State != null
                                   select new
                                   {
                                       //CustID = c.CustID,
                                       CustID = c.ID,
                                       CreatedDate = c.CreatedDate,
                                       SubCategoryID = cpc.SubCategoryId,
                                       StateID = c.State,
                                       CityID = c.City
                                   }).AsQueryable();

                    var stateList = subCatRpt.StateList == null ? Enumerable.Empty<int>() : subCatRpt.StateList.AsEnumerable();
                    var categoryList = subCatRpt.CategoryList == null ? Enumerable.Empty<int>() : subCatRpt.CategoryList.AsEnumerable();

                    consolidatedReport.TotalCategories = (from c in CatList
                                                          where !categoryList.Any() || categoryList.Contains(c.SubCategoryID.Value)
                                                          && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                          && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                          && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                          select c.SubCategoryID
                                                          ).Distinct().Count();

                    consolidatedReport.TotalStates = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.SubCategoryID.Value)
                                                      && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                      && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                      && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.StateID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCities = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.SubCategoryID.Value)
                                                      && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                      && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                      && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.CityID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCustomers = (from c in CatList
                                                         where !categoryList.Any() || categoryList.Contains(c.SubCategoryID.Value)
                                                         && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                         && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                         && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                         select c.CustID
                                                          ).Distinct().Count();

                    consolidatedReport.categoryRpts = subCatRpts;

                    return consolidatedReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public ConsolidatedReport ChildCatReport(CategoryRpt childCatRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime? FromDate = null, ToDate = null;
                    if (!string.IsNullOrEmpty(childCatRpt.FromDate) && !string.IsNullOrEmpty(childCatRpt.ToDate))
                    {
                        FromDate = Convert.ToDateTime(childCatRpt.FromDate);
                        ToDate = Convert.ToDateTime(childCatRpt.ToDate);
                    }

                    ConsolidatedReport consolidatedReport = new ConsolidatedReport();
                    List<CategoryRpt> childCatRpts = new List<CategoryRpt>();
                    childCatRpts = (from cc in dbContext.USP_ChildCatRpt(FromDate, ToDate)
                                    where cc.StateID != null
                                    select new CategoryRpt
                                    {
                                        StateID = cc.StateID.Value,
                                        StateName = cc.StateName,
                                        CategoryID = cc.childcategoryid,
                                        CategoryName = cc.childcategoryname,
                                        CustomersCount = cc.CustomerCount.Value,
                                        CitiesCount = cc.CityCount.Value,
                                        ChildCategories = cc.ItemCount.Value
                                    }).Distinct().ToList();

                    if (childCatRpt.StateList != null && childCatRpt.StateList.Count() > 0)
                    {
                        childCatRpts = childCatRpts.Where(m => childCatRpt.StateList.Contains(m.StateID)).ToList();
                    }

                    if (childCatRpt.CategoryList != null && childCatRpt.CategoryList.Count() > 0)
                    {
                        childCatRpts = childCatRpts.Where(m => childCatRpt.CategoryList.Contains(m.CategoryID)).ToList();
                    }

                    var CatList = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                       //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                   join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                   join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                   where c.IsActive == true && c.State != null && c.City != null
                                   select new
                                   {
                                       //CustID = c.CustID,
                                       CustID = c.ID,
                                       CreatedDate = c.CreatedDate,
                                       //ChildCategoryID = cc.ChildCategoryId,
                                       ChildCategoryID = cc.ID,
                                       StateID = c.State,
                                       CityID = c.City
                                   }).AsQueryable();

                    var stateList = childCatRpt.StateList == null ? Enumerable.Empty<int>() : childCatRpt.StateList.AsEnumerable();
                    var categoryList = childCatRpt.CategoryList == null ? Enumerable.Empty<int>() : childCatRpt.CategoryList.AsEnumerable();

                    consolidatedReport.TotalCategories = (from c in CatList
                                                          where !categoryList.Any() || categoryList.Contains(c.ChildCategoryID)
                                                         && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                         && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                         && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                          select c.ChildCategoryID
                                                          ).Distinct().Count();

                    consolidatedReport.TotalStates = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.ChildCategoryID)
                                                     && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                     && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                     && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.StateID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCities = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.ChildCategoryID)
                                                     && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                     && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                     && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.CityID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCustomers = (from c in CatList
                                                         where !categoryList.Any() || categoryList.Contains(c.ChildCategoryID)
                                                        && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                        && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                        && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                         select c.CustID
                                                          ).Distinct().Count();

                    consolidatedReport.categoryRpts = childCatRpts;

                    return consolidatedReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public ConsolidatedReport ItemCatReport(CategoryRpt itemCatRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime? FromDate = null, ToDate = null;
                    if (!string.IsNullOrEmpty(itemCatRpt.FromDate) && !string.IsNullOrEmpty(itemCatRpt.ToDate))
                    {
                        FromDate = Convert.ToDateTime(itemCatRpt.FromDate);
                        ToDate = Convert.ToDateTime(itemCatRpt.ToDate);
                    }
                    ConsolidatedReport consolidatedReport = new ConsolidatedReport();
                    List<CategoryRpt> itemCatRpts = new List<CategoryRpt>();
                    var res = dbContext.USP_ItemCatRpt(FromDate, ToDate);

                    itemCatRpts = (from cc in res
                                   where cc.StateID != null
                                   select new CategoryRpt
                                   {
                                       StateID = cc.StateID.Value,
                                       StateName = cc.StateName,
                                       CategoryID = cc.ItemID,
                                       CategoryName = cc.itemname,
                                       CustomersCount = cc.CustomerCount.Value,
                                       CitiesCount = cc.CityCount.Value,
                                   }).Distinct().ToList();

                    if (itemCatRpt.StateList != null && itemCatRpt.StateList.Count() > 0)
                    {
                        itemCatRpts = itemCatRpts.Where(m => itemCatRpt.StateList.Contains(m.StateID)).ToList();
                    }

                    if (itemCatRpt.CategoryList != null && itemCatRpt.CategoryList.Count() > 0)
                    {
                        itemCatRpts = itemCatRpts.Where(m => itemCatRpt.CategoryList.Contains(m.CategoryID)).ToList();
                    }

                    var CatList = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                       //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                   join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                   join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                   join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID
                                   where c.IsActive == true && c.State != null && c.City != null
                                   select new
                                   {
                                       //CustID = c.CustID,
                                       CustID = c.ID,
                                       CreatedDate = c.CreatedDate,
                                       //ItemID = cc.ItemId,
                                       ItemID = ic.ID,
                                       StateID = c.State,
                                       CityID = c.City
                                   }).AsQueryable();

                    var stateList = itemCatRpt.StateList == null ? Enumerable.Empty<int>() : itemCatRpt.StateList.AsEnumerable();
                    var categoryList = itemCatRpt.CategoryList == null ? Enumerable.Empty<int>() : itemCatRpt.CategoryList.AsEnumerable();

                    consolidatedReport.TotalCategories = (from c in CatList
                                                          where !categoryList.Any() || categoryList.Contains(c.ItemID)
                                                        && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                        && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                        && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                          select c.ItemID
                                                          ).Distinct().Count();

                    consolidatedReport.TotalStates = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.ItemID)
                                                    && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                    && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                    && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.StateID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCities = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.ItemID)
                                                        && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                        && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                        && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.CityID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCustomers = (from c in CatList
                                                         where !categoryList.Any() || categoryList.Contains(c.ItemID)
                                                        && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                        && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                        && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                         select c.CustID
                                                          ).Distinct().Count();

                    consolidatedReport.categoryRpts = itemCatRpts;

                    return consolidatedReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        //End StateWise Report

        //Get List of Categories
        public List<MainCategory> GetMainCategoryList()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<MainCategory> mainCatList = new List<MainCategory>();
                    mainCatList = (from cc in dbContext.tblCategoryProducts
                                   join cp in dbContext.tblCategoryProductWithCusts on cc.ID equals cp.CategoryProductID //cc.CategoryProductID equals cp.CategoryProductID
                                   select new MainCategory
                                   {
                                       ID = cc.ID,
                                       CategoryProductID = cc.ID, //CategoryProductID
                                       MainCategoryName = cc.MainCategoryName
                                   }).Distinct().ToList();

                    return mainCatList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<SubCat> GetSubCategoryList()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<SubCat> subCatList = new List<SubCat>();
                    subCatList = (from cc in dbContext.tblSubCategories
                                  join cp in dbContext.tblSubCategoryProductWithCusts on cc.ID equals cp.SubCategoryId  //cc.SubCategoryId equals cp.SubCategoryId
                                  select new SubCat
                                  {
                                      //SubCategoryID = cc.SubCategoryId,
                                      ID = cc.ID,
                                      SubCategoryName = cc.SubCategoryName
                                  }).Distinct().ToList();

                    return subCatList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<childcategory> GetChildCategoryList()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<childcategory> childList = new List<childcategory>();
                    childList = (from cc in dbContext.tblChildCategories
                                 join cp in dbContext.tblSubCategoryProductWithCusts on cc.SubCategoryId equals cp.SubCategoryId
                                 select new childcategory
                                 {
                                     //ChildCategoryId = cc.ChildCategoryId,
                                     ID = cc.ID,
                                     ChildCategoryName = cc.ChildCategoryName
                                 }).Distinct().ToList();

                    return childList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<ItemCategory> GetItemCategoryList()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<ItemCategory> itemList = new List<ItemCategory>();
                    itemList = (from cc in dbContext.tblChildCategories
                                join scp in dbContext.tblSubCategoryProductWithCusts on cc.SubCategoryId equals scp.SubCategoryId
                                join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID
                                select new ItemCategory
                                {
                                    ID = ic.ID,
                                    ItemName = ic.ItemName
                                }).Distinct().ToList();

                    return itemList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        //Get List of Categories End
        /**/

        //Start CityWise Reports
        public ConsolidatedReport MainCatCityWiseReport(CategoryRpt mainCatRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime? FromDate = null, ToDate = null;
                    if (!string.IsNullOrEmpty(mainCatRpt.FromDate) && !string.IsNullOrEmpty(mainCatRpt.ToDate))
                    {
                        FromDate = Convert.ToDateTime(mainCatRpt.FromDate);
                        ToDate = Convert.ToDateTime(mainCatRpt.ToDate);
                    }
                    ConsolidatedReport consolidatedReport = new ConsolidatedReport();
                    List<CategoryRpt> mainCatRpts = new List<CategoryRpt>();
                    mainCatRpts = (from u in dbContext.USP_MainCatCityWiseRpt(mainCatRpt.StateID, FromDate, ToDate)
                                   where u.City != null
                                   select new CategoryRpt
                                   {
                                       CityID = u.City.Value,
                                       CityName = u.VillageLocalityname,
                                       StateID = u.StateID.Value,
                                       StateName = u.StateName,
                                       CategoryID = u.CategoryProductID.Value,
                                       CategoryName = u.MainCategoryName,
                                       CustomersCount = u.CustomerCount.Value,
                                   }).ToList();

                    if (mainCatRpt.StateList != null && mainCatRpt.StateList.Count() > 0)
                    {
                        mainCatRpts = mainCatRpts.Where(m => mainCatRpt.StateList.Contains(m.StateID)).ToList();
                    }

                    if (mainCatRpt.CategoryList != null && mainCatRpt.CategoryList.Count() > 0)
                    {
                        mainCatRpts = mainCatRpts.Where(m => mainCatRpt.CategoryList.Contains(m.CategoryID)).ToList();
                    }

                    var CatList = (from cpc in dbContext.tblCategoryProductWithCusts
                                       //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                   join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                   where c.IsActive == true && c.State != null && c.City != null
                                   select new
                                   {
                                       //CustID = c.CustID,
                                       CustID = c.ID,
                                       CreatedDate = c.CreatedDate,
                                       CategoryProductID = cpc.CategoryProductID,
                                       StateID = c.State,
                                       CityID = c.City
                                   }).AsQueryable();

                    var stateList = mainCatRpt.StateList == null ? Enumerable.Empty<int>() : mainCatRpt.StateList.AsEnumerable();
                    var categoryList = mainCatRpt.CategoryList == null ? Enumerable.Empty<int>() : mainCatRpt.CategoryList.AsEnumerable();

                    consolidatedReport.TotalCategories = (from c in CatList
                                                          where !categoryList.Any() || categoryList.Contains(c.CategoryProductID.Value)
                                                          && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                         && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                         && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                          select c.CategoryProductID
                                                          ).Distinct().Count();

                    consolidatedReport.TotalStates = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.CategoryProductID.Value)
                                                      && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                     && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                     && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.StateID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCities = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.CategoryProductID.Value)
                                                      && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                     && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                     && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.CityID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCustomers = (from c in CatList
                                                         where !categoryList.Any() || categoryList.Contains(c.CategoryProductID.Value)
                                                         && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                        && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                        && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                         select c.CustID
                                                          ).Distinct().Count();
                    consolidatedReport.categoryRpts = mainCatRpts;

                    return consolidatedReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public ConsolidatedReport SubCatCityWiseReport(CategoryRpt subCatRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime? FromDate = null, ToDate = null;
                    if (!string.IsNullOrEmpty(subCatRpt.FromDate) && !string.IsNullOrEmpty(subCatRpt.ToDate))
                    {
                        FromDate = Convert.ToDateTime(subCatRpt.FromDate);
                        ToDate = Convert.ToDateTime(subCatRpt.ToDate);
                    }
                    ConsolidatedReport consolidatedReport = new ConsolidatedReport();
                    List<CategoryRpt> subCatRpts = new List<CategoryRpt>();
                    subCatRpts = (from u in dbContext.USP_SubCatCityWiseRpt(subCatRpt.StateID, FromDate, ToDate)
                                  where u.City != null
                                  select new CategoryRpt
                                  {
                                      CityID = u.City.Value,
                                      CityName = u.VillageLocalityname,
                                      StateID = u.StateID.Value,
                                      StateName = u.StateName,
                                      CategoryID = u.subcategoryid.Value,
                                      CategoryName = u.SubCategoryName,
                                      CustomersCount = u.CustomerCount.Value,
                                  }).ToList();

                    if (subCatRpt.StateList != null && subCatRpt.StateList.Count() > 0)
                    {
                        subCatRpts = subCatRpts.Where(m => subCatRpt.StateList.Contains(m.StateID)).ToList();
                    }

                    if (subCatRpt.CategoryList != null && subCatRpt.CategoryList.Count() > 0)
                    {
                        subCatRpts = subCatRpts.Where(m => subCatRpt.CategoryList.Contains(m.CategoryID)).ToList();
                    }

                    var CatList = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                       //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                   join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                   where c.IsActive == true && c.State != null
                                   select new
                                   {
                                       //CustID = c.CustID,
                                       CustID = c.ID,
                                       CreatedDate = c.CreatedDate,
                                       SubCategoryID = cpc.SubCategoryId,
                                       StateID = c.State,
                                       CityID = c.City
                                   }).AsQueryable();

                    var stateList = subCatRpt.StateList == null ? Enumerable.Empty<int>() : subCatRpt.StateList.AsEnumerable();
                    var categoryList = subCatRpt.CategoryList == null ? Enumerable.Empty<int>() : subCatRpt.CategoryList.AsEnumerable();

                    consolidatedReport.TotalCategories = (from c in CatList
                                                          where !categoryList.Any() || categoryList.Contains(c.SubCategoryID.Value)
                                                          && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                          && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                          && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                          select c.SubCategoryID
                                                          ).Distinct().Count();

                    consolidatedReport.TotalStates = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.SubCategoryID.Value)
                                                      && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                      && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                      && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.StateID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCities = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.SubCategoryID.Value)
                                                      && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                      && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                      && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.CityID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCustomers = (from c in CatList
                                                         where !categoryList.Any() || categoryList.Contains(c.SubCategoryID.Value)
                                                         && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                         && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                         && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                         select c.CustID
                                                          ).Distinct().Count();

                    consolidatedReport.categoryRpts = subCatRpts;

                    return consolidatedReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public ConsolidatedReport ChildCatCityWiseReport(CategoryRpt childCatRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime? FromDate = null, ToDate = null;
                    if (!string.IsNullOrEmpty(childCatRpt.FromDate) && !string.IsNullOrEmpty(childCatRpt.ToDate))
                    {
                        FromDate = Convert.ToDateTime(childCatRpt.FromDate);
                        ToDate = Convert.ToDateTime(childCatRpt.ToDate);
                    }
                    ConsolidatedReport consolidatedReport = new ConsolidatedReport();
                    List<CategoryRpt> childCatRpts = new List<CategoryRpt>();
                    childCatRpts = (from u in dbContext.USP_ChildCatCityWiseRpt(childCatRpt.StateID, FromDate, ToDate)
                                    where u.City != null
                                    select new CategoryRpt
                                    {
                                        CityID = u.City.Value,
                                        CityName = u.VillageLocalityname,
                                        StateID = u.StateID.Value,
                                        StateName = u.StateName,
                                        CategoryID = u.childcategoryid,
                                        CategoryName = u.childcategoryname,
                                        CustomersCount = u.CustomerCount.Value,
                                    }).ToList();

                    if (childCatRpt.StateList != null && childCatRpt.StateList.Count() > 0)
                    {
                        childCatRpts = childCatRpts.Where(m => childCatRpt.StateList.Contains(m.StateID)).ToList();
                    }

                    if (childCatRpt.CategoryList != null && childCatRpt.CategoryList.Count() > 0)
                    {
                        childCatRpts = childCatRpts.Where(m => childCatRpt.CategoryList.Contains(m.CategoryID)).ToList();
                    }

                    var CatList = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                       //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                   join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                   join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                   where c.IsActive == true && c.State != null && c.City != null
                                   select new
                                   {
                                       //CustID = c.CustID,
                                       CustID = c.ID,
                                       CreatedDate = c.CreatedDate,
                                       //ChildCategoryID = cc.ChildCategoryId,
                                       ChildCategoryID = cc.ID,
                                       StateID = c.State,
                                       CityID = c.City
                                   }).AsQueryable();

                    var stateList = childCatRpt.StateList == null ? Enumerable.Empty<int>() : childCatRpt.StateList.AsEnumerable();
                    var categoryList = childCatRpt.CategoryList == null ? Enumerable.Empty<int>() : childCatRpt.CategoryList.AsEnumerable();

                    consolidatedReport.TotalCategories = (from c in CatList
                                                          where !categoryList.Any() || categoryList.Contains(c.ChildCategoryID)
                                                         && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                         && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                         && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                          select c.ChildCategoryID
                                                          ).Distinct().Count();

                    consolidatedReport.TotalStates = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.ChildCategoryID)
                                                     && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                     && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                     && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.StateID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCities = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.ChildCategoryID)
                                                     && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                     && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                     && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.CityID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCustomers = (from c in CatList
                                                         where !categoryList.Any() || categoryList.Contains(c.ChildCategoryID)
                                                        && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                        && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                        && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                         select c.CustID
                                                          ).Distinct().Count();
                    consolidatedReport.categoryRpts = childCatRpts;

                    return consolidatedReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public ConsolidatedReport ItemCatCityWiseReport(CategoryRpt itemCatRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime? FromDate = null, ToDate = null;
                    if (!string.IsNullOrEmpty(itemCatRpt.FromDate) && !string.IsNullOrEmpty(itemCatRpt.ToDate))
                    {
                        FromDate = Convert.ToDateTime(itemCatRpt.FromDate);
                        ToDate = Convert.ToDateTime(itemCatRpt.ToDate);
                    }
                    var categoryLists = string.Empty;
                    if(itemCatRpt.CategoryList != null && itemCatRpt.CategoryList.Count() > 0)
                    {
                        categoryLists = string.Join(",", itemCatRpt.CategoryList);
                    }

                    ConsolidatedReport consolidatedReport = new ConsolidatedReport();
                    List<CategoryRpt> itemCatRpts = new List<CategoryRpt>();
                    itemCatRpts = (from u in dbContext.USP_ItemCatCityWiseRpt(itemCatRpt.StateID, FromDate, ToDate, categoryLists)
                                   where u.City != null
                                   select new CategoryRpt
                                   {
                                       CityID = u.City.Value,
                                       CityName = u.VillageLocalityname,
                                       StateID = u.StateID.Value,
                                       StateName = u.StateName,
                                       CategoryID = u.ItemID,
                                       CategoryName = u.ItemName,
                                       CustomersCount = u.CustomerCount.Value,
                                   }).ToList();

                    //if (itemCatRpt.StateList != null && itemCatRpt.StateList.Count() > 0)
                    //{
                    //    itemCatRpts = itemCatRpts.Where(m => itemCatRpt.StateList.Contains(m.StateID)).ToList();
                    //}

                    //if (itemCatRpt.CategoryList != null && itemCatRpt.CategoryList.Count() > 0)
                    //{
                    //    itemCatRpts = itemCatRpts.Where(m => itemCatRpt.CategoryList.Contains(m.CategoryID)).ToList();
                    //}

                    var CatList = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                       //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                   join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                   join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                   join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID
                                   where c.IsActive == true && c.State != null && c.City != null
                                   select new
                                   {
                                       //CustID = c.CustID,
                                       CustID = c.ID,
                                       CreatedDate = c.CreatedDate,
                                       //ItemID = cc.ItemId,
                                       ItemID = ic.ID,
                                       StateID = c.State,
                                       CityID = c.City
                                   }).AsQueryable();

                    var stateList = itemCatRpt.StateList == null ? Enumerable.Empty<int>() : itemCatRpt.StateList.AsEnumerable();
                    var categoryList = itemCatRpt.CategoryList == null ? Enumerable.Empty<int>() : itemCatRpt.CategoryList.AsEnumerable();

                    consolidatedReport.TotalCategories = (from c in CatList
                                                          where !categoryList.Any() || categoryList.Contains(c.ItemID)
                                                        && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                        && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                        && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                          select c.ItemID
                                                          ).Distinct().Count();

                    consolidatedReport.TotalStates = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.ItemID)
                                                    && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                    && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                    && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.StateID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCities = (from c in CatList
                                                      where !categoryList.Any() || categoryList.Contains(c.ItemID)
                                                        && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                        && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                        && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                      select c.CityID
                                                          ).Distinct().Count();
                    consolidatedReport.TotalCustomers = (from c in CatList
                                                         where !categoryList.Any() || categoryList.Contains(c.ItemID)
                                                        && !stateList.Any() || stateList.Contains(c.StateID.Value)
                                                        && (FromDate == null || DbFunctions.TruncateTime(c.CreatedDate) >= FromDate)
                                                        && (ToDate == null || DbFunctions.TruncateTime(c.CreatedDate) <= ToDate)
                                                         select c.CustID
                                                          ).Distinct().Count();

                    consolidatedReport.categoryRpts = itemCatRpts;

                    return consolidatedReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //End CityWise Reports

        //Promotion for all categories
        public string Promotion(ConsolidatedReport consolidatedReport, int UserID, int CategoryType, List<Attachment> MailAttachments, string ImageURL)
        {
            try
            {
                string Result = string.Empty;
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                JBNDBClass jBNDBClass = new JBNDBClass();
                string AppName = ConfigurationManager.AppSettings["AppName"].ToString();
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (consolidatedReport.IsEmail == true)
                    {
                        #region Email 
                        string Bcc = string.Empty;
                        List<CustomerDetails> bccList = new List<CustomerDetails>();

                        foreach (var item in consolidatedReport.categoryRpts)
                        {
                            //Main Category
                            if (item.IsChecked == true)
                            {
                                if (CategoryType == 1)
                                {
                                    List<CustomerDetails> customer = (from cpc in dbContext.tblCategoryProductWithCusts
                                                                          //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                      where c.IsActive == true
                                                                      && cpc.CategoryProductID == item.CategoryID
                                                                      && c.State == item.StateID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }
                                                                 ).Distinct().ToList();
                                    foreach (var custItem in customer)
                                    {
                                        if (!string.IsNullOrEmpty(custItem.EmailID))
                                        {
                                            bccList.Add(custItem);
                                        }
                                    }
                                }
                                //Sub Category
                                else if (CategoryType == 2)
                                {

                                    List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                          //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                      where c.IsActive == true
                                                                      && cpc.SubCategoryId == item.CategoryID
                                                                      && c.State == item.StateID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }
                                                                     ).Distinct().ToList();
                                    foreach (var custItem in customer)
                                    {
                                        if (!string.IsNullOrEmpty(custItem.EmailID))
                                        {
                                            bccList.Add(custItem);
                                        }
                                    }
                                }
                                //Child Category
                                else if (CategoryType == 3)
                                {
                                    List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                      join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                      //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                      join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID

                                                                      where c.IsActive == true
                                                                      //&& cc.ChildCategoryId == item.CategoryID
                                                                      && cc.ID == item.CategoryID
                                                                      && c.State == item.StateID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }
                                                                     ).Distinct().ToList();
                                    foreach (var custItem in customer)
                                    {
                                        if (!string.IsNullOrEmpty(custItem.EmailID))
                                        {
                                            bccList.Add(custItem);
                                        }
                                    }
                                }
                                //Item Category
                                else if (CategoryType == 4)
                                {
                                    List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                      join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                      join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID
                                                                      //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID

                                                                      where c.IsActive == true
                                                                      //&& cc.ItemId == item.CategoryID
                                                                      && ic.ID == item.CategoryID
                                                                      && c.State == item.StateID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }
                                                                     ).Distinct().ToList();
                                    foreach (var custItem in customer)
                                    {
                                        if (!string.IsNullOrEmpty(custItem.EmailID))
                                        {
                                            bccList.Add(custItem);
                                        }
                                    }
                                }
                            }
                        }

                        string ToEmailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                        string FromMailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                        string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
                        string MailServerHost = ConfigurationManager.AppSettings["MailServerHost"].ToString();
                        string SendingPort = ConfigurationManager.AppSettings["SendingPort"].ToString();
                        //string APKPath = ConfigurationManager.AppSettings["APKPath"].ToString();
                        string MailSubject = consolidatedReport.MailSubject;

                        Helper.SendMail(ToEmailID, FromMailID, consolidatedReport.MailBody, MailSubject, MailServerHost, MailPassword, SendingPort, bccList, MailAttachments);
                        Result = "Email Sent Successfully!!";
                        #endregion
                    }
                    else if (consolidatedReport.IsSMS == true)
                    {
                        #region SMS
                        foreach (var item in consolidatedReport.categoryRpts)
                        {
                            //Main Category
                            if (CategoryType == 1)
                            {
                                if (item.IsChecked == true)
                                {
                                    List<CustomerDetails> customer = (from cpc in dbContext.tblCategoryProductWithCusts
                                                                          //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                      where c.IsActive == true
                                                                      && cpc.CategoryProductID == item.CategoryID
                                                                      && c.State == item.StateID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }
                                                                 ).Distinct().ToList();

                                    string MobileNumbers = string.Join(",", customer.Select(c => c.MobileNumber));

                                    string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                                    string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                                    string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                                    Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, consolidatedReport.SMSBody, SenderID);
                                }
                            }
                            else if (CategoryType == 2)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                      //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  where c.IsActive == true
                                                                  && cpc.SubCategoryId == item.CategoryID
                                                                  && c.State == item.StateID
                                                                  select new CustomerDetails
                                                                  {
                                                                      // CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                  }).Distinct().ToList();

                                string MobileNumbers = string.Join(",", customer.Select(c => c.MobileNumber));

                                string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                                string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                                string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                                Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, consolidatedReport.SMSBody, SenderID);
                            }
                            else if (CategoryType == 3)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                  join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                  //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  join ic in dbContext.tblItemCategories on cc.ID equals ic.ID
                                                                  where c.IsActive == true
                                                                  //&& cc.ChildCategoryId == item.CategoryID
                                                                  && cc.ID == item.CategoryID
                                                                  && c.State == item.StateID
                                                                  select new CustomerDetails
                                                                  {
                                                                      //CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                  }
                                                                 ).Distinct().ToList();
                                string MobileNumbers = string.Join(",", customer.Select(c => c.MobileNumber));

                                string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                                string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                                string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                                Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, consolidatedReport.SMSBody, SenderID);
                            }
                            else if (CategoryType == 4)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                  join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                  //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID

                                                                  where c.IsActive == true
                                                                  //&& cc.ItemId == item.CategoryID
                                                                  && cc.ID == item.CategoryID
                                                                  && c.State == item.StateID
                                                                  select new CustomerDetails
                                                                  {
                                                                      //CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                  }
                                                                 ).Distinct().ToList();

                                string MobileNumbers = string.Join(",", customer.Select(c => c.MobileNumber));

                                string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                                string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                                string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                                Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, consolidatedReport.SMSBody, SenderID);
                            }
                        }
                        #endregion
                    }
                    else if (consolidatedReport.IsNotification == true)
                    {
                        foreach (var item in consolidatedReport.categoryRpts)
                        {
                            //Main Category
                            if (CategoryType == 1)
                            {
                                if (item.IsChecked == true)
                                {
                                    List<CustomerDetails> customer = (from cpc in dbContext.tblCategoryProductWithCusts
                                                                          //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                      where c.IsActive == true
                                                                      && cpc.CategoryProductID == item.CategoryID
                                                                      && c.State == item.StateID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                          DeviceID = c.DeviceID
                                                                      }
                                                                 ).Distinct().ToList();

                                    string[] Registration_Ids = customer.Select(c => c.DeviceID).ToArray();
                                    int[] Cust_Ids = customer.Select(c => c.CustID).ToArray();
                                    Notification notification = new Notification { Title = consolidatedReport.Title, Body = consolidatedReport.Body, NotificationDate = DateTimeNow, Image = ImageURL };
                                    Helper.SendNotificationMultiple(Registration_Ids, notification);
                                    PushNotifications pushNotifications = new PushNotifications()
                                    {
                                        Title = consolidatedReport.Title,
                                        NotificationDate = DateTimeNow,
                                        CategoryName = string.Empty,
                                        ImageURL = ImageURL,
                                        PushNotification = consolidatedReport.Body,
                                    };
                                    jBNDBClass.SavePushNotificationsList(Cust_Ids, pushNotifications, UserID);
                                }
                            }
                            else if (CategoryType == 2)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                      //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  where c.IsActive == true
                                                                  && cpc.SubCategoryId == item.CategoryID
                                                                  && c.State == item.StateID
                                                                  select new CustomerDetails
                                                                  {
                                                                      //CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                      DeviceID = c.DeviceID
                                                                  }).Distinct().ToList();

                                string[] Registration_Ids = customer.Select(c => c.DeviceID).ToArray();
                                int[] Cust_Ids = customer.Select(c => c.CustID).ToArray();
                                Notification notification = new Notification { Title = consolidatedReport.Title, Body = consolidatedReport.Body, NotificationDate = DateTimeNow, Image = ImageURL };
                                Helper.SendNotificationMultiple(Registration_Ids, notification);
                                PushNotifications pushNotifications = new PushNotifications()
                                {
                                    Title = consolidatedReport.Title,
                                    NotificationDate = DateTimeNow,
                                    CategoryName = string.Empty,
                                    ImageURL = ImageURL,
                                    PushNotification = consolidatedReport.Body,
                                };
                                jBNDBClass.SavePushNotificationsList(Cust_Ids, pushNotifications, UserID);
                            }
                            else if (CategoryType == 3)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                  join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                  //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID

                                                                  where c.IsActive == true
                                                                  //&& cc.ChildCategoryId == item.CategoryID
                                                                  && ic.ID == item.CategoryID
                                                                  && c.State == item.StateID
                                                                  select new CustomerDetails
                                                                  {
                                                                      //CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                      DeviceID = c.DeviceID
                                                                  }
                                                                 ).Distinct().ToList();
                                string[] Registration_Ids = customer.Select(c => c.DeviceID).ToArray();
                                int[] Cust_Ids = customer.Select(c => c.CustID).ToArray();
                                Notification notification = new Notification { Title = consolidatedReport.Title, Body = consolidatedReport.Body, NotificationDate = DateTimeNow, Image = ImageURL };
                                Helper.SendNotificationMultiple(Registration_Ids, notification);
                                PushNotifications pushNotifications = new PushNotifications()
                                {
                                    Title = consolidatedReport.Title,
                                    NotificationDate = DateTimeNow,
                                    CategoryName = string.Empty,
                                    ImageURL = ImageURL,
                                    PushNotification = consolidatedReport.Body,
                                };
                                jBNDBClass.SavePushNotificationsList(Cust_Ids, pushNotifications, UserID);
                            }
                            else if (CategoryType == 4)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                  join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                  //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID


                                                                  where c.IsActive == true
                                                                  //&& cc.ItemId == item.CategoryID
                                                                   && ic.ID == item.CategoryID
                                                                  && c.State == item.StateID
                                                                  select new CustomerDetails
                                                                  {
                                                                      //CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                      DeviceID = c.DeviceID
                                                                  }
                                                                 ).Distinct().ToList();

                                string[] Registration_Ids = customer.Select(c => c.DeviceID).ToArray();
                                int[] Cust_Ids = customer.Select(c => c.CustID).ToArray();
                                Notification notification = new Notification { Title = consolidatedReport.Title, Body = consolidatedReport.Body, NotificationDate = DateTimeNow, Image = ImageURL };
                                Helper.SendNotificationMultiple(Registration_Ids, notification);
                                PushNotifications pushNotifications = new PushNotifications()
                                {
                                    Title = consolidatedReport.Title,
                                    NotificationDate = DateTimeNow,
                                    CategoryName = string.Empty,
                                    ImageURL = ImageURL,
                                    PushNotification = consolidatedReport.Body,
                                };
                                jBNDBClass.SavePushNotificationsList(Cust_Ids, pushNotifications, UserID);
                            }
                        }
                        Result = "Success";
                    }
                }
                return Result;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return "Error!! Please contact administrator";
            }
        }

        public string CityWisePromotion(ConsolidatedReport consolidatedReport, int UserID, int CategoryType, List<Attachment> MailAttachments, string ImageURL)
        {
            try
            {
                string Result = string.Empty;
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                JBNDBClass jBNDBClass = new JBNDBClass();
                string AppName = ConfigurationManager.AppSettings["AppName"].ToString();
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (consolidatedReport.IsEmail == true)
                    {
                        #region Email 
                        string Bcc = string.Empty;
                        List<CustomerDetails> bccList = new List<CustomerDetails>();

                        foreach (var item in consolidatedReport.categoryRpts)
                        {
                            //Main Category
                            if (item.IsChecked == true)
                            {

                                if (CategoryType == 1)
                                {
                                    List<CustomerDetails> customer = (from cpc in dbContext.tblCategoryProductWithCusts
                                                                          //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                      where c.IsActive == true
                                                                      && cpc.CategoryProductID == item.CategoryID
                                                                      && c.City == item.CityID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }
                                                                 ).Distinct().ToList();
                                    foreach (var custItem in customer)
                                    {
                                        if (!string.IsNullOrEmpty(custItem.EmailID))
                                        {
                                            bccList.Add(custItem);
                                        }
                                    }
                                }
                                //Sub Category
                                else if (CategoryType == 2)
                                {

                                    List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                          //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                      where c.IsActive == true
                                                                      && cpc.SubCategoryId == item.CategoryID
                                                                      && c.City == item.CityID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }
                                                                     ).Distinct().ToList();
                                    foreach (var custItem in customer)
                                    {
                                        if (!string.IsNullOrEmpty(custItem.EmailID))
                                        {
                                            bccList.Add(custItem);
                                        }
                                    }
                                }
                                //Child Category
                                else if (CategoryType == 3)
                                {
                                    List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                      join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                      //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                      join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID

                                                                      where c.IsActive == true
                                                                      //&& cc.ChildCategoryId == item.CategoryID
                                                                      && ic.ID == item.CategoryID
                                                                      && c.City == item.CityID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }
                                                                     ).Distinct().ToList();
                                    foreach (var custItem in customer)
                                    {
                                        if (!string.IsNullOrEmpty(custItem.EmailID))
                                        {
                                            bccList.Add(custItem);
                                        }
                                    }
                                }
                                //Item Category
                                else if (CategoryType == 4)
                                {
                                    List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                      join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                      //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                      join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID


                                                                      where c.IsActive == true
                                                                     // && cc.ItemId == item.CategoryID
                                                                     && ic.ID == item.CategoryID
                                                                      && c.City == item.CityID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }
                                                                     ).Distinct().ToList();
                                    foreach (var custItem in customer)
                                    {
                                        if (!string.IsNullOrEmpty(custItem.EmailID))
                                        {
                                            bccList.Add(custItem);
                                        }
                                    }
                                }
                            }
                        }

                        string ToEmailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                        string FromMailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                        string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
                        string MailServerHost = ConfigurationManager.AppSettings["MailServerHost"].ToString();
                        string SendingPort = ConfigurationManager.AppSettings["SendingPort"].ToString();
                        //string APKPath = ConfigurationManager.AppSettings["APKPath"].ToString();
                        string MailSubject = consolidatedReport.MailSubject;

                        Helper.SendMail(ToEmailID, FromMailID, consolidatedReport.MailBody, MailSubject, MailServerHost, MailPassword, SendingPort, bccList, MailAttachments);
                        Result = "Email Sent Successfully!!";
                        #endregion
                    }
                    else if (consolidatedReport.IsSMS == true)
                    {
                        #region SMS
                        foreach (var item in consolidatedReport.categoryRpts)
                        {
                            //Main Category
                            if (CategoryType == 1)
                            {
                                if (item.IsChecked == true)
                                {
                                    List<CustomerDetails> customer = (from cpc in dbContext.tblCategoryProductWithCusts
                                                                          //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                      where c.IsActive == true
                                                                      && cpc.CategoryProductID == item.CategoryID
                                                                      && c.City == item.CityID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }
                                                                 ).Distinct().ToList();

                                    string MobileNumbers = string.Join(",", customer.Select(c => c.MobileNumber));

                                    string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                                    string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                                    string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                                    Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, consolidatedReport.SMSBody, SenderID);
                                }
                            }
                            else if (CategoryType == 2)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                      //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  where c.IsActive == true
                                                                  && cpc.SubCategoryId == item.CategoryID
                                                                  && c.City == item.CityID
                                                                  select new CustomerDetails
                                                                  {
                                                                      //CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                  }).Distinct().ToList();

                                string MobileNumbers = string.Join(",", customer.Select(c => c.MobileNumber));

                                string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                                string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                                string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                                Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, consolidatedReport.SMSBody, SenderID);
                            }
                            else if (CategoryType == 3)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                  join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                  //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID


                                                                  where c.IsActive == true
                                                                 // && cc.ChildCategoryId == item.CategoryID
                                                                 && ic.ID == item.CategoryID
                                                                  && c.City == item.CityID
                                                                  select new CustomerDetails
                                                                  {
                                                                      //CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                  }
                                                                 ).Distinct().ToList();
                                string MobileNumbers = string.Join(",", customer.Select(c => c.MobileNumber));

                                string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                                string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                                string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                                Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, consolidatedReport.SMSBody, SenderID);
                            }
                            else if (CategoryType == 4)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                  join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                  //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID

                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID

                                                                  where c.IsActive == true
                                                                  //&& cc.ItemId == item.CategoryID
                                                                  && ic.ID == item.CategoryID
                                                                  && c.City == item.CityID
                                                                  select new CustomerDetails
                                                                  {
                                                                      //CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                  }
                                                                 ).Distinct().ToList();

                                string MobileNumbers = string.Join(",", customer.Select(c => c.MobileNumber));

                                string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                                string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                                string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                                Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, consolidatedReport.SMSBody, SenderID);
                            }
                        }
                        #endregion
                    }
                    else if (consolidatedReport.IsNotification == true)
                    {
                        foreach (var item in consolidatedReport.categoryRpts)
                        {
                            //Main Category
                            if (CategoryType == 1)
                            {
                                if (item.IsChecked == true)
                                {
                                    List<CustomerDetails> customer = (from cpc in dbContext.tblCategoryProductWithCusts
                                                                          //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                      join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                      where c.IsActive == true
                                                                      && cpc.CategoryProductID == item.CategoryID
                                                                      && c.City == item.CityID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                          DeviceID = c.DeviceID,
                                                                      }
                                                                 ).Distinct().ToList();

                                    string[] Registration_Ids = customer.Select(c => c.DeviceID).ToArray();
                                    int[] Cust_Ids = customer.Select(c => c.CustID).ToArray();
                                    Notification notification = new Notification { Title = consolidatedReport.Title, Body = consolidatedReport.Body, NotificationDate = DateTimeNow, Image = ImageURL };
                                    Helper.SendNotificationMultiple(Registration_Ids, notification);
                                    PushNotifications pushNotifications = new PushNotifications()
                                    {
                                        Title = consolidatedReport.Title,
                                        NotificationDate = DateTimeNow,
                                        CategoryName = string.Empty,
                                        ImageURL = ImageURL,
                                        PushNotification = consolidatedReport.Body,
                                    };
                                    jBNDBClass.SavePushNotificationsList(Cust_Ids, pushNotifications, UserID);
                                }
                            }
                            else if (CategoryType == 2)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                      //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  where c.IsActive == true
                                                                  && cpc.SubCategoryId == item.CategoryID
                                                                  && c.City == item.CityID
                                                                  select new CustomerDetails
                                                                  {
                                                                      //CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                      DeviceID = c.DeviceID,
                                                                  }).Distinct().ToList();

                                string[] Registration_Ids = customer.Select(c => c.DeviceID).ToArray();
                                int[] Cust_Ids = customer.Select(c => c.CustID).ToArray();
                                Notification notification = new Notification { Title = consolidatedReport.Title, Body = consolidatedReport.Body, NotificationDate = DateTimeNow, Image = ImageURL };
                                Helper.SendNotificationMultiple(Registration_Ids, notification);
                                PushNotifications pushNotifications = new PushNotifications()
                                {
                                    Title = consolidatedReport.Title,
                                    NotificationDate = DateTimeNow,
                                    CategoryName = string.Empty,
                                    ImageURL = ImageURL,
                                    PushNotification = consolidatedReport.Body,
                                };
                                jBNDBClass.SavePushNotificationsList(Cust_Ids, pushNotifications, UserID);
                            }
                            else if (CategoryType == 3)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                  join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                  //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID

                                                                  where c.IsActive == true
                                                                  //&& cc.ChildCategoryId == item.CategoryID
                                                                  && ic.ID == item.CategoryID
                                                                  && c.City == item.CityID
                                                                  select new CustomerDetails
                                                                  {
                                                                      //CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                      DeviceID = c.DeviceID,
                                                                  }
                                                                 ).Distinct().ToList();
                                string[] Registration_Ids = customer.Select(c => c.DeviceID).ToArray();
                                int[] Cust_Ids = customer.Select(c => c.CustID).ToArray();
                                Notification notification = new Notification { Title = consolidatedReport.Title, Body = consolidatedReport.Body, NotificationDate = DateTimeNow, Image = ImageURL };
                                Helper.SendNotificationMultiple(Registration_Ids, notification);
                                PushNotifications pushNotifications = new PushNotifications()
                                {
                                    Title = consolidatedReport.Title,
                                    NotificationDate = DateTimeNow,
                                    CategoryName = string.Empty,
                                    ImageURL = ImageURL,
                                    PushNotification = consolidatedReport.Body,
                                };
                                jBNDBClass.SavePushNotificationsList(Cust_Ids, pushNotifications, UserID);
                            }
                            else if (CategoryType == 4)
                            {
                                List<CustomerDetails> customer = (from cpc in dbContext.tblSubCategoryProductWithCusts
                                                                  join cc in dbContext.tblChildCategories on cpc.SubCategoryId equals cc.SubCategoryId
                                                                  //join c in dbContext.tblCustomerDetails on cpc.CustID equals c.CustID
                                                                  join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                  join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID

                                                                  where c.IsActive == true
                                                                  //&& cc.ItemId == item.CategoryID
                                                                  && ic.ID == item.CategoryID
                                                                  && c.City == item.CityID
                                                                  select new CustomerDetails
                                                                  {
                                                                      //CustID = c.CustID,
                                                                      CustID = c.ID,
                                                                      MobileNumber = c.MobileNumber,
                                                                      EmailID = c.EmailID,
                                                                      DeviceID = c.DeviceID,
                                                                  }
                                                                 ).Distinct().ToList();

                                string[] Registration_Ids = customer.Select(c => c.DeviceID).ToArray();
                                int[] Cust_Ids = customer.Select(c => c.CustID).ToArray();
                                Notification notification = new Notification { Title = consolidatedReport.Title, Body = consolidatedReport.Body, NotificationDate = DateTimeNow, Image = ImageURL };
                                Helper.SendNotificationMultiple(Registration_Ids, notification);
                                PushNotifications pushNotifications = new PushNotifications()
                                {
                                    Title = consolidatedReport.Title,
                                    NotificationDate = DateTimeNow,
                                    CategoryName = string.Empty,
                                    ImageURL = ImageURL,
                                    PushNotification = consolidatedReport.Body,
                                };
                                jBNDBClass.SavePushNotificationsList(Cust_Ids, pushNotifications, UserID);
                            }
                        }
                    }
                }
                return Result;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return "Error!! Please contact administrator";
            }
        }

        public class Select2Repository
        {
            IQueryable<Select2OptionModel> AllOptionsList;
            public Select2Repository()
            {
                AllOptionsList = GetSelect2Options();
            }

            IQueryable<Select2OptionModel> GetSelect2Options()
            {
                string cacheKey = "Select2Options";

                //check cache

                try
                {
                    using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                    {
                        if (HttpContext.Current.Cache[cacheKey] != null)
                        {
                            return (IQueryable<Select2OptionModel>)HttpContext.Current.Cache[cacheKey];
                        }

                        var optionList = (from cc in dbContext.tblChildCategories
                                          join cp in dbContext.tblSubCategoryProductWithCusts on cc.SubCategoryId equals cp.SubCategoryId
                                          join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID


                                          where ic.ItemName != null
                                          select new Select2OptionModel
                                          {
                                              // id = cc.ItemId.ToString(),
                                              //text = cc.ItemName
                                              id = ic.ID.ToString(),
                                              text = ic.ItemName
                                          }).Distinct().ToList();

                        //var optionText = "Option Number ";
                        //for (int i = 1; i < 1000; i++)
                        //{
                        //    optionList.Add(new Select2OptionModel
                        //    {
                        //        id = i.ToString(),
                        //        text = optionText + i
                        //    });
                        //}

                        var result = optionList.AsQueryable();

                        //cache results
                        HttpContext.Current.Cache[cacheKey] = result;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                    return null;
                }
            }

            List<Select2OptionModel> GetPagedListOptions(string searchTerm, int pageSize, int pageNumber, out int totalSearchRecords)
            {
                var allSearchedResults = GetAllSearchResults(searchTerm);
                totalSearchRecords = allSearchedResults.Count;
                return allSearchedResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }

            List<Select2OptionModel> GetAllSearchResults(string searchTerm)
            {
                var resultList = new List<Select2OptionModel>();
                if (!string.IsNullOrEmpty(searchTerm))
                    resultList = AllOptionsList.Where(n => n.text.ToLower().Contains(searchTerm.ToLower())).ToList();
                else
                    resultList = AllOptionsList.ToList();
                return resultList;
            }

            public Select2PagedResult GetSelect2PagedResult(string searchTerm, int pageSize, int pageNumber)
            {
                var select2pagedResult = new Select2PagedResult();
                var totalResults = 0;
                select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
                select2pagedResult.Total = totalResults;
                return select2pagedResult;
            }
        }
    }
}
