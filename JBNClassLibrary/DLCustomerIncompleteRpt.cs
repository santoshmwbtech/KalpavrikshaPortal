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
using System.Web.Mvc;

namespace JBNClassLibrary
{
    public class CustomerIncompleteRpt
    {
        public int CustID { get; set; }
        public string FirmName { get; set; }
        [Required(ErrorMessage = "Please enter owner name")]
        public string OwnerName { get; set; }
        public string EmailID { get; set; }
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Please select City")]
        public int? CityID { get; set; }
        [Required(ErrorMessage = "Please select State")]
        public int? StateID { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string SearchByOption { get; set; }
        public bool IsChecked { get; set; }
        [Required(ErrorMessage = "Please select Business Types")]
        public int[] BusinessTypeList { get; set; }
        [Required(ErrorMessage = "Please select Products")]
        public int[] SubcategoryList { get; set; }
        public string BillingAddress { get; set; }
        public string Area { get; set; }
        public string Pincode { get; set; }
        public bool City { get; set; }
        public bool District { get; set; }
        public bool State { get; set; }
        public bool National { get; set; }
        public string DeviceID { get; set; }
        public List<BusinessTypes> BusinessTypes { get; set; }
        public List<BusinessDemand> BusinessDemands { get; set; }
    }
    public class PromoWithCustomerIncompleteRpt
    {
        public List<CustomerIncompleteRpt> customerIncompleteRpts { get; set; }
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
        public string Result { get; set; }
        public int SMSTemplateID { get; set; }
    }
    public class DLCustomerIncompleteRpt
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public List<CustomerIncompleteRpt> GetCustomerList(CustomerIncompleteRpt search)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    List<CustomerIncompleteRpt> customerList = new List<CustomerIncompleteRpt>();
                    customerList = (from c in dbContext.tblCustomerDetails
                                    where c.IsActive == true && c.IsRegistered.HasValue && c.IsRegistered.Value == 0
                                    select new CustomerIncompleteRpt
                                    {
                                        DeviceID = c.DeviceID,
                                        CustID = c.ID,
                                        OwnerName = c.CustName,
                                        FirmName = c.FirmName,
                                        MobileNumber = c.MobileNumber,
                                        EmailID = c.EmailID,
                                        CityID = c.City,
                                        StateID = c.State,
                                        CityName = (dbContext.tblStateWithCities.Where(sc => sc.ID == c.City)).FirstOrDefault().VillageLocalityName,
                                        StateName = (dbContext.tblStates.Where(sc => sc.ID == c.State)).FirstOrDefault().StateName,
                                    }).ToList();

                    if (!string.IsNullOrEmpty(search.SearchByOption))
                    {
                        if (search.SearchByOption.ToLower() == "ownername")
                            customerList = customerList.Where(c => c.OwnerName == "" || c.OwnerName == null).ToList();

                        if (search.SearchByOption.ToLower() == "state")
                            customerList = customerList.Where(c => c.StateID == null || c.StateID == 0).ToList();

                        if (search.SearchByOption.ToLower() == "city")
                            customerList = customerList.Where(c => c.CityID == null || c.CityID == 0).ToList();
                        if (search.SearchByOption.ToLower() == "kyc")
                            customerList = customerList.Where(c => c.CityID == null || c.CityID == 0 || c.StateID == null || c.StateID == 0).ToList();
                    }

                    return customerList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return new List<CustomerIncompleteRpt>();
            }
        }
        public CustomerIncompleteRpt GetCustomerDetails(int? CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    CustomerIncompleteRpt customerDetails = new CustomerIncompleteRpt();
                    customerDetails = (from c in dbContext.tblCustomerDetails
                                       where c.IsActive == true
                                        && c.ID == CustID
                                       select new CustomerIncompleteRpt
                                       {
                                           //CustID = c.CustID,
                                           CustID = c.ID,
                                           OwnerName = c.CustName,
                                           FirmName = c.FirmName,
                                           MobileNumber = c.MobileNumber,
                                           EmailID = c.EmailID,
                                           CityID = c.City,
                                           StateID = c.State,
                                           CityName = (dbContext.tblStateWithCities.Where(sc => sc.ID == c.City)).FirstOrDefault().VillageLocalityName,
                                           StateName = (dbContext.tblStates.Where(sc => sc.ID == c.State)).FirstOrDefault().StateName,
                                           State = c.InterstState == null ? false : c.InterstState.Value,
                                           City = c.InterstCity == null ? false : c.InterstCity.Value,
                                           District = c.InterstDistrict,
                                           National = c.InterstCountry == null ? false : c.InterstCountry.Value,
                                       }).FirstOrDefault();

                    customerDetails.BusinessTypes = (from b in dbContext.tblBusinessTypes
                                                     select new BusinessTypes
                                                     {
                                                         ID = b.ID,
                                                         BusinessTypeName = b.Type,
                                                         Checked = dbContext.tblBusinessTypewithCusts.Where(bt => bt.CustID == customerDetails.CustID).Any(btc => btc.BusinessTypeID == b.ID)
                                                     }).ToList();
                    if (!customerDetails.BusinessTypes.Where(b => b.ID == 6).FirstOrDefault().Checked)
                    {
                        customerDetails.BusinessDemands = (from b in dbContext.tblBusinessDemands
                                                           select new BusinessDemand
                                                           {
                                                               Demand = b.Demand,
                                                               ID = b.ID,
                                                               IsChecked = b.ID == 3 ? false : true
                                                           }).ToList();
                    }

                    return customerDetails;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public PromoWithCustomerIncompleteRpt BlockCustomer(int? CustID, int UserID)
        {
            try
            {
                PromoWithCustomerIncompleteRpt Result = new PromoWithCustomerIncompleteRpt();

                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    var user = (from u in dbContext.tblCustomerDetails.AsNoTracking()
                                    //where u.CustID == CustID
                                where u.ID == CustID
                                select u).FirstOrDefault();

                    if (user == null)
                    {
                        Result.Result = "Invalid User";
                        return Result;
                    }

                    tblCustomerDetail tblCustomerDetails = new tblCustomerDetail();
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    //tblCustomerDetails.CustID = CustID.Value;
                    tblCustomerDetails.ID = CustID.Value;
                    tblCustomerDetails.IsActive = false;
                    tblCustomerDetails.ModifiedBy = UserID;
                    tblCustomerDetails.ModifiedDate = DateTimeNow;
                    dbContext.tblCustomerDetails.Attach(tblCustomerDetails);
                    dbContext.Entry(tblCustomerDetails).Property(c => c.IsActive).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.ModifiedDate).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.ModifiedBy).IsModified = true;
                    dbContext.SaveChanges();
                    Result.Result = "success";
                    return Result;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                PromoWithCustomerIncompleteRpt Result = new PromoWithCustomerIncompleteRpt();
                Result.Result = "Error while blocking the customer..Please try again later";
                return Result;
            }
        }
        public bool UpdateCustomerDetails(CustomerIncompleteRpt incompleteRpt, int UserID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    var user = (from u in dbContext.tblCustomerDetails.AsNoTracking()
                                    //where u.CustID == incompleteRpt.CustID
                                where u.ID == incompleteRpt.CustID
                                select u).FirstOrDefault();

                    if (user == null)
                    {
                        return false;
                    }

                    tblCustomerDetail tblCustomerDetails = new tblCustomerDetail();
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    //tblCustomerDetails.CustID = incompleteRpt.CustID;
                    tblCustomerDetails.ID = incompleteRpt.CustID;
                    tblCustomerDetails.ModifiedBy = UserID;
                    tblCustomerDetails.ModifiedDate = DateTimeNow;
                    tblCustomerDetails.FirmName = incompleteRpt.FirmName;
                    tblCustomerDetails.MobileNumber = incompleteRpt.MobileNumber;
                    tblCustomerDetails.CustName = incompleteRpt.OwnerName;
                    tblCustomerDetails.EmailID = incompleteRpt.EmailID;

                    dbContext.tblCustomerDetails.Attach(tblCustomerDetails);
                    dbContext.Entry(tblCustomerDetails).Property(c => c.ModifiedDate).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.ModifiedBy).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.FirmName).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.MobileNumber).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.CustName).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.EmailID).IsModified = true;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return false;
            }
        }
        public bool UpdateCustomerDetailsKYC(CustomerIncompleteRpt incompleteRpt, int UserID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        var user = (from u in dbContext.tblCustomerDetails.AsNoTracking()
                                    where u.ID == incompleteRpt.CustID
                                    select u).FirstOrDefault();

                        if (user == null)
                        {
                            return false;
                        }
                        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        user.ModifiedBy = incompleteRpt.CustID;//
                        user.ModifiedDate = DateTimeNow;//
                        user.FirmName = incompleteRpt.FirmName;
                        user.MobileNumber = incompleteRpt.MobileNumber;
                        user.CustName = incompleteRpt.OwnerName;
                        user.EmailID = incompleteRpt.EmailID;
                        user.BillingAddress = incompleteRpt.BillingAddress;
                        user.Area = incompleteRpt.Area;
                        user.Pincode = incompleteRpt.Pincode;
                        if (incompleteRpt.StateID != null)
                            user.State = incompleteRpt.StateID;
                        if (incompleteRpt.CityID != null)
                            user.City = incompleteRpt.CityID;

                        user.InterstCountry = incompleteRpt.National;
                        user.InterstState = incompleteRpt.State;
                        user.InterstCity = incompleteRpt.City;
                        user.IsRegistered = 1;

                        if (incompleteRpt.BusinessTypeList != null)
                        {
                            dbContext.tblBusinessTypewithCusts.RemoveRange(dbContext.tblBusinessTypewithCusts.Where(x => x.CustID == incompleteRpt.CustID));
                            foreach (var item in incompleteRpt.BusinessTypeList)
                            {
                                tblBusinessTypewithCust tblBusinessTypeWithCustomer = new tblBusinessTypewithCust();
                                tblBusinessTypeWithCustomer.BusinessTypeID = item;
                                tblBusinessTypeWithCustomer.CustID = incompleteRpt.CustID;
                                tblBusinessTypeWithCustomer.CreatedBy = incompleteRpt.CustID;
                                tblBusinessTypeWithCustomer.CreatedDate = DateTimeNow;
                                dbContext.tblBusinessTypewithCusts.Add(tblBusinessTypeWithCustomer);
                            }
                        }
                        if (incompleteRpt.SubcategoryList != null)
                        {
                            //insert main category
                            List<int> CategoryProductIDS = new List<int>();
                            var subCategoryWithCust = (from c in dbContext.tblCategoryProducts
                                                       join s in dbContext.tblSubCategories on c.ID equals s.CategoryProductID
                                                       where incompleteRpt.SubcategoryList.Contains(s.ID)
                                                       select new SubCategoryTypeWithCust
                                                       {
                                                           CategoryProductID = c.ID,
                                                           SubCategoryId = s.ID,
                                                       }).Distinct().ToList();
                            CategoryProductIDS = subCategoryWithCust.Select(s => s.CategoryProductID.Value).Distinct().ToList();

                            if (CategoryProductIDS != null && CategoryProductIDS.Count > 0)
                            {
                                dbContext.tblCategoryProductWithCusts.RemoveRange(dbContext.tblCategoryProductWithCusts.Where(x => x.CustID == incompleteRpt.CustID));
                                foreach (var item in CategoryProductIDS)
                                {
                                    tblCategoryProductWithCust tblCategoryProductWithCustomer = new tblCategoryProductWithCust();
                                    tblCategoryProductWithCustomer.CategoryProductID = item;
                                    tblCategoryProductWithCustomer.CustID = incompleteRpt.CustID;
                                    tblCategoryProductWithCustomer.CreatedDate = DateTimeNow;
                                    tblCategoryProductWithCustomer.CreatedBy = incompleteRpt.CustID;
                                    dbContext.tblCategoryProductWithCusts.Add(tblCategoryProductWithCustomer);
                                }
                            }
                            //insert main category

                            dbContext.tblSubCategoryProductWithCusts.RemoveRange(dbContext.tblSubCategoryProductWithCusts.Where(x => x.CustID == incompleteRpt.CustID));
                            foreach (var item in subCategoryWithCust)
                            {
                                tblSubCategoryProductWithCust tblSubCategoryProductWithCustomer = new tblSubCategoryProductWithCust();
                                tblSubCategoryProductWithCustomer.CategoryProductID = item.CategoryProductID;
                                tblSubCategoryProductWithCustomer.CustID = incompleteRpt.CustID;
                                tblSubCategoryProductWithCustomer.CreatedBy = incompleteRpt.CustID;
                                tblSubCategoryProductWithCustomer.SubCategoryId = item.SubCategoryId;
                                tblSubCategoryProductWithCustomer.CreatedDate = DateTimeNow;
                                dbContext.tblSubCategoryProductWithCusts.Add(tblSubCategoryProductWithCustomer);
                            }
                        }

                        dbContext.Entry(user).State = EntityState.Modified;
                        dbContext.SaveChanges();
                        dbcxtransaction.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return false;
            }
        }
        public List<SubCat> GetAllSubCategories()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<SubCat> subCatList = new List<SubCat>();
                    subCatList = (from subcat in dbContext.ProductsViews
                                  select new SubCat
                                  {
                                      MainCategoryName = subcat.MainCategoryName,
                                      ID = subcat.SubCategoryID,
                                      SubCategoryName = subcat.ChildCategoryName + " (" + subcat.SubCategoryName + ")",
                                  }).Distinct().ToList();

                    subCatList = subCatList.Where(d => !d.MainCategoryName.ToLower().Contains("professionals")).ToList();
                    return subCatList.OrderBy(d => d.SubCategoryName).ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<CustomerIncompleteRpt> GetUsers()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    var customersByCategory = dbContext.tblCategoryProductWithCusts.Select(c => c.CustID).Distinct().ToList();

                    List<CustomerIncompleteRpt> customerList = new List<CustomerIncompleteRpt>();
                    customerList = (from c in dbContext.tblCustomerDetails
                                        //join bt in dbContext.tblBusinessTypewithCusts on c.ID equals bt.CustID
                                    where c.IsActive == true && c.IsRegistered.HasValue && c.IsRegistered.Value == 1
                                    && !customersByCategory.Contains(c.ID)
                                    //&& bt.BusinessTypeID != 6
                                    select new CustomerIncompleteRpt
                                    {
                                        DeviceID = c.DeviceID,
                                        CustID = c.ID,
                                        OwnerName = c.CustName,
                                        FirmName = c.FirmName,
                                        MobileNumber = c.MobileNumber,
                                        EmailID = c.EmailID,
                                        CityID = c.City,
                                        StateID = c.State,
                                        CityName = (dbContext.tblStateWithCities.Where(sc => sc.ID == c.City)).FirstOrDefault().VillageLocalityName,
                                        StateName = (dbContext.tblStates.Where(sc => sc.ID == c.State)).FirstOrDefault().StateName,
                                    }).ToList();
                    return customerList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return new List<CustomerIncompleteRpt>();
            }
        }

        public string Promotion(PromoWithCustomerIncompleteRpt promo, List<Attachment> MailAttachments, string ImageURL, int UserID)
        {
            try
            {
                string Result = string.Empty;
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                if (promo.IsEmail == true)
                {
                    string Bcc = string.Empty;
                    List<CustomerDetails> bccList = new List<CustomerDetails>();

                    foreach (var item in promo.customerIncompleteRpts)
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
                    string MobileNumbers = string.Join(",", promo.customerIncompleteRpts.Where(b => b.IsChecked == true).Select(c => c.MobileNumber));

                    string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                    string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                    string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                    Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, promo.SMSBody, SenderID);
                }
                else if (promo.IsNotification == true)
                {
                    string[] Registration_Ids = promo.customerIncompleteRpts.Where(b => b.IsChecked == true).Select(c => c.DeviceID).ToArray();
                    int[] Cust_Ids = promo.customerIncompleteRpts.Where(b => b.IsChecked == true).Select(c => c.CustID).ToArray();
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
                return Result;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return "Error!! Please contact administrator";
            }
        }

        public List<childcategory> GetProducts(int? CustID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<childcategory> subCatList = new List<childcategory>();
                    subCatList = (from pView in dbContext.ProductsViews
                                  select new childcategory
                                  {
                                      ID = pView.SubCategoryID,
                                      MainCategoryName = pView.MainCategoryName,
                                      SubCategoryName = pView.SubCategoryName + " (" + pView.MainCategoryName + ")",
                                  }).Distinct().ToList();

                    //var businessTypes = dbContext.tblBusinessTypewithCusts.Where(b => b.CustID == CustID).ToList();

                    //if (businessTypes.Any(b => b.BusinessTypeID == 6))
                    //{
                    //    subCatList = subCatList.FindAll(x => x.MainCategoryName.ToLower().Contains("professionals")).Distinct().ToList();
                    //}
                    //else
                    //{
                    //    subCatList = subCatList.FindAll(x => !x.MainCategoryName.ToLower().Contains("professionals")).Distinct().ToList();
                    //}

                    return subCatList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public string UpdateCustomerProduct(CustomerIncompleteRpt incompleteRpt)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        var user = (from u in dbContext.tblCustomerDetails.AsNoTracking()
                                    where u.ID == incompleteRpt.CustID
                                    select u).FirstOrDefault();

                        if (user == null)
                        {
                            return "User Not Found!!";
                        }

                        tblCustomerDetail tblCustomerDetails = new tblCustomerDetail();
                        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        tblCustomerDetails.ID = incompleteRpt.CustID;
                        tblCustomerDetails.ModifiedBy = incompleteRpt.CustID;
                        tblCustomerDetails.ModifiedDate = DateTimeNow;
                        dbContext.tblCustomerDetails.Attach(tblCustomerDetails);
                        dbContext.Entry(tblCustomerDetails).Property(c => c.ModifiedDate).IsModified = true;
                        dbContext.Entry(tblCustomerDetails).Property(c => c.ModifiedBy).IsModified = true;

                        //Insert BT

                        foreach (var businessType in incompleteRpt.BusinessTypes)
                        {
                            if (businessType.Checked)
                            {
                                var tblBusinessTypewithCust = new tblBusinessTypewithCust();
                                tblBusinessTypewithCust.CustID = incompleteRpt.CustID;
                                tblBusinessTypewithCust.BusinessTypeID = businessType.ID;
                                tblBusinessTypewithCust.CreatedDate = DateTimeNow;
                                tblBusinessTypewithCust.CreatedBy = incompleteRpt.CustID;
                                dbContext.tblBusinessTypewithCusts.Add(tblBusinessTypewithCust);
                            }
                        }

                        if(!incompleteRpt.BusinessTypes.Where(b => b.ID == 6).Select(b => b.Checked).FirstOrDefault())
                        {
                            if (incompleteRpt.BusinessDemands != null && incompleteRpt.BusinessDemands.Count > 0)
                            {
                                foreach (var businedDemand in incompleteRpt.BusinessDemands)
                                {
                                    if (businedDemand.IsChecked)
                                    {
                                        tblBusinessDemandwithCust businessDemandwithCust = new tblBusinessDemandwithCust();
                                        businessDemandwithCust.CustID = incompleteRpt.CustID;
                                        businessDemandwithCust.BusinessDemandID = businedDemand.ID;
                                        businessDemandwithCust.CreatedDate = DateTimeNow;
                                        businessDemandwithCust.CreatedBy = incompleteRpt.CustID;
                                        dbContext.tblBusinessDemandwithCusts.Add(businessDemandwithCust);
                                    }
                                }
                            }
                        }

                        var categoryProducts = (from c in dbContext.tblCategoryProducts
                                                join s in dbContext.tblSubCategories on c.ID equals s.CategoryProductID
                                                where incompleteRpt.SubcategoryList.Contains(s.ID)
                                                select new SubCat
                                                {
                                                    CategoryProductID = c.ID,
                                                    ID = s.ID,
                                                    SubCategoryName = s.SubCategoryName,
                                                }).Distinct().ToList();

                        var distinctCategories = categoryProducts.GroupBy(p => p.CategoryProductID).Select(g => g.First()).ToList();

                        foreach (var categoryProduct in distinctCategories)
                        {
                            var tblCategoryProductWithCust = new tblCategoryProductWithCust();
                            tblCategoryProductWithCust.CustID = incompleteRpt.CustID;
                            tblCategoryProductWithCust.CategoryProductID = categoryProduct.ID;
                            tblCategoryProductWithCust.CreatedBy = incompleteRpt.CustID;
                            tblCategoryProductWithCust.CreatedDate = DateTimeNow;
                            dbContext.tblCategoryProductWithCusts.Add(tblCategoryProductWithCust);
                        }
                        foreach (var subCategory in incompleteRpt.SubcategoryList)
                        {
                            var tblSubCategoryProductWithCust = new tblSubCategoryProductWithCust();
                            tblSubCategoryProductWithCust.CustID = incompleteRpt.CustID;
                            tblSubCategoryProductWithCust.CategoryProductID = distinctCategories.Where(d => d.ID == subCategory).Select(d => d.CategoryProductID).FirstOrDefault();
                            tblSubCategoryProductWithCust.SubCategoryId = subCategory;
                            tblSubCategoryProductWithCust.CreatedBy = incompleteRpt.CustID;
                            tblSubCategoryProductWithCust.CreatedDate = DateTimeNow;
                            dbContext.tblSubCategoryProductWithCusts.Add(tblSubCategoryProductWithCust);
                        }

                        dbContext.SaveChanges();
                        dbcxtransaction.Commit();
                        return "Customer Details updated";
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ex.Message;
            }
        }
    }
}
