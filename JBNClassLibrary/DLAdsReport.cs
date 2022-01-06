using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Net.Mail;
using System.Web.Mvc;
using System.Globalization;

namespace JBNClassLibrary
{
    public class DLAdsReport
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        int IsLogWrite = Convert.ToInt32(ConfigurationManager.AppSettings["IsLogWrite"].ToString());
        public AdMainReport GetAdvertisementReport(AdSearchOptions searchOptions)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE).Date;
                    AdMainReport adMainReport = new AdMainReport();

                    IQueryable<AdvertisementMain> advertisements = (from a in dbContext.tblAdvertisementMains
                                                                    join ar in dbContext.tblAdvertisementAreas on a.AdvertisementAreaID equals ar.ID //AdvertisementAreaID
                                                                    join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                                                                    join c in dbContext.tblItemCategories on a.ProductID equals c.ID        //ItemId
                                                                    //join c in dbContext.tblChildCategories on a.ProductID equals c.ID        //ItemId
                                                                    join cu in dbContext.tblCustomerDetails on a.CustID equals cu.ID        //CustID
                                                                    select new AdvertisementMain
                                                                    {
                                                                        FirmName = cu.FirmName,
                                                                        AdvertisementMainID = a.ID,// AdvertisementMainID
                                                                        AdvertisementName = a.AdvertisementName,
                                                                        CreatedDate = a.CreatedDate,
                                                                        AdvertisementType = at.Type,
                                                                        ProductName = c.ItemName,
                                                                        AdvertisementAreaID = a.AdvertisementAreaID,
                                                                        AdvertisementArea = ar.AdvertisementAreaName,
                                                                        FromDate = a.FromDate.Value,
                                                                        ToDate = a.ToDate.Value,
                                                                        ProductID = a.ProductID,
                                                                        TypeOfAdvertisementID = a.TypeOfAdvertisementID,
                                                                        CustID = a.CustID,
                                                                        MobileNumber = cu.MobileNumber,
                                                                        DeviceID = cu.DeviceID,
                                                                        EmailID = cu.EmailID,
                                                                        customerInfo = new CustomerInfo
                                                                        {
                                                                            //FirmName = dbContext.tblCustomerDetails.Where(cust => cust.CustID == a.CustID).FirstOrDefault().FirmName,
                                                                            FirmName = dbContext.tblCustomerDetails.Where(cust => cust.ID == a.CustID).FirstOrDefault().FirmName,
                                                                            StateID = cu.State,
                                                                            CityID = cu.City,
                                                                            DeviceID = cu.DeviceID,
                                                                        },
                                                                        //states = dbContext.tblAdvertisementStates.Where(ast => ast.AdvertisementMainID == a.AdvertisementMainID).ToList(),
                                                                        //cities = dbContext.tblAdvertisementCities.Where(ast => ast.AdvertisementMainID == a.AdvertisementMainID).ToList(),
                                                                        states = dbContext.tblAdvertisementStates.Where(ast => ast.AdvertisementMainID == a.ID).ToList(),
                                                                        cities = dbContext.tblAdvertisementCities.Where(ast => ast.AdvertisementMainID == a.ID).ToList(),

                                                                        ApprovalStatus = a.IsApproved == false ? "Pending" : "Approved",
                                                                        PaymentStatus = a.PaymentStatus == false ? "Pending" : "Approved",
                                                                    }).AsQueryable();

                    //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
                    if (!string.IsNullOrEmpty(searchOptions.sortColumn) && !string.IsNullOrEmpty(searchOptions.sortColumnDir))
                    {
                        advertisements = advertisements.OrderBy(searchOptions.sortColumn + " " + searchOptions.sortColumnDir);
                    }

                    var iAdslist = advertisements.ToList();
                    iAdslist.ForEach(a => a.FromDateStr = a.FromDate.Value.ToString("dd/MM/yyyy"));
                    iAdslist.ForEach(a => a.ToDateStr = a.ToDate.Value.ToString("dd/MM/yyyy"));

                    if (!string.IsNullOrEmpty(searchOptions.FirmName))
                    {
                        iAdslist = iAdslist.Where(a => a.FirmName.ToLower() == searchOptions.FirmName.ToLower()).ToList();
                    }

                    if (!string.IsNullOrEmpty(searchOptions.FromDate))
                    {
                        DateTime fromDate = Convert.ToDateTime(searchOptions.FromDate);
                        iAdslist = iAdslist.Where(a => a.FromDate.Value.Date == fromDate.Date).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.ToDate))
                    {
                        DateTime toDate = Convert.ToDateTime(searchOptions.ToDate);
                        iAdslist = iAdslist.Where(a => a.FromDate.Value.Date == toDate.Date).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.AdvertisementArea))
                    {
                        iAdslist = iAdslist.Where(a => a.AdvertisementArea.ToLower() == searchOptions.AdvertisementArea.ToLower()).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.AdvertisementType))
                    {
                        iAdslist = iAdslist.Where(a => a.AdvertisementType.ToLower() == searchOptions.AdvertisementType.ToLower()).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.ProductName))
                    {
                        iAdslist = iAdslist.Where(a => a.ProductName.ToLower() == searchOptions.ProductName.ToLower()).ToList();
                    }

                    adMainReport.recordsTotal = iAdslist.Count();
                    iAdslist = iAdslist.Skip(searchOptions.skip).Take(searchOptions.pageSize).ToList();
                    adMainReport.AdsList = iAdslist;
                    return adMainReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException == null ? null : ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public AdMainReport GetApprovedAdReport(AdSearchOptions searchOptions)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE).Date;
                    AdMainReport adMainReport = new AdMainReport();

                    IQueryable<AdvertisementMain> advertisements = (from a in dbContext.tblAdvertisementMains
                                                                    join ar in dbContext.tblAdvertisementAreas on a.AdvertisementAreaID equals ar.ID  //AdvertisementAreaID
                                                                    join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                                                                    join c in dbContext.tblItemCategories on a.ProductID equals c.ID
                                                                    //join cu in dbContext.tblCustomerDetails on a.CustID equals cu.CustID
                                                                    join cu in dbContext.tblCustomerDetails on a.CustID equals cu.ID
                                                                    where a.IsApproved == true && a.PaymentStatus == true
                                                                    select new AdvertisementMain
                                                                    {
                                                                        FirmName = cu.FirmName,
                                                                        AdvertisementMainID = a.ID,//AdvertisementMainID
                                                                        AdvertisementName = a.AdvertisementName,
                                                                        CreatedDate = a.CreatedDate,
                                                                        AdvertisementType = at.Type,
                                                                        ProductName = c.ItemName,
                                                                        AdvertisementAreaID = a.AdvertisementAreaID,
                                                                        AdvertisementArea = ar.AdvertisementAreaName,
                                                                        FromDate = a.FromDate.Value,
                                                                        ToDate = a.ToDate.Value,
                                                                        ProductID = a.ProductID,
                                                                        TypeOfAdvertisementID = a.TypeOfAdvertisementID,
                                                                        CustID = a.CustID,
                                                                        MobileNumber = cu.MobileNumber,
                                                                        DeviceID = cu.DeviceID,
                                                                        EmailID = cu.EmailID,
                                                                        customerInfo = new CustomerInfo
                                                                        {
                                                                            //FirmName = dbContext.tblCustomerDetails.Where(cust => cust.CustID == a.CustID).FirstOrDefault().FirmName,
                                                                            FirmName = dbContext.tblCustomerDetails.Where(cust => cust.ID == a.CustID).FirstOrDefault().FirmName,
                                                                            StateID = cu.State,
                                                                            CityID = cu.City,
                                                                            DeviceID = cu.DeviceID,
                                                                        },
                                                                        //states = dbContext.tblAdvertisementStates.Where(ast => ast.AdvertisementMainID == a.AdvertisementMainID).ToList(),
                                                                        //cities = dbContext.tblAdvertisementCities.Where(ast => ast.AdvertisementMainID == a.AdvertisementMainID).ToList(),
                                                                        states = dbContext.tblAdvertisementStates.Where(ast => ast.AdvertisementMainID == a.ID).ToList(),
                                                                        cities = dbContext.tblAdvertisementCities.Where(ast => ast.AdvertisementMainID == a.ID).ToList(),
                                                                        ApprovalStatus = a.IsApproved == false ? "Pending" : "Approved",
                                                                        PaymentStatus = a.PaymentStatus == false ? "Pending" : "Approved",
                                                                    }).AsQueryable();

                    //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
                    if (!string.IsNullOrEmpty(searchOptions.sortColumn) && !string.IsNullOrEmpty(searchOptions.sortColumnDir))
                    {
                        advertisements = advertisements.OrderBy(searchOptions.sortColumn + " " + searchOptions.sortColumnDir);
                    }

                    var iAdslist = advertisements.ToList();
                    iAdslist.ForEach(a => a.FromDateStr = a.FromDate.Value.ToString("dd/MM/yyyy"));
                    iAdslist.ForEach(a => a.ToDateStr = a.ToDate.Value.ToString("dd/MM/yyyy"));

                    if (!string.IsNullOrEmpty(searchOptions.FirmName))
                    {
                        iAdslist = iAdslist.Where(a => a.FirmName.ToLower() == searchOptions.FirmName.ToLower()).ToList();
                    }

                    if (!string.IsNullOrEmpty(searchOptions.FromDate))
                    {
                        DateTime fromDate = Convert.ToDateTime(searchOptions.FromDate);
                        iAdslist = iAdslist.Where(a => a.FromDate.Value.Date == fromDate.Date).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.ToDate))
                    {
                        DateTime toDate = Convert.ToDateTime(searchOptions.ToDate);
                        iAdslist = iAdslist.Where(a => a.FromDate.Value.Date == toDate.Date).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.AdvertisementArea))
                    {
                        iAdslist = iAdslist.Where(a => a.AdvertisementArea.ToLower() == searchOptions.AdvertisementArea.ToLower()).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.AdvertisementType))
                    {
                        iAdslist = iAdslist.Where(a => a.AdvertisementType.ToLower() == searchOptions.AdvertisementType.ToLower()).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.ProductName))
                    {
                        iAdslist = iAdslist.Where(a => a.ProductName.ToLower() == searchOptions.ProductName.ToLower()).ToList();
                    }

                    adMainReport.recordsTotal = iAdslist.Count();
                    iAdslist = iAdslist.Skip(searchOptions.skip).Take(searchOptions.pageSize).ToList();
                    adMainReport.AdsList = iAdslist;
                    return adMainReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException == null ? null : ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<AdvertisementMain> GetAdvertisementAllReport(AdSearchOptions searchOptions)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE).Date;
                    AdMainReport adMainReport = new AdMainReport();

                    IQueryable<AdvertisementMain> advertisements = (from a in dbContext.tblAdvertisementMains
                                                                    join ar in dbContext.tblAdvertisementAreas on a.AdvertisementAreaID equals ar.ID
                                                                    join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                                                                    join c in dbContext.tblItemCategories on a.ProductID equals c.ID
                                                                    join cu in dbContext.tblCustomerDetails on a.CustID equals cu.ID
                                                                    select new AdvertisementMain
                                                                    {
                                                                        FirmName = cu.FirmName,
                                                                        AdvertisementMainID = a.ID,
                                                                        AdvertisementName = a.AdvertisementName,
                                                                        CreatedDate = a.CreatedDate,
                                                                        AdvertisementType = at.Type,
                                                                        ProductName = c.ItemName,
                                                                        AdvertisementAreaID = a.AdvertisementAreaID,
                                                                        AdvertisementArea = ar.AdvertisementAreaName,
                                                                        FromDate = a.FromDate.Value,
                                                                        ToDate = a.ToDate.Value,
                                                                        ProductID = a.ProductID,
                                                                        TypeOfAdvertisementID = a.TypeOfAdvertisementID,
                                                                        CustID = a.CustID,
                                                                        MobileNumber = cu.MobileNumber,
                                                                        DeviceID = cu.DeviceID,
                                                                        EmailID = cu.EmailID,
                                                                    }).AsQueryable();

                    var iAdslist = advertisements.ToList();
                    iAdslist.ForEach(a => a.FromDateStr = a.FromDate.Value.ToString("dd/MM/yyyy"));
                    iAdslist.ForEach(a => a.ToDateStr = a.ToDate.Value.ToString("dd/MM/yyyy"));

                    if (!string.IsNullOrEmpty(searchOptions.FirmName))
                    {
                        iAdslist = iAdslist.Where(a => a.FirmName.ToLower() == searchOptions.FirmName.ToLower()).ToList();
                    }

                    if (!string.IsNullOrEmpty(searchOptions.FromDate))
                    {
                        DateTime fromDate = Convert.ToDateTime(searchOptions.FromDate);
                        iAdslist = iAdslist.Where(a => a.FromDate.Value.Date == fromDate.Date).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.ToDate))
                    {
                        DateTime toDate = Convert.ToDateTime(searchOptions.ToDate);
                        iAdslist = iAdslist.Where(a => a.FromDate.Value.Date == toDate.Date).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.AdvertisementArea))
                    {
                        iAdslist = iAdslist.Where(a => a.AdvertisementArea.ToLower() == searchOptions.AdvertisementArea.ToLower()).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.AdvertisementType))
                    {
                        iAdslist = iAdslist.Where(a => a.AdvertisementType.ToLower() == searchOptions.AdvertisementType.ToLower()).ToList();
                    }
                    if (!string.IsNullOrEmpty(searchOptions.ProductName))
                    {
                        iAdslist = iAdslist.Where(a => a.ProductName.ToLower() == searchOptions.ProductName.ToLower()).ToList();
                    }
                    return iAdslist;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException == null ? null : ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<CustomerInfo> GetCustomers(string prefix)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<CustomerInfo> customers = new List<CustomerInfo>();
                        customers = (from c in dbContext.tblCustomerDetails
                                     join a in dbContext.tblAdvertisementMains on c.ID equals a.CustID
                                     //join a in dbContext.tblAdvertisementMains on c.CustID equals a.CustID
                                     where c.FirmName.ToLower().Contains(prefix.ToLower())
                                     select new CustomerInfo
                                     {
                                         //CustID = c.CustID,
                                         CustID = c.ID,
                                         FirmName = c.FirmName
                                     }).Distinct().ToList();

                        return customers;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<BillingRptVM> GetYearlyReport()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    int CurrentYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE).Year;
                    List<BillingRptVM> billingRpts = new List<BillingRptVM>();
                    DateTime StartDate = new DateTime(CurrentYear, 1, 1);
                    DateTime EndDate = new DateTime(CurrentYear, 12, 31);
                    List<MonthData> Months = MonthsBetween(StartDate, EndDate);
                    var advertisementMain = (from a in dbContext.tblAdvertisementMains
                                             where a.IsRejected == false && a.IsCancelled == false
                                             && a.CreatedDate < a.BookingExpiryDate
                                             select a);

                    foreach (var item in Months)
                    {
                        item.MonthName = item.MonthName + "-" + item.Year;

                        var QuotionAmt = (from a in advertisementMain
                                          where a.FromDate.Value.Month == item.ID && a.FromDate.Value.Year == item.Year
                                          select new BillingRptVM
                                          {
                                              Month = item.MonthName,
                                              Sale = a.FinalPrice.Value,
                                              ReceivedAmt = 0,
                                              OutstandingAmt = 0,
                                              Year = item.Year.ToString(),
                                              TaxAmt = a.TaxAmount.Value,
                                          }).AsQueryable();
                        var ReceivedAmt = (from a in advertisementMain
                                           where a.FromDate.Value.Month == item.ID && a.FromDate.Value.Year == item.Year
                                           && a.IsApproved == true && a.PaymentStatus == true
                                           select new BillingRptVM
                                           {
                                               Month = item.MonthName,
                                               Sale = 0,
                                               ReceivedAmt = a.FinalPrice.Value,
                                               OutstandingAmt = 0,
                                               Year = item.Year.ToString(),
                                               TaxAmt = a.TaxAmount.Value,
                                           }).AsQueryable();
                        var OutstandingAmt = (from a in advertisementMain
                                              where a.FromDate.Value.Month == item.ID && a.FromDate.Value.Year == item.Year
                                              && a.IsApproved == true && a.PaymentStatus == false
                                              select new BillingRptVM
                                              {
                                                  Month = item.MonthName,
                                                  Sale = 0,
                                                  ReceivedAmt = 0,
                                                  OutstandingAmt = a.FinalPrice.Value,
                                                  Year = item.Year.ToString(),
                                                  TaxAmt = a.TaxAmount.Value,
                                              }).AsQueryable();
                        billingRpts.AddRange(QuotionAmt);
                        billingRpts.AddRange(ReceivedAmt);
                        billingRpts.AddRange(OutstandingAmt);
                    }

                    billingRpts = billingRpts.GroupBy(ac => new
                    {
                        ac.Year
                    }).Select(bv => new BillingRptVM
                    {
                        OutstandingAmt = Math.Round(bv.Sum(i => i.OutstandingAmt), 2),
                        ReceivedAmt = bv.Sum(i => i.ReceivedAmt),
                        Sale = bv.Sum(i => i.Sale),
                        Year = bv.Select(i => i.Year).FirstOrDefault(),
                        TaxAmt = Math.Round(bv.Sum(i => i.TaxAmt), 2),
                    }).ToList();

                    return billingRpts;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<BillingRptVM> GetMonthlyReport(SearchVM searchVM)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    int CurrentYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE).Year;
                    List<BillingRptVM> billingRpts = new List<BillingRptVM>();
                    DateTime StartDate = new DateTime();
                    DateTime EndDate = new DateTime();
                    //get advertisements
                    var advertisementMain = (from a in dbContext.tblAdvertisementMains
                                             where a.IsRejected == false && a.IsCancelled == false
                                             && a.CreatedDate < a.BookingExpiryDate
                                             select new AdvertisementMain
                                             {
                                                 AdvertisementMainID = a.ID, //a.AdvertisementMainID,
                                                 AdvertisementAreaID = a.AdvertisementAreaID,
                                                 TypeOfAdvertisementID = a.TypeOfAdvertisementID,
                                                 FromDate = a.FromDate,
                                                 IsPaymentPaid = a.PaymentStatus == null ? false : a.PaymentStatus,
                                                 FinalPrice = a.FinalPrice,
                                                 TaxAmount = a.TaxAmount,
                                                 IsApproved = a.IsApproved == null ? false : a.IsApproved,
                                                 advertisementStates = (from ast in dbContext.tblAdvertisementStates
                                                                        where ast.AdvertisementMainID == a.ID //a.AdvertisementMainID
                                                                        select new AdvertisementStates
                                                                        {
                                                                            StateID = ast.StateID
                                                                        }).ToList(),
                                                 advertisementCities = (from ast in dbContext.tblAdvertisementCities
                                                                        where ast.AdvertisementMainID == a.ID //a.AdvertisementMainID
                                                                        select new AdvertisementCities
                                                                        {
                                                                            StateWithCityID = ast.StateWithCityID
                                                                        }).ToList(),
                                                 advertisementDistricts = (from ast in dbContext.tblAdvertisementDistricts
                                                                           where ast.AdvertisementMainID == a.ID //a.AdvertisementMainID
                                                                           select new AdvertisementDistricts
                                                                           {
                                                                               DistrictID = ast.DistrictID
                                                                           }).ToList(),
                                             }).ToList();

                    if (searchVM.StateID != null && searchVM.StateID != 0)
                    {
                        advertisementMain = advertisementMain.Where(a => a.advertisementStates.Any(ast => ast.StateID == searchVM.StateID)).ToList();
                    }
                    if (searchVM.DistrictID != null && searchVM.DistrictID != 0)
                    {
                        advertisementMain = advertisementMain.Where(a => a.advertisementDistricts.Any(ast => ast.DistrictID == searchVM.DistrictID)).ToList();
                    }
                    if (searchVM.CityID != null && searchVM.CityID != 0)
                    {
                        advertisementMain = advertisementMain.Where(a => a.advertisementCities.Any(ast => ast.StateWithCityID == searchVM.CityID)).ToList();
                    }

                    if (!string.IsNullOrEmpty(searchVM.FromDate) && !string.IsNullOrEmpty(searchVM.ToDate))
                    {
                        StartDate = Convert.ToDateTime(searchVM.FromDate);
                        EndDate = Convert.ToDateTime(searchVM.ToDate);
                    }
                    else if (!string.IsNullOrEmpty(searchVM.Year))
                    {
                        StartDate = new DateTime(Convert.ToInt32(searchVM.Year), 4, 1);
                        EndDate = new DateTime(Convert.ToInt32(searchVM.Year) + 1, 4, 1).AddDays(-1);
                    }
                    else
                    {
                        StartDate = new DateTime(CurrentYear, 4, 1);
                        EndDate = new DateTime(CurrentYear + 1, 4, 1).AddDays(-1);
                    }
                    List<MonthData> Months = MonthsBetween(StartDate, EndDate);
                    //var advertisementMain = (from a in dbContext.tblAdvertisementMains
                    //                         where a.IsRejected == false && a.IsCancelled == false
                    //                         && (searchVM.AdvertisementAreaID == null || a.AdvertisementAreaID == searchVM.AdvertisementAreaID)
                    //                         && (searchVM.AdvertisementTypeID == null || a.TypeOfAdvertisementID == searchVM.AdvertisementTypeID)
                    //                         && (searchVM.StateID == null || a.TypeOfAdvertisementID == searchVM.AdvertisementTypeID)
                    //                         && (searchVM.AdvertisementTypeID == null || a.TypeOfAdvertisementID == searchVM.AdvertisementTypeID)
                    //                         && (searchVM.AdvertisementTypeID == null || a.TypeOfAdvertisementID == searchVM.AdvertisementTypeID)
                    //                         && a.CreatedDate.Value < a.BookingExpiryDate.Value
                    //                         select a);

                    if (advertisementMain.Count() > 0)
                    {
                        foreach (var item in Months)
                        {
                            BillingRptVM billingRpt = new BillingRptVM();

                            //item.MonthName = item.MonthName + "-" + item.Year;
                            billingRpt.Month = item.MonthName + "-" + item.Year;

                            billingRpt.Sale = (from a in advertisementMain
                                               where a.FromDate.Value.Month == item.ID && a.FromDate.Value.Year == item.Year
                                               select a.FinalPrice.Value).Sum();

                            billingRpt.TaxAmt = (from a in advertisementMain
                                                 where a.FromDate.Value.Month == item.ID && a.FromDate.Value.Year == item.Year
                                                 select a.TaxAmount.Value).Sum();

                            billingRpt.ReceivedAmt = (from a in advertisementMain
                                                      where a.FromDate.Value.Month == item.ID && a.FromDate.Value.Year == item.Year
                                                      && a.IsApproved == true && a.IsPaymentPaid == true
                                                      select a.FinalPrice.Value).Sum();
                            billingRpt.OutstandingAmt = (from a in advertisementMain
                                                         where a.FromDate.Value.Month == item.ID && a.FromDate.Value.Year == item.Year
                                                         && a.IsPaymentPaid == false
                                                         select a.FinalPrice.Value).Sum();
                            billingRpt.Year = item.Year.ToString();
                            billingRpts.Add(billingRpt);
                        }

                        billingRpts = billingRpts.GroupBy(ac => new
                        {
                            ac.Month,
                            ac.Year
                        }).Select(bv => new BillingRptVM
                        {
                            Month = bv.Select(i => i.Month).FirstOrDefault(),
                            OutstandingAmt = bv.Sum(i => i.OutstandingAmt),
                            ReceivedAmt = bv.Sum(i => i.ReceivedAmt),
                            Sale = bv.Sum(i => i.Sale),
                            Year = bv.Select(i => i.Year).FirstOrDefault(),
                            TaxAmt = Math.Round(bv.Sum(i => i.TaxAmt), 2),
                            MonthID = bv.Select(i => i.MonthID).FirstOrDefault(),
                        }).OrderBy(c => c.MonthID).ToList();
                    }
                    return billingRpts;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<BillingRptVM> GetAdvertisementRptOfMonth(SearchVM searchVM)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    int CurrentYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE).Year;
                    int CurrentMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE).Month;
                    List<BillingRptVM> billingRpts = new List<BillingRptVM>();
                    DateTime? StartDate, EndDate;
                    if (!string.IsNullOrEmpty(searchVM.FromDate) && !string.IsNullOrEmpty(searchVM.ToDate))
                    {
                        StartDate = Convert.ToDateTime(searchVM.FromDate).Date;
                        EndDate = Convert.ToDateTime(searchVM.ToDate).Date;
                    }
                    else if (!string.IsNullOrEmpty(searchVM.Month))
                    {
                        StartDate = new DateTime(Convert.ToInt32(searchVM.Year), Convert.ToInt32(searchVM.Month), 1).Date;
                        EndDate = StartDate.Value.AddMonths(1).AddDays(-1).Date;
                    }
                    else
                    {
                        StartDate = new DateTime(CurrentYear, CurrentMonth, 1).Date;
                        EndDate = new DateTime(CurrentYear, CurrentMonth, 31).Date;
                    }
                    //var advertisementMain = (from a in dbContext.tblAdvertisementMains
                    //                         where a.IsRejected == false && a.IsCancelled == false
                    //                         && (searchVM.AdvertisementAreaID == null || a.AdvertisementAreaID == searchVM.AdvertisementAreaID)
                    //                         && (searchVM.AdvertisementTypeID == null || a.TypeOfAdvertisementID == searchVM.AdvertisementTypeID)
                    //                         && (StartDate == null || DbFunctions.TruncateTime(a.CreatedDate.Value) >= StartDate)
                    //                         && (EndDate == null || DbFunctions.TruncateTime(a.CreatedDate.Value) <= EndDate)
                    //                         select a);

                    var advertisementMain = (from a in dbContext.tblAdvertisementMains
                                             where a.IsRejected == false && a.IsCancelled == false
                                             && a.CreatedDate < a.BookingExpiryDate
                                             && (searchVM.AdvertisementAreaID == null || a.AdvertisementAreaID == searchVM.AdvertisementAreaID)
                                             && (searchVM.AdvertisementTypeID == null || a.TypeOfAdvertisementID == searchVM.AdvertisementTypeID)
                                             && DbFunctions.TruncateTime(a.FromDate.Value) >= StartDate
                                             && DbFunctions.TruncateTime(a.FromDate.Value) <= EndDate
                                             select new AdvertisementMain
                                             {
                                                 CustID = a.CustID,
                                                 AdvertisementMainID = a.ID,        //a.AdvertisementMainID,
                                                 AdvertisementAreaID = a.AdvertisementAreaID,
                                                 TypeOfAdvertisementID = a.TypeOfAdvertisementID,
                                                 FromDate = a.FromDate,
                                                 AdvertisementName = a.AdvertisementName,
                                                 IsPaymentPaid = a.PaymentStatus,
                                                 FinalPrice = a.FinalPrice,
                                                 TaxAmount = a.TaxAmount,
                                                 IsApproved = a.IsApproved,
                                                 advertisementStates = (from ast in dbContext.tblAdvertisementStates
                                                                        where ast.AdvertisementMainID == a.ID // a.AdvertisementMainID
                                                                        select new AdvertisementStates
                                                                        {
                                                                            StateID = ast.StateID
                                                                        }).ToList(),
                                                 advertisementCities = (from ast in dbContext.tblAdvertisementCities
                                                                        where ast.AdvertisementMainID == a.ID // a.AdvertisementMainID
                                                                        select new AdvertisementCities
                                                                        {
                                                                            StateWithCityID = ast.StateWithCityID
                                                                        }).ToList(),
                                                 advertisementDistricts = (from ast in dbContext.tblAdvertisementDistricts
                                                                           where ast.AdvertisementMainID == a.ID // a.AdvertisementMainID
                                                                           select new AdvertisementDistricts
                                                                           {
                                                                               DistrictID = ast.DistrictID
                                                                           }).ToList(),
                                             }).ToList();

                    if (searchVM.StateID != null && searchVM.StateID != 0)
                    {
                        advertisementMain = advertisementMain.Where(a => a.advertisementStates.Any(ast => ast.StateID == searchVM.StateID)).ToList();
                    }
                    if (searchVM.DistrictID != null && searchVM.DistrictID != 0)
                    {
                        advertisementMain = advertisementMain.Where(a => a.advertisementDistricts.Any(ast => ast.DistrictID == searchVM.DistrictID)).ToList();
                    }
                    if (searchVM.CityID != null && searchVM.CityID != 0)
                    {
                        advertisementMain = advertisementMain.Where(a => a.advertisementCities.Any(ast => ast.StateWithCityID == searchVM.CityID)).ToList();
                    }

                    if (advertisementMain.Count() > 0)
                    {
                        var billingRptsWithUser = (from a in advertisementMain
                                                   join c in dbContext.tblCustomerDetails on a.CustID equals c.ID // c.CustID
                                                   select new BillingRptVM
                                                   {
                                                       FirmName = c.FirmName,
                                                       AdvertisementName = a.AdvertisementName,
                                                       Sale = a.FinalPrice.Value,
                                                       TaxAmt = a.TaxAmount.Value,
                                                       ReceivedAmt = a.IsPaymentPaid == true ? a.FinalPrice.Value : 0,
                                                       OutstandingAmt = a.IsPaymentPaid == true ? 0 : a.FinalPrice.Value,
                                                       Date = a.FromDate.Value,
                                                       AdvertisementMainID = a.AdvertisementMainID,
                                                   }).AsQueryable();
                        billingRpts = billingRptsWithUser.ToList();
                    }
                    return billingRpts.OrderBy(c => c.Date).ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<Year> GetYears()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<Year> adYears = new List<Year>();
                        adYears = (from a in dbContext.tblAdvertisementMains
                                   where a.IsApproved == true && a.PaymentStatus == true
                                   select new Year
                                   {
                                       AdYear = a.FromDate.Value.Year.ToString()
                                   }).Distinct().ToList();

                        return adYears;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public IEnumerable<SelectListItem> Months
        {
            get
            {
                return DateTimeFormatInfo
                       .InvariantInfo
                       .MonthNames
                       .Select((monthName, index) => new SelectListItem
                       {
                           Value = (index + 1).ToString(),
                           Text = monthName
                       });
            }
        }
        //Send Promotion
        public string Promotion(AdSearchOptions promo, List<Attachment> MailAttachments, string ImageURL, int UserID)
        {
            try
            {
                string Result = string.Empty;
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                var AList = GetAdvertisementAllReport(promo);
                string AppName = ConfigurationManager.AppSettings["AppName"].ToString();

                if (AList != null && AList.Count() > 0)
                {
                    var CustomerList = (from a in AList
                                        select new
                                        {
                                            CustID = a.CustID,
                                            MobileNumber = a.MobileNumber,
                                            EmailID = a.EmailID,
                                            DeviceID = a.DeviceID
                                        }).Distinct().ToList();

                    if (promo.IsEmail == true)
                    {
                        string Bcc = string.Empty;
                        List<CustomerDetails> bccList = new List<CustomerDetails>();

                        foreach (var item in CustomerList)
                        {
                            if (!string.IsNullOrEmpty(item.EmailID))
                            {
                                CustomerDetails custDetails1 = new CustomerDetails();
                                custDetails1.EmailID = item.EmailID;
                                bccList.Add(custDetails1);
                            }
                        }

                        string ToEmailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                        string FromMailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                        string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
                        string MailServerHost = ConfigurationManager.AppSettings["MailServerHost"].ToString();
                        string SendingPort = ConfigurationManager.AppSettings["SendingPort"].ToString();

                        string MailSubject = promo.MailSubject;

                        Helper.SendMail(ToEmailID, FromMailID, promo.MailBody, MailSubject, MailServerHost, MailPassword, SendingPort, bccList, MailAttachments);
                        Result = "Email Sent Successfully!!";
                    }
                    else if (promo.IsSMS == true)
                    {
                        string MobileNumbers = string.Empty;
                        MobileNumbers = string.Join(",", CustomerList.Select(c => c.MobileNumber));

                        string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                        string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                        string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                        Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, promo.SMSBody, SenderID);
                    }
                    else if (promo.IsNotification == true)
                    {
                        string[] Registration_Ids = CustomerList.Select(c => c.DeviceID).ToArray();
                        int[] Cust_Ids = CustomerList.Select(c => c.CustID.Value).ToArray();
                        Notification notification = new Notification { Title = promo.Title, Body = promo.Body, NotificationDate = DateTimeNow, Image = ImageURL };
                        Helper.SendNotificationMultiple(Registration_Ids, notification);

                        PushNotifications pushNotifications = new PushNotifications()
                        {
                            Title = promo.Title,
                            NotificationDate = DateTimeNow,
                            CategoryName = string.Empty,
                            ImageURL = ImageURL,
                            PushNotification = promo.Body,
                        };
                        JBNDBClass jBNDBClass = new JBNDBClass();
                        jBNDBClass.SavePushNotificationsList(Cust_Ids, pushNotifications, UserID);
                        Result = "Success";
                    }
                }
                else
                {
                    Result = "No records to promote..";
                }
                return Result;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return "Error!! Please contact administrator";
            }
        }

        public List<MonthData> MonthsBetween(DateTime startDate, DateTime endDate)
        {
            List<MonthData> getMonthLists = new List<MonthData>();
            DateTime iterator;

            DateTime limit;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            while (iterator <= limit)
            {
                MonthData monthdata = new MonthData();
                monthdata.ID = iterator.Month;
                monthdata.MonthName = iterator.ToString("MMMM");
                monthdata.Year = iterator.Year;
                getMonthLists.Add(monthdata);
                iterator = iterator.AddMonths(1);
            }
            return getMonthLists;

        }
    }
}
