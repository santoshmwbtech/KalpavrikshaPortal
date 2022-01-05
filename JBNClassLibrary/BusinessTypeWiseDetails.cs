using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.Entity;

namespace JBNClassLibrary
{
    public class BusinessTypeWiseRpt
    {
        public int CustID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime? MDateTime { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string Address { get; set; }
        public string DealerName { get; set; }
        public string OwnerName { get; set; }

        public string EmailID { get; set; }
        public string ContactPersonName { get; set; }
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
        public string DeviceID { get; set; }
        public int CategoryID { get; set; }
        public int BusinessTypeID { get; set; }
        public IQueryable<BusinessTypes> BusinessTypesList { get; set; }
        public IQueryable<CategoryProducts> MainCategories { get; set; }
        public IQueryable<SubCat> SubCategories { get; set; }
        public IQueryable<childcategory> ChildCategories { get; set; }
        public IQueryable<ItemCategory> ItemCategories { get; set; }
        public string TypeOfBusiness { get; set; }
        public bool IsChecked { get; set; }
        public bool CheckAll { get; set; }
        public int[] StateList { get; set; }
        public int[] CityList { get; set; }
        public int[] BusinessTypeList { get; set; }
        public int[] SubCategoryList { get; set; }
        public int[] MainCategoryList { get; set; }
        public int[] ChildCategoryList { get; set; }
        public int[] ItemCategoryList { get; set; }
    }
    public class PromoWithBusinessTypeRptList
    {
        public IList<BusinessTypeWiseRpt> businessTypeWiseRpts { get; set; }
        public bool IsEmail { get; set; }
        public bool IsSMS { get; set; }
        public bool IsWhatsApp { get; set; }
        public bool IsNotification { get; set; }
        [Required(ErrorMessage = "Enter SMS body")]
        public string SMSBody { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Enter your message")]
        public string MailBody { get; set; }
        [Required(ErrorMessage = "Enter Mail Subject")]
        public string MailSubject { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int TotalCategories { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalStates { get; set; }
        public int TotalCities { get; set; }
        public int SMSTemplateID { get; set; }
    }
    public class BusinessTypeWiseDetails
    {
        public PromoWithBusinessTypeRptList BusinessList(BusinessTypeWiseRpt search)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    PromoWithBusinessTypeRptList promoWithBusinessTypeRptList = new PromoWithBusinessTypeRptList();
                    IQueryable<BusinessTypeWiseRpt> BusinessTypeWise;
                    BusinessTypeWise = (from u in dbContext.tblCustomerDetails
                                        join sc in dbContext.tblStateWithCities on u.City equals sc.ID
                                        join s in dbContext.tblStates on u.State equals s.ID
                                        where u.IsActive == true && u.FirmName != null && u.City.HasValue && u.State.HasValue
                                        select new BusinessTypeWiseRpt
                                        {
                                            //CustID = u.CustID,
                                            CustID = u.ID,
                                            DeviceID = u.DeviceID,
                                            DealerName = u.FirmName,
                                            Mobile1 = u.MobileNumber,
                                            StateID = s.ID,
                                            CityID = sc.ID,
                                            CityName = sc.VillageLocalityName,
                                            StateName = s.StateName,
                                            OwnerName = u.CustName,
                                            EmailID = u.EmailID,
                                            MDateTime = u.CreatedDate,
                                            //BusinessTypesList = (from b in dbContext.tblBusinessTypewithCusts
                                            //                     join bt in dbContext.tblBusinessTypes on b.BusinessTypeID
                                            //                     equals bt.BusinessTypeID
                                            //                     where b.CustID == u.CustID
                                            //                     select new BusinessTypes
                                            //                     {
                                            //                         BusinessTypeID = b.BusinessTypeID,
                                            //                         BusinessTypeName = bt.BusinessType.Trim()
                                            //                     }).AsQueryable(),
                                            //SubCategories = (from sc in dbContext.tblSubCategoryProductWithCusts
                                            //                 join scc in dbContext.tblSubCategories on sc.SubCategoryId
                                            //                 equals scc.SubCategoryId
                                            //                 where sc.CustID == u.CustID
                                            //                 select new SubCat
                                            //                 {
                                            //                     SubCategoryID = sc.SubCategoryId.Value,
                                            //                     SubCategoryName = scc.SubCategoryName
                                            //                 }).AsQueryable(),
                                            //MainCategories = (from sc in dbContext.tblCategoryProductWithCusts
                                            //                  join scc in dbContext.tblCategoryProducts on sc.CategoryProductID
                                            //                  equals scc.CategoryProductID
                                            //                  where sc.CustID == u.CustID
                                            //                  select new CategoryProducts
                                            //                  {
                                            //                      CategoryProductID = sc.CategoryProductID.Value,
                                            //                      MainCategoryName = scc.MainCategoryName
                                            //                  }).AsQueryable(),
                                            //ChildCategories = (from scc in dbContext.tblSubCategoryProductWithCusts
                                            //                   join scg in dbContext.tblSubCategories on scc.SubCategoryId.Value equals scg.SubCategoryId
                                            //                   join cc in dbContext.tblChildCategories on scc.SubCategoryId equals cc.SubCategoryId
                                            //                   where scc.CustID == u.CustID
                                            //                   select new childcategory
                                            //                   {
                                            //                       ChildCategoryName = cc.ChildCategoryName,
                                            //                       ChildCategoryId = cc.ChildCategoryId
                                            //                   }).AsQueryable(),
                                            //ItemCategories = (from scc in dbContext.tblSubCategoryProductWithCusts
                                            //                  join scg in dbContext.tblSubCategories on scc.SubCategoryId.Value equals scg.SubCategoryId
                                            //                  join cc in dbContext.tblChildCategories on scc.SubCategoryId equals cc.SubCategoryId
                                            //                  where scc.CustID == u.CustID
                                            //                  select new ItemCategory
                                            //                  {
                                            //                      ItemId = cc.ItemId,
                                            //                      ItemName = cc.ItemName
                                            //                  }).AsQueryable(),
                                        }).AsQueryable();

                    if (!string.IsNullOrEmpty(search.DealerName))
                        BusinessTypeWise = BusinessTypeWise.Where(i => i.DealerName.ToLower().Contains(search.DealerName.ToLower())).AsQueryable();

                    if (!string.IsNullOrEmpty(search.DealerName))
                        BusinessTypeWise = BusinessTypeWise.Where(i => i.DealerName.ToLower().Contains(search.DealerName.ToLower())).AsQueryable();

                    if (search.BusinessTypeList != null && search.BusinessTypeList.Count() > 0)
                    {

                        BusinessTypeWise = (from b in BusinessTypeWise
                                            join btc in dbContext.tblBusinessTypewithCusts on b.CustID equals btc.CustID  //b.CustID equals btc.CustID
                                            where search.BusinessTypeList.Contains(btc.BusinessTypeID)
                                            select b);
                    }

                    if (search.MainCategoryList != null && search.MainCategoryList.Count() > 0)
                    {
                        BusinessTypeWise = (from b in BusinessTypeWise
                                            join btc in dbContext.tblSubCategoryProductWithCusts on b.CustID equals btc.CustID
                                            where search.MainCategoryList.Contains(btc.CategoryProductID.Value)
                                            select b);
                    }

                    if (search.SubCategoryList != null && search.SubCategoryList.Count() > 0)
                    {
                        BusinessTypeWise = (from b in BusinessTypeWise
                                            join btc in dbContext.tblSubCategoryProductWithCusts on b.CustID equals btc.CustID
                                            where search.SubCategoryList.Contains(btc.SubCategoryId.Value)
                                            select b);
                    }
                    if (search.ChildCategoryList != null && search.ChildCategoryList.Count() > 0)
                    {
                        BusinessTypeWise = (from b in BusinessTypeWise
                                            join btc in dbContext.tblSubCategoryProductWithCusts on b.CustID equals btc.CustID
                                            join cc in dbContext.tblChildCategories on btc.SubCategoryId equals cc.SubCategoryId
                                            where search.ChildCategoryList.Contains(cc.ID)//(cc.ChildCategoryId)
                                            select b);
                    }
                    if (search.ItemCategoryList != null && search.ItemCategoryList.Count() > 0)
                    {
                        BusinessTypeWise = (from b in BusinessTypeWise
                                            join btc in dbContext.tblSubCategoryProductWithCusts on b.CustID equals btc.CustID
                                            join cc in dbContext.tblChildCategories on btc.SubCategoryId equals cc.SubCategoryId
                                            where search.ChildCategoryList.Contains(cc.ID)//(cc.ChildCategoryId)
                                            select b);
                    }

                    if (!string.IsNullOrEmpty(search.FromDate) && !string.IsNullOrEmpty(search.ToDate))
                    {
                        DateTime FromDate = DateTime.ParseExact(search.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime ToDate = DateTime.ParseExact(search.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        BusinessTypeWise = BusinessTypeWise.Where(i => DbFunctions.TruncateTime(i.MDateTime.Value) >= FromDate.Date && DbFunctions.TruncateTime(i.MDateTime.Value) <= ToDate.Date).AsQueryable();
                    }

                    if (search.StateList != null && search.StateList.Count() > 0)
                    {
                        BusinessTypeWise = BusinessTypeWise.Where(i => search.StateList.Contains(i.StateID)).AsQueryable();
                    }

                    if (search.CityList != null && search.CityList.Count() > 0)
                    {
                        BusinessTypeWise = BusinessTypeWise.Where(i => search.CityList.Contains(i.CityID)).AsQueryable();
                    }

                    promoWithBusinessTypeRptList.businessTypeWiseRpts = BusinessTypeWise.Distinct().ToList();

                    promoWithBusinessTypeRptList.TotalCustomers = BusinessTypeWise.Distinct().ToList().Count();
                    promoWithBusinessTypeRptList.TotalCities = BusinessTypeWise.ToList().Select(c => c.CityID).Distinct().Count();
                    promoWithBusinessTypeRptList.TotalStates = BusinessTypeWise.ToList().Select(bt => bt.StateID).Distinct().Count();
                    //promoWithBusinessTypeRptList.businessTypeWiseRpts = BusinessTypeWise.ToList();
                    return promoWithBusinessTypeRptList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public DateTime GetDateinFormat(DateTime? DateTimeVar)
        {
            DateTime FormattedDate = DateTime.ParseExact(DateTimeVar.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return FormattedDate;
        }
    }
}

