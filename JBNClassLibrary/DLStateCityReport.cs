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
    public class StateCityRpt
    {
        public string CityName { get; set; }
        public int CityID { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalCities { get; set; }
        public int[] StateList { get; set; }
        public int[] CityList { get; set; }
        public bool IsChecked { get; set; }
    }
    public class ConsolidatedStateCityReport
    {
        public int TotalStates { get; set; }
        public int TotalCities { get; set; }
        public int TotalCustomers { get; set; }
        public List<StateCityRpt> stateCityRpts { get; set; }
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
    public class DLStateCityReport
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public ConsolidatedStateCityReport GetStateReport(StateCityRpt stateCityRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    ConsolidatedStateCityReport consolidatedReport = new ConsolidatedStateCityReport();
                    List<StateCityRpt> stateCityRpts = new List<StateCityRpt>();

                    DateTime? dtFromDate = Convert.ToDateTime(stateCityRpt.FromDate);
                    DateTime? dtToDate = Convert.ToDateTime(stateCityRpt.ToDate);
                    IEnumerable<CustomerDetails> customerList = new List<CustomerDetails>();
                    customerList = (from c in dbContext.tblCustomerDetails
                                    where c.State != null && c.City != null
                                    select new CustomerDetails
                                    {
                                        //CustID = c.CustID,
                                        CustID = c.ID,
                                        CreatedDate = c.CreatedDate,
                                        State = c.State.Value,
                                        City = c.City.Value,
                                        IsActive = c.IsActive,
                                    }).AsEnumerable();

                    if (!string.IsNullOrEmpty(stateCityRpt.FromDate) && !string.IsNullOrEmpty(stateCityRpt.ToDate))
                    {
                        customerList = customerList.Where(c => c.CreatedDate.Value.Date >= dtFromDate.Value.Date && c.CreatedDate.Value.Date <= dtFromDate.Value.Date).ToList();
                        
                        stateCityRpts = (from u in customerList
                                         join ct in dbContext.tblStateWithCities on u.City equals ct.ID
                                         join btc in dbContext.tblBusinessTypewithCusts on u.CustID equals btc.CustID
                                         where u.State.HasValue && u.City.HasValue
                                         select new StateCityRpt
                                         {
                                             CityID = u.City.Value,
                                             StateID = u.State.Value,
                                             TotalCustomers = (from a in dbContext.tblCustomerDetails
                                                               where a.State == u.State
                                                               select new { a.ID }).Distinct().Count(),
                                             TotalCities = (from sc in dbContext.tblStateWithCities
                                                            where sc.StateID == u.State
                                                            select new { sc.ID }).Distinct().Count(),
                                             StateName = (from c in dbContext.tblStates
                                                          where c.ID == u.State.Value
                                                          select c.StateName).FirstOrDefault(),
                                         }).Distinct().ToList();
                    }
                    else
                    {
                        stateCityRpts = (from u in dbContext.tblCustomerDetails
                                         join ct in dbContext.tblStateWithCities on u.City equals ct.ID
                                         join btc in dbContext.tblBusinessTypewithCusts on u.ID equals btc.CustID
                                         where u.State.HasValue && u.City.HasValue
                                         select new StateCityRpt
                                         {
                                             CityID = u.City.Value,
                                             StateID = u.State.Value,
                                             TotalCustomers = (from a in dbContext.tblCustomerDetails
                                                               where a.State == u.State
                                                               select new { a.ID }).Distinct().Count(),
                                             TotalCities = (from sc in dbContext.tblStateWithCities
                                                            where sc.StateID == u.State
                                                            select new { sc.ID }).Distinct().Count(),
                                             StateName = (from c in dbContext.tblStates
                                                          where c.ID == u.State.Value
                                                          select c.StateName).FirstOrDefault(),
                                         }).Distinct().ToList();
                    }
                    stateCityRpts = stateCityRpts.GroupBy(ac => new
                    {
                        ac.StateID,
                    }).Select(g => new StateCityRpt()
                    {
                        StateID = g.Select(i => i.StateID).FirstOrDefault(),
                        StateName = g.Select(i => i.StateName).FirstOrDefault(),
                        TotalCities = (from sc in dbContext.tblStateWithCities
                                       join c in dbContext.tblCustomerDetails on sc.ID equals c.City
                                       where c.State == g.Key.StateID
                                       select sc.ID).Distinct().Count(),
                        TotalCustomers = (from sc in dbContext.tblCustomerDetails
                                          where sc.State == g.Key.StateID
                                          select sc.ID).Distinct().Count(),
                    }).Distinct().ToList();

                    if (stateCityRpt.StateList != null && stateCityRpt.StateList.Count() > 0)
                    {
                        stateCityRpts = stateCityRpts.Where(m => stateCityRpt.StateList.Contains(m.StateID)).ToList();

                        consolidatedReport.TotalStates = (from c in customerList 
                                                          join cpc in dbContext.tblCategoryProductWithCusts on c.CustID equals cpc.CustID
                                                          where c.IsActive == true
                                                          && stateCityRpt.StateList.Contains(c.State.Value)
                                                          select c.State
                                                                  ).Distinct().Count();
                        consolidatedReport.TotalCities = (from c in customerList
                                                          join cpc in dbContext.tblCategoryProductWithCusts on c.CustID equals cpc.CustID
                                                          where c.IsActive == true
                                                          && stateCityRpt.StateList.Contains(c.State.Value)
                                                          select c.City
                                                              ).Distinct().Count();
                        consolidatedReport.TotalCustomers = (from c in customerList
                                                             join cpc in dbContext.tblCategoryProductWithCusts on c.CustID equals cpc.CustID
                                                             where c.IsActive == true
                                                             && stateCityRpt.StateList.Contains(c.State.Value)
                                                             select c.CustID
                                                              ).Distinct().Count();
                    }
                    else
                    {
                        consolidatedReport.TotalStates = (from c in customerList
                                                          join cpc in dbContext.tblCategoryProductWithCusts on c.CustID equals cpc.CustID
                                                          where c.IsActive == true
                                                          select c.State).Distinct().Count();
                        consolidatedReport.TotalCities = (from c in customerList
                                                          join cpc in dbContext.tblCategoryProductWithCusts on c.CustID equals cpc.CustID
                                                          where c.IsActive == true
                                                          select c.City).Distinct().Count();
                        consolidatedReport.TotalCustomers = (from c in customerList
                                                             join cpc in dbContext.tblCategoryProductWithCusts on c.CustID equals cpc.CustID
                                                             where c.IsActive == true
                                                             select c.CustID).Distinct().Count();
                    }
                    consolidatedReport.stateCityRpts = stateCityRpts;
                    return consolidatedReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public ConsolidatedStateCityReport GetCityReport(StateCityRpt stateCityRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    ConsolidatedStateCityReport consolidatedReport = new ConsolidatedStateCityReport();
                    List<StateCityRpt> stateCityRpts = new List<StateCityRpt>();
                    DateTime? dtFromDate = Convert.ToDateTime(stateCityRpt.FromDate);
                    DateTime? dtToDate = Convert.ToDateTime(stateCityRpt.ToDate);

                    IEnumerable<CustomerDetails> customerList = new List<CustomerDetails>();
                    customerList = (from c in dbContext.tblCustomerDetails
                                    where c.State != null && c.City != null
                                    select new CustomerDetails
                                    {
                                        //CustID = c.CustID,
                                        CustID = c.ID,
                                        CreatedDate = c.CreatedDate,
                                        State = c.State.Value,
                                        City = c.City.Value,
                                        IsActive = c.IsActive,
                                    }).AsEnumerable();

                    if(!string.IsNullOrEmpty(stateCityRpt.FromDate) && !string.IsNullOrEmpty(stateCityRpt.ToDate))
                    {
                        customerList = customerList.Where(c => c.CreatedDate.Value.Date >= dtFromDate.Value.Date && c.CreatedDate.Value.Date <= dtFromDate.Value.Date).ToList();

                        stateCityRpts = (from u in customerList
                                         join ct in dbContext.tblStateWithCities on u.City equals ct.ID
                                         join btc in dbContext.tblBusinessTypewithCusts on u.CustID equals btc.CustID
                                         where u.State != null && u.City != null
                                         select new StateCityRpt
                                         {
                                             CityID = u.City.Value,
                                             StateID = u.State.Value,
                                             TotalCustomers = (from a in dbContext.tblCustomerDetails
                                                               where a.State == u.State
                                                               select new { a.ID }).Distinct().Count(),
                                             StateName = (from c in dbContext.tblStates
                                                          where c.StateID == u.State.Value
                                                          select c.StateName).FirstOrDefault(),
                                             CityName = ct.VillageLocalityName,
                                         }).Distinct().ToList();
                    }
                    else
                    {
                        stateCityRpts = (from u in dbContext.tblCustomerDetails
                                         join ct in dbContext.tblStateWithCities on u.City equals ct.ID
                                         join btc in dbContext.tblBusinessTypewithCusts on u.ID equals btc.CustID
                                         where u.State != null && u.City != null
                                         select new StateCityRpt
                                         {
                                             CityID = u.City.Value,
                                             StateID = u.State.Value,
                                             TotalCustomers = (from a in dbContext.tblCustomerDetails
                                                               where a.State == u.State
                                                               select new { a.ID }).Distinct().Count(),
                                             StateName = (from c in dbContext.tblStates
                                                          where c.ID == u.State.Value
                                                          select c.StateName).FirstOrDefault(),
                                             CityName = ct.VillageLocalityName,
                                         }).Distinct().ToList();
                    }

                    stateCityRpts = stateCityRpts.GroupBy(ac => new
                    {
                        ac.CityID,
                    }).Select(g => new StateCityRpt()
                    {
                        StateID = g.Select(i => i.StateID).FirstOrDefault(),
                        StateName = g.Select(i => i.StateName).FirstOrDefault(),
                        TotalCustomers = (from sc in dbContext.tblCustomerDetails
                                          where sc.City == g.Key.CityID
                                          select sc.ID).Distinct().Count(),
                        CityName = g.Select(i => i.CityName).FirstOrDefault(),
                        CityID = g.Select(i => i.CityID).FirstOrDefault(),
                    }).Distinct().ToList();

                    if (stateCityRpt.StateList != null && stateCityRpt.StateList.Count() > 0)
                    {
                        stateCityRpts = stateCityRpts.Where(m => stateCityRpt.StateList.Contains(m.StateID)).ToList();
                    }
                    if (stateCityRpt.CityList != null && stateCityRpt.CityList.Count() > 0)
                    {
                        stateCityRpts = stateCityRpts.Where(m => stateCityRpt.CityList.Contains(m.CityID)).ToList();
                    }

                    if ((stateCityRpt.StateList != null && stateCityRpt.StateList.Count() > 0) || (stateCityRpt.CityList != null && stateCityRpt.CityList.Count() > 0))
                    {
                        if ((stateCityRpt.StateList != null && stateCityRpt.StateList.Count() > 0) && (stateCityRpt.CityList != null && stateCityRpt.CityList.Count() > 0))
                        {
                            consolidatedReport.TotalStates = (from cpc in dbContext.tblCategoryProductWithCusts
                                                              join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                              where c.IsActive == true && c.State != null
                                                              && stateCityRpt.StateList.Contains(c.State.Value)
                                                              && stateCityRpt.CityList.Contains(c.City.Value)
                                                              select c.State
                                                                  ).Distinct().Count();
                            consolidatedReport.TotalCities = (from cpc in dbContext.tblCategoryProductWithCusts
                                                              join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                              where c.IsActive == true && c.City != null
                                                              && stateCityRpt.StateList.Contains(c.State.Value)
                                                              && stateCityRpt.CityList.Contains(c.City.Value)
                                                              select c.City
                                                                  ).Distinct().Count();
                            consolidatedReport.TotalCustomers = (from cpc in dbContext.tblCategoryProductWithCusts
                                                                 join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                 where c.IsActive == true
                                                                 && stateCityRpt.StateList.Contains(c.State.Value)
                                                                 && stateCityRpt.CityList.Contains(c.City.Value)
                                                                 //select c.CustID
                                                                 select c.ID
                                                                  ).Distinct().Count();
                        }
                        else if ((stateCityRpt.StateList == null || stateCityRpt.StateList.Count() <= 0) && (stateCityRpt.CityList != null && stateCityRpt.CityList.Count() > 0))
                        {
                            consolidatedReport.TotalStates = (from cpc in dbContext.tblCategoryProductWithCusts
                                                              join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                              where c.IsActive == true && c.State != null
                                                              && stateCityRpt.CityList.Contains(c.City.Value)
                                                              select c.State
                                                                  ).Distinct().Count();
                            consolidatedReport.TotalCities = (from cpc in dbContext.tblCategoryProductWithCusts
                                                              join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                              where c.IsActive == true && c.City != null
                                                              && stateCityRpt.CityList.Contains(c.City.Value)
                                                              select c.City
                                                                  ).Distinct().Count();
                            consolidatedReport.TotalCustomers = (from cpc in dbContext.tblCategoryProductWithCusts
                                                                 join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                 where c.IsActive == true
                                                                 && stateCityRpt.CityList.Contains(c.City.Value)
                                                                 //select c.CustID
                                                                 select c.ID
                                                                  ).Distinct().Count();
                        }
                        else if ((stateCityRpt.StateList != null && stateCityRpt.StateList.Count() > 0) && (stateCityRpt.CityList == null || stateCityRpt.CityList.Count() <= 0))
                        {
                            consolidatedReport.TotalStates = (from cpc in dbContext.tblCategoryProductWithCusts
                                                              join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                              where c.IsActive == true && c.State != null
                                                              && stateCityRpt.StateList.Contains(c.State.Value)
                                                              select c.State
                                                                  ).Distinct().Count();
                            consolidatedReport.TotalCities = (from cpc in dbContext.tblCategoryProductWithCusts
                                                              join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                              where c.IsActive == true && c.City != null
                                                              && stateCityRpt.StateList.Contains(c.State.Value)
                                                              select c.City
                                                                  ).Distinct().Count();
                            consolidatedReport.TotalCustomers = (from cpc in dbContext.tblCategoryProductWithCusts
                                                                 join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                                 where c.IsActive == true
                                                                 && stateCityRpt.StateList.Contains(c.State.Value)
                                                                 select c.ID
                                                                  ).Distinct().Count();
                        }
                    }
                    else
                    {
                        consolidatedReport.TotalStates = (from cpc in dbContext.tblCategoryProductWithCusts
                                                          join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                          where c.IsActive == true && c.State != null
                                                          select c.State
                                                                  ).Distinct().Count();
                        consolidatedReport.TotalCities = (from cpc in dbContext.tblCategoryProductWithCusts
                                                          join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                          where c.IsActive == true && c.City != null
                                                          select c.City
                                                              ).Distinct().Count();
                        consolidatedReport.TotalCustomers = (from cpc in dbContext.tblCategoryProductWithCusts
                                                             join c in dbContext.tblCustomerDetails on cpc.CustID equals c.ID
                                                             where c.IsActive == true
                                                             select c.ID
                                                              ).Distinct().Count();
                    }

                    consolidatedReport.stateCityRpts = stateCityRpts;
                    return consolidatedReport;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<City> GetAllCities()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<City> cities = new List<City>();

                    cities = (from c in dbContext.tblCustomerDetails
                              join ct in dbContext.tblStateWithCities on c.City equals ct.ID
                              select new City
                              {
                                  StateWithCityID = ct.ID,
                                  VillageLocalityName = ct.VillageLocalityName
                              }).Distinct().ToList();
                    return cities;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public string Promotion(ConsolidatedStateCityReport consolidatedReport, int UserID, int PrmotionType, List<Attachment> MailAttachments, string ImageURL)
        {
            try
            {
                string Result = string.Empty;
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                JBNDBClass jBNDBClass = new JBNDBClass();
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (consolidatedReport.IsEmail == true)
                    {
                        #region Email 
                        string Bcc = string.Empty;
                        List<CustomerDetails> bccList = new List<CustomerDetails>();

                        foreach (var item in consolidatedReport.stateCityRpts)
                        {
                            //State
                            if (item.IsChecked == true)
                            {
                                if (PrmotionType == 1)
                                {
                                    List<CustomerDetails> customer = (from c in dbContext.tblCustomerDetails
                                                                      join ct in dbContext.tblStates on c.State equals ct.ID
                                                                      where c.IsActive == true
                                                                      && c.State == item.StateID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }).Distinct().ToList();
                                    foreach (var custItem in customer)
                                    {
                                        if (!string.IsNullOrEmpty(custItem.EmailID))
                                        {
                                            bccList.Add(custItem);
                                        }
                                    }
                                }
                                //City
                                else if (PrmotionType == 2)
                                {
                                    List<CustomerDetails> customer = (from sc in dbContext.tblStateWithCities
                                                                      join c in dbContext.tblCustomerDetails on sc.ID equals c.City
                                                                      where c.IsActive == true
                                                                      && sc.ID == item.CityID
                                                                      select new CustomerDetails
                                                                      {
                                                                          //CustID = c.CustID,
                                                                          CustID = c.ID,
                                                                          MobileNumber = c.MobileNumber,
                                                                          EmailID = c.EmailID,
                                                                      }).Distinct().ToList();
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
                        foreach (var item in consolidatedReport.stateCityRpts)
                        {
                            //Main Category
                            if (PrmotionType == 1)
                            {
                                if (item.IsChecked == true)
                                {
                                    List<CustomerDetails> customer = (from c in dbContext.tblCustomerDetails
                                                                      join ct in dbContext.tblStates on c.State equals ct.ID
                                                                      where c.IsActive == true
                                                                      && c.State == item.StateID
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
                            }
                            else if (PrmotionType == 2)
                            {
                                List<CustomerDetails> customer = (from sc in dbContext.tblStateWithCities
                                                                  join c in dbContext.tblCustomerDetails on sc.ID equals c.City
                                                                  where c.IsActive == true
                                                                  && sc.ID == item.CityID
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
                        }
                        #endregion
                    }
                    else if(consolidatedReport.IsNotification == true)
                    {
                        #region Notification 
                        string Bcc = string.Empty;
                        List<CustomerDetails> bccList = new List<CustomerDetails>();

                        foreach (var item in consolidatedReport.stateCityRpts)
                        {
                            //State
                            if (item.IsChecked == true)
                            {
                                if (PrmotionType == 1)
                                {
                                    List<CustomerDetails> customer = (from c in dbContext.tblCustomerDetails
                                                                      join ct in dbContext.tblStates on c.State equals ct.ID
                                                                      where c.IsActive == true
                                                                      && c.State == item.StateID
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
                                //City
                                else if (PrmotionType == 2)
                                {
                                    List<CustomerDetails> customer = (from sc in dbContext.tblStateWithCities
                                                                      join c in dbContext.tblCustomerDetails on sc.ID equals c.City
                                                                      where c.IsActive == true
                                                                      && sc.ID == item.CityID
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
                            }
                        }

                        Result = "Success";
                        #endregion
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

    }
}
