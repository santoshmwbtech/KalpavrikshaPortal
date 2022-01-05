using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace JBNClassLibrary
{
    public class CityWiseDetails
    {
        public class CityWiseRpt
        {
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public string CityName { get; set; }
            public int CityID { get; set; }
            public int StateID { get; set; }
            public string StateName { get; set; }
            public int CategoryID { get; set; }
            public int BusinessTypeID { get; set; }
            public int TtlRegDealers { get; set; }
            public int TotalCities { get; set; }
            public bool IsChecked { get; set; }
        }
        public class PromoWithList
        {
            public List<CityWiseRpt> cityWiseRpts { get; set; }
            public List<CityWiseRpt> stateList { get; set; }
            public bool IsEmail { get; set; }
            public bool IsSMS { get; set; }
            public bool IsWhatsApp { get; set; }
            public string SMSBody { get; set; }
            public string MailSubject { get; set; }
            [AllowHtml]
            public string MailBody { get; set; }
            public List<CityWiseDetailedRpt> detailedList { get; set; }
        }
        public List<tblStateWithCity> GetCities(string prefix)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<tblStateWithCity> cityList = new List<tblStateWithCity>();
                        cityList = (from s in dbContext.tblStateWithCities
                                    where s.VillageLocalityName.Contains(prefix)
                                    select s).ToList();

                        return cityList;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<CityWiseRpt> CityWiseRegDealersList(CityWiseRpt cityWiseDetailsRpt, int IsState = 0)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<CityWiseRpt> CityList = new List<CityWiseRpt>();
                    
                    if(cityWiseDetailsRpt.BusinessTypeID != 0)
                    {
                        if(IsState == 0)
                        {
                            CityList = (from u in dbContext.tblCustomerDetails
                                        join ct in dbContext.tblStateWithCities on u.City equals ct.StatewithCityID
                                        join btc in dbContext.tblBusinessTypewithCusts on u.ID equals btc.CustID
                                        where btc.BusinessTypeID == cityWiseDetailsRpt.BusinessTypeID
                                        select new CityWiseRpt
                                        {
                                            CityID = u.City.Value,
                                            StateID = u.State.Value,
                                            CityName = (from c in dbContext.tblStateWithCities
                                                        where c.StatewithCityID == u.City.Value
                                                        select c.VillageLocalityName).FirstOrDefault(),
                                            FromDate = u.CreatedDate.ToString(),
                                            TtlRegDealers = (from a in dbContext.tblCustomerDetails
                                                             join btc in dbContext.tblBusinessTypewithCusts on a.ID equals btc.CustID
                                                             where a.City == u.City && btc.BusinessTypeID == cityWiseDetailsRpt.BusinessTypeID
                                                             select new { a.City }).Count(),
                                            TotalCities = (from a in dbContext.tblCustomerDetails
                                                           join sc in dbContext.tblStateWithCities on a.City equals sc.StatewithCityID
                                                           where sc.StateID == u.State
                                                           select new { a.City }).Distinct().Count(),
                                            StateName = (from c in dbContext.tblStates
                                                         where c.StateID == u.State.Value
                                                         select c.StateName).FirstOrDefault(),
                                        }).Distinct().ToList();
                        }
                        else
                        {
                            CityList = (from cust in dbContext.tblCustomerDetails
                                        join ct in dbContext.tblStateWithCities on cust.City equals ct.StatewithCityID
                                        join btc in dbContext.tblBusinessTypewithCusts on cust.ID equals btc.CustID
                                        where btc.BusinessTypeID == cityWiseDetailsRpt.BusinessTypeID
                                        select new CityWiseRpt
                                        {
                                            CityID = cust.City.Value,
                                            StateID = cust.State.Value,
                                            CityName = (from c in dbContext.tblStateWithCities
                                                        where c.StatewithCityID == cust.City.Value
                                                        select c.VillageLocalityName).FirstOrDefault(),
                                            FromDate = cust.CreatedDate.ToString(),
                                            TtlRegDealers = (from a in dbContext.tblCustomerDetails
                                                             join btc in dbContext.tblBusinessTypewithCusts on a.ID equals btc.CustID
                                                             where a.City == cust.City && btc.BusinessTypeID == cityWiseDetailsRpt.BusinessTypeID
                                                             select new { a.City }).Count(),
                                            TotalCities = (from a in dbContext.tblCustomerDetails
                                                           join sc in dbContext.tblStateWithCities on a.City equals sc.StatewithCityID
                                                           where sc.StateID == cust.State
                                                           select new { a.City }).Distinct().Count(),
                                            StateName = (from c in dbContext.tblStates
                                                         where c.StateID == cust.State.Value
                                                         select c.StateName).FirstOrDefault(),
                                        }).Distinct().ToList();
                        }
                    }
                    else
                    {
                        if(IsState == 0)
                        {
                            CityList = (from u in dbContext.tblCustomerDetails
                                        join ct in dbContext.tblStateWithCities on u.City equals ct.StatewithCityID
                                        select new CityWiseRpt
                                        {
                                            CityID = u.City.Value,
                                            StateID = u.State.Value,
                                            CityName = (from c in dbContext.tblStateWithCities
                                                        where c.StatewithCityID == u.City.Value
                                                        select c.VillageLocalityName).FirstOrDefault(),
                                            FromDate = u.CreatedDate.ToString(),
                                            TtlRegDealers = (from a in dbContext.tblCustomerDetails
                                                             where a.City == u.City
                                                             select new { a.City }).Count(),
                                            StateName = (from c in dbContext.tblStates
                                                         where c.StateID == u.State.Value
                                                         select c.StateName).FirstOrDefault(),
                                        }).Distinct().ToList();
                        }
                        else
                        {
                            CityList = (from u in dbContext.tblCustomerDetails
                                        join ct in dbContext.tblStates on u.State equals ct.StateID
                                        select new CityWiseRpt
                                        {
                                            CityID = u.City.Value,
                                            StateID = u.State.Value,
                                            FromDate = u.CreatedDate.ToString(),
                                            TtlRegDealers = (from a in dbContext.tblCustomerDetails
                                                             where a.State == u.State
                                                             select new { a.State }).Count(),
                                            StateName = (from c in dbContext.tblStates
                                                         where c.StateID == u.State.Value
                                                         select c.StateName).FirstOrDefault(),
                                        }).Distinct().ToList();
                        }
                    }

                    CityList = CityList.GroupBy(ac => new
                    {
                        ac.CityID,
                    }).Select(g => new CityWiseRpt()
                    {
                        CityID = g.Key.CityID,
                        StateID = g.Select(i => i.StateID).FirstOrDefault(),
                        CityName = g.Select(i => i.CityName).FirstOrDefault(),
                        FromDate = g.Select(i => i.FromDate).FirstOrDefault(),
                        StateName = g.Select(i => i.StateName).FirstOrDefault(),
                        TtlRegDealers = g.Sum(i => i.TtlRegDealers)
                    }).Distinct().ToList();

                    if (!string.IsNullOrEmpty(cityWiseDetailsRpt.FromDate) && !string.IsNullOrEmpty(cityWiseDetailsRpt.ToDate))
                    {
                        DateTime FromDate = DateTime.ParseExact(cityWiseDetailsRpt.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime ToDate = DateTime.ParseExact(cityWiseDetailsRpt.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        CityList = CityList.Where(i => Convert.ToDateTime(i.FromDate).Date >= FromDate.Date && Convert.ToDateTime(i.FromDate).Date <= ToDate.Date).ToList();
                    }

                    if(cityWiseDetailsRpt.StateID != 0)
                        CityList = CityList.Where(i => i.StateID == cityWiseDetailsRpt.StateID).ToList();
                    if (cityWiseDetailsRpt.CityID != 0)
                        CityList = CityList.Where(i => i.CityID == cityWiseDetailsRpt.CityID).ToList();

                    return CityList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<CityWiseDetailedRpt> CityDetailedList(CityWiseDetailedRpt cityWiseDetails)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<CityWiseDetailedRpt> CityList = new List<CityWiseDetailedRpt>();

                    CityList = (from cust in dbContext.tblCustomerDetails
                                join c in dbContext.tblStateWithCities on cust.City equals c.ID
                                join st in dbContext.tblStates on cust.State equals st.ID
                                select new CityWiseDetailedRpt
                                {
                                    CityID = cust.City,
                                    CityName = c.VillageLocalityName.Trim(),
                                    FirmName = cust.FirmName,
                                    DealerName = cust.CustName,
                                    FromDate = cust.CreatedDate.ToString(),
                                    Mobile1 = cust.MobileNumber,
                                    EmailID = cust.EmailID,
                                    StateName = st.StateName,
                                    Address = cust.BillingAddress,
                                    StateID = st.StateID,
                                    BusinessTypesList = (from b in dbContext.tblBusinessTypewithCusts
                                                         join bt in dbContext.tblBusinessTypes on b.BusinessTypeID
                                                         equals bt.ID //bt.BusinessTypeID
                                                         where b.CustID == cust.ID//u.CustID
                                                         select new BusinessTypes
                                                         {
                                                             BusinessTypeID = bt.ID, // b.BusinessTypeID,
                                                             BusinessTypeName = bt.Type
                                                         }).ToList(),
                                    SubCategoryList = (from sp in dbContext.tblSubCategoryProductWithCusts
                                                       join s in dbContext.tblSubCategories on sp.SubCategoryId
                                                       equals s.SubCategoryId
                                                       where sp.CustID == cust.ID //u.CustID
                                                       select new SubCat
                                                       {
                                                           //SubCategoryID = sp.SubCategoryId.Value,
                                                           ID = sp.SubCategoryId.Value,
                                                           SubCategoryName = s.SubCategoryName.Trim()
                                                       }).ToList(),
                                }).ToList();

                    if(cityWiseDetails.BusinessTypeID != 0)
                    {
                        CityList = CityList.Where(c => c.BusinessTypesList.Any(bt => bt.BusinessTypeID == cityWiseDetails.BusinessTypeID)).ToList();
                    }
                    if (cityWiseDetails.SubCategoryID != 0)
                    {
                        //CityList = CityList.Where(c => c.SubCategoryList.Any(bt => bt.SubCategoryID == cityWiseDetails.SubCategoryID)).ToList();
                        CityList = CityList.Where(c => c.SubCategoryList.Any(bt => bt.ID == cityWiseDetails.SubCategoryID)).ToList();
                    }

                    //if(cityWiseDetails.BusinessTypeID != 0)
                    //{
                    //    CityList = (from u in dbContext.tblCustomerDetails
                    //                join c in dbContext.tblStateWithCities on u.City equals c.StatewithCityID
                    //                join st in dbContext.tblStates on u.State equals st.StateID
                    //                join sc in dbContext.tblSubCategoryProductWithCusts on u.CustID equals sc.CustID
                    //                join btc in dbContext.tblBusinessTypewithCusts on u.CustID equals btc.CustID
                    //                join sct in dbContext.tblSubCategoryProductWithCusts on u.CustID equals sct.CustID
                    //                where btc.BusinessTypeID == cityWiseDetails.BusinessTypeID.ToString()
                    //                select new CityWiseDetailedRpt
                    //                {
                    //                    CityID = u.City,
                    //                    CityName = c.VillageLocalityname.Trim(),
                    //                    FirmName = u.FirmName,
                    //                    DealerName = u.CustName,
                    //                    FromDate = u.CreatedDate.ToString(),
                    //                    Mobile1 = u.MobileNumber,
                    //                    StateName = st.StateName,
                    //                    Address = u.BillingAddress,
                    //                    StateID = st.StateID,
                    //                    BusinessTypesList = (from b in dbContext.tblBusinessTypewithCusts
                    //                                         join bt in dbContext.tblBusinessTypes on b.BusinessTypeID
                    //                                         equals bt.BusinessTypeID
                    //                                         where b.CustID == u.CustID
                    //                                         && bt.BusinessTypeID == cityWiseDetails.BusinessTypeID.ToString()
                    //                                         select new BusinessTypes
                    //                                         {
                    //                                             BusinessTypeID = b.BusinessTypeID,
                    //                                             BusinessTypeName = bt.BusinessType.Trim()
                    //                                         }).ToList(),
                    //                    SubCategoryList = (from sp in dbContext.tblSubCategoryProductWithCusts
                    //                                         join s in dbContext.tblSubCategories on sp.SubCategoryId
                    //                                         equals s.SubCategoryId
                    //                                         where sp.CustID == u.CustID
                    //                                         select new SubCat
                    //                                         {
                    //                                             SubCategoryID = sp.SubCategoryId.Value,
                    //                                             SubCategoryName = s.SubCategoryName.Trim()
                    //                                         }).ToList(),
                    //                }).ToList();
                    //}
                    //else if(cityWiseDetails.BusinessTypeID != 0 && cityWiseDetails.SubCategoryID != 0)
                    //{
                    //    CityList = (from u in dbContext.tblCustomerDetails
                    //                join c in dbContext.tblStateWithCities on u.City equals c.StatewithCityID
                    //                join st in dbContext.tblStates on u.State equals st.StateID
                    //                join sc in dbContext.tblSubCategoryProductWithCusts on u.CustID equals sc.CustID
                    //                join btc in dbContext.tblBusinessTypewithCusts on u.CustID equals btc.CustID
                    //                join sct in dbContext.tblSubCategoryProductWithCusts on u.CustID equals sct.CustID
                    //                where btc.BusinessTypeID == cityWiseDetails.BusinessTypeID.ToString()
                    //                && sct.SubCategoryId == cityWiseDetails.SubCategoryID
                    //                select new CityWiseDetailedRpt
                    //                {
                    //                    CityID = u.City,
                    //                    CityName = c.VillageLocalityname.Trim(),
                    //                    FirmName = u.FirmName,
                    //                    DealerName = u.CustName,
                    //                    FromDate = u.CreatedDate.ToString(),
                    //                    Mobile1 = u.MobileNumber,
                    //                    StateName = st.StateName,
                    //                    Address = u.BillingAddress,
                    //                    StateID = st.StateID,
                    //                    BusinessTypesList = (from b in dbContext.tblBusinessTypewithCusts
                    //                                         join bt in dbContext.tblBusinessTypes on b.BusinessTypeID
                    //                                         equals bt.BusinessTypeID
                    //                                         where b.CustID == u.CustID
                    //                                         && bt.BusinessTypeID == cityWiseDetails.BusinessTypeID.ToString()
                    //                                         select new BusinessTypes
                    //                                         {
                    //                                             BusinessTypeID = b.BusinessTypeID,
                    //                                             BusinessTypeName = bt.BusinessType.Trim()
                    //                                         }).ToList(),
                    //                    SubCategoryList = (from sp in dbContext.tblSubCategoryProductWithCusts
                    //                                       join s in dbContext.tblSubCategories on sp.SubCategoryId
                    //                                       equals s.SubCategoryId
                    //                                       where sp.CustID == u.CustID
                    //                                       && sp.SubCategoryId == cityWiseDetails.SubCategoryID
                    //                                       select new SubCat
                    //                                       {
                    //                                           SubCategoryID = sp.SubCategoryId.Value,
                    //                                           SubCategoryName = s.SubCategoryName.Trim()
                    //                                       }).ToList(),
                    //                    //.Select(p => p.BusinessTypeID).ToList(),
                    //                }).Distinct().ToList();
                    //}
                    //else if (cityWiseDetails.BusinessTypeID == 0 && cityWiseDetails.SubCategoryID != 0)
                    //{
                    //    CityList = (from u in dbContext.tblCustomerDetails
                    //                join c in dbContext.tblStateWithCities on u.City equals c.StatewithCityID
                    //                join st in dbContext.tblStates on u.State equals st.StateID
                    //                join sc in dbContext.tblSubCategoryProductWithCusts on u.CustID equals sc.CustID
                    //                join btc in dbContext.tblBusinessTypewithCusts on u.CustID equals btc.CustID
                    //                join sct in dbContext.tblSubCategoryProductWithCusts on u.CustID equals sct.CustID
                    //                where sct.SubCategoryId == cityWiseDetails.SubCategoryID
                    //                select new CityWiseDetailedRpt
                    //                {
                    //                    CityID = u.City,
                    //                    CityName = c.VillageLocalityname.Trim(),
                    //                    FirmName = u.FirmName,
                    //                    DealerName = u.CustName,
                    //                    FromDate = u.CreatedDate.ToString(),
                    //                    Mobile1 = u.MobileNumber,
                    //                    StateName = st.StateName,
                    //                    Address = u.BillingAddress,
                    //                    StateID = st.StateID,
                    //                    BusinessTypesList = (from b in dbContext.tblBusinessTypewithCusts
                    //                                         join bt in dbContext.tblBusinessTypes on b.BusinessTypeID
                    //                                         equals bt.BusinessTypeID
                    //                                         where b.CustID == u.CustID
                    //                                         select new BusinessTypes
                    //                                         {
                    //                                             BusinessTypeID = b.BusinessTypeID,
                    //                                             BusinessTypeName = bt.BusinessType.Trim()
                    //                                         }).ToList(),
                    //                    SubCategoryList = (from sp in dbContext.tblSubCategoryProductWithCusts
                    //                                       join s in dbContext.tblSubCategories on sp.SubCategoryId
                    //                                       equals s.SubCategoryId
                    //                                       where sp.CustID == u.CustID
                    //                                       && sp.SubCategoryId == cityWiseDetails.SubCategoryID
                    //                                       select new SubCat
                    //                                       {
                    //                                           SubCategoryID = sp.SubCategoryId.Value,
                    //                                           SubCategoryName = s.SubCategoryName.Trim()
                    //                                       }).ToList(),
                    //                    //.Select(p => p.BusinessTypeID).ToList(),
                    //                }).Distinct().ToList();
                    //}
                    //else
                    //{
                    //    CityList = (from u in dbContext.tblCustomerDetails
                    //                join c in dbContext.tblStateWithCities on u.City equals c.StatewithCityID
                    //                join st in dbContext.tblStates on u.State equals st.StateID
                    //                select new CityWiseDetailedRpt
                    //                {
                    //                    CityID = u.City,
                    //                    CityName = c.VillageLocalityname.Trim(),
                    //                    FirmName = u.FirmName,
                    //                    DealerName = u.CustName,
                    //                    FromDate = u.CreatedDate.ToString(),
                    //                    Mobile1 = u.MobileNumber,
                    //                    StateName = st.StateName,
                    //                    Address = u.BillingAddress,
                    //                    StateID = st.StateID,
                    //                    BusinessTypesList = (from b in dbContext.tblBusinessTypewithCusts
                    //                                         join bt in dbContext.tblBusinessTypes on b.BusinessTypeID
                    //                                         equals bt.BusinessTypeID
                    //                                         where b.CustID == u.CustID
                    //                                         select new BusinessTypes
                    //                                         {
                    //                                             BusinessTypeID = b.BusinessTypeID,
                    //                                             BusinessTypeName = bt.BusinessType.Trim()
                    //                                         }).ToList(),
                    //                    SubCategoryList = (from sp in dbContext.tblSubCategoryProductWithCusts
                    //                                       join s in dbContext.tblSubCategories on sp.SubCategoryId
                    //                                       equals s.SubCategoryId
                    //                                       where sp.CustID == u.CustID
                    //                                       select new SubCat
                    //                                       {
                    //                                           SubCategoryID = sp.SubCategoryId.Value,
                    //                                           SubCategoryName = s.SubCategoryName.Trim()
                    //                                       }).ToList(),
                    //                    //.Select(p => p.BusinessTypeID).ToList(),
                    //                }).ToList();
                    //}

                    //foreach (var item in CityList)
                    //{
                    //    foreach (var item1 in item.BusinessTypesList)
                    //    {
                    //        item.TypeOfBusiness += item1.BusinessTypeName + ' ';
                    //    }
                    //}

                    if (cityWiseDetails.CustID > 0)
                        CityList = CityList.Where(i => i.CustID == cityWiseDetails.CustID).ToList();

                    if (cityWiseDetails.CityID > 0)
                        CityList = CityList.Where(i => i.CityID == cityWiseDetails.CityID).ToList();

                    if (cityWiseDetails.StateID > 0)
                        CityList = CityList.Where(i => i.StateID == cityWiseDetails.StateID).ToList();

                    return CityList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public string Promotion(PromoWithList promo, List<Attachment> MailAttachments)
        {
            try
            {
                string Result = string.Empty;
                if (promo.IsEmail == true)
                {
                    string Bcc = string.Empty;
                    List<CustomerDetails> bccList = new List<CustomerDetails>();

                    foreach (var item in promo.detailedList)
                    {
                        if (!string.IsNullOrEmpty(item.EmailID))
                        {
                            if (item.IsChecked == true)
                            {
                                CustomerDetails custDetails1 = new CustomerDetails();
                                custDetails1.EmailID = item.EmailID;
                                bccList.Add(custDetails1);
                            }
                        }
                    }

                    string ToEmailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                    string FromMailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                    string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
                    string MailServerHost = ConfigurationManager.AppSettings["MailServerHost"].ToString();
                    string SendingPort = ConfigurationManager.AppSettings["SendingPort"].ToString();
                    //string APKPath = ConfigurationManager.AppSettings["APKPath"].ToString();
                    string MailSubject = promo.MailSubject;

                    Helper.SendMail(ToEmailID, FromMailID, promo.MailBody, MailSubject, MailServerHost, MailPassword, SendingPort, bccList, MailAttachments);
                    Result = "Email Sent Successfully!!";
                }
                else if (promo.IsSMS == true)
                {
                    string SMSUserName = ConfigurationManager.AppSettings["SMSUserName"];
                    string SMSPassword = ConfigurationManager.AppSettings["SMSPassword"];

                    foreach (var item in promo.detailedList)
                    {
                        if (item.IsChecked == true)
                        {
                            string Message = "Welcome to MWB Technology New customer details name Your OTP is : 123456.ID Test Ph Tets";
                            Helper.SendSMS(SMSUserName, SMSPassword, item.Mobile1, Message, "N");
                        }
                    }
                    Result = "SMS Sent Successfully!!";
                }
                return Result;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return "Error!! Please contact administrator";
            }
        }
    }
}
