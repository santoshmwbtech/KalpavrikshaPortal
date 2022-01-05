using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net.Mail;
using Korzh.EasyQuery.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Web.Script.Serialization;
using System.Web;

namespace JBNClassLibrary
{

    public class ChildCategoryList
    {
        public int ChildCategoryId { get; set; }
    }
    public class SubCategoryList
    {
        public int SubCategoryId { get; set; }
    }
    public class Brands
    {
        public int BrandID { get; set; }
        public string BrandName { get; set; }
    }
    public class CustomerListForSearch
    {
        public int CustID { get; set; }
        public int QueryID { get; set; }
        public string FirmName { get; set; }
        public int SenderID { get; set; }
    }
    public class DeleteConversations
    {
        public int CustID { get; set; }
        public int QueryId { get; set; }
        public int ReceiverID { get; set; }
    }
    public class DeleteChat
    {
        public int CustID { get; set; }
        public int QueryId { get; set; }
        public int ReceiverID { get; set; }
        public int ID { get; set; }
    }
    public class DeleteEnquiry
    {
        public int CustID { get; set; }
        public int QueryId { get; set; }
    }
    public class BusinessTypes
    {
        public int ID { get; set; }
        public int BusinessTypeID { get; set; }
        public string BusinessTypeName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public bool Checked { get; set; }
    }
    public class ImageContents
    {
        public string ImageName { get; set; }

        public string ImageNameBase64 { get; set; }
        public byte[] Image { get; set; }

    }
    public class CategoryProducts
    {
        public int ID { get; set; }
        public int CategoryProductID { get; set; }
        public string MainCategoryName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedID { get; set; }
        public Nullable<int> BusinessTypeID { get; set; }
        public bool IsChecked { get; set; }
    }
    public class SubCategoryProducts
    {
        public int CategoryProductID { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        //public string ProductName { get; set; }
        public string MainCategoryName { get; set; }
        public bool IsChecked { get; set; }
    }
    public class DealersDetails
    {
        public int IsRead { get; set; }
        public int SenderRead { get; set; }
        public int ReceiverID { get; set; }
        public int QueryId { get; set; }
        public int CustID { get; set; }
    }
    public class ChildCategories
    {
        public int ID { get; set; }
        public int ChildCategoryId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> SubCategoryId { get; set; }
        public string ChildCategoryName { get; set; }
        public string MainCategoryName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public bool IsProfessional { get; set; }
    }
    public class EnquiryTypes
    {
        public string EnquiryType { get; set; }
    }
    public class JBNDBClass
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        //Login API
        //ValidateCredentials
        public tblCustomerDetail ValidateCredentials(string MobileNo, string Password, string DeviceID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    Password = Helper.Encrypt(Password);
                    var user = (from c in dbContext.tblCustomerDetails
                                where c.MobileNumber == MobileNo && c.Password == Password
                                select c).AsNoTracking().FirstOrDefault();
                    var latestTAndC = dbContext.tblTAndCs.Where(c => c.IsCurrentVersion == true).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.IsActive == true)
                        {
                            if (user.TAndCVersion == latestTAndC.TAndCVersion)
                                user.IsTAndCAgreed = true;
                            else
                                user.IsTAndCAgreed = false;

                            tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();
                            tblCustomerDetail.ID = user.ID;
                            tblCustomerDetail.LastLoginDate = DateTimeNow;
                            tblCustomerDetail.DeviceID = DeviceID;
                            dbContext.tblCustomerDetails.Attach(tblCustomerDetail);
                            dbContext.Entry(tblCustomerDetail).Property(C => C.LastLoginDate).IsModified = true;
                            dbContext.Entry(tblCustomerDetail).Property(C => C.DeviceID).IsModified = true;
                            dbContext.SaveChanges();

                            string OTPRequired = ConfigurationManager.AppSettings["OTPRequired"].ToString();
                            if (OTPRequired == "0")
                                user.IsOTPVerified = true;
                            user.Password = Helper.Decrypt(user.Password);
                            return user;
                        }
                        else
                        {
                            tblCustomerDetail customerDetail = new tblCustomerDetail();
                            customerDetail.IsActive = user.IsActive;
                            return customerDetail;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //`tration of User
        //RegisterUser

        public CustomerDetails RegisterUser(CustomerDetails CustomerDetails)
        {
            try
            {
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    // System.Data.IsolationLevel= System.Data.IsolationLevel.Serializable
                    var IsFirmExists = (from c in dbContext.tblCustomerDetails
                                        where c.MobileNumber == CustomerDetails.MobileNumber
                                        select c).AsNoTracking().FirstOrDefault();

                    if (IsFirmExists != null)
                    {
                        CustomerDetails.DisplayMessage = "conflict";
                        return CustomerDetails;
                    }
                    else
                    {
                        tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();
                        tblCustomerDetail.CreatedBy = 1;
                        tblCustomerDetail.CreatedDate = DateTimeNow;
                        tblCustomerDetail.IsRegistered = 0;
                        tblCustomerDetail.IsOTPVerified = false;
                        tblCustomerDetail.FirmName = CustomerDetails.FirmName;
                        tblCustomerDetail.MobileNumber = CustomerDetails.MobileNumber;
                        tblCustomerDetail.Password = Helper.Encrypt(CustomerDetails.Password);
                        tblCustomerDetail.UserType = CustomerDetails.UserType;
                        tblCustomerDetail.LastLoginDate = DateTimeNow;
                        tblCustomerDetail.IsActive = true;
                        tblCustomerDetail.IsTAndCAgreed = CustomerDetails.IsTAndCAgreed;
                        tblCustomerDetail.TAndCVersion = CustomerDetails.TAndCVersion;
                        dbContext.tblCustomerDetails.Add(tblCustomerDetail);
                        dbContext.SaveChanges();

                        string AppName = ConfigurationManager.AppSettings["AppName"].ToString();

                        //string Message = "Dear Customer, You have successfully registered on " + AppName + " One India-One App. You can start using the App and Reach out to any part of India to buy any Business Products.Thank You - MWB Tech India Pvt Ltd";

                        string Message = ConfigurationManager.AppSettings["RegisterSMS"].ToString();

                        string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
                        string APIKEY = ConfigurationManager.AppSettings["SMSAPIKey"];
                        Helper.SendMessage(BaseURL, APIKEY, CustomerDetails.MobileNumber, Message);
                        return CustomerDetails;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Onclick of Forgotpassword --Validate User and Send OTP to reset the password
        public CustDetails ValidateUserForgotPwd(string MobileNo)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    //Get Customer Details
                    tblCustomerDetail customer = dbContext.tblCustomerDetails.AsNoTracking().Where(c => c.MobileNumber == MobileNo).FirstOrDefault();

                    if (customer != null)
                    {
                        if (customer.MobileNumber == MobileNo)
                        {
                            string SMSOTP = Helper.GenerateOTP();

                            //insert OTP in the database
                            tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();
                            tblCustomerDetail.ID = customer.ID;
                            tblCustomerDetail.SMSOTP = SMSOTP;
                            dbContext.tblCustomerDetails.Attach(tblCustomerDetail);
                            dbContext.Entry(tblCustomerDetail).Property(C => C.SMSOTP).IsModified = true;
                            dbContext.SaveChanges();

                            string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
                            string APIKEY = ConfigurationManager.AppSettings["SMSAPIKey"];
                            string AppName = ConfigurationManager.AppSettings["AppName"];

                            string MobileNumber = MobileNo;
                            string message = "Your OTP is " + SMSOTP + " for " + AppName + " login. - MWB Tech India Pvt Ltd";

                            string OTPStatus = Helper.SendMessage(BaseURL, APIKEY, MobileNumber, message);

                            CustDetails customerDetails = new CustDetails();
                            customerDetails.CustID = customer.ID;
                            customerDetails.IsRegistered = customer.IsRegistered;
                            customerDetails.UserType = customer.UserType;
                            customerDetails.Password = customer.Password;
                            customerDetails.SMSOTP = SMSOTP;
                            customerDetails.IsOTPVerified = customer.IsOTPVerified ?? false;
                            customerDetails.IsActive = customer.IsActive;
                            customerDetails.OTPStatus = OTPStatus;

                            return customerDetails;
                        }
                        else
                        {
                            CustDetails custDetails = new CustDetails();
                            custDetails.OTPStatus = "Mobile Number is incorrect!";
                            return custDetails;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                CustDetails custDetails = new CustDetails();
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                custDetails.OTPStatus = ex.Message;
                return custDetails;
            }
        }

        //25062021 Block Number if 3 Answers are wrong
        public bool BlockMobileNumber(string MobileNumber)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    var user = (from u in dbContext.tblCustomerDetails.AsNoTracking()
                                where u.MobileNumber == MobileNumber
                                select u).FirstOrDefault();

                    if (user == null)
                    {
                        return false;
                    }

                    tblCustomerDetail tblCustomerDetails = new tblCustomerDetail();
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    dbContext.Entry(user).State = EntityState.Detached;

                    tblCustomerDetails.ID = user.ID;
                    tblCustomerDetails.BlockedDate = DateTimeNow;
                    tblCustomerDetails.IsBlocked = true;
                    tblCustomerDetails.UpdatedByDate = DateTimeNow;
                    tblCustomerDetails.UpdatedByID = user.ID;
                    dbContext.tblCustomerDetails.Attach(tblCustomerDetails);
                    dbContext.Entry(tblCustomerDetails).Property(c => c.BlockedDate).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.IsBlocked).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.UpdatedByDate).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.UpdatedByID).IsModified = true;
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

        //25062021 Check isblocked
        public string CheckStatus(string MobileNumber)
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    if (dbcontext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbcontext.Database.Connection.Open();

                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    var customerDetails = dbcontext.tblCustomerDetails.AsNoTracking().Where(u => u.MobileNumber == MobileNumber).FirstOrDefault();
                    string Result = "false";

                    if (customerDetails != null)
                    {
                        bool IsBlocked = false;
                        DateTime? BlockedDate = null;

                        if (customerDetails.IsBlocked != null)
                            IsBlocked = customerDetails.IsBlocked.Value;
                        if (customerDetails.BlockedDate != null)
                            BlockedDate = customerDetails.BlockedDate.Value;

                        if (IsBlocked == false)
                        {
                            Result = "true";
                        }
                        else
                        {
                            if (BlockedDate.Value.AddHours(24) > DateTimeNow)
                                Result = "true";
                        }
                    }
                    else
                        Result = "false";
                    return Result;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return "false";
            }
        }

        //public tblCustomerDetail ChangePassword(string MobileNo, string Password,string SMSOTP )
        //{
        //    try
        //    {
        //        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        //        var user = dbContext.tblCustomerDetails.AsNoTracking().Where(u => u.MobileNumber == MobileNo && u.SMSOTP == SMSOTP).FirstOrDefault();
        //        if (user != null)
        //        {
        //            if (user.IsActive.Value == true)
        //            {
        //                tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();
        //                tblCustomerDetail.CustID = user.CustID;
        //                tblCustomerDetail.Password = Password;
        //                tblCustomerDetail.LastLoginDate = DateTimeNow;
        //                dbContext.tblCustomerDetails.Attach(tblCustomerDetail);
        //                dbContext.Entry(tblCustomerDetail).Property(C => C.LastLoginDate).IsModified = true;
        //                dbContext.Entry(tblCustomerDetail).Property(C => C.Password).IsModified = true;
        //                dbContext.SaveChanges();

        //                string OTPRequired = ConfigurationManager.AppSettings["OTPRequired"].ToString();
        //                if (OTPRequired == "0")
        //                    user.IsOTPVerified = true;
        //                return user;
        //            }
        //            else
        //            {
        //                tblCustomerDetail customerDetail = new tblCustomerDetail();
        //                customerDetail.IsActive = user.IsActive;
        //                return customerDetail;
        //            }
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}

        public tblCustomerDetail ForgotRest_Password(string MobileNo, string Password, string SMSOTP)
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    var user = dbContext.tblCustomerDetails.AsNoTracking().Where(u => u.MobileNumber == MobileNo && u.SMSOTP == SMSOTP).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.IsActive == true)
                        {
                            tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();
                            tblCustomerDetail.ID = user.ID;
                            tblCustomerDetail.Password = Helper.Encrypt(Password);
                            tblCustomerDetail.LastLoginDate = DateTimeNow;
                            dbContext.tblCustomerDetails.Attach(tblCustomerDetail);
                            dbContext.Entry(tblCustomerDetail).Property(C => C.LastLoginDate).IsModified = true;
                            dbContext.Entry(tblCustomerDetail).Property(C => C.Password).IsModified = true;
                            dbContext.SaveChanges();

                            string OTPRequired = ConfigurationManager.AppSettings["OTPRequired"].ToString();
                            if (OTPRequired == "0")
                                user.IsOTPVerified = true;
                            return user;
                        }
                        else
                        {
                            tblCustomerDetail customerDetail = new tblCustomerDetail();
                            customerDetail.IsActive = user.IsActive;
                            return customerDetail;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public tblCustomerDetail ChangePassword(string MobileNo, string OldPassword, string Newpassword)
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    OldPassword = Helper.Encrypt(OldPassword);

                    var user = dbContext.tblCustomerDetails.AsNoTracking().Where(u => u.MobileNumber == MobileNo && u.Password == OldPassword).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.IsActive == true)
                        {
                            tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();
                            tblCustomerDetail.ID = user.ID;
                            tblCustomerDetail.Password = Helper.Encrypt(Newpassword);
                            tblCustomerDetail.LastLoginDate = DateTimeNow;
                            dbContext.tblCustomerDetails.Attach(tblCustomerDetail);
                            dbContext.Entry(tblCustomerDetail).Property(C => C.LastLoginDate).IsModified = true;
                            dbContext.Entry(tblCustomerDetail).Property(C => C.Password).IsModified = true;
                            dbContext.SaveChanges();

                            return user;
                        }
                        else
                        {
                            tblCustomerDetail customerDetail = new tblCustomerDetail();
                            customerDetail.IsActive = user.IsActive;
                            return customerDetail;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //old Code
        //public tblCustomerDetail RegisterUser(tblCustomerDetail CustomerDetails)
        //{
        //    try
        //    {
        //        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        //        using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
        //        {
        //            var IsFirmExists = (from u in dbContext.tblCustomerDetails
        //                                where u.MobileNumber == CustomerDetails.MobileNumber
        //                                select u).AsNoTracking().FirstOrDefault();

        //            if (IsFirmExists != null)
        //            {
        //                return null;
        //            }
        //            else
        //            {
        //                tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();
        //                tblCustomerDetail.CreatedByID = 1;
        //                tblCustomerDetail.CreatedDate = DateTimeNow;
        //                tblCustomerDetail.IsRegistered = 0;
        //                tblCustomerDetail.QuestionAnswered = true;
        //                tblCustomerDetail.IsOTPVerified = false;
        //                tblCustomerDetail.FirmName = CustomerDetails.FirmName;
        //                tblCustomerDetail.MobileNumber = CustomerDetails.MobileNumber;
        //                tblCustomerDetail.Password = CustomerDetails.Password;
        //                tblCustomerDetail.UserType = CustomerDetails.UserType;
        //                tblCustomerDetail.LastLoginDate = DateTimeNow;
        //                tblCustomerDetail.IsActive = true;
        //                dbContext.tblCustomerDetails.Add(tblCustomerDetail);
        //                dbContext.SaveChanges();
        //                return CustomerDetails;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}

        //Get Customer Details for Update
        //CustomerDetails
        public CustomerDetails GetCustomerDetails(int CustID)
        {
            string temp = string.Empty;
            try
            {
                List<BusinessTypes> businesstypes;
                businesstypes = GetBusinessTypes();
                //List<SubCategoryProducts> subCategoryProducts = new List<SubCategoryProducts>();
                //subCategoryProducts = GetSubCatagoryList(string.Empty, CustID);

                using (dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    //using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    //{
                    CustomerDetails customerDetails = new CustomerDetails();
                    tblCustomerDetail tblCustomerDetails = dbContext.tblCustomerDetails.Find(CustID);
                    customerDetails.CustID = tblCustomerDetails.ID;
                    customerDetails.FirmName = tblCustomerDetails.FirmName;
                    customerDetails.Password = Helper.Decrypt(tblCustomerDetails.Password);
                    customerDetails.UserType = tblCustomerDetails.UserType;
                    customerDetails.CustName = tblCustomerDetails.CustName;
                    customerDetails.EmailID = tblCustomerDetails.EmailID;
                    customerDetails.MobileNumber = tblCustomerDetails.MobileNumber;
                    customerDetails.MobileNumber2 = tblCustomerDetails.MobileNumber2;
                    customerDetails.TelephoneNumber = tblCustomerDetails.TelephoneNumber;
                    customerDetails.ShopImage = tblCustomerDetails.ShopImage;
                    customerDetails.BillingAddress = tblCustomerDetails.BillingAddress;
                    customerDetails.Area = tblCustomerDetails.Area;
                    customerDetails.CityCode = tblCustomerDetails.CityCode;
                    customerDetails.Pincode = tblCustomerDetails.Pincode;
                    customerDetails.Lattitude = tblCustomerDetails.Lattitude;
                    customerDetails.Langitude = tblCustomerDetails.Langitude;
                    customerDetails.IsBGSMember = tblCustomerDetails.IsBGSMember == null ? false : tblCustomerDetails.IsBGSMember.Value;
                    customerDetails.UserImage = tblCustomerDetails.UserImage;

                    if (tblCustomerDetails.InterstCity != null)
                        customerDetails.InterstCity = tblCustomerDetails.InterstCity;
                    else
                        customerDetails.InterstCity = false;

                    if (tblCustomerDetails.InterstState != null)
                        customerDetails.InterstState = tblCustomerDetails.InterstState;
                    else
                        customerDetails.InterstState = false;

                    if (tblCustomerDetails.InterstCountry != null)
                        customerDetails.InterstCountry = tblCustomerDetails.InterstCountry;
                    else
                        customerDetails.InterstCountry = false;

                    customerDetails.RegistrationType = tblCustomerDetails.RegistrationType;
                    customerDetails.TinNumber = tblCustomerDetails.TinNumber;
                    customerDetails.PanNumber = tblCustomerDetails.PanNumber;
                    customerDetails.Bankname = tblCustomerDetails.Bankname;
                    customerDetails.BankBranchName = tblCustomerDetails.BankBranchName;
                    customerDetails.Accountnumber = tblCustomerDetails.Accountnumber;
                    customerDetails.IFSCCode = tblCustomerDetails.IFSCCode;
                    customerDetails.BankCity = tblCustomerDetails.BankCity;
                    customerDetails.SalesmanID = tblCustomerDetails.SalesmanID;
                    customerDetails.CreatedByID = tblCustomerDetails.CreatedBy;//
                    customerDetails.CreatedDate = tblCustomerDetails.CreatedDate;//
                    customerDetails.IsRegistered = tblCustomerDetails.IsRegistered;
                    customerDetails.UpdatedByID = tblCustomerDetails.ModifiedBy;//
                    customerDetails.UpdatedByDate = tblCustomerDetails.ModifiedDate;//
                    customerDetails.AdditionalPersonName = tblCustomerDetails.AdditionalPersonName;
                    customerDetails.IsActive = tblCustomerDetails.IsActive;
                    customerDetails.ReasonForDeactivate = tblCustomerDetails.ReasonForDeactivate;

                    if (tblCustomerDetails.State != null && tblCustomerDetails.State != 0)
                    {
                        State customerState = new State();
                        customerState.StateID = tblCustomerDetails.State;
                        customerState.StateName = dbContext.tblStates.Where(u => u.ID == tblCustomerDetails.State).FirstOrDefault().StateName;
                        customerDetails.state = customerState;
                    }

                    if (tblCustomerDetails.DistrictID != null && tblCustomerDetails.DistrictID != 0)
                    {
                        District customerDistrict = new District();
                        customerDistrict.DistrictID = tblCustomerDetails.DistrictID;
                        customerDistrict.DistrictName = dbContext.tblDistricts.Where(u => u.ID == tblCustomerDetails.DistrictID).FirstOrDefault().DistrictName;
                        customerDetails.district = customerDistrict;
                    }

                    if (tblCustomerDetails.City != null && tblCustomerDetails.City != 0)
                    {
                        City customerCity = new City();
                        customerCity.StateWithCityID = tblCustomerDetails.City;
                        customerCity.VillageLocalityName = dbContext.tblStateWithCities.Where(u => u.ID == tblCustomerDetails.City).FirstOrDefault().VillageLocalityName;
                        customerDetails.city = customerCity;
                    }

                    List<BusinessTypeWithCusts> list = (from btc in dbContext.tblBusinessTypewithCusts
                                                        join b in dbContext.tblBusinessTypes on btc.BusinessTypeID equals b.ID
                                                        where btc.CustID == CustID
                                                        select new BusinessTypeWithCusts
                                                        {
                                                            CustID = btc.CustID,
                                                            BusinessTypeID = btc.BusinessTypeID,
                                                            BusinessTypeName = b.Type,
                                                            Checked = true,
                                                        }).ToList();

                    //foreach (var businessItem in businesstypes)
                    //{
                    //    foreach (var item in list)
                    //    {
                    //        if (businessItem.ID == item.ID)
                    //        {
                    //            businessItem.Checked = true;
                    //        }
                    //    }
                    //}

                    customerDetails.BusinessTypeWithCust = list;

                    List<tblCategoryProductWithCust> categorylist = dbContext.tblCategoryProductWithCusts.Where(u => u.CustID == CustID).ToList();
                    List<CategoryTypeWithCust> categoryProducts = new List<CategoryTypeWithCust>();
                    var categories = (from cp in dbContext.tblCategoryProductWithCusts
                                      where cp.CustID == CustID
                                      select new CategoryTypeWithCust
                                      {
                                          CustID = cp.CustID,
                                          CategoryProductID = cp.CategoryProductID,
                                          MainCategoryName = cp.tblCategoryProduct.MainCategoryName,
                                          ID = cp.ID
                                      });
                    categoryProducts = categories.ToList();

                    //foreach (var item in categorylist)
                    //{
                    //    CategoryTypeWithCust obj = new CategoryTypeWithCust();
                    //    obj.CategoryProductID = item.CategoryProductID;
                    //    obj.MainCategoryName = dbContext.tblCategoryProducts.Find(item.CategoryProductID).MainCategoryName;
                    //    obj.CustID = item.CustID;
                    //    obj.ID = item.ID;
                    //    categoryProducts.Add(obj);
                    //}

                    List<tblSubCategoryProductWithCust> SubCategorylist = dbContext.tblSubCategoryProductWithCusts.Where(u => u.CustID == CustID).ToList();
                    List<SubCategoryProducts> subCategorytypes = new List<SubCategoryProducts>();
                    var subCategories = (from sp in dbContext.tblSubCategoryProductWithCusts
                                         join s in dbContext.tblSubCategories on sp.SubCategoryId equals s.SubCategoryId
                                         where sp.CustID == CustID
                                         select new SubCategoryProducts
                                         {
                                             CategoryProductID = sp.CategoryProductID.Value,
                                             SubCategoryId = sp.SubCategoryId.Value,
                                             IsChecked = true,
                                             MainCategoryName = dbContext.tblCategoryProducts.Where(u => u.CategoryProductID == sp.CategoryProductID.Value).FirstOrDefault().MainCategoryName,
                                             SubCategoryName = s.SubCategoryName,
                                         });
                    subCategorytypes = subCategories.ToList();

                    //foreach (var item in SubCategorylist)
                    //{
                    //    SubCategoryProducts obj = new SubCategoryProducts();
                    //    obj.SubCategoryId = item.SubCategoryId.Value;
                    //    obj.SubCategoryName = dbContext.tblSubCategories.Where(u => u.SubCategoryId == item.SubCategoryId).FirstOrDefault().SubCategoryName;
                    //    obj.CategoryProductID = item.CategoryProductID.Value;
                    //    obj.MainCategoryName = dbContext.tblCategoryProducts.Where(u => u.CategoryProductID == item.CategoryProductID).FirstOrDefault().MainCategoryName;
                    //    obj.IsChecked = true;
                    //    subCategorytypes.Add(obj);
                    //}

                    //List<tblChildCategoryProductWithCust> childCategorylist = dbContext.tblChildCategoryProductWithCusts.Where(u => u.CustID == CustID).ToList();
                    //List<ChildCategoryTypeWithCust> ChildCategoryTypes = new List<ChildCategoryTypeWithCust>();
                    //var childCategories = from cp in dbContext.tblChildCategories
                    //foreach (var item in childCategorylist)
                    //{
                    //    ChildCategoryTypeWithCust obj = new ChildCategoryTypeWithCust();
                    //    obj.ChildCategoryId = item.ChildCategoryId;
                    //    obj.ChildCategoryName = dbContext.tblChildCategories.Where(u => u.ItemId == item.ChildCategoryId).FirstOrDefault().ItemName;
                    //    obj.SubCategoryId = item.SubCategoryId;
                    //    obj.CustID = item.CustID;
                    //    obj.ID = item.ID;
                    //    ChildCategoryTypes.Add(obj);
                    //}

                    //customerDetails.BusinessTypeWithCust = businesstypes;
                    customerDetails.CategoryTypeWithCust = categoryProducts;
                    //customerDetails.ChildCategoryTypeWithCust = ChildCategoryTypes;
                    customerDetails.SubCategoryTypeWithCust = subCategorytypes;

                    //get history
                    var status = (from s in dbContext.tblCustomerStatusHistories
                                  join c in dbContext.tblCustomerDetails on s.CustID equals c.ID
                                  join u in dbContext.tblUsers on s.CreatedBy equals u.UserID
                                  where c.ID == CustID
                                  select new StatusHistory
                                  {
                                      CreatedDate = s.CreatedDate,
                                      CustID = s.CustID,
                                      CreatedByUser = u.FullName,
                                      Comments = s.Comments,
                                      CustomerStatus = s.CustomerStatus
                                  });
                    customerDetails.statusHistories = status.ToList();
                    return customerDetails;
                    //}
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return new CustomerDetails();
            }
        }

        //Update custoemer details
        //UpdateCustomerDetails
        public bool UpdateCustomerDetails(int CustID, CustomerDetails customerDetails)
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
                                    where u.ID == customerDetails.CustID
                                    select u).FirstOrDefault();

                        if (user == null)
                        {
                            return false;
                        }
                        tblCustomerDetail tblCustomerDetails = new tblCustomerDetail();
                        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                        dbContext.Entry(user).State = EntityState.Detached;
                        tblCustomerDetails.ModifiedBy = CustID;//
                        tblCustomerDetails.ModifiedDate = DateTimeNow;//
                        if (customerDetails.state != null)
                            tblCustomerDetails.State = customerDetails.state.StateID;
                        if (customerDetails.district != null)
                            tblCustomerDetails.DistrictID = customerDetails.district.DistrictID;

                        if (customerDetails.city != null)
                            tblCustomerDetails.City = customerDetails.city.StateWithCityID;

                        tblCustomerDetails.ID = customerDetails.CustID;
                        tblCustomerDetails.FirmName = customerDetails.FirmName;
                        tblCustomerDetails.Password = Helper.Encrypt(customerDetails.Password);
                        tblCustomerDetails.UserType = customerDetails.UserType;
                        tblCustomerDetails.CustName = customerDetails.CustName;
                        tblCustomerDetails.EmailID = customerDetails.EmailID;
                        tblCustomerDetails.MobileNumber = customerDetails.MobileNumber;
                        tblCustomerDetails.MobileNumber2 = customerDetails.MobileNumber2;
                        tblCustomerDetails.TelephoneNumber = customerDetails.TelephoneNumber;
                        tblCustomerDetails.ShopImage = customerDetails.ShopImage;
                        tblCustomerDetails.BillingAddress = customerDetails.BillingAddress;
                        tblCustomerDetails.Area = customerDetails.Area;
                        tblCustomerDetails.CityCode = customerDetails.CityCode;
                        tblCustomerDetails.Pincode = customerDetails.Pincode;
                        tblCustomerDetails.Lattitude = customerDetails.Lattitude;
                        tblCustomerDetails.Langitude = customerDetails.Langitude;
                        tblCustomerDetails.InterstCity = customerDetails.InterstCity;
                        tblCustomerDetails.InterstState = customerDetails.InterstState;
                        tblCustomerDetails.InterstCountry = customerDetails.InterstCountry;
                        tblCustomerDetails.RegistrationType = customerDetails.RegistrationType;
                        tblCustomerDetails.TinNumber = customerDetails.TinNumber;
                        tblCustomerDetails.PanNumber = customerDetails.PanNumber;
                        tblCustomerDetails.Bankname = customerDetails.Bankname;
                        tblCustomerDetails.BankBranchName = customerDetails.BankBranchName;
                        tblCustomerDetails.Accountnumber = customerDetails.Accountnumber;
                        tblCustomerDetails.IFSCCode = customerDetails.IFSCCode;
                        tblCustomerDetails.BankCity = customerDetails.BankCity;
                        tblCustomerDetails.SalesmanID = customerDetails.SalesmanID;
                        tblCustomerDetails.IsRegistered = customerDetails.IsRegistered;
                        tblCustomerDetails.AdditionalPersonName = customerDetails.AdditionalPersonName;
                        tblCustomerDetails.SMSOTP = user.SMSOTP;
                        tblCustomerDetails.IsOTPVerified = user.IsOTPVerified;
                        tblCustomerDetails.Question = user.Question;
                        tblCustomerDetails.QuestionAnswered = user.QuestionAnswered;
                        tblCustomerDetails.AnswerForQuestion = user.AnswerForQuestion;
                        tblCustomerDetails.DeviceID = user.DeviceID;
                        tblCustomerDetails.IsActive = user.IsActive;
                        tblCustomerDetails.CreatedDate = user.CreatedDate;
                        tblCustomerDetails.CreatedBy = user.CreatedBy;
                        tblCustomerDetails.LastLoginDate = DateTimeNow;

                        tblCustomerDetails.IsBGSMember = customerDetails.IsBGSMember;

                        if (!string.IsNullOrEmpty(customerDetails.UserImage))
                            tblCustomerDetails.UserImage = customerDetails.UserImage;
                        else
                            tblCustomerDetails.UserImage = user.UserImage;

                        //if (customerDetails.UserPhoto != null)
                        //{                        
                        //    tblCustomerDetails.UserPhoto = customerDetails.UserPhoto;
                        //}
                        //else
                        //    tblCustomerDetails.UserPhoto = user.UserPhoto;

                        if (customerDetails.BusinessTypeWithCust != null)
                        {
                            dbContext.tblBusinessTypewithCusts.RemoveRange(dbContext.tblBusinessTypewithCusts.Where(x => x.CustID == CustID));
                            foreach (var item in customerDetails.BusinessTypeWithCust)
                            {
                                if (item.Checked == true)
                                {
                                    tblBusinessTypewithCust tblBusinessTypeWithCustomer = new tblBusinessTypewithCust();
                                    tblBusinessTypeWithCustomer.ID = item.ID;
                                    tblBusinessTypeWithCustomer.CustID = CustID;
                                    dbContext.tblBusinessTypewithCusts.Add(tblBusinessTypeWithCustomer);
                                }
                            }
                        }
                        //if (customerDetails.CategoryTypeWithCust != null)
                        //{
                        //    dbContext.tblCategoryProductWithCusts.RemoveRange(dbContext.tblCategoryProductWithCusts.Where(x => x.CustID == CustID));
                        //    foreach (var item in customerDetails.CategoryTypeWithCust)
                        //    {
                        //        tblCategoryProductWithCust tblCategoryProductWithCustomer = new tblCategoryProductWithCust();
                        //        tblCategoryProductWithCustomer.CategoryProductID = item.CategoryProductID;
                        //        tblCategoryProductWithCustomer.CustID = CustID;
                        //        dbContext.tblCategoryProductWithCusts.Add(tblCategoryProductWithCustomer);
                        //    }
                        //}
                        if (customerDetails.SubCategoryTypeWithCust != null)
                        {
                            //insert main category
                            List<int> CategoryProductIDS = new List<int>();
                            CategoryProductIDS = (List<int>)customerDetails.SubCategoryTypeWithCust.Select(s => s.CategoryProductID).Distinct().ToList();

                            List<tblCategoryProduct> CategoryProducts = new List<tblCategoryProduct>();
                            CategoryProducts = dbContext.tblCategoryProducts.ToList();
                            if (CategoryProducts != null && CategoryProducts.Count > 0)
                            {
                                CategoryProducts = CategoryProducts.Where(c => CategoryProductIDS.Contains(c.CategoryProductID)).ToList();
                                dbContext.tblCategoryProductWithCusts.RemoveRange(dbContext.tblCategoryProductWithCusts.Where(x => x.CustID == CustID));
                                foreach (var item in CategoryProducts)
                                {
                                    tblCategoryProductWithCust tblCategoryProductWithCustomer = new tblCategoryProductWithCust();
                                    tblCategoryProductWithCustomer.CategoryProductID = item.CategoryProductID;
                                    tblCategoryProductWithCustomer.CustID = CustID;
                                    tblCategoryProductWithCustomer.CreatedDate = DateTimeNow;
                                    tblCategoryProductWithCustomer.CreatedBy = CustID;
                                    dbContext.tblCategoryProductWithCusts.Add(tblCategoryProductWithCustomer);
                                }
                            }
                            //insert main category

                            dbContext.tblSubCategoryProductWithCusts.RemoveRange(dbContext.tblSubCategoryProductWithCusts.Where(x => x.CustID == CustID));
                            customerDetails.SubCategoryTypeWithCust = customerDetails.SubCategoryTypeWithCust.Distinct().ToList();
                            foreach (var item in customerDetails.SubCategoryTypeWithCust)
                            {
                                if (item.IsChecked == true)
                                {
                                    tblSubCategoryProductWithCust tblSubCategoryProductWithCustomer = new tblSubCategoryProductWithCust();
                                    tblSubCategoryProductWithCustomer.CategoryProductID = item.CategoryProductID;
                                    tblSubCategoryProductWithCustomer.CustID = CustID;
                                    tblSubCategoryProductWithCustomer.SubCategoryId = item.SubCategoryId;
                                    tblSubCategoryProductWithCustomer.CreatedDate = DateTimeNow;
                                    dbContext.tblSubCategoryProductWithCusts.Add(tblSubCategoryProductWithCustomer);
                                }
                            }
                        }
                        //if (customerDetails.SubCategoryTypeWithCust != null)
                        //{

                        //    List<int> SubCategoryIDs = new List<int>();
                        //    SubCategoryIDs = (List<int>)customerDetails.SubCategoryTypeWithCust.Select(s => s.SubCategoryId).Distinct().ToList();

                        //    List<tblChildCategory> ChildCategoryProducts = new List<tblChildCategory>();
                        //    ChildCategoryProducts = dbContext.tblChildCategories.Where(c => SubCategoryIDs.Contains(c.SubCategoryId.Value)).ToList();


                        //    if (ChildCategoryProducts != null && ChildCategoryProducts.Count > 0)
                        //    { 
                        //        //ChildCategoryProducts = ChildCategoryProducts.Where(c => SubCategoryIDs.Contains(c.SubCategoryId.Value)).ToList();
                        //        dbContext.tblChildCategoryProductWithCusts.RemoveRange(dbContext.tblChildCategoryProductWithCusts.Where(x => x.CustID == CustID));
                        //        foreach (var item in ChildCategoryProducts)
                        //        {
                        //            tblChildCategoryProductWithCust tblChildCategoryProductWithCustomer = new tblChildCategoryProductWithCust();
                        //            tblChildCategoryProductWithCustomer.SubCategoryId = item.SubCategoryId;
                        //            tblChildCategoryProductWithCustomer.ChildCategoryId = item.ChildCategoryId;
                        //            tblChildCategoryProductWithCustomer.CustID = CustID;
                        //            dbContext.tblChildCategoryProductWithCusts.Add(tblChildCategoryProductWithCustomer);
                        //        }
                        //    }
                        //}
                        dbContext.Entry(tblCustomerDetails).State = EntityState.Modified;
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

        public string ActivateDeactivateUser(UserStatus userStatus, int UserID)
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    if (dbcontext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbcontext.Database.Connection.Open();

                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    var customerDetails = dbcontext.tblCustomerDetails.AsNoTracking().Where(u => u.ID == userStatus.CustID).FirstOrDefault();

                    if (customerDetails != null)
                    {
                        tblCustomerDetail tblCustomer = new tblCustomerDetail();
                        tblCustomer.ID = customerDetails.ID;
                        if (userStatus.StatusType == 1)
                            tblCustomer.IsActive = true;
                        if (userStatus.StatusType == 0)
                            tblCustomer.IsActive = false;
                        dbcontext.tblCustomerDetails.Attach(tblCustomer);
                        dbcontext.Entry(tblCustomer).Property(c => c.IsActive).IsModified = true;

                        tblCustomerStatusHistory tblCustomerStatus = new tblCustomerStatusHistory();
                        tblCustomerStatus.CustID = customerDetails.ID;
                        tblCustomerStatus.Comments = userStatus.Comments;
                        if (userStatus.StatusType == 1)
                            tblCustomerStatus.CustomerStatus = "Actiated";
                        if (userStatus.StatusType == 0)
                            tblCustomerStatus.CustomerStatus = "Deactivated";
                        tblCustomerStatus.CreatedBy = UserID;
                        tblCustomerStatus.CreatedDate = DateTimeNow;
                        dbcontext.tblCustomerStatusHistories.Add(tblCustomerStatus);

                        //Insert into History Table
                        tblHistory history = new tblHistory();
                        history.UserID = UserID;
                        history.CustID = customerDetails.ID;
                        history.ProductCategory = customerDetails.FirmName;
                        history.CreatedDate = DateTimeNow;
                        history.ActivityPage = "appusers";
                        history.ActivityType = "update";
                        history.Comments = userStatus.Comments;
                        dbcontext.tblHistories.Add(history);

                        dbcontext.SaveChanges();
                        return "success";
                    }
                    else
                        return "notfound";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return "error";
            }
        }

        //GetChildCatagories
        //Enquiry Page
        public List<ChildCategories> GetChildCatagories(string SearchText, bool isProfessional, bool IsAdvertisement = false)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    if (IsAdvertisement)
                    {
                        List<ChildCategories> ChildCategoryList = new List<ChildCategories>();
                        ChildCategoryList = (from PView in dbContext.ProductsViews
                                             where PView.ChildCategoryName.Contains(SearchText) || PView.SubCategoryName.Contains(SearchText)
                                             || PView.MainCategoryName.Contains(SearchText)
                                             || PView.ItemName.Contains(SearchText)
                                             && PView != null
                                             select new ChildCategories
                                             {
                                                 ChildCategoryId = PView.ItemID,
                                                 ChildCategoryName = PView.ItemName.Trim() + "(" + PView.SubCategoryName + ")",
                                                 SubCategoryId = PView.SubCategoryID,
                                                 MainCategoryName = PView.MainCategoryName
                                             }).Distinct().OrderBy(c => c.ChildCategoryName).ToList();
                        return ChildCategoryList;
                    }
                    else
                    {
                        List<ChildCategories> ChildCategoryList = (from PView in dbContext.ProductsViews
                                                                   where PView.ChildCategoryName.Contains(SearchText) || PView.SubCategoryName.Contains(SearchText)
                                                                   || PView.MainCategoryName.Contains(SearchText)
                                                                   || PView.ItemName.Contains(SearchText)
                                                                   && PView != null
                                                                   select new ChildCategories
                                                                   {
                                                                       ChildCategoryId = PView.ItemID,
                                                                       ChildCategoryName = PView.ItemName.Trim() + "(" + PView.SubCategoryName + ")",
                                                                       SubCategoryId = PView.SubCategoryID,
                                                                       MainCategoryName = PView.MainCategoryName
                                                                   }).Distinct().OrderBy(c => c.ChildCategoryName).ToList();

                        if (isProfessional)
                            ChildCategoryList = ChildCategoryList.Where(c => c.MainCategoryName.ToLower().Contains("professionals")).ToList();
                        else
                            ChildCategoryList = ChildCategoryList.Where(c => !c.MainCategoryName.Contains("professionals")).ToList();

                        return ChildCategoryList;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Get Product Names for 1 + 1
        public List<ChildCategories> GetAllItems(string SearchText)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<ChildCategories> ChildCategoryList = new List<ChildCategories>();
                    ChildCategoryList = (from PView in dbContext.ProductsViews
                                         where PView.ChildCategoryName.Contains(SearchText) || PView.SubCategoryName.Contains(SearchText)
                                         || PView.MainCategoryName.Contains(SearchText)
                                         || PView.ItemName.Contains(SearchText)
                                         && PView != null
                                         select new ChildCategories
                                         {
                                             ChildCategoryId = PView.ItemID,
                                             ChildCategoryName = PView.ItemName.Trim(),
                                             SubCategoryId = PView.SubCategoryID,
                                             MainCategoryName = PView.MainCategoryName
                                         }).Distinct().OrderBy(c => c.ChildCategoryName).ToList();
                    return ChildCategoryList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //GetSubCatagories Currently not using
        public List<tblSubCategory> GetSubCatagories(List<ChildCategoryList> ChildCategoryList)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<tblSubCategory> categoryList = new List<tblSubCategory>();

                    foreach (var item in ChildCategoryList)
                    {
                        tblSubCategory obj = new tblSubCategory();
                        obj = (from s in dbContext.tblSubCategories
                               join c in dbContext.tblChildCategories on s.SubCategoryId equals c.SubCategoryId
                               where c.ID == item.ChildCategoryId
                               select s).FirstOrDefault();
                        categoryList.Add(obj);
                    }

                    //categoryList = (from s in dbContext.tblCategoryProducts
                    //                join sl in dbContext.tblSubCategories on s.CategoryProductID equals sl.ParentCategoryId
                    //                where sl.SubCategoryId == SubCategoryId
                    //                select s).ToList();

                    return categoryList.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        #region 
        //Get SubCategory List
        //public List<tblSubCategory> GetSubCatagoryList(string SearchText)
        //{
        //    try
        //    {
        //        using (dbContext = new MWBTDealerEntities())
        //        {
        //            using (var dbcxtransaction = dbContext.Database.BeginTransaction())
        //            {
        //                List<tblSubCategory> categoryList = new List<tblSubCategory>();
        //                categoryList = (from s in dbContext.tblSubCategories
        //                       join sl in dbContext.tblChildCategories on s.SubCategoryId equals sl.SubCategoryId
        //                       where sl.ItemName.Contains(SearchText)
        //                       select s).Distinct().ToList();
        //                return categoryList.Distinct().ToList();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, true);
        //        return null;
        //    }
        //}

        //GetCategory
        #endregion

        //Get SubCategory List
        public List<tblSubCategory> GetSubCatagoryList()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<tblSubCategory> categoryList = new List<tblSubCategory>();
                        categoryList = (from s in dbContext.tblSubCategories
                                        select s).Distinct().ToList();
                        return categoryList.Distinct().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, true);
                return null;
            }
        }

        /*Search product in update profile page*/
        //added isProfessional on 05-10-2020
        public List<SubCategoryProducts> GetSubCatagoryList(string SearchText, int CustID, bool isProfessional)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    IQueryable<SubCategoryProducts> AllcategoryProducts;
                    List<SubCategoryProducts> SelectedcategoryProducts = new List<SubCategoryProducts>();

                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        AllcategoryProducts = (from PView in dbContext.ProductsViews
                                               where PView.ChildCategoryName.Contains(SearchText) || PView.SubCategoryName.Contains(SearchText) || PView.MainCategoryName.Contains(SearchText)
                                               || PView.ItemName.Contains(SearchText)
                                               select new SubCategoryProducts
                                               {
                                                   CategoryProductID = PView.CategoryProductID,
                                                   MainCategoryName = PView.MainCategoryName,
                                                   SubCategoryId = PView.SubCategoryID,
                                                   SubCategoryName = PView.SubCategoryName,
                                                   //ProductName = cc.ChildCategoryName,
                                                   IsChecked = (from data in dbContext.tblSubCategoryProductWithCusts where data.SubCategoryId == PView.SubCategoryID && data.CustID == CustID select data).Count() > 0 ? true : false,
                                               }).Distinct().AsQueryable();

                        //AllcategoryProducts = AllcategoryProducts.Where(p => p.ProductName.Contains(SearchText)).ToList();
                        IQueryable<SubCategoryProducts> newList;// = AllcategoryProducts.Distinct().ToList();
                        if (isProfessional)
                            newList = AllcategoryProducts.ToList().FindAll(x => x.MainCategoryName.ToLower() == "professionals").AsQueryable();
                        // newList = AllcategoryProducts.Where(x => x.MainCategoryName.ToLower() == "professionals").ToList();
                        else
                            newList = AllcategoryProducts.ToList().FindAll(x => x.MainCategoryName.ToLower() != "professionals").AsQueryable(); //newList = newList.Where(x => x.MainCategoryName.ToLower() != "professionals").ToList();

                        return newList.Distinct().ToList();
                    }
                    else
                    {
                        AllcategoryProducts = (from sc in dbContext.tblSubCategories
                                               join cc in dbContext.tblChildCategories on sc.SubCategoryId equals cc.SubCategoryId
                                               join cp in dbContext.tblCategoryProducts on sc.CategoryProductID equals cp.CategoryProductID
                                               select new SubCategoryProducts
                                               {
                                                   CategoryProductID = cp.ID,
                                                   MainCategoryName = cp.MainCategoryName,
                                                   SubCategoryId = sc.ID,
                                                   SubCategoryName = sc.SubCategoryName,
                                                   IsChecked = (from data in dbContext.tblSubCategoryProductWithCusts where data.SubCategoryId == sc.SubCategoryId && data.CustID == CustID select data).Count() > 0 ? true : false,
                                               }).Distinct().AsQueryable();

                        if (isProfessional)
                            AllcategoryProducts = AllcategoryProducts.ToList().FindAll(x => x.MainCategoryName.ToLower() == "professionals").AsQueryable();
                        else
                            AllcategoryProducts = AllcategoryProducts.ToList().FindAll(x => x.MainCategoryName.ToLower() != "professionals").AsQueryable();
                        return AllcategoryProducts.Distinct().ToList();
                    }

                    #region not working
                    //SelectedcategoryProducts = (from s in dbContext.tblSubCategories
                    //                            join sl in dbContext.tblSubCategoryProductWithCusts on s.SubCategoryId equals sl.SubCategoryId
                    //                            where sl.CustID == CustID
                    //                            select new SubCategoryProducts
                    //                            {                                                        
                    //                                CategoryProductID = s.CategoryProductID,
                    //                                SubCategoryId = s.SubCategoryId,
                    //                                SubCategoryName = s.SubCategoryName,
                    //                                IsChecked = true,
                    //                            }).Distinct().ToList();
                    //AllcategoryProducts = AllcategoryProducts.Except(SelectedcategoryProducts).ToList();
                    //AllcategoryProducts = AllcategoryProducts.Where(s => s.SubCategoryId == 0).ToList();
                    //AllcategoryProducts = (from s in dbContext.tblSubCategoryProductWithCusts 
                    //                       join sl1 in dbContext.tblSubCategories on s.SubCategoryId equals sl1.SubCategoryId
                    //                       where sl1.SubCategoryId !=s.SubCategoryId
                    //                       select new SubCategoryProducts
                    //                       {
                    //                           ID = sl1.ID,
                    //                           //CategoryProductID = s.CategoryProductID,
                    //                           //SubCategoryId = s.SubCategoryId,
                    //                           //SubCategoryName = s.SubCategoryName,
                    //                           //IsChecked = false,
                    //                           //CreatedDate = s.CreatedDate,
                    //                           //CreatedID = s.CreatedID
                    //                       }).ToList();

                    //AllcategoryProducts.AddRange(SelectedcategoryProducts);

                    //List<SubCategoryProducts> SubcategoryList = new List<SubCategoryProducts>();

                    //SelectedcategoryProducts = (from sl in dbContext.tblChildCategories
                    //                            join sc in dbContext.tblSubCategoryProductWithCusts on sl.SubCategoryId equals sc.SubCategoryId
                    //                            join scl in dbContext.tblSubCategories on sc.SubCategoryId equals scl.SubCategoryId
                    //                            join cp in dbContext.tblCategoryProductWithCusts on sc.CategoryProductID equals cp.CategoryProductID
                    //                            join cpl in dbContext.tblCategoryProducts on cp.CategoryProductID equals cpl.CategoryProductID
                    //                            where sl.ItemName.Contains(SearchText)
                    //                            select new SubCategoryProducts
                    //                            {
                    //                                SubCategoryId = sc.SubCategoryId.Value,
                    //                                SubCategoryName = scl.SubCategoryName,
                    //                                MainCategoryName = cpl.MainCategoryName,
                    //                                IsChecked = false
                    //                            }).Distinct().ToList();

                    //SubcategoryList = (from s in AllcategoryProducts
                    //                   join sl in SelectedcategoryProducts on s.SubCategoryId equals sl.SubCategoryId
                    //                select new SubCategoryProducts
                    //                {
                    //                    SubCategoryId = s.SubCategoryId,
                    //                    SubCategoryName = s.SubCategoryName,
                    //                    MainCategoryName = s.MainCategoryName,
                    //                    IsChecked = s.IsChecked
                    //                }).Distinct().ToList();

                    //SubcategoryList = SubcategoryList.Distinct().ToList();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<tblCategoryProduct> GetCategory(List<SubCategoryList> SubCategoryList)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<tblCategoryProduct> categoryList = new List<tblCategoryProduct>();

                        foreach (var item in SubCategoryList)
                        {
                            tblCategoryProduct obj = new tblCategoryProduct();
                            obj = (from s in dbContext.tblCategoryProducts
                                   join sl in dbContext.tblSubCategories on s.CategoryProductID equals sl.CategoryProductID
                                   where sl.SubCategoryId == item.SubCategoryId
                                   select s).FirstOrDefault();
                            categoryList.Add(obj);
                        }

                        return categoryList.Distinct().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //GetBusinessTypes
        public List<BusinessTypes> GetBusinessTypes()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    IQueryable<BusinessTypes> businessTypeList;
                    businessTypeList = (from s in dbContext.tblBusinessTypes
                                        select new BusinessTypes
                                        {
                                            ID = s.ID,
                                            BusinessTypeID = s.ID,
                                            BusinessTypeName = s.Type,
                                            CreatedDate = s.CreatedDate,
                                            // CreatedID = s.CreatedBy
                                            CreatedBy = s.CreatedBy
                                        });

                    return businessTypeList.ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //GetStateList
        public List<tblState> GetStateList()
        {
            try
            {
                List<tblState> stateList = new List<tblState>();
                using (dbContext = new mwbtDealerEntities())
                {
                    stateList = (from s in dbContext.tblStates select s).ToList();
                }
                return stateList;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //GetCities
        public List<CityView> GetCities(int StateID, string CityName)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<CityView> cityList = new List<CityView>();
                    if (!string.IsNullOrEmpty(CityName))
                    {

                        var cityListw = (from s in dbContext.CityViews where s.ID == StateID select s);

                        cityList = cityListw.Where(d => d.VillageLocalityName.ToLower().Contains(CityName.ToLower())).ToList();
                        cityList.ForEach(c => c.VillageLocalityName = c.VillageLocalityName + c.DistrictName);//.ToList();
                    }
                    else
                    {
                        var cityListw = (from s in dbContext.CityViews
                                         where s.ID == StateID
                                         orderby s.VillageLocalityName
                                         select s);
                        cityList = cityListw.ToList();
                    }

                    return cityList.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<City> GetTierOneCities(string CityName = "")
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<City> cityList = new List<City>();
                    if (!string.IsNullOrEmpty(CityName))
                    {

                        var cityListw = (from s in dbContext.tblStateWithCities
                                         where s.TairTypeOfCityID == 1 && s.VillageLocalityName.ToLower().Contains(CityName)
                                         select s);
                        cityList = (from c in cityListw
                                    where c.VillageLocalityName.ToLower().Contains(CityName.ToLower())
                                    select new City
                                    {
                                        StateWithCityID = c.StatewithCityID,
                                        VillageLocalityName = c.VillageLocalityName,
                                    }).ToList();
                    }
                    else
                    {
                        var cityListw = (from s in dbContext.tblStateWithCities where s.TairTypeOfCityID == 1 select s);
                        cityList = (from c in cityListw
                                    select new City
                                    {
                                        StateWithCityID = c.StatewithCityID,
                                        VillageLocalityName = c.VillageLocalityName
                                    }).ToList();
                    }

                    return cityList.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Get Cities from a file
        public List<tblStateWithCity> GetCitiesOfState(int StateID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    //var jsonText = File.ReadAllText(FilePath);
                    //var cityList = JsonConvert.DeserializeObject<List<CityView>>(jsonText);

                    List<tblStateWithCity> cityList = new List<tblStateWithCity>();
                    if (StateID != 0)
                    {
                        cityList = (from s in dbContext.tblStateWithCities
                                    join c in dbContext.tblCustomerDetails on s.ID equals c.City
                                    where c.State == StateID
                                    orderby s.VillageLocalityName
                                    select s).ToList();
                    }
                    else
                    {
                        cityList = (from s in dbContext.tblStateWithCities
                                    join c in dbContext.tblCustomerDetails on s.ID equals c.City
                                    orderby s.VillageLocalityName
                                    select s).ToList();
                    }

                    return cityList.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //SearchProductDealer
        public object SearchProductDealer(SearchParameters searchParameters)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    //int? SubCategoryID = dbContext.tblChildCategories.AsNoTracking().Where(s => s.ItemId == searchParameters.ProductID).Select(s => s.SubCategoryId.Value).Distinct().FirstOrDefault();
                    int? SubCategoryID = (from c in dbContext.tblChildCategories join ic in dbContext.tblItemCategories on c.ID equals ic.ChildCategoryID where ic.ID == searchParameters.ProductID select c).
                        Select(s => s.SubCategoryId.Value).Distinct().FirstOrDefault();


                    int StateID = dbContext.tblStateWithCities.Find(searchParameters.CityId).StateID;

                    var customerList = (from c in dbContext.tblCustomerDetails
                                            //join btc in dbContext.tblBusinessTypewithCusts on c.CustID equals btc.CustID
                                        join btc in dbContext.tblBusinessTypewithCusts on c.ID equals btc.CustID
                                        //join bt in dbContext.tblBusinessTypes on btc.BusinessTypeID equals bt.BusinessTypeID
                                        join bt in dbContext.tblBusinessTypes on btc.BusinessTypeID equals bt.ID
                                        join st in dbContext.tblStateWithCities on c.City equals st.ID
                                        //join ccpc in dbContext.tblChildCategoryProductWithCusts on c.CustID equals ccpc.CustID
                                        //join sc in dbContext.tblSubCategoryProductWithCusts on c.CustID equals sc.CustID
                                        join sc in dbContext.tblSubCategoryProductWithCusts on c.ID equals sc.CustID
                                        where c.InterstCountry == true || (c.State == StateID && c.InterstState == true && c.InterstCountry == false) ||
                                        (c.InterstCity == true && c.City == searchParameters.CityId)
                                        //&& c.IsActive == true && c.CustID != searchParameters.CustID
                                         && c.IsActive == true && c.ID != searchParameters.CustID
                                        select new SearchResults
                                        {
                                            //CustID = c.CustID,
                                            CustID = c.ID,
                                            MobileNumber = c.MobileNumber,
                                            CustomerName = c.FirmName,
                                            BusinessType = bt.Type,
                                            //BusinessTypeID = btc.BusinessTypeID,
                                            BusinessTypeID = btc.BusinessTypeID.ToString(),
                                            //ChildCategoryId = ccpc.ChildCategoryId,
                                            City = c.City,
                                            InterestCity = c.InterstCity.Value,
                                            InterestState = c.InterstState.Value,
                                            InterestCountry = c.InterstCountry.Value,
                                            VillageLocalityname = st.VillageLocalityName,
                                            SubCategoryId = sc.SubCategoryId,
                                        }).Distinct().AsQueryable();


                    customerList = customerList.Where(c => c.CustID != searchParameters.CustID).AsQueryable();

                    #region
                    //var customerListInterstState = (from c in dbContext.tblCustomerDetails
                    //                                  join btc in dbContext.tblBusinessTypewithCusts on c.CustID equals btc.CustID
                    //                                  join bt in dbContext.tblBusinessTypes on btc.BusinessTypeID equals bt.BusinessTypeID
                    //                                  join st in dbContext.tblStateWithCities on c.City equals st.StatewithCityID
                    //                                  join ccpc in dbContext.tblChildCategoryProductWithCusts on c.CustID equals ccpc.CustID
                    //                                  where (c.InterstState== true && c.State == StateID) || c.InterstCity == true
                    //                                  select new SearchResults
                    //                                  {
                    //                                      CustID = c.CustID,
                    //                                      MobileNumber = c.MobileNumber,
                    //                                      CustomerName = c.FirmName,
                    //                                      BusinessType = bt.BusinessType,
                    //                                      BusinessTypeID = btc.BusinessTypeID,
                    //                                      ChildCategoryId = ccpc.ChildCategoryId,
                    //                                      City = c.City,
                    //                                      InterestCity = c.InterstCity.Value,
                    //                                      InterestState = c.InterstState.Value,
                    //                                      InterestCountry = c.InterstCountry.Value,
                    //                                      VillageLocalityname = st.VillageLocalityname
                    //                                  }).Distinct().ToList();



                    //var customerList = (from c in dbContext.tblCustomerDetails
                    //                    join btc in dbContext.tblBusinessTypewithCusts on c.CustID equals btc.CustID
                    //                    join bt in dbContext.tblBusinessTypes on btc.BusinessTypeID equals bt.BusinessTypeID
                    //                    join st in dbContext.tblStateWithCities on c.City equals st.StatewithCityID
                    //                    join ccpc in dbContext.tblChildCategoryProductWithCusts on c.CustID equals ccpc.CustID
                    //                    where c.CustID != searchParameters.CustID
                    //                    select new SearchResults
                    //                    {
                    //                        CustID = c.CustID,
                    //                        MobileNumber = c.MobileNumber,
                    //                        CustomerName = c.FirmName,
                    //                        BusinessType = bt.BusinessType,
                    //                        BusinessTypeID = btc.BusinessTypeID,
                    //                        ChildCategoryId = ccpc.ChildCategoryId,
                    //                        City = c.City,
                    //                        InterestCity = c.InterstCity.Value,
                    //                        InterestState = c.InterstState.Value,
                    //                        InterestCountry = c.InterstCountry.Value,
                    //                        VillageLocalityname = st.VillageLocalityname
                    //                    }).Distinct().ToList();


                    //customerList = customerList.Where(c => c.InterestState = true).ToList();
                    #endregion

                    //List<SearchResults> SearchResult = new List<SearchResults>();

                    //bool flag = false;

                    if (searchParameters.BusinessTypeIds != null)
                    {
                        customerList = customerList.ToList().FindAll(u => searchParameters.BusinessTypeIds.Exists(b => b.BusinessTypeID.ToString() == u.BusinessTypeID)).Distinct().AsQueryable();

                        //foreach (var item in searchParameters.BusinessTypeIds)
                        //{
                        //    for (int i = 0; i < customerList.Count; i++)
                        //    {
                        //        if (customerList[i].BusinessTypeID == item.BusinessTypeID.ToString())
                        //        {
                        //            flag = true;
                        //            SearchResults list = new SearchResults();
                        //            list = customerList[i];
                        //            SearchResult.Add(list);
                        //        }
                        //    }
                        //}
                    }

                    //if (flag == false)
                    //    SearchResult = customerList;

                    if (searchParameters.ProductID != null)
                    {
                        //customerList = SearchResult.Where(b => b.ChildCategoryId.Equals(searchParameters.ProductID)).ToList();
                        //customerList = SearchResult.Where(b => b.SubCategoryId.Equals(SubCategoryID)).ToList();
                        customerList = customerList.ToList().FindAll(b => b.SubCategoryId.Equals(SubCategoryID)).AsQueryable();
                    }

                    var NewCustomerList = customerList.Select(a => new
                    {
                        CustID = a.CustID,
                        CustomerName = a.CustomerName,
                        BusinessType = a.BusinessType,
                        MobileNumber = a.MobileNumber,
                        City = a.City,
                        VillageLocalityname = a.VillageLocalityname
                    }).OrderBy(ac => ac.BusinessType).Distinct().ToList();

                    if (NewCustomerList != null)
                    {
                        return NewCustomerList.Distinct().ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //SubmitCustomerQuery
        public SubmitQuery SubmitCustomerQuery(SubmitQuery submitQuery)
        {
            SubmitQuery Result = new SubmitQuery();
            //Insert Requirements
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                        tblselectedDealer tblselectedDealer = new tblselectedDealer();
                        tblselectedDealer.CustID = submitQuery.CustID;
                        tblselectedDealer.CityId = submitQuery.CityId;
                        tblselectedDealer.BusinessDemand = submitQuery.BusinessDemand;
                        tblselectedDealer.PurposeBusiness = submitQuery.TypeOfUse;
                        tblselectedDealer.OpenText = submitQuery.Requirements;
                        tblselectedDealer.Image = submitQuery.ProductPhoto;
                        tblselectedDealer.Image1 = submitQuery.ProductPhoto2;
                        tblselectedDealer.CreatedBy = submitQuery.CustID;
                        tblselectedDealer.CreatedDate = DateTimeNow;
                        tblselectedDealer.LastUpdatedMsgDate = DateTimeNow;
                        tblselectedDealer.BusinessDemandID = submitQuery.BusinessDemandID;
                        tblselectedDealer.ProductID = submitQuery.ProductID;
                        tblselectedDealer.ProfessionalRequirementID = submitQuery.ProfessionalRequirementID;
                        tblselectedDealer.IsAdEnquiry = submitQuery.IsAdEnquiry;
                        tblselectedDealer.EnquiryType = submitQuery.EnquiryType;
                        dbContext.tblselectedDealers.Add(tblselectedDealer);
                        dbContext.SaveChanges();
                        dbContext.Entry(tblselectedDealer).GetDatabaseValues();
                        int QueryID = tblselectedDealer.ID;
                        int count = 0;

                        //Insert Business Types
                        foreach (var item in submitQuery.BusinessTypeIds)
                        {
                            tblSelectedDealerBusinessType businessType = new tblSelectedDealerBusinessType();
                            businessType.QueryID = QueryID;
                            businessType.CustID = submitQuery.CustID;
                            businessType.BusinessTypeID = item.BusinessTypeID;
                            dbContext.tblSelectedDealerBusinessTypes.Add(businessType);
                        }

                        List<int> lstInt = submitQuery.CustIDS.Select(c => c.CustID).Distinct().ToList();
                        if (lstInt != null)
                        {
                            if (count == 0)
                            {
                                tblselectedDealerDetail tblselectedDealerDetails = new tblselectedDealerDetail();
                                tblselectedDealerDetails.QueryId = QueryID;
                                tblselectedDealerDetails.CustID = tblselectedDealer.CustID;
                                tblselectedDealerDetails.CreatedBy = tblselectedDealer.CustID;
                                tblselectedDealerDetails.CreatedDate = DateTimeNow;
                                tblselectedDealerDetails.LastUpdatedMsgDate = DateTimeNow;
                                tblselectedDealerDetails.IsRead = 0;
                                tblselectedDealerDetails.CityId = submitQuery.CityId;
                                tblselectedDealerDetails.SenderID = tblselectedDealer.CustID;
                                tblselectedDealerDetails.IsFavorite = 0;
                                tblselectedDealerDetails.IsDeleted = 0;
                                tblselectedDealerDetails.FavoriteCustID = "0";
                                tblselectedDealerDetails.DeletedCustID = "0";
                                tblselectedDealerDetails.SenderRead = 1;
                                dbContext.tblselectedDealerDetails.Add(tblselectedDealerDetails);
                                count++;
                            }
                            foreach (var item in lstInt)
                            {
                                tblselectedDealerDetail tblselectedDealerDetails = new tblselectedDealerDetail();
                                tblselectedDealerDetails.QueryId = QueryID;
                                tblselectedDealerDetails.CustID = Convert.ToInt32(item.ToString());
                                tblselectedDealerDetails.CreatedBy = tblselectedDealer.CustID;
                                tblselectedDealerDetails.CreatedDate = DateTimeNow;
                                tblselectedDealerDetails.LastUpdatedMsgDate = DateTimeNow;
                                tblselectedDealerDetails.IsRead = 0;
                                tblselectedDealerDetails.SenderID = tblselectedDealer.CustID;
                                tblselectedDealerDetails.IsFavorite = 0;
                                tblselectedDealerDetails.IsDeleted = 0;
                                tblselectedDealerDetails.FavoriteCustID = "0";
                                tblselectedDealerDetails.DeletedCustID = "0";
                                tblselectedDealerDetails.CityId = submitQuery.CityId;
                                tblselectedDealerDetails.SenderRead = 1;
                                dbContext.tblselectedDealerDetails.Add(tblselectedDealerDetails);

                                //insert into read/unread table for the first time
                                //tblReadWriteConversation tblReadWriteConversation = new tblReadWriteConversation();
                                //tblReadWriteConversation.CustID = submitQuery.CustID;
                                //tblReadWriteConversation.ReceiverID = Convert.ToInt32(item.ToString());
                                //tblReadWriteConversation.QueryId = QueryID;
                                //tblReadWriteConversation.CreatedBy = submitQuery.CustID;
                                //tblReadWriteConversation.CreatedDateTime = DateTime.Now;
                                //tblReadWriteConversation.ReadBy = submitQuery.CustID;

                                //Insert into conversation table
                                //tblUserConversation Conversations = new tblUserConversation();
                                //Conversations.CustID = obj.CustID;
                                //Conversations.IsDealer = item.CustID;
                                //Conversations.Message = obj.OpenText;
                                //Conversations.CreatedBy = obj.CustID;
                                //Conversations.CreatedDate = DateTimeNow;
                                //Conversations.QueryId = QueryID;
                                //Conversations.IsRead = 0;
                                //dbContext.tblUserConversations.Add(Conversations);
                            }

                            //Send Notification to all users
                            int[] arrayOfCustIDs = lstInt.ToArray();
                            //var users = dbContext.tblCustomerDetails.Where(c => arrayOfCustIDs.Contains(c.CustID)).ToList();
                            var users = dbContext.tblCustomerDetails.Where(c => arrayOfCustIDs.Contains(c.ID)).ToList();
                            //string ProductName = dbContext.tblChildCategories.Where(c => c.ItemId == submitQuery.ProductID).FirstOrDefault().ItemName;
                            string ProductName = (from c in dbContext.tblChildCategories join ic in dbContext.tblItemCategories on c.ID equals ic.ChildCategoryID where ic.ID == submitQuery.ProductID select ic).FirstOrDefault().ItemName;

                            //string SenderName = dbContext.tblCustomerDetails.Where(c => c.CustID == submitQuery.CustID).FirstOrDefault().FirmName;
                            string SenderName = dbContext.tblCustomerDetails.Where(c => c.ID == submitQuery.CustID).FirstOrDefault().FirmName;

                            string Body = "You have received a new enquiry for " + ProductName + " from " + SenderName + ", Enquiry Type :" + submitQuery.EnquiryType;
                            string[] Registration_Ids = users.Select(u => u.DeviceID).ToArray();
                            Notification notification = new Notification { Title = submitQuery.EnquiryType, Body = Body, NotificationDate = DateTimeNow, CategoryName = ProductName };
                            Helper.SendNotificationMultiple(Registration_Ids, notification);
                            PushNotifications pushNotifications = new PushNotifications()
                            {
                                Title = submitQuery.EnquiryType,
                                NotificationDate = DateTimeNow,
                                CategoryName = ProductName,
                                ImageURL = string.Empty,
                                PushNotification = Body,
                            };
                            SavePushNotificationsList(arrayOfCustIDs, pushNotifications, submitQuery.CustID);
                            dbContext.SaveChanges();
                            dbcxtransaction.Commit();
                            Result.ResponseMessage = "Enquiry sent Successfully";
                            Result.StatusCode = HttpStatusCode.OK;
                            return Result;
                        }
                        else
                        {
                            Result.ResponseMessage = "Please select atleast one user";
                            Result.StatusCode = HttpStatusCode.NotAcceptable;
                            return Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.ResponseMessage = "Internal Server Error";
                Result.StatusCode = HttpStatusCode.InternalServerError;
                return Result;
            }
        }

        public List<City> GetCityOfDealer(int UserId)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<CustomerQueries> list = (from sd in dbContext.tblselectedDealers
                                                  join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                                  join city in dbContext.tblStateWithCities on sdd.CityId equals city.StatewithCityID
                                                  where sdd.CustID == UserId && sdd.CreatedBy != UserId
                                                  select new CustomerQueries
                                                  {
                                                      QueryId = sdd.QueryId,
                                                      CustID = sdd.CustID.Value,
                                                      CityId = sdd.CityId,
                                                      VillageLocalityname = city.VillageLocalityName,
                                                      EnquiryDate = sd.CreatedDate,
                                                  }).Distinct().ToList<CustomerQueries>();

                    DateTime EnquiryConfiguredDate = DateTime.Now.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch));
                    list = list.Where(q => q.EnquiryDate.Value.Date > EnquiryConfiguredDate.Date).ToList();

                    //get deleted list
                    var deletedList = dbContext.tblDeleteConversations.Where(d => d.CustID == UserId).ToList();
                    //var conversationList = dbContext.tblUserConversations.Where(u => u.IsDealer == UserId).ToList();
                    //List<CustomerQueries> newList = (from lst in list
                    //                                 join d in deletedList on lst.QueryId equals d.QueryId
                    //                                 where lst.QueryId != d.QueryId
                    //                                 select new CustomerQueries
                    //                                 {
                    //                                     QueryId = lst.QueryId,
                    //                                     CityId = lst.CityId,
                    //                                     VillageLocalityname = lst.VillageLocalityname,
                    //                                 }).Distinct().ToList<CustomerQueries>();

                    if (deletedList != null)
                    {
                        list.RemoveAll(l => deletedList.Exists(d => d.QueryId == l.QueryId && (d.CustID == l.CustID)));
                    }

                    //if (conversationList != null)
                    //{
                    //    list = list.Where(l => conversationList.Exists(c => c.QueryId == l.QueryId && (c.CustID == l.CustID || c.IsDealer == l.CustID))).ToList();
                    //    //list.RemoveAll(l => !conversationList.Exists(c => c.QueryId == l.QueryId && (c.CustID == l.CustID || c.IsDealer == l.CustID)));
                    //}

                    List<CustomerQueries> distinctList = (from lst in list
                                                          select new CustomerQueries
                                                          {
                                                              CityId = lst.CityId,
                                                              VillageLocalityname = lst.VillageLocalityname
                                                          }).Distinct().ToList<CustomerQueries>();

                    distinctList = distinctList.Distinct().ToList();

                    distinctList = distinctList.GroupBy(p => p.CityId).Select(g => g.First()).ToList();

                    List<City> cityList = (from l in distinctList
                                           select new City
                                           {
                                               StateWithCityID = l.CityId,
                                               VillageLocalityName = l.VillageLocalityname
                                           }
                                           ).Distinct().ToList<City>();

                    return cityList.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }

        //GetCustomerQueries
        public List<CustomerQueries> GetCustomerQueries(int CustID, int IsFavorite)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<CustomerQueries> QueryList = new List<CustomerQueries>();

                    string WebsiteURL = ConfigurationManager.AppSettings["WebsiteURL"];

                    //admin configured date filter
                    DateTime SFromDate = DateTime.Now.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch)).Date;
                    //QueryList = QueryList.Where(q => q.EnquiryDate > SFromDate).ToList();

                    var custRecLists = (from sdd in dbContext.tblselectedDealerDetails
                                        join sd in dbContext.tblselectedDealers on sdd.QueryId equals sd.ID
                                        //join cd in dbContext.tblCustomerDetails on sd.CustID equals cd.CustID
                                        join cd in dbContext.tblCustomerDetails on sd.CustID equals cd.ID
                                        join sc in dbContext.tblStateWithCities on sd.CityId equals sc.StatewithCityID
                                        //join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ItemId
                                        join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ID
                                        join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID
                                        where sdd.CustID == CustID && sdd.IsDeleted == 0 && cd.IsActive == true
                                        && (DbFunctions.TruncateTime(sdd.CreatedDate) >= SFromDate)
                                        && cd.IsActive == true
                                        select new CustomerQueries
                                        {
                                            QueryId = sdd.QueryId,
                                            //CustID = cd.CustID,
                                            CustID = cd.ID,
                                            FirmName = cd.FirmName.Trim(),
                                            EmailID = cd.EmailID.Trim(),
                                            MobileNumber = cd.MobileNumber,
                                            //BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.BusinessDemandID == sd.BusinessDemandID).FirstOrDefault().BusinessDemand),
                                            BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.ID == sd.BusinessDemandID).FirstOrDefault().Demand),
                                            BusinessDemandID = sd.BusinessDemandID,
                                            PurposeOfBusiness = sd.PurposeBusiness.Trim(),
                                            VillageLocalityname = sc.VillageLocalityName.Trim(),
                                            Requirements = sd.OpenText.Trim(),
                                            LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                            IsRead = sdd.IsRead.Value,
                                            SenderRead = sdd.SenderRead.Value,
                                            SenderID = sdd.SenderID.Value,
                                            //ChildCategoryName = cc.ItemName.Trim(),
                                            ChildCategoryName = ic.ItemName.Trim(),
                                            //IsFavorite = (dbContext.tblFavoriteConversations.Where(f => f.CustID == CustID && f.ReceiverID == cd.CustID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                            IsFavorite = (dbContext.tblFavoriteConversations.Where(f => f.CustID == CustID && f.ReceiverID == cd.ID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                            FavoriteCustID = sdd.FavoriteCustID,
                                            //IsDeleted = (dbContext.tblDeleteConversations.Where(f => f.CustID == CustID && f.ReceiverID == cd.CustID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                            IsDeleted = (dbContext.tblDeleteConversations.Where(f => f.CustID == CustID && f.ReceiverID == cd.ID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                            CityId = sdd.CityId,
                                            CreatedBy = sdd.CreatedBy,
                                            //RequirementName = (dbContext.tblProfessionalRequirements.Where(p => p.ProfessionalRequirementID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),
                                            RequirementName = (dbContext.tblProfessionalRequirements.Where(p => p.ID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),
                                            ProfessionalRequirementID = sd.ProfessionalRequirementID,
                                            EnquiryDate = sd.CreatedDate,
                                            EnquiryType = sd.EnquiryType,
                                            SenderImage = !string.IsNullOrEmpty(cd.UserImage) ? WebsiteURL + cd.UserImage : string.Empty,
                                            //UserPhoto = cd.UserPhoto,
                                        }).AsQueryable();

                    QueryList = custRecLists.ToList();
                    if (QueryList != null && QueryList.Count() > 0)
                    {
                        QueryList = QueryList.Where(u => u.CustID != CustID).Distinct().ToList();

                        //remove deleted
                        QueryList = QueryList.Where(u => u.IsDeleted != 1).Distinct().ToList();

                        //Set IsRead
                        foreach (var item in QueryList)
                        {
                            if (item.CreatedBy == CustID)
                                item.IsRead = item.SenderRead;
                            else
                                item.IsRead = item.IsRead;
                        }

                        if (IsFavorite == 1)
                        {
                            QueryList = QueryList.Where(q => q.IsFavorite == 1).ToList();
                        }

                        QueryList = QueryList.Distinct().OrderByDescending(m => m.LastUpdatedMsgDate).ThenByDescending(m => m.IsFavorite).ToList();
                    }
                    return QueryList;
                }
            }
            catch (Exception ex)
            {
                //string strMessage= ex.Message;

                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }
        private class customerQueriesComparer : IEqualityComparer<CustomerQueries>
        {
            public bool Equals(CustomerQueries x, CustomerQueries y)
            {
                return x.CustID == y.CustID && x.QueryId == y.QueryId;
            }

            // If Equals() returns true for a pair of objects 
            // then GetHashCode() must return the same value for these objects.

            public int GetHashCode(CustomerQueries myModel)
            {
                return myModel.QueryId.GetHashCode();
            }
        }

        public List<CustomerQueries> FilterCustomerQueries(SearchParameters searchParameters)
        {
            try
            {
                List<CustomerQueries> customerQueries = GetCustomerQueries(searchParameters.CustID, searchParameters.IsFavorite);

                using (var dbContext = new mwbtDealerEntities())
                {
                    List<CustomerQueries> FilteredList = new List<CustomerQueries>();
                    bool flag = false;

                    if (searchParameters.CityIdList != null && searchParameters.CityIdList.Count() > 0)
                        customerQueries = customerQueries.Where(u => searchParameters.CityIdList.Exists(b => b.StateWithCityID == u.CityId)).Distinct().ToList();
                    //customerQueries = customerQueries.Where(u => u.CityId == searchParameters.CityId).Distinct().ToList();

                    if (searchParameters.BusinessDemand != null && searchParameters.BusinessDemand.Count() > 0)
                        //customerQueries = customerQueries.Where(u => searchParameters.BusinessDemand.Exists(b => b.BusinessDemandID == u.BusinessDemandID)).Distinct().ToList();
                        customerQueries = customerQueries.Where(u => searchParameters.BusinessDemand.Exists(b => b.ID == u.BusinessDemandID)).Distinct().ToList();

                    if (!string.IsNullOrEmpty(searchParameters.EnquiryType))
                        customerQueries = customerQueries.Where(u => u.EnquiryType.ToLower() == searchParameters.EnquiryType.ToLower()).Distinct().ToList();

                    if (searchParameters.BusinessTypeIds.Count() > 0)
                    {
                        customerQueries = (from sdd in customerQueries
                                           join btc in dbContext.tblBusinessTypewithCusts on sdd.CustID equals btc.CustID
                                           select new CustomerQueries
                                           {
                                               QueryId = sdd.QueryId,
                                               CustID = sdd.CustID,
                                               FirmName = sdd.FirmName,
                                               EmailID = sdd.EmailID,
                                               MobileNumber = sdd.MobileNumber,
                                               BusinessDemand = sdd.BusinessDemand,
                                               PurposeOfBusiness = sdd.PurposeOfBusiness,
                                               VillageLocalityname = sdd.VillageLocalityname,
                                               Requirements = sdd.Requirements,
                                               LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                               IsRead = sdd.IsRead,
                                               SenderRead = sdd.SenderRead,
                                               SenderID = sdd.SenderID,
                                               ChildCategoryName = sdd.ChildCategoryName,
                                               IsFavorite = sdd.IsFavorite,
                                               FavoriteCustID = sdd.FavoriteCustID,
                                               IsDeleted = sdd.IsDeleted,
                                               CityId = sdd.CityId,
                                               CreatedBy = sdd.CreatedBy,
                                               BusinessTID = btc.BusinessTypeID.ToString(),
                                               ProfessionalRequirementID = sdd.ProfessionalRequirementID,
                                               RequirementName = sdd.RequirementName,
                                               EnquiryDate = sdd.EnquiryDate,
                                               EnquiryType = sdd.EnquiryType,
                                               SenderImage = sdd.SenderImage,
                                           }).ToList();

                        foreach (var item in searchParameters.BusinessTypeIds)
                        {

                            var CustBusinessTList = customerQueries.Where(d => d.BusinessTID == item.BusinessTypeID.ToString()).ToList();
                            if (CustBusinessTList != null)
                            {
                                var isExists = (from getOld in FilteredList
                                                join newDAta in customerQueries on getOld.CustID equals newDAta.CustID
                                                where getOld.CustID == newDAta.CustID && getOld.QueryId == newDAta.QueryId
                                                select getOld);

                                if (isExists == null || isExists.Count() == 0)
                                {
                                    flag = true;
                                    FilteredList.AddRange(CustBusinessTList);
                                }
                            }
                        }
                        FilteredList = (from sdd in FilteredList
                                        select new CustomerQueries
                                        {
                                            QueryId = sdd.QueryId,
                                            CustID = sdd.CustID,
                                            FirmName = sdd.FirmName,
                                            EmailID = sdd.EmailID,
                                            MobileNumber = sdd.MobileNumber,
                                            BusinessDemand = sdd.BusinessDemand,
                                            PurposeOfBusiness = sdd.PurposeOfBusiness,
                                            VillageLocalityname = sdd.VillageLocalityname,
                                            Requirements = sdd.Requirements,
                                            LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                            IsRead = sdd.IsRead,
                                            SenderID = sdd.SenderID,
                                            ChildCategoryName = sdd.ChildCategoryName,
                                            IsFavorite = sdd.IsFavorite,
                                            FavoriteCustID = sdd.FavoriteCustID,
                                            IsDeleted = sdd.IsDeleted,
                                            CityId = sdd.CityId,
                                            CreatedBy = sdd.CreatedBy,
                                            BusinessTID = sdd.BusinessTID,
                                            ProfessionalRequirementID = sdd.ProfessionalRequirementID,
                                            RequirementName = sdd.RequirementName,
                                            EnquiryDate = sdd.EnquiryDate,
                                            EnquiryType = sdd.EnquiryType,
                                            SenderImage = sdd.SenderImage,
                                        }).Distinct().ToList();
                    }

                    if (flag == false)
                        FilteredList = customerQueries;

                    //Date Filter
                    if (!string.IsNullOrEmpty(searchParameters.FromDate) && !string.IsNullOrEmpty(searchParameters.ToDate))
                    {
                        DateTime FromDate = Convert.ToDateTime(searchParameters.FromDate);
                        DateTime ToDate = Convert.ToDateTime(searchParameters.ToDate);
                        DateTime SFromDate = ToDate.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch));

                        int daysFromDate = (ToDate.Date - FromDate.Date).Days;
                        int daysSFromDate = (ToDate.Date - SFromDate.Date).Days;

                        if (daysFromDate <= daysSFromDate)
                        {
                            FilteredList = FilteredList.Where(q => q.LastUpdatedMsgDate.Value.Date >= FromDate.Date && q.LastUpdatedMsgDate.Value.Date <= ToDate.Date).ToList();
                        }
                        else
                        {
                            FilteredList = FilteredList.Where(q => q.LastUpdatedMsgDate.Value.Date >= SFromDate.Date && q.LastUpdatedMsgDate.Value.Date <= ToDate.Date).ToList();
                        }

                        #region
                        //if (FromDate <= SFromDate)
                        //{
                        //    FilteredList = FilteredList.Where(q => q.LastUpdatedMsgDate.Value.Date >= FromDate.Date && q.LastUpdatedMsgDate.Value.Date <= ToDate.Date).ToList();
                        //}
                        //else
                        //{
                        //    FilteredList = FilteredList.Where(q => q.LastUpdatedMsgDate.Value.Date >= SFromDate.Date && q.LastUpdatedMsgDate.Value.Date <= ToDate.Date).ToList();
                        //}
                        #endregion
                    }

                    //admin configured date filter

                    DateTime EnquiryFromDate = DateTime.Now.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch));
                    FilteredList = FilteredList.Where(q => q.EnquiryDate > EnquiryFromDate).ToList();

                    FilteredList = FilteredList.Distinct().OrderByDescending(m => m.LastUpdatedMsgDate).ToList();
                    return FilteredList;
                }

            }
            catch (Exception ex)
            {
                //string strMessage= ex.Message;

                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }

        //GetDealerBusinessTypes
        public List<tblBusinessType> GetDealerBusinessTypes(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<tblBusinessType> BusinessTypeList = new List<tblBusinessType>();
                        BusinessTypeList = (from s in dbContext.tblBusinessTypes
                                            join sl in dbContext.tblBusinessTypewithCusts on s.ID equals sl.BusinessTypeID
                                            where sl.CustID == CustID
                                            select s).ToList();

                        return BusinessTypeList;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //GetAllCities
        public List<tblStateWithCity> GetAllCities(string prefix = null)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<tblStateWithCity> cityList = new List<tblStateWithCity>();
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        cityList = (from s in dbContext.tblStateWithCities
                                    join c in dbContext.tblCustomerDetails on s.ID equals c.City //s.StatewithCityID equals c.City
                                    where s.VillageLocalityName.Contains(prefix)
                                    select s).Distinct().ToList();
                    }
                    else
                    {
                        cityList = (from s in dbContext.tblStateWithCities
                                    join c in dbContext.tblCustomerDetails on s.ID equals c.City //s.StatewithCityID equals c.City
                                    select s).Distinct().ToList();
                    }
                    return cityList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //SendMessage
        public tblUserConversation SendMessage(tblUserConversation obj)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    //add back the conversation if the receiver deleted the conversation from his end
                    tblDeleteConversation tblDeleteConversation = dbContext.tblDeleteConversations.AsNoTracking().Where(d => d.QueryId == obj.QueryId && d.CustID == obj.IsDealer && d.ReceiverID == obj.CustID).FirstOrDefault();
                    if (tblDeleteConversation != null)
                    {
                        dbContext.Entry(tblDeleteConversation).State = EntityState.Deleted;
                    }

                    //add back the enquiry if the sender deleted the enquiry from his end
                    tblDeleteEnquiry tblDeleteEnquiry = dbContext.tblDeleteEnquiries.AsNoTracking().Where(x => x.QueryId == obj.QueryId && x.CustID == obj.IsDealer).FirstOrDefault();
                    if (tblDeleteEnquiry != null)
                    {
                        dbContext.Entry(tblDeleteEnquiry).State = EntityState.Deleted;
                    }

                    //Update the last updated time
                    #region
                    //Set Sender ID
                    tblselectedDealerDetail sdDe = (from sd in dbContext.tblselectedDealerDetails where sd.CustID == obj.CustID && sd.QueryId == obj.QueryId && sd.CreatedBy == obj.IsDealer select sd).FirstOrDefault();
                    if (sdDe != null)
                    {
                        //If the sender is Enquirer
                        sdDe.IsRead = 1;
                        sdDe.SenderID = obj.CustID;
                        sdDe.SenderRead = 0;
                        sdDe.LastUpdatedMsgDate = DateTimeNow;

                        dbContext.tblselectedDealerDetails.Attach(sdDe);
                        dbContext.Entry(sdDe).Property(x => x.IsRead).IsModified = true;
                        dbContext.Entry(sdDe).Property(x => x.SenderRead).IsModified = true;
                        dbContext.Entry(sdDe).Property(x => x.SenderID).IsModified = true;
                        dbContext.Entry(sdDe).Property(x => x.LastUpdatedMsgDate).IsModified = true;
                        dbContext.SaveChanges();

                        tblselectedDealer newDealerItem = new tblselectedDealer();
                        newDealerItem = dbContext.tblselectedDealers.Where(d => d.ID == obj.QueryId).FirstOrDefault();
                        if (newDealerItem != null)
                        {
                            newDealerItem.LastUpdatedMsgDate = DateTimeNow;
                            dbContext.tblselectedDealers.Attach(newDealerItem);
                            dbContext.Entry(newDealerItem).Property(x => x.LastUpdatedMsgDate).IsModified = true;
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        //if the sender is the enquired person
                        tblselectedDealerDetail sdDeC = (from sd in dbContext.tblselectedDealerDetails where sd.CustID == obj.IsDealer && sd.QueryId == obj.QueryId && sd.CreatedBy == obj.CustID select sd).FirstOrDefault();
                        if (sdDeC != null)
                        {
                            sdDeC.SenderRead = 1;
                            sdDeC.IsRead = 0;
                            sdDeC.SenderID = obj.CustID;
                            sdDeC.LastUpdatedMsgDate = DateTimeNow;

                            dbContext.tblselectedDealerDetails.Attach(sdDeC);
                            dbContext.Entry(sdDeC).Property(x => x.SenderRead).IsModified = true;
                            dbContext.Entry(sdDeC).Property(x => x.IsRead).IsModified = true;
                            dbContext.Entry(sdDeC).Property(x => x.SenderID).IsModified = true;
                            dbContext.Entry(sdDeC).Property(x => x.LastUpdatedMsgDate).IsModified = true;
                            dbContext.SaveChanges();
                        }
                        tblselectedDealer newDealerItem = new tblselectedDealer();
                        newDealerItem = dbContext.tblselectedDealers.Where(q => q.ID == obj.QueryId).FirstOrDefault();
                        if (newDealerItem != null)
                        {
                            newDealerItem.LastUpdatedMsgDate = DateTimeNow;
                            dbContext.tblselectedDealers.Attach(newDealerItem);
                            dbContext.Entry(newDealerItem).Property(x => x.LastUpdatedMsgDate).IsModified = true;
                            dbContext.SaveChanges();
                        }
                    }
                    #endregion
                    obj.CreatedDate = DateTimeNow;
                    //obj.CreatedBy = obj.CustID;
                    obj.CreatedBy = obj.ID;
                    if (string.IsNullOrEmpty(obj.Message))
                        obj.Message = null;
                    obj.IsRead = 0;
                    obj.IsDeleted = 0;
                    obj.DeletedCustID = "0";
                    dbContext.tblUserConversations.Add(obj);
                    dbContext.SaveChanges();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }


        //public tblUserConversation SendMessage(tblUserConversation obj)
        //{
        //    try
        //    {
        //        using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
        //        {
        //            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        //            //add the conversation if the receiver deleted the conversation from his end
        //            tblDeleteConversation tblDeleteConversation = dbContext.tblDeleteConversations.AsNoTracking().Where(d => d.QueryId == obj.QueryId && d.CustID == obj.IsDealer && d.ReceiverID == obj.CustID).FirstOrDefault();
        //            if (tblDeleteConversation != null)
        //            {
        //                dbContext.Entry(tblDeleteConversation).State = EntityState.Deleted;
        //            }
        //            //add the conversation if the receiver deleted the conversation from his end

        //            tblselectedDealer newDealerItem = new tblselectedDealer();
        //            newDealerItem = dbContext.tblselectedDealers.Where(q => q.ID == obj.QueryId && q.CustID == obj.CustID).FirstOrDefault();
        //            if (newDealerItem != null)
        //            {
        //                newDealerItem.LastUpdatedMsgDate = DateTimeNow;
        //                newDealerItem.IsRead = 0;
        //                dbContext.Entry(newDealerItem).State = EntityState.Modified;
        //            }

        //            var dealerList = dbContext.tblselectedDealerDetails.Where(q => q.QueryId == obj.QueryId && (q.CustID == obj.CustID || q.CreatedBy == obj.CustID)).ToList();
        //            foreach (var item in dealerList)
        //            {

        //                item.LastUpdatedMsgDate = DateTimeNow;
        //                item.IsRead = 0;
        //                item.IsDeleted = 0;
        //                dbContext.Entry(item).State = EntityState.Modified;

        //                var newDealer = dbContext.tblselectedDealers.Where(x => x.ID == item.QueryId).FirstOrDefault();
        //                newDealer.LastUpdatedMsgDate = DateTimeNow;
        //                dbContext.Entry(newDealer).State = EntityState.Modified;
        //            }                    

        //            obj.CreatedDate = DateTimeNow;
        //            obj.CreatedBy = obj.CustID;
        //            if (string.IsNullOrEmpty(obj.Message))
        //                obj.Message = null;
        //            obj.IsRead = 0;
        //            obj.IsDeleted = 0;
        //            obj.DeletedCustID = "0";
        //            dbContext.tblUserConversations.Add(obj);
        //            dbContext.SaveChanges();
        //            return obj;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}

        //GetMessages
        public SendMessageParameters GetMessages(int CustID, int QueryId, int ReceiverID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    //Set IsRead to 1 
                    #region
                    //tblUserConversation Conversation = (from uc in dbContext.tblUserConversations where uc.CustID == ReceiverID && uc.QueryId == QueryId && uc.IsDealer == CustID select uc).FirstOrDefault();
                    //Conversation.IsRead = 1;

                    //dbContext.tblUserConversations.Attach(Conversation);
                    //dbContext.Entry(Conversation).Property(x => x.IsRead).IsModified = true;
                    //dbContext.SaveChanges();

                    //#region Set read/unread to dealer
                    ////insert into read/unread table 
                    //tblReadWriteConversation tblReadWriteConversation = dbContext.tblReadWriteConversations.AsNoTracking().Where(r => r.QueryId == QueryId && (r.CustID == CustID || r.CustID == ReceiverID)).FirstOrDefault();
                    //if (tblReadWriteConversation != null)
                    //{
                    //    tblReadWriteConversation.ReadBy = CustID;
                    //    dbContext.tblReadWriteConversations.Attach(tblReadWriteConversation);
                    //    dbContext.Entry(tblReadWriteConversation).Property(r => r.ReadBy).IsModified = true;
                    //    dbContext.SaveChanges();
                    //}

                    tblselectedDealerDetail sdDe = (from sd in dbContext.tblselectedDealerDetails where sd.CreatedBy == ReceiverID && sd.QueryId == QueryId && sd.CustID == CustID select sd).FirstOrDefault();
                    if (sdDe != null)
                    {
                        //If the sender is Enquirer
                        sdDe.IsRead = 1;

                        dbContext.tblselectedDealerDetails.Attach(sdDe);
                        dbContext.Entry(sdDe).Property(x => x.IsRead).IsModified = true;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        //if the sender is the enquired person
                        tblselectedDealerDetail sdDeC = (from sd in dbContext.tblselectedDealerDetails where sd.CreatedBy == CustID && sd.QueryId == QueryId && sd.CustID == ReceiverID select sd).FirstOrDefault();
                        if (sdDeC != null)
                        {
                            //sdDeC.IsRead = 1;
                            sdDeC.SenderRead = 1;

                            dbContext.tblselectedDealerDetails.Attach(sdDeC);
                            dbContext.Entry(sdDeC).Property(x => x.SenderRead).IsModified = true;
                            //dbContext.Entry(sdDeC).Property(x => x.SenderRead).IsModified = true;
                            dbContext.SaveChanges();
                        }
                    }
                    #endregion

                    List<MessageList> messageList = new List<MessageList>();
                    messageList = (from s in dbContext.tblUserConversations
                                   where s.QueryId == QueryId && (s.CustID == CustID && s.IsDealer == ReceiverID) || (s.CustID == ReceiverID && s.IsDealer == CustID)
                                   select new MessageList
                                   {
                                       ID = s.ID,
                                       QueryId = s.QueryId,
                                       CustID = s.CustID,
                                       Message = s.Message,
                                       IsDealer = s.IsDealer,
                                       IsCustomer = s.IsCustomer,
                                       Image = s.Image,
                                       CreatedDate = s.CreatedDate,
                                       IsRead = s.IsRead,
                                       IsArchived = s.IsArchived
                                   }).ToList().Where(u => u.QueryId == QueryId).ToList();

                    List<tblDeleteChat> deleteChatList = dbContext.tblDeleteChats.Where(d => d.CustID == CustID && d.QueryId == QueryId).ToList();

                    if (deleteChatList != null)
                    {
                        messageList.RemoveAll(l => deleteChatList.Exists(d => d.QueryId == l.QueryId && (d.CustID == l.CustID || d.CustID == l.IsDealer) && d.ChatID == l.ID));
                    }

                    //messageList = messageList.Where(m => m.IsArchived != 1).ToList();

                    var isexists = dbContext.tblselectedDealers.Where(sd => sd.CustID == CustID && sd.ID == QueryId).AsNoTracking().FirstOrDefault();
                    SendMessageParameters sendMessageParameters = new SendMessageParameters();
                    if (isexists != null)
                    {
                        sendMessageParameters = (from sd in dbContext.tblselectedDealers
                                                 join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                                 //join cd in dbContext.tblCustomerDetails on sdd.CustID equals cd.CustID
                                                 join cd in dbContext.tblCustomerDetails on sdd.CustID equals cd.ID
                                                 join sc in dbContext.tblStateWithCities on cd.City equals sc.StatewithCityID
                                                 where sdd.CustID == ReceiverID && sd.ID == QueryId
                                                 && sd.CustID == CustID
                                                 select new SendMessageParameters
                                                 {
                                                     QueryId = sd.ID,
                                                     CustID = sd.CustID,
                                                     FirmName = cd.FirmName,
                                                     MobileNumber = cd.MobileNumber,
                                                     VillageLocalityname = sc.VillageLocalityName,
                                                     //BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.BusinessDemandID == sd.BusinessDemandID).FirstOrDefault().BusinessDemand),
                                                     BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.ID == sd.BusinessDemandID).FirstOrDefault().Demand),
                                                     PurposeBusiness = sd.PurposeBusiness,
                                                     Requirements = sd.OpenText,
                                                     Image = sd.Image,
                                                     Image2 = sd.Image1,
                                                     CreatedDate = sd.CreatedDate,
                                                     IsSender = (sd.CustID == CustID ? 1 : 0)
                                                 }).FirstOrDefault();
                    }
                    else
                    {
                        sendMessageParameters = (from sd in dbContext.tblselectedDealers
                                                 join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                                 //join cd in dbContext.tblCustomerDetails on sd.CustID equals cd.CustID
                                                 join cd in dbContext.tblCustomerDetails on sd.CustID equals cd.ID
                                                 join sc in dbContext.tblStateWithCities on cd.City equals sc.StatewithCityID
                                                 where sdd.CustID == CustID && sd.ID == QueryId
                                                && sd.CustID == ReceiverID
                                                 select new SendMessageParameters
                                                 {
                                                     QueryId = sd.ID,
                                                     CustID = sd.CustID,
                                                     FirmName = cd.FirmName,
                                                     MobileNumber = cd.MobileNumber,
                                                     VillageLocalityname = sc.VillageLocalityName,
                                                     //BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.BusinessDemandID == sd.BusinessDemandID).FirstOrDefault().BusinessDemand),
                                                     BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.ID == sd.BusinessDemandID).FirstOrDefault().Demand),
                                                     PurposeBusiness = sd.PurposeBusiness,
                                                     Requirements = sd.OpenText,
                                                     Image = sd.Image,
                                                     Image2 = sd.Image1,
                                                     CreatedDate = sd.CreatedDate,
                                                     IsSender = (sd.CustID == CustID ? 1 : 0)
                                                 }).FirstOrDefault();
                    }

                    if (messageList.Count <= 0)
                    {
                        sendMessageParameters.MessageList = new List<MessageList>();
                    }
                    else
                    {
                        sendMessageParameters.MessageList = messageList;
                    }

                    return sendMessageParameters;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Delete Message Conversation
        //public List<CustomerQueries> DeleteConversation(int CustID, List<DeleteConversations> deleteConversations)
        //{
        //    try
        //    {
        //        using (dbContext = new mwbtDealerEntities())
        //        {
        //            foreach (var item in deleteConversations)
        //            {
        //                //set isdeleted to 1 in conversation table
        //                var Conversations = dbContext.tblUserConversations.Where(uc => uc.CustID == item.CustID && uc.QueryId == item.QueryId && uc.IsDealer == item.ReceiverID).ToList();
        //                if (Conversations != null)
        //                {
        //                    foreach (var ConvoList in Conversations)
        //                    {
        //                        ConvoList.IsDeleted = 1;
        //                        ConvoList.DeletedCustID = CustID.ToString().Trim();
        //                        dbContext.Entry(ConvoList).Property(c => c.IsDeleted).IsModified = true;
        //                        dbContext.Entry(ConvoList).Property(c => c.DeletedCustID).IsModified = true;
        //                        dbContext.SaveChanges();
        //                    }
        //                }

        //                var MainCustomer = dbContext.tblselectedDealerDetails.Where(sdd => sdd.CustID == item.ReceiverID && sdd.QueryId == item.QueryId && sdd.CreatedBy == item.CustID).FirstOrDefault();

        //                if (MainCustomer != null)
        //                {
        //                    if (MainCustomer.IsDeleted == 0 && MainCustomer.DeletedCustID == "0")
        //                    {
        //                        MainCustomer.IsDeleted = 1;
        //                        MainCustomer.DeletedCustID = CustID.ToString().Trim();
        //                    }
        //                    else if (MainCustomer.IsDeleted == 1 && MainCustomer.DeletedCustID != "0")
        //                    {
        //                        MainCustomer.IsDeleted = 1;
        //                        MainCustomer.DeletedCustID = "All";
        //                    }
        //                    dbContext.tblselectedDealerDetails.Attach(MainCustomer);
        //                    dbContext.Entry(MainCustomer).Property(sd => sd.IsDeleted).IsModified = true;
        //                    dbContext.Entry(MainCustomer).Property(sd => sd.DeletedCustID).IsModified = true;
        //                    dbContext.SaveChanges();
        //                }
        //                else
        //                {
        //                    var Receiver = dbContext.tblselectedDealerDetails.Where(sdd => sdd.CustID == item.CustID && sdd.QueryId == item.QueryId && sdd.CreatedBy == item.ReceiverID).FirstOrDefault();

        //                    if (Receiver != null)
        //                    {
        //                        if (Receiver.IsDeleted == 0 && Receiver.DeletedCustID == "0")
        //                        {
        //                            Receiver.IsDeleted = 1;
        //                            Receiver.DeletedCustID = CustID.ToString().Trim();
        //                        }
        //                        else if (Receiver.IsDeleted == 1 && Receiver.DeletedCustID != "0")
        //                        {
        //                            Receiver.IsDeleted = 1;
        //                            Receiver.DeletedCustID = "All";
        //                        }
        //                        dbContext.tblselectedDealerDetails.Attach(Receiver);
        //                        dbContext.Entry(Receiver).Property(sd => sd.IsDeleted).IsModified = true;
        //                        dbContext.Entry(Receiver).Property(sd => sd.DeletedCustID).IsModified = true;
        //                        dbContext.SaveChanges();
        //                    }
        //                }
        //            }
        //            return GetCustomerQueries(CustID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}



        //Set Favorite for conversation
        //public List<CustomerQueries> AddOrDeleteFavorite(int CustID, int QueryId, int ReceiverID, int IsFavorite)
        //{
        //    try
        //    {
        //        using (dbContext = new mwbtDealerEntities())
        //        {
        //            if (IsFavorite == 1)
        //            {
        //                var MainCustomer = dbContext.tblselectedDealerDetails.Where(sdd => sdd.CustID == ReceiverID && sdd.QueryId == QueryId && sdd.CreatedBy == CustID).FirstOrDefault();

        //                if (MainCustomer != null)
        //                {
        //                    if (MainCustomer.IsFavorite == 0 && MainCustomer.FavoriteCustID == "0")
        //                    {
        //                        MainCustomer.IsFavorite = 1;
        //                        MainCustomer.FavoriteCustID = CustID.ToString().Trim();
        //                    }
        //                    else if (MainCustomer.IsFavorite == 1 && MainCustomer.FavoriteCustID != "0")
        //                    {
        //                        MainCustomer.IsFavorite = 1;
        //                        MainCustomer.FavoriteCustID = "All";
        //                    }
        //                    dbContext.tblselectedDealerDetails.Attach(MainCustomer);
        //                    dbContext.Entry(MainCustomer).Property(sd => sd.IsFavorite).IsModified = true;
        //                    dbContext.Entry(MainCustomer).Property(sd => sd.FavoriteCustID).IsModified = true;
        //                    dbContext.SaveChanges();
        //                }
        //                else
        //                {
        //                    var Receiver = dbContext.tblselectedDealerDetails.Where(sdd => sdd.CustID == CustID && sdd.QueryId == QueryId && sdd.CreatedBy == ReceiverID).FirstOrDefault();

        //                    if (Receiver != null)
        //                    {
        //                        if (Receiver.IsFavorite == 0 && Receiver.FavoriteCustID == "0")
        //                        {
        //                            Receiver.IsFavorite = 1;
        //                            Receiver.FavoriteCustID = CustID.ToString().Trim();
        //                        }
        //                        else if (Receiver.IsFavorite == 1 && Receiver.FavoriteCustID != "0")
        //                        {
        //                            Receiver.IsFavorite = 1;
        //                            Receiver.FavoriteCustID = "All";
        //                        }
        //                        dbContext.tblselectedDealerDetails.Attach(Receiver);
        //                        dbContext.Entry(Receiver).Property(sd => sd.IsFavorite).IsModified = true;
        //                        dbContext.Entry(Receiver).Property(sd => sd.FavoriteCustID).IsModified = true;
        //                        dbContext.SaveChanges();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var MainCustomer = dbContext.tblselectedDealerDetails.Where(sdd => sdd.CustID == ReceiverID && sdd.QueryId == QueryId && sdd.CreatedBy == CustID).FirstOrDefault();

        //                if (MainCustomer != null)
        //                {
        //                    if (MainCustomer.IsFavorite == 1 && MainCustomer.FavoriteCustID == "All")
        //                    {
        //                        MainCustomer.IsFavorite = 0;
        //                        MainCustomer.FavoriteCustID = ReceiverID.ToString().Trim();
        //                    }
        //                    else if (MainCustomer.IsFavorite == 1 && MainCustomer.FavoriteCustID != "0")
        //                    {
        //                        MainCustomer.IsFavorite = 0;
        //                        MainCustomer.FavoriteCustID = "0";
        //                    }
        //                    dbContext.tblselectedDealerDetails.Attach(MainCustomer);
        //                    dbContext.Entry(MainCustomer).Property(sd => sd.IsFavorite).IsModified = true;
        //                    dbContext.Entry(MainCustomer).Property(sd => sd.FavoriteCustID).IsModified = true;
        //                    dbContext.SaveChanges();
        //                }
        //                else
        //                {
        //                    var Receiver = dbContext.tblselectedDealerDetails.Where(sdd => sdd.CustID == CustID && sdd.QueryId == QueryId && sdd.CreatedBy == ReceiverID).FirstOrDefault();

        //                    if (Receiver != null)
        //                    {
        //                        if (Receiver.IsFavorite == 1 && Receiver.FavoriteCustID == "All")
        //                        {
        //                            Receiver.IsFavorite = 0;
        //                            Receiver.FavoriteCustID = ReceiverID.ToString().Trim();
        //                        }
        //                        else if (Receiver.IsFavorite == 1 && Receiver.FavoriteCustID != "0")
        //                        {
        //                            Receiver.IsFavorite = 0;
        //                            Receiver.FavoriteCustID = "0";
        //                        }
        //                        dbContext.tblselectedDealerDetails.Attach(Receiver);
        //                        dbContext.Entry(Receiver).Property(sd => sd.IsFavorite).IsModified = true;
        //                        dbContext.Entry(Receiver).Property(sd => sd.FavoriteCustID).IsModified = true;
        //                        dbContext.SaveChanges();
        //                    }
        //                }
        //            }
        //            return GetCustomerQueries(CustID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}
        public List<CustomerQueries> DeleteConversation(int CustID, List<DeleteConversations> deleteConversations)
        {
            try
            {
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                using (dbContext = new mwbtDealerEntities())
                {
                    foreach (var item in deleteConversations)
                    {
                        #region set IsRead to 1 in enquiry table to avoid unread error
                        tblselectedDealerDetail sdDe = (from sd in dbContext.tblselectedDealerDetails where sd.CustID == item.CustID && sd.QueryId == item.QueryId && sd.CreatedBy == item.ReceiverID select sd).FirstOrDefault();
                        if (sdDe != null)
                        {
                            //If the sender is Enquirer
                            sdDe.CustID = item.CustID;
                            sdDe.IsRead = 1;

                            dbContext.tblselectedDealerDetails.Attach(sdDe);
                            dbContext.Entry(sdDe).Property(x => x.IsRead).IsModified = true;
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            //if the sender is the enquired person
                            tblselectedDealerDetail sdDeC = (from sd in dbContext.tblselectedDealerDetails where sd.CustID == item.ReceiverID && sd.QueryId == item.QueryId && sd.CreatedBy == item.CustID select sd).FirstOrDefault();
                            if (sdDeC != null)
                            {
                                sdDeC.SenderRead = 1;
                                sdDeC.SenderID = item.CustID;

                                dbContext.tblselectedDealerDetails.Attach(sdDeC);
                                dbContext.Entry(sdDeC).Property(x => x.SenderRead).IsModified = true;
                                dbContext.SaveChanges();
                            }
                        }
                        #endregion

                        //set isdeleted to 1 in conversation table
                        var Conversations = dbContext.tblUserConversations.Where(uc => uc.CustID == item.CustID && uc.QueryId == item.QueryId && uc.IsDealer == item.ReceiverID).ToList();
                        if (Conversations != null)
                        {
                            foreach (var ConvoList in Conversations)
                            {
                                ConvoList.IsDeleted = 1;
                                ConvoList.DeletedCustID = CustID.ToString().Trim();
                                dbContext.Entry(ConvoList).Property(c => c.IsDeleted).IsModified = true;
                                dbContext.Entry(ConvoList).Property(c => c.DeletedCustID).IsModified = true;
                                dbContext.SaveChanges();
                            }
                        }

                        //set IsRead to 1 in enquiry table to avoid unread error
                        //var enquiries = dbContext.tblselectedDealerDetails.Where(uc => uc.CustID == item.CustID && uc.QueryId == item.QueryId && uc.IsDealer == item.ReceiverID).ToList();
                        //if (Conversations != null)
                        //{
                        //    foreach (var ConvoList in Conversations)
                        //    {
                        //        ConvoList.IsDeleted = 1;
                        //        ConvoList.DeletedCustID = CustID.ToString().Trim();
                        //        dbContext.Entry(ConvoList).Property(c => c.IsDeleted).IsModified = true;
                        //        dbContext.Entry(ConvoList).Property(c => c.DeletedCustID).IsModified = true;
                        //        dbContext.SaveChanges();
                        //    }
                        //}

                        //add list in a different table
                        tblDeleteConversation tblDeleteConversation = new tblDeleteConversation();
                        tblDeleteConversation.CustID = CustID;
                        tblDeleteConversation.ReceiverID = item.ReceiverID;
                        tblDeleteConversation.QueryId = item.QueryId;
                        tblDeleteConversation.CreatedBy = CustID;
                        //tblDeleteConversation.CreatedDateTime = DateTimeNow;
                        tblDeleteConversation.CreatedDate = DateTimeNow;
                        dbContext.tblDeleteConversations.Add(tblDeleteConversation);
                    }
                    dbContext.SaveChanges();
                    return GetCustomerQueries(CustID, 0);
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public SendMessageParameters DeleteChat(int CustID, List<DeleteChat> deleteChats)
        {
            try
            {
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                using (dbContext = new mwbtDealerEntities())
                {
                    foreach (var item in deleteChats)
                    {
                        var Conversation = dbContext.tblUserConversations.Where(uc => uc.ID == item.ID).FirstOrDefault();
                        if (Conversation != null)
                        {

                            Conversation.IsArchived = 1;
                            Conversation.CreatedDate = DateTimeNow;
                            dbContext.Entry(Conversation).Property(c => c.IsArchived).IsModified = true;
                            dbContext.Entry(Conversation).Property(c => c.CreatedDate).IsModified = true;

                            //insert into deletechat table
                            tblDeleteChat tblDeleteChat = new tblDeleteChat();
                            tblDeleteChat.ChatID = Conversation.ID;
                            tblDeleteChat.CustID = CustID;
                            tblDeleteChat.ReceiverID = Conversation.IsDealer;
                            tblDeleteChat.QueryId = Conversation.QueryId;
                            tblDeleteChat.CreatedBy = CustID;
                            //tblDeleteChat.CreatedDateTime = DateTimeNow;
                            tblDeleteChat.CreatedDate = DateTimeNow;
                            dbContext.tblDeleteChats.Add(tblDeleteChat);

                            dbContext.SaveChanges();
                        }
                    }
                    return GetMessages(CustID, deleteChats.FirstOrDefault().QueryId, deleteChats.FirstOrDefault().ReceiverID);
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<CustomerQueries> AddOrDeleteFavorite(int CustID, int QueryId, int ReceiverID, int IsFavorite)
        {
            try
            {
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                using (dbContext = new mwbtDealerEntities())
                {
                    if (IsFavorite == 1)
                    {
                        tblFavoriteConversation tblFavoriteConversation = new tblFavoriteConversation();
                        tblFavoriteConversation.CustID = CustID;
                        tblFavoriteConversation.ReceiverID = ReceiverID;
                        tblFavoriteConversation.QueryId = QueryId;
                        tblFavoriteConversation.CreatedBy = CustID;
                        //tblFavoriteConversation.CreatedDateTime = DateTimeNow;
                        tblFavoriteConversation.CreatedDate = DateTimeNow;
                        dbContext.tblFavoriteConversations.Add(tblFavoriteConversation);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        tblFavoriteConversation tblFavoriteConversation = dbContext.tblFavoriteConversations.AsNoTracking().Where(f => f.CustID == CustID && f.QueryId == QueryId && f.ReceiverID == ReceiverID).FirstOrDefault();
                        if (tblFavoriteConversation != null)
                        {
                            //dbContext.tblFavoriteConversations.Remove(tblFavoriteConversation);
                            dbContext.Entry(tblFavoriteConversation).State = EntityState.Deleted;
                            dbContext.SaveChanges();
                        }
                    }
                    return GetCustomerQueries(CustID, 0);
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public bool SendImage(byte[] imageArray)
        {
            try
            {
                //tblselectedDealer tblselectedDealer = new tblselectedDealer();
                //tblselectedDealer = dbContext.tblselectedDealers.Where(s => s.ID == 1).FirstOrDefault();
                ////tblselectedDealer.Image = imageArray;

                //dbContext.Entry(tblselectedDealer).State = EntityState.Modified;
                //dbContext.tblselectedDealers.Add(tblselectedDealer);
                //dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return false;
            }
        }

        public string SendMails(SendMailParameters sendMailParameters, List<Attachment> MailAttachment)
        {
            string ToEmailID = ConfigurationManager.AppSettings["ToEmailID"].ToString();
            string FromMailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
            string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
            string MailServerHost = ConfigurationManager.AppSettings["MailServerHost"].ToString();
            string SendingPort = ConfigurationManager.AppSettings["SendingPort"].ToString();
            try
            {
                //Mail Body
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Hello, <br/>");
                stringBuilder.Append("New Mail from User - <br/>");
                stringBuilder.Append("Feedback / Request - " + sendMailParameters.MailBody + "<br/><br/>");
                stringBuilder.Append("Firm Name : " + sendMailParameters.FirmName + "<br/>");
                stringBuilder.Append("Mobile Number : " + sendMailParameters.MobileNumber + "<br/>");
                stringBuilder.Append("City : " + sendMailParameters.CityName + "<br/><br/>");
                sendMailParameters.MailBody = stringBuilder.ToString();

                Helper.SendMail(ToEmailID, string.Empty, FromMailID, sendMailParameters.MailBody, sendMailParameters.MailSubject, MailServerHost, SendingPort, MailPassword, MailAttachment);
                return "Mail Sent";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string SendMails(SendMailParameters sendMailParameters)
        {
            string ToEmailID = ConfigurationManager.AppSettings["ToEmailID"].ToString();
            string FromMailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
            string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
            string MailServerHost = ConfigurationManager.AppSettings["MailServerHost"].ToString();
            string SendingPort = ConfigurationManager.AppSettings["SendingPort"].ToString();

            //Mail Body
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Hello, <br/>");
            stringBuilder.Append("New Mail from User - <br/>");
            stringBuilder.Append("Feedback / Request - " + sendMailParameters.MailBody + "<br/><br/>");
            stringBuilder.Append("Firm Name : " + sendMailParameters.FirmName + "<br/>");
            stringBuilder.Append("Mobile Number : " + sendMailParameters.MobileNumber + "<br/>");
            stringBuilder.Append("City : " + sendMailParameters.CityName + "<br/><br/>");
            sendMailParameters.MailBody = stringBuilder.ToString();

            sendMailParameters.MailBody = stringBuilder.ToString();
            try
            {
                Helper.SendMail(ToEmailID, FromMailID, sendMailParameters.MailBody, sendMailParameters.MailSubject, MailServerHost, MailPassword, SendingPort);
                return "Mail Sent";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public CustDetails SendOTP(int? CustID, string MobileNumber)
        {
            try
            {
                //Get Customer Details
                //tblCustomerDetail customer = dbContext.tblCustomerDetails.AsNoTracking().Where(c => c.CustID == CustID).FirstOrDefault();
                tblCustomerDetail customer = dbContext.tblCustomerDetails.AsNoTracking().Where(c => c.ID == CustID).FirstOrDefault();

                if (customer.MobileNumber == MobileNumber)
                {
                    string SMSOTP = Helper.GenerateOTP();
                    string OTPHashCode = ConfigurationManager.AppSettings["OTPHashCode"].ToString();
                    //string smsMessage = SMSOTP + " is your OTP to login with JBN Application.Have a good day!";
                    //string smsMessage = "Welcome to MWB Technology New customer details name Your OTP is : " + SMSOTP + ".ID Test Ph Tets";
                    string smsMessage = "<#> Welcome to MWB Technology New customer details name Your OTP is : " + SMSOTP + "\n" + OTPHashCode.Trim();

                    //insert OTP in the database
                    tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();
                    //tblCustomerDetail.CustID = CustID.Value;
                    tblCustomerDetail.ID = CustID.Value;
                    tblCustomerDetail.SMSOTP = SMSOTP;
                    dbContext.tblCustomerDetails.Attach(tblCustomerDetail);
                    dbContext.Entry(tblCustomerDetail).Property(C => C.SMSOTP).IsModified = true;
                    dbContext.SaveChanges();

                    //string SMSUserName = ConfigurationManager.AppSettings["SMSUserName"];
                    //string SMSPassword = ConfigurationManager.AppSettings["SMSPassword"];
                    //string OTPStatus = Helper.SendSMS(SMSUserName, SMSPassword, MobileNumber, smsMessage, "N");
                    string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
                    string APIKEY = ConfigurationManager.AppSettings["SMSAPIKey"];
                    //string Message = "<#> Your URL link is " + SMSOTP + " for resetting " + AppName + " login password. link expire in " + Minutes + " Minutes MWB Tech India Pvt Ltd";
                    //string Message = "Your OTP is " + SMSOTP + " for "+ AppName + " login";

                    //string Message = "Hello Customer, ORDER VIDE PO NO:" + SMSOTP + " IS CONFIRMED, KINDLY SUPPLY ASAP. Thanks. MWB Technologies India Pvt. Ltd";
                    string message = "Your OTP is " + SMSOTP + " for Kalpavriksha login.- MWB Tech India Pvt Ltd";

                    var customers = dbContext.tblCustomerDetails.Where(c => c.IsOTPVerified == false).ToList();

                    string mobileNumbers = string.Empty;

                    //foreach (var item in customers)
                    //{
                    //    mobileNumbers += item.MobileNumber + ",";
                    //}

                    string OTPStatus = Helper.SendMessage(BaseURL, APIKEY, MobileNumber, message);

                    CustDetails customerDetails = new CustDetails();
                    //customerDetails.CustID = customer.CustID;
                    customerDetails.CustID = customer.ID;
                    customerDetails.IsRegistered = customer.IsRegistered;
                    customerDetails.UserType = customer.UserType;
                    customerDetails.Password = customer.Password;
                    customerDetails.SMSOTP = customer.SMSOTP;
                    customerDetails.IsOTPVerified = customer.IsOTPVerified ?? false;
                    customerDetails.OTPStatus = OTPStatus;
                    return customerDetails;
                }
                else
                {
                    CustDetails custDetails = new CustDetails();
                    custDetails.OTPStatus = "Mobile Number is incorrect!";
                    return custDetails;
                }
            }
            catch (Exception ex)
            {
                CustDetails custDetails = new CustDetails();
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                custDetails.OTPStatus = ex.Message;
                return custDetails;
            }
        }
        public CustDetails ApproveOTP(int? CustID, string MobileNumber, string OTP)
        {
            try
            {
                //tblCustomerDetail customer = dbContext.tblCustomerDetails.AsNoTracking().Where(c => c.CustID == CustID).FirstOrDefault();
                tblCustomerDetail customer = dbContext.tblCustomerDetails.AsNoTracking().Where(c => c.ID == CustID).FirstOrDefault();
                CustDetails customerDetails = new CustDetails();
                string OTPStatus = string.Empty;
                if (customer.SMSOTP == OTP)
                {
                    //insert OTP in the database
                    tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();
                    //tblCustomerDetail.CustID = CustID.Value;
                    tblCustomerDetail.ID = CustID.Value;
                    tblCustomerDetail.IsOTPVerified = true;
                    dbContext.tblCustomerDetails.Attach(tblCustomerDetail);
                    dbContext.Entry(tblCustomerDetail).Property(C => C.IsOTPVerified).IsModified = true;
                    dbContext.SaveChanges();
                    OTPStatus = "OTP Verified";
                    customerDetails.IsOTPVerified = true;
                }
                else
                {
                    OTPStatus = "Invalid OTP";
                    customerDetails.IsOTPVerified = false;
                }

                //customerDetails.CustID = customer.CustID;
                customerDetails.CustID = customer.ID;
                customerDetails.IsRegistered = customer.IsRegistered;
                customerDetails.UserType = customer.UserType;
                customerDetails.Password = customer.Password;
                customerDetails.SMSOTP = customer.SMSOTP;
                customerDetails.MobileNumber = customer.MobileNumber;
                //customerDetails.DeviceID = customer.DeviceID;
                customerDetails.QuestionAnswered = customer.QuestionAnswered;
                customerDetails.OTPStatus = OTPStatus;
                return customerDetails;
            }
            catch (Exception ex)
            {
                CustDetails custDetails = new CustDetails();
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                custDetails.OTPStatus = ex.Message;
                return custDetails;
            }
        }

        //Get Help Videos
        public List<tblVideo> GetVideos()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<tblVideo> Videos = new List<tblVideo>();
                        Videos = dbContext.tblVideos.ToList();

                        return Videos;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Get Districts
        public List<District> GetDistricts(List<tblState> StateList, string DistrictName)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<District> Districts = new List<District>();
                        Districts = (from get in dbContext.tblDistricts.ToList()
                                     join st in StateList.ToList() on get.StateID equals st.StateID
                                     where get.DistrictName.ToLower().Contains(DistrictName.ToLower())
                                     select new District
                                     {
                                         DistrictID = get.DistrictID,
                                         DistrictName = get.DistrictName,
                                     }).ToList();
                        return Districts;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Get Cities of District
        public List<City> GetCitiesOfDistrict(List<tblDistrict> Districts, string CityName)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<City> Cities = new List<City>();
                        Cities = (from st in dbContext.CityViews.ToList()
                                  join d in Districts.ToList() on st.DistrictID equals d.DistrictID
                                  where st.VillageLocalityName.ToLower().Contains(CityName.ToLower())
                                  select new City
                                  {
                                      //StateWithCityID = st.StateWithCityID,
                                      StateWithCityID = st.ID,
                                      VillageLocalityName = st.VillageLocalityName,
                                  }).ToList();

                        return Cities;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Get Cities of Multiple States
        public List<City> GetStateWiseCities(List<tblState> StateList, string CityName)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<City> Cities = new List<City>();
                        Cities = (from st in dbContext.CityViews.ToList()
                                  join d in StateList.ToList() on st.StateID equals d.StateID
                                  where st.VillageLocalityName.ToLower().Contains(CityName.ToLower())
                                  select new City
                                  {
                                      //StateWithCityID = st.StateWithCityID,
                                      StateWithCityID = st.ID,
                                      VillageLocalityName = st.VillageLocalityName,
                                  }).ToList();
                        return Cities;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public CustDetails GetCustomerOfDeviceID(string DeviceID)
        {
            try
            {
                tblCustomerDetail customer = dbContext.tblCustomerDetails.AsNoTracking().Where(c => c.DeviceID == DeviceID).FirstOrDefault();
                if (customer == null)
                {
                    return null;
                }

                CustDetails customerDetails = new CustDetails();
                if (customer != null)
                {
                    //customerDetails.CustID = customer.CustID;
                    customerDetails.CustID = customer.ID;
                    //customerDetails.DeviceID = customer.DeviceID;
                    customerDetails.QuestionAnswered = customer.QuestionAnswered;
                }
                return customerDetails;
            }
            catch (Exception ex)
            {
                CustDetails custDetails = new CustDetails();
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                custDetails.OTPStatus = ex.Message;
                return custDetails;
            }
        }

        //Not using from 31082020
        public CustDetails SendOTPToMobileNumber(string DeviceID, string MobileNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(MobileNumber))
                {
                    var IsValueExists = dbContext.tblCustomerDetails.Where(c => c.MobileNumber == MobileNumber).AsNoTracking().FirstOrDefault();
                    if (IsValueExists != null)
                    {
                        CustDetails custDetails = new CustDetails();
                        custDetails.OTPStatus = "Mobile Number is already exists";
                        return custDetails;
                    }
                    string SMSOTP = Helper.GenerateOTP();
                    //string smsMessage = SMSOTP + " is your OTP to login with JBN Application.Have a good day!";
                    string smsMessage = "Welcome to MWB Technology New customer details name Your OTP is : " + SMSOTP + ".ID Test Ph Tets";

                    //insert OTP in the database
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    tblCustomerDetail CustomerDetails = new tblCustomerDetail();
                    //CustomerDetails.CreatedByID = 1;
                    CustomerDetails.CreatedBy = 1;
                    CustomerDetails.CreatedDate = DateTimeNow;
                    CustomerDetails.IsRegistered = 0;
                    CustomerDetails.SMSOTP = SMSOTP;
                    CustomerDetails.QuestionAnswered = false;
                    CustomerDetails.IsOTPVerified = false;
                    CustomerDetails.MobileNumber = MobileNumber;
                    CustomerDetails.DeviceID = DeviceID;
                    dbContext.tblCustomerDetails.Add(CustomerDetails);
                    dbContext.SaveChanges();

                    string SMSUserName = ConfigurationManager.AppSettings["SMSUserName"];
                    string SMSPassword = ConfigurationManager.AppSettings["SMSPassword"];

                    string OTPStatus = Helper.SendSMS(SMSUserName, SMSPassword, MobileNumber, smsMessage, "N");

                    CustDetails customerDetails = new CustDetails();
                    //customerDetails.CustID = CustomerDetails.CustID;
                    customerDetails.CustID = CustomerDetails.ID;
                    customerDetails.IsRegistered = CustomerDetails.IsRegistered;
                    customerDetails.SMSOTP = CustomerDetails.SMSOTP;
                    //customerDetails.DeviceID = DeviceID;
                    customerDetails.MobileNumber = MobileNumber;
                    customerDetails.IsOTPVerified = CustomerDetails.IsOTPVerified ?? false;
                    customerDetails.QuestionAnswered = CustomerDetails.QuestionAnswered ?? false;
                    customerDetails.OTPStatus = OTPStatus;
                    return customerDetails;
                }
                else
                {
                    CustDetails custDetails = new CustDetails();
                    custDetails.OTPStatus = "Mobile Number is incorrect!";
                    return custDetails;
                }
            }
            catch (Exception ex)
            {
                CustDetails custDetails = new CustDetails();
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                custDetails.OTPStatus = ex.Message;
                return custDetails;
            }
        }

        public CustDetails ResendSendOTPToMobileNumber(string DeviceID, string MobileNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(MobileNumber))
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    CustDetails customerDetails = new CustDetails();
                    var IsValueExists = dbContext.tblCustomerDetails.Where(c => c.MobileNumber == MobileNumber).AsNoTracking().FirstOrDefault();
                    if (IsValueExists != null)
                    {
                        string SMSOTP = Helper.GenerateOTP();
                        //string smsMessage = SMSOTP + " is your OTP to login with JBN Application.Have a good day!";
                        string smsMessage = "Welcome to MWB Technology New customer details name Your OTP is : " + SMSOTP + ".ID Test Ph Tets";

                        //update OTP in the database
                        tblCustomerDetail CustomerDetails = new tblCustomerDetail();
                        //CustomerDetails.CustID = IsValueExists.CustID;
                        CustomerDetails.ID = IsValueExists.ID;
                        CustomerDetails.SMSOTP = SMSOTP;
                        dbContext.tblCustomerDetails.Attach(CustomerDetails);
                        dbContext.Entry(CustomerDetails).Property(C => C.SMSOTP).IsModified = true;
                        dbContext.SaveChanges();

                        string SMSUserName = ConfigurationManager.AppSettings["SMSUserName"];
                        string SMSPassword = ConfigurationManager.AppSettings["SMSPassword"];

                        string OTPStatus = Helper.SendSMS(SMSUserName, SMSPassword, MobileNumber, smsMessage, "N");


                        //customerDetails.CustID = CustomerDetails.CustID;
                        customerDetails.CustID = CustomerDetails.ID;
                        customerDetails.IsRegistered = CustomerDetails.IsRegistered;
                        customerDetails.SMSOTP = CustomerDetails.SMSOTP;
                        //customerDetails.DeviceID = DeviceID;
                        customerDetails.MobileNumber = MobileNumber;
                        customerDetails.IsOTPVerified = CustomerDetails.IsOTPVerified ?? false;
                        customerDetails.QuestionAnswered = CustomerDetails.QuestionAnswered ?? false;
                        customerDetails.OTPStatus = OTPStatus;
                    }
                    return customerDetails;
                }
                else
                {
                    CustDetails custDetails = new CustDetails();
                    custDetails.OTPStatus = "Mobile Number is incorrect!";
                    return custDetails;
                }
            }
            catch (Exception ex)
            {
                CustDetails custDetails = new CustDetails();
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                custDetails.OTPStatus = ex.Message;
                return custDetails;
            }
        }

        public tblQuestion GetQuestion()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        tblQuestion Question = new tblQuestion();
                        Question = dbContext.tblQuestions.OrderBy(r => Guid.NewGuid()).Take(1).First();

                        return Question;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public CustDetails AnswerQuestion(string DeviceID, string MobileNumber, int QuestionID, string AnswerForQuestion)
        {
            try
            {
                CustDetails customerDetails = new CustDetails();
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                var IsValueExists = dbContext.tblCustomerDetails.AsNoTracking().Where(c => c.MobileNumber == MobileNumber).FirstOrDefault();
                tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();
                if (IsValueExists != null)
                {
                    tblQuestion tblQuestion = dbContext.tblQuestions.Where(q => q.QuestionID == QuestionID).FirstOrDefault();

                    if (tblQuestion.CorrectAnswer.ToLower() == AnswerForQuestion.ToLower())
                    {
                        customerDetails.QuestionAnswered = true;
                        //insert Question Details in the database
                        //tblCustomerDetail.CustID = IsValueExists.CustID;
                        tblCustomerDetail.ID = IsValueExists.ID;
                        tblCustomerDetail.QuestionAnswered = true;
                        tblCustomerDetail.MobileNumber = MobileNumber;
                        tblCustomerDetail.Question = tblQuestion.QuestionName;
                        tblCustomerDetail.AnswerForQuestion = AnswerForQuestion;
                        tblCustomerDetail.DeviceID = DeviceID;
                        dbContext.tblCustomerDetails.Attach(tblCustomerDetail);
                        dbContext.Entry(tblCustomerDetail).Property(C => C.Question).IsModified = true;
                        dbContext.Entry(tblCustomerDetail).Property(C => C.QuestionAnswered).IsModified = true;
                        dbContext.Entry(tblCustomerDetail).Property(C => C.AnswerForQuestion).IsModified = true;
                        dbContext.Entry(tblCustomerDetail).Property(C => C.DeviceID).IsModified = true;
                        dbContext.SaveChanges();

                        //customerDetails.CustID = tblCustomerDetail.CustID;
                        customerDetails.CustID = tblCustomerDetail.ID;
                        customerDetails.MobileNumber = tblCustomerDetail.MobileNumber;
                        customerDetails.Question = tblQuestion.QuestionName;
                        customerDetails.AnswerForQuestion = AnswerForQuestion;

                    }
                    else
                    {
                        customerDetails.QuestionAnswered = false;
                    }
                }
                return customerDetails;
                //else
                //{
                //    if (tblQuestion.CorrectAnswer.ToLower() == AnswerForQuestion.ToLower())
                //    {
                //        customerDetails.QuestionAnswered = true;
                //        //insert Question Details in the database

                //        tblCustomerDetail.CreatedByID = 1;
                //        tblCustomerDetail.CreatedDate = DateTimeNow;
                //        tblCustomerDetail.IsRegistered = 0;
                //        tblCustomerDetail.QuestionAnswered = true;
                //        tblCustomerDetail.IsOTPVerified = false;
                //        tblCustomerDetail.MobileNumber = MobileNumber;
                //        tblCustomerDetail.Question = tblQuestion.QuestionName;
                //        tblCustomerDetail.AnswerForQuestion = AnswerForQuestion;
                //        tblCustomerDetail.UserType = "1";
                //        tblCustomerDetail.IsActive = false;
                //        tblCustomerDetail.DeviceID = DeviceID;
                //        dbContext.tblCustomerDetails.Add(tblCustomerDetail);
                //        dbContext.SaveChanges();
                //    }
                //    else
                //    {
                //        customerDetails.QuestionAnswered = false;
                //    }
                //}


            }
            catch (Exception ex)
            {
                CustDetails custDetails = new CustDetails();
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                custDetails.OTPStatus = ex.Message;
                return custDetails;
            }
        }

        public List<tblPromo> GetPromoImages()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<tblPromo> PromoImages = new List<tblPromo>();
                        PromoImages = dbContext.tblPromoes.ToList();

                        return PromoImages;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<Advertisement> GetPromotions(int PromoType)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<Advertisement> PromoImages = new List<Advertisement>();
                        if (PromoType == 1)
                        {
                            PromoImages = (from a in dbContext.tblPromoes
                                           where a.PromoType == "image"
                                           select new Advertisement
                                           {
                                               AdImageURL = a.ImageURL,
                                               AdvertisementType = a.PromoType,
                                               AdText = a.PromoText,
                                               IsCompanyAd = true,
                                           }).ToList();
                        }
                        else
                        {
                            PromoImages = (from a in dbContext.tblPromoes
                                           where a.PromoType == "text"
                                           select new Advertisement
                                           {
                                               AdImageURL = a.ImageURL,
                                               AdvertisementType = a.PromoType,
                                               AdText = a.PromoText,
                                               IsCompanyAd = true,
                                           }).ToList();
                        }
                        return PromoImages;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Feature added 24th Aug 2020
        public List<Enquiries> GetEnquiries(SentEnquirySearchParameters sentEnquirySearchParameters)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<Enquiries> QueryList = new List<Enquiries>();
                        //admin configured Date
                        DateTime EnquiryConfiguredDate = DateTime.Now.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch)).Date;

                        if (sentEnquirySearchParameters.ReceiverID != 0)
                        {
                            QueryList = (from sdd in dbContext.tblselectedDealerDetails
                                         join sd in dbContext.tblselectedDealers on sdd.QueryId equals sd.ID
                                         //join cd in dbContext.tblCustomerDetails on sdd.CustID equals cd.CustID
                                         join cd in dbContext.tblCustomerDetails on sdd.CustID equals cd.ID
                                         join sc in dbContext.tblStateWithCities on sd.CityId equals sc.StatewithCityID
                                         //join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ItemId
                                         join cc in dbContext.tblItemCategories on sd.ProductID equals cc.ID
                                         where sdd.CreatedBy == sentEnquirySearchParameters.CustID && sdd.CustID != sentEnquirySearchParameters.CustID
                                         && !(from de in dbContext.tblDeleteEnquiries select de.QueryId).Contains(sdd.QueryId)
                                         && sdd.CustID == sentEnquirySearchParameters.ReceiverID
                                         && cd.IsActive == true && DbFunctions.TruncateTime(sd.CreatedDate) >= EnquiryConfiguredDate
                                         select new Enquiries
                                         {
                                             QueryId = sdd.QueryId,
                                             //BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.BusinessDemandID == sd.BusinessDemandID).FirstOrDefault().BusinessDemand),
                                             //RequirementName = (dbContext.tblProfessionalRequirements.Where(b => b.ProfessionalRequirementID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),

                                             BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.ID == sd.BusinessDemandID).FirstOrDefault().Demand),
                                             RequirementName = (dbContext.tblProfessionalRequirements.Where(b => b.ID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),

                                             ChildCategoryName = cc.ItemName,
                                             CityId = sc.StatewithCityID,
                                             //LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                             PurposeOfBusiness = sd.PurposeBusiness,
                                             VillageLocalityname = sc.VillageLocalityName,
                                             ReplyCount = (dbContext.tblselectedDealerDetails.Where(c => c.QueryId == sdd.QueryId && c.CreatedBy == sentEnquirySearchParameters.CustID && c.SenderRead == 0 && c.CustID != sentEnquirySearchParameters.CustID).Distinct().Count()),
                                             CreatedDate = sdd.CreatedDate,
                                             EnquiryDate = sd.CreatedDate,
                                             EnquiryType = sd.EnquiryType,
                                         }).Distinct().ToList();

                            QueryList = (from q in QueryList
                                         join sd in dbContext.tblselectedDealers
                                         on q.QueryId equals sd.ID
                                         select new Enquiries
                                         {
                                             QueryId = q.QueryId,
                                             BusinessDemand = q.BusinessDemand,
                                             ChildCategoryName = q.ChildCategoryName,
                                             CityId = q.CityId,
                                             LastUpdatedMsgDate = sd.LastUpdatedMsgDate,
                                             PurposeOfBusiness = q.PurposeOfBusiness,
                                             VillageLocalityname = q.VillageLocalityname,
                                             ReplyCount = q.ReplyCount,
                                             CreatedDate = q.CreatedDate,
                                             RequirementName = q.RequirementName,
                                             EnquiryDate = sd.CreatedDate,
                                         }).ToList();
                        }
                        else
                        {
                            QueryList = (from sdd in dbContext.tblselectedDealerDetails
                                         join sd in dbContext.tblselectedDealers on sdd.QueryId equals sd.ID
                                         //join cd in dbContext.tblCustomerDetails on sdd.CustID equals cd.CustID
                                         join cd in dbContext.tblCustomerDetails on sdd.CustID equals cd.ID
                                         join sc in dbContext.tblStateWithCities on sd.CityId equals sc.StatewithCityID
                                         //join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ItemId
                                         join cc in dbContext.tblItemCategories on sd.ProductID equals cc.ID
                                         where sdd.CreatedBy == sentEnquirySearchParameters.CustID && sdd.CustID != sentEnquirySearchParameters.CustID
                                         && !(from de in dbContext.tblDeleteEnquiries select de.QueryId).Contains(sdd.QueryId)
                                         && cd.IsActive == true && DbFunctions.TruncateTime(sd.CreatedDate) >= EnquiryConfiguredDate
                                         select new Enquiries
                                         {
                                             QueryId = sdd.QueryId,
                                             //BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.BusinessDemandID == sd.BusinessDemandID).FirstOrDefault().BusinessDemand),
                                             //RequirementName = (dbContext.tblProfessionalRequirements.Where(b => b.ProfessionalRequirementID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),
                                             BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.ID == sd.BusinessDemandID).FirstOrDefault().Demand),
                                             RequirementName = (dbContext.tblProfessionalRequirements.Where(b => b.ID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),

                                             ChildCategoryName = cc.ItemName,
                                             CityId = sc.StatewithCityID,
                                             //LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                             PurposeOfBusiness = sd.PurposeBusiness,
                                             VillageLocalityname = sc.VillageLocalityName,
                                             ReplyCount = (dbContext.tblselectedDealerDetails.Where(c => c.QueryId == sdd.QueryId && c.CreatedBy == sentEnquirySearchParameters.CustID && c.SenderRead == 0 && c.CustID != sentEnquirySearchParameters.CustID).Distinct().Count()),
                                             CreatedDate = sdd.CreatedDate,
                                             EnquiryDate = sd.CreatedDate,
                                             EnquiryType = sd.EnquiryType,
                                         }).Distinct().ToList();

                            QueryList = (from q in QueryList
                                         join sd in dbContext.tblselectedDealers
                                         on q.QueryId equals sd.ID
                                         select new Enquiries
                                         {
                                             QueryId = q.QueryId,
                                             BusinessDemand = q.BusinessDemand,
                                             ChildCategoryName = q.ChildCategoryName,
                                             CityId = q.CityId,
                                             LastUpdatedMsgDate = sd.LastUpdatedMsgDate,
                                             PurposeOfBusiness = q.PurposeOfBusiness,
                                             VillageLocalityname = q.VillageLocalityname,
                                             ReplyCount = q.ReplyCount,
                                             CreatedDate = q.CreatedDate,
                                             RequirementName = q.RequirementName,
                                             EnquiryDate = sd.CreatedDate,
                                             EnquiryType = q.EnquiryType,
                                         }).ToList();
                        }

                        if (!string.IsNullOrEmpty(sentEnquirySearchParameters.EnquiryType))
                            QueryList = QueryList.Where(u => u.EnquiryType.ToLower() == sentEnquirySearchParameters.EnquiryType.ToLower()).Distinct().ToList();

                        if (!string.IsNullOrEmpty(sentEnquirySearchParameters.FromDate) && !string.IsNullOrEmpty(sentEnquirySearchParameters.ToDate))
                        {
                            DateTime SToDate = Convert.ToDateTime(sentEnquirySearchParameters.ToDate);
                            DateTime FromDateFromApp = Convert.ToDateTime(sentEnquirySearchParameters.FromDate);
                            DateTime SFromDate = SToDate.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch));

                            if (FromDateFromApp <= SFromDate)
                            {
                                QueryList = QueryList.Where(q => q.LastUpdatedMsgDate.Value.Date >= FromDateFromApp.Date && q.LastUpdatedMsgDate.Value.Date <= SToDate.Date).ToList();
                            }
                            else
                            {
                                QueryList = QueryList.Where(q => q.LastUpdatedMsgDate.Value.Date >= SFromDate.Date && q.LastUpdatedMsgDate.Value.Date <= SToDate.Date).ToList();
                            }
                        }

                        if (sentEnquirySearchParameters.CityIdList != null && sentEnquirySearchParameters.CityIdList.Count() > 0)
                        {
                            QueryList = QueryList.Where(u => sentEnquirySearchParameters.CityIdList.Exists(b => b.StateWithCityID == u.CityId)).Distinct().ToList();
                            //QueryList = QueryList.Where(q => q.CityId == sentEnquirySearchParameters.CityId).ToList();
                        }

                        QueryList = QueryList.Distinct().OrderByDescending(m => m.LastUpdatedMsgDate).ToList();
                        return QueryList;
                    }
                }
            }
            catch (Exception ex)
            {
                //string strMessage= ex.Message;

                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }

        public List<CustomerQueries> GetConversationsOfEnquiry(int QueryID, int CustID, int ReceiverID, int IsFavorite)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<CustomerQueries> QueryList = new List<CustomerQueries>();

                        //admin configured date filter
                        DateTime SFromDate = DateTime.Now.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch)).Date;

                        QueryList = (from sdd in dbContext.tblselectedDealerDetails
                                     join sd in dbContext.tblselectedDealers on sdd.QueryId equals sd.ID
                                     join uc in dbContext.tblUserConversations on sdd.QueryId equals uc.QueryId
                                     //join cd in dbContext.tblCustomerDetails on sdd.CustID equals cd.CustID
                                     join cd in dbContext.tblCustomerDetails on sdd.CustID equals cd.ID
                                     join sc in dbContext.tblStateWithCities on sd.CityId equals sc.StatewithCityID
                                     //join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ItemId
                                     join cc in dbContext.tblItemCategories on sd.ProductID equals cc.ID
                                     where sdd.QueryId == QueryID && sdd.CreatedBy == CustID
                                     && (uc.CustID == sdd.CustID && uc.IsDealer == CustID && uc.QueryId == QueryID)
                                     && cd.IsActive == true
                                     && (DbFunctions.TruncateTime(sdd.CreatedDate) >= SFromDate)
                                     select new CustomerQueries
                                     {
                                         QueryId = sdd.QueryId,
                                         //CustID = cd.CustID,
                                         CustID = cd.ID,
                                         FirmName = cd.FirmName.Trim(),
                                         EmailID = cd.EmailID.Trim(),
                                         MobileNumber = cd.MobileNumber,
                                         //BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.BusinessDemandID == sd.BusinessDemandID).FirstOrDefault().BusinessDemand),
                                         //RequirementName = (dbContext.tblProfessionalRequirements.Where(b => b.ProfessionalRequirementID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),
                                         BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.ID == sd.BusinessDemandID).FirstOrDefault().Demand),
                                         RequirementName = (dbContext.tblProfessionalRequirements.Where(b => b.ID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),
                                         BusinessDemandID = sd.BusinessDemandID,
                                         PurposeOfBusiness = sd.PurposeBusiness.Trim(),
                                         VillageLocalityname = sc.VillageLocalityName.Trim(),
                                         Requirements = sd.OpenText.Trim(),
                                         LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                         IsRead = sdd.IsRead.Value,
                                         SenderRead = sdd.SenderRead.Value,
                                         SenderID = sdd.SenderID.Value,
                                         ChildCategoryName = cc.ItemName.Trim(),
                                         IsFavorite = (dbContext.tblFavoriteConversations.Where(f => f.CustID == CustID && f.ReceiverID == sdd.CustID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                         FavoriteCustID = sdd.FavoriteCustID,
                                         IsDeleted = (dbContext.tblDeleteConversations.Where(f => f.CustID == CustID && f.ReceiverID == sdd.CustID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                         CityId = sdd.CityId,
                                         CreatedBy = sdd.CreatedBy,
                                         EnquiryDate = sd.CreatedDate,
                                         EnquiryType = sd.EnquiryType,
                                         SenderImage = cd.UserImage,
                                     }).Distinct().ToList();

                        QueryList = QueryList.Where(u => u.CustID != CustID).Distinct().ToList();

                        if (ReceiverID != 0)
                        {
                            QueryList = QueryList.Where(u => u.CustID == ReceiverID).Distinct().ToList();
                        }

                        //remove deleted
                        QueryList = QueryList.Where(u => u.IsDeleted != 1).Distinct().ToList();

                        //Set IsRead
                        foreach (var item in QueryList)
                        {
                            if (item.CreatedBy == CustID)
                                item.IsRead = item.SenderRead;
                            else
                                item.IsRead = item.IsRead;
                        }

                        if (IsFavorite == 1)
                        {
                            QueryList = QueryList.Where(q => q.IsFavorite == 1).ToList();
                        }

                        QueryList = QueryList.Distinct().OrderByDescending(m => m.LastUpdatedMsgDate).ThenByDescending(m => m.IsFavorite).ToList();
                        return QueryList;
                    }
                }
            }
            catch (Exception ex)
            {
                //string strMessage= ex.Message;

                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }

        public List<CustomerListForSearch> GetCustomerNamesForFilter(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<CustomerListForSearch> CustomerList = new List<CustomerListForSearch>();

                        CustomerList = (from sdd in dbContext.tblselectedDealerDetails
                                        join uc in dbContext.tblUserConversations on sdd.QueryId equals uc.QueryId
                                        join cd in dbContext.tblCustomerDetails on uc.CustID equals cd.ID
                                        //join cd in dbContext.tblCustomerDetails on uc.CustID equals cd.ID
                                        where sdd.CreatedBy == CustID && sdd.CustID != CustID && uc.CustID != CustID
                                        && cd.IsActive == true
                                        select new CustomerListForSearch
                                        {
                                            SenderID = CustID,
                                            CustID = cd.ID,
                                            FirmName = cd.FirmName,
                                            QueryID = sdd.QueryId.Value
                                        }).Distinct().ToList();

                        //get deleted list
                        var deletedList = dbContext.tblDeleteConversations.Where(d => d.CustID == CustID).ToList();
                        var deletedConversations = dbContext.tblDeleteEnquiries.Where(d => d.CustID == CustID).ToList();

                        if (deletedList != null)
                        {
                            CustomerList.RemoveAll(l => deletedList.Exists(d => d.QueryId == l.QueryID && d.CustID == CustID && d.ReceiverID == l.CustID));
                        }

                        if (deletedConversations != null)
                        {
                            CustomerList.RemoveAll(l => deletedConversations.Exists(d => d.QueryId == l.QueryID && (d.CustID == CustID)));
                        }

                        CustomerList = CustomerList.GroupBy(x => new
                        {
                            x.CustID,
                            x.FirmName
                        }).Select(x => new CustomerListForSearch
                        {
                            CustID = x.Key.CustID,
                            FirmName = x.Key.FirmName
                        }).OrderBy(xc => xc.FirmName).ToList();

                        List<CustomerListForSearch> DistinctCustomerList = CustomerList.Distinct().ToList();
                        return DistinctCustomerList;
                    }
                }
            }
            catch (Exception ex)
            {
                //string strMessage= ex.Message;

                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }

        public List<CustomerQueries> FilterCustomerSentEnquiries(SentEnquirySearchParameters sentEnquirySearchParameters)
        {
            try
            {
                List<CustomerQueries> customerQueries = GetConversationsOfEnquiry(sentEnquirySearchParameters.QueryID, sentEnquirySearchParameters.CustID, sentEnquirySearchParameters.ReceiverID, sentEnquirySearchParameters.IsFavorite);

                using (var dbContext = new mwbtDealerEntities())
                {
                    List<CustomerQueries> FilteredList = new List<CustomerQueries>();
                    bool flag = false;

                    if (sentEnquirySearchParameters.CityIdList != null && sentEnquirySearchParameters.CityIdList.Count() > 0)
                        customerQueries = customerQueries.Where(u => sentEnquirySearchParameters.CityIdList.Exists(b => b.StateWithCityID == u.CityId)).Distinct().ToList();
                    //customerQueries = customerQueries.Where(u => u.CityId == sentEnquirySearchParameters.CityId).Distinct().ToList();

                    if (sentEnquirySearchParameters.BusinessDemand != null)
                        //customerQueries = customerQueries.Where(u => sentEnquirySearchParameters.BusinessDemand.Exists(b => b.BusinessDemandID == u.BusinessDemandID)).Distinct().ToList();
                        customerQueries = customerQueries.Where(u => sentEnquirySearchParameters.BusinessDemand.Exists(b => b.ID == u.BusinessDemandID)).Distinct().ToList();

                    if (sentEnquirySearchParameters.BusinessTypeIds.Count() > 0)
                    {
                        customerQueries = (from sdd in customerQueries
                                           join btc in dbContext.tblBusinessTypewithCusts on sdd.CustID equals btc.CustID
                                           select new CustomerQueries
                                           {
                                               QueryId = sdd.QueryId,
                                               CustID = sdd.CustID,
                                               FirmName = sdd.FirmName,
                                               EmailID = sdd.EmailID,
                                               MobileNumber = sdd.MobileNumber,
                                               BusinessDemand = sdd.BusinessDemand,
                                               PurposeOfBusiness = sdd.PurposeOfBusiness,
                                               VillageLocalityname = sdd.VillageLocalityname,
                                               Requirements = sdd.Requirements,
                                               LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                               IsRead = sdd.IsRead,
                                               SenderRead = sdd.SenderRead,
                                               SenderID = sdd.SenderID,
                                               ChildCategoryName = sdd.ChildCategoryName,
                                               IsFavorite = sdd.IsFavorite,
                                               FavoriteCustID = sdd.FavoriteCustID,
                                               IsDeleted = sdd.IsDeleted,
                                               CityId = sdd.CityId,
                                               CreatedBy = sdd.CreatedBy,
                                               // BusinessTID = btc.BusinessTypeID,
                                               BusinessTID = btc.BusinessTypeID.ToString(),
                                               EnquiryType = sdd.EnquiryType,
                                               SenderImage = sdd.SenderImage,
                                           }).ToList();

                        foreach (var item in sentEnquirySearchParameters.BusinessTypeIds)
                        {

                            var CustBusinessTList = customerQueries.Where(d => d.BusinessTID == item.BusinessTypeID.ToString()).ToList();
                            if (CustBusinessTList != null)
                            {
                                var isExists = (from getOld in FilteredList
                                                join newDAta in customerQueries on getOld.CustID equals newDAta.CustID
                                                where getOld.CustID == newDAta.CustID && getOld.QueryId == newDAta.QueryId
                                                select getOld);

                                if (isExists == null || isExists.Count() == 0)
                                {
                                    flag = true;
                                    FilteredList.AddRange(CustBusinessTList);
                                }
                            }
                        }
                        FilteredList = (from sdd in FilteredList
                                        select new CustomerQueries
                                        {
                                            QueryId = sdd.QueryId,
                                            CustID = sdd.CustID,
                                            FirmName = sdd.FirmName,
                                            EmailID = sdd.EmailID,
                                            MobileNumber = sdd.MobileNumber,
                                            BusinessDemand = sdd.BusinessDemand,
                                            PurposeOfBusiness = sdd.PurposeOfBusiness,
                                            VillageLocalityname = sdd.VillageLocalityname,
                                            Requirements = sdd.Requirements,
                                            LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                            IsRead = sdd.IsRead,
                                            SenderID = sdd.SenderID,
                                            ChildCategoryName = sdd.ChildCategoryName,
                                            IsFavorite = sdd.IsFavorite,
                                            FavoriteCustID = sdd.FavoriteCustID,
                                            IsDeleted = sdd.IsDeleted,
                                            EnquiryType = sdd.EnquiryType,
                                            SenderImage = sdd.SenderImage,
                                        }).Distinct().ToList();
                    }

                    if (flag == false)
                        FilteredList = customerQueries;

                    if (!string.IsNullOrEmpty(sentEnquirySearchParameters.EnquiryType))
                        FilteredList = FilteredList.Where(u => u.EnquiryType.ToLower() == sentEnquirySearchParameters.EnquiryType.ToLower()).Distinct().ToList();

                    //Date Filter
                    if (!string.IsNullOrEmpty(sentEnquirySearchParameters.FromDate) && !string.IsNullOrEmpty(sentEnquirySearchParameters.ToDate))
                    {
                        DateTime FromDate = Convert.ToDateTime(sentEnquirySearchParameters.FromDate);
                        DateTime ToDate = Convert.ToDateTime(sentEnquirySearchParameters.ToDate);
                        DateTime FromDateFromApp = ToDate.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch));

                        int daysFromDate = (ToDate.Date - FromDate.Date).Days;
                        int daysSFromDate = (ToDate.Date - FromDateFromApp.Date).Days;

                        if (daysFromDate <= daysSFromDate)
                        {
                            FilteredList = FilteredList.Where(f => f.LastUpdatedMsgDate.Value.Date >= FromDateFromApp && f.LastUpdatedMsgDate.Value.Date <= ToDate).ToList();
                        }
                        else
                        {
                            FilteredList = FilteredList.Where(f => f.LastUpdatedMsgDate.Value.Date >= FromDate && f.LastUpdatedMsgDate.Value.Date <= ToDate).ToList();
                        }
                    }

                    FilteredList = FilteredList.Distinct().OrderByDescending(m => m.LastUpdatedMsgDate).ToList();
                    return FilteredList;
                }
            }
            catch (Exception ex)
            {
                //string strMessage= ex.Message;

                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }

        public List<Enquiries> DeleteEnquiry(int CustID, List<DeleteEnquiry> deleteConversations)
        {
            try
            {
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                using (dbContext = new mwbtDealerEntities())
                {
                    foreach (var item in deleteConversations)
                    {
                        //set isdeleted to 1 in conversation table
                        var Conversations = dbContext.tblUserConversations.Where(uc => uc.CustID == item.CustID && uc.QueryId == item.QueryId).ToList();
                        if (Conversations != null)
                        {
                            foreach (var ConvoList in Conversations)
                            {
                                ConvoList.IsDeleted = 1;
                                ConvoList.DeletedCustID = CustID.ToString().Trim();
                                dbContext.Entry(ConvoList).Property(c => c.IsDeleted).IsModified = true;
                                dbContext.Entry(ConvoList).Property(c => c.DeletedCustID).IsModified = true;
                                dbContext.SaveChanges();
                            }
                        }

                        //add list in a different table
                        tblDeleteEnquiry tblDeleteEnquiry = new tblDeleteEnquiry();
                        tblDeleteEnquiry.CustID = CustID;
                        tblDeleteEnquiry.QueryId = item.QueryId;
                        tblDeleteEnquiry.CreatedBy = CustID;
                        //tblDeleteEnquiry.CreatedDateTime = DateTimeNow;
                        tblDeleteEnquiry.CreatedDate = DateTimeNow;
                        dbContext.tblDeleteEnquiries.Add(tblDeleteEnquiry);
                    }
                    dbContext.SaveChanges();
                    SentEnquirySearchParameters sentEnquirySearchParameters = new SentEnquirySearchParameters();
                    sentEnquirySearchParameters.CustID = CustID;
                    return GetEnquiries(sentEnquirySearchParameters);
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<FavoriteConversations> GetFavoriteConversations(SearchParameters searchParameters)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<FavoriteConversations> FilteredList = new List<FavoriteConversations>();
                        bool flag = false;
                        List<FavoriteConversations> QueryList = new List<FavoriteConversations>();
                        IQueryable<tblselectedDealer> sentEnquiries = null;
                        IQueryable<tblselectedDealerDetail> receivedEnquiries = null;
                        //admin configured Date
                        DateTime EnquiriesFromDate = DateTime.Now.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch)).Date;

                        if (!string.IsNullOrEmpty(searchParameters.FromDate) && !string.IsNullOrEmpty(searchParameters.FromDate))
                        {
                            DateTime FromDate = Convert.ToDateTime(searchParameters.FromDate);
                            DateTime ToDate = Convert.ToDateTime(searchParameters.ToDate);
                            DateTime AdminConfigDate = ToDate.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch));

                            int diffDaysFromToDate = (ToDate.Date - FromDate.Date).Days;
                            int diffDaysFromAdminDate = (ToDate.Date - AdminConfigDate.Date).Days;

                            if (diffDaysFromToDate <= diffDaysFromAdminDate)
                            {
                                QueryList = QueryList.Where(q => q.LastUpdatedMsgDate.Value.Date >= FromDate.Date && q.LastUpdatedMsgDate.Value.Date <= ToDate.Date).ToList();
                                sentEnquiries = dbContext.tblselectedDealers.Where(t => DbFunctions.TruncateTime(t.LastUpdatedMsgDate.Value) >= FromDate.Date && DbFunctions.TruncateTime(t.LastUpdatedMsgDate.Value) <= ToDate.Date && t.CustID == searchParameters.CustID);
                                receivedEnquiries = dbContext.tblselectedDealerDetails.Where(t => DbFunctions.TruncateTime(t.LastUpdatedMsgDate.Value) >= FromDate.Date && DbFunctions.TruncateTime(t.LastUpdatedMsgDate.Value) <= ToDate.Date && t.CustID == searchParameters.CustID);
                            }
                            else
                            {
                                QueryList = QueryList.Where(q => q.LastUpdatedMsgDate.Value.Date >= AdminConfigDate.Date && q.LastUpdatedMsgDate.Value.Date <= ToDate.Date).ToList();
                                sentEnquiries = dbContext.tblselectedDealers.Where(t => DbFunctions.TruncateTime(t.LastUpdatedMsgDate.Value) >= AdminConfigDate.Date && DbFunctions.TruncateTime(t.LastUpdatedMsgDate.Value) <= ToDate.Date && t.CustID == searchParameters.CustID);
                                receivedEnquiries = dbContext.tblselectedDealerDetails.Where(t => DbFunctions.TruncateTime(t.LastUpdatedMsgDate.Value) >= FromDate.Date && DbFunctions.TruncateTime(t.LastUpdatedMsgDate.Value) <= ToDate.Date && t.CustID == searchParameters.CustID);
                            }
                        }
                        else
                        {
                            sentEnquiries = dbContext.tblselectedDealers.Where(t => DbFunctions.TruncateTime(t.CreatedDate) >= EnquiriesFromDate && t.CustID == searchParameters.CustID);
                            receivedEnquiries = dbContext.tblselectedDealerDetails.Where(t => DbFunctions.TruncateTime(t.CreatedDate) >= EnquiriesFromDate && t.CustID == searchParameters.CustID);
                        }

                        IQueryable<tblUserConversation> user = (from u in dbContext.tblselectedDealers
                                                                join uc in dbContext.tblUserConversations on u.CustID equals uc.IsDealer
                                                                where u.CustID == searchParameters.CustID
                                                                select uc).AsQueryable();

                        if (user != null && user.Count() > 0)
                        {
                            #region Old Code
                            //foreach (var item in user)
                            //{
                            //    var custSendList1 = (from sdd in dbContext.tblselectedDealerDetails
                            //                        join sd in sentEnquiries on sdd.QueryId equals sd.ID
                            //                        join uc in user on sdd.CustID equals uc.CustID
                            //                        join cd in dbContext.tblCustomerDetails on sdd.CustID equals cd.CustID
                            //                        join sc in dbContext.tblStateWithCities on sd.CityId equals sc.StatewithCityID
                            //                        join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ItemId
                            //                        where sdd.QueryId == item.QueryId.Value
                            //                        && uc.IsDealer == item.IsDealer.Value && sdd.CreatedBy == searchParameters.CustID
                            //                        && cd.IsActive == true
                            //                        select new FavoriteConversations
                            //                        {
                            //                            QueryId = sdd.QueryId,
                            //                            CustID = item.CustID.Value,
                            //                            FirmName = cd.FirmName,
                            //                            EmailID = cd.EmailID,
                            //                            MobileNumber = cd.MobileNumber,
                            //                            BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.BusinessDemandID == sd.BusinessDemandID).FirstOrDefault().BusinessDemand),
                            //                            BusinessDemandID = sd.BusinessDemandID.Value,
                            //                            PurposeOfBusiness = sd.PurposeBusiness,
                            //                            VillageLocalityname = sc.VillageLocalityName,
                            //                            Requirements = sd.OpenText,
                            //                            LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                            //                            IsRead = sdd.IsRead.Value,
                            //                            SenderRead = sdd.SenderRead.Value,
                            //                            SenderID = sdd.SenderID.Value,
                            //                            ChildCategoryName = cc.ItemName,
                            //                            IsFavorite = (dbContext.tblFavoriteConversations.Where(f => f.CustID == searchParameters.CustID && f.ReceiverID == item.CustID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                            //                            FavoriteCustID = sdd.FavoriteCustID,
                            //                            IsDeleted = (dbContext.tblDeleteConversations.Where(f => f.CustID == searchParameters.CustID && f.ReceiverID == item.CustID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                            //                            CityId = sdd.CityId,
                            //                            CreatedBy = sdd.CreatedBy.Value,
                            //                            isSentEnquiry = 1,
                            //                            RequirementName = (dbContext.tblProfessionalRequirements.Where(p => p.ProfessionalRequirementID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),
                            //                            ProfessionalRequirementID = sd.ProfessionalRequirementID,
                            //                            EnquiryDate = sd.CreatedDate,
                            //                            EnquiryType = sd.EnquiryType,
                            //                            SenderImage = cd.UserImage,
                            //                        }).ToList();

                            //    //if (custSendList != null)
                            //    //    QueryList.AddRange(custSendList);
                            //}
                            #endregion
                            var custSendList = (from sdd in dbContext.tblselectedDealerDetails
                                                join sd in sentEnquiries on sdd.QueryId equals sd.ID
                                                join uc in user on sdd.CustID equals uc.CustID
                                                join cd in dbContext.tblCustomerDetails on sdd.CustID equals cd.ID
                                                join sc in dbContext.tblStateWithCities on sd.CityId equals sc.StatewithCityID
                                                join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ID
                                                //join cc in dbContext.tblItemCategories on sd.ProductID equals cc.ID
                                                where sdd.QueryId == uc.QueryId.Value
                                                && uc.IsDealer == uc.IsDealer.Value && sdd.CreatedBy == searchParameters.CustID
                                                && cd.IsActive == true
                                                select new FavoriteConversations
                                                {
                                                    QueryId = sdd.QueryId,
                                                    CustID = uc.CustID.Value,
                                                    FirmName = cd.FirmName,
                                                    EmailID = cd.EmailID,
                                                    MobileNumber = cd.MobileNumber,
                                                    //BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.BusinessDemandID == sd.BusinessDemandID).FirstOrDefault().BusinessDemand),
                                                    BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.ID == sd.BusinessDemandID).FirstOrDefault().Demand),
                                                    BusinessDemandID = sd.BusinessDemandID.Value,
                                                    PurposeOfBusiness = sd.PurposeBusiness,
                                                    VillageLocalityname = sc.VillageLocalityName,
                                                    Requirements = sd.OpenText,
                                                    LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                                    IsRead = sdd.IsRead.Value,
                                                    SenderRead = sdd.SenderRead.Value,
                                                    SenderID = sdd.SenderID.Value,
                                                    ChildCategoryName = cc.ChildCategoryName,
                                                    IsFavorite = (dbContext.tblFavoriteConversations.Where(f => f.CustID == searchParameters.CustID && f.ReceiverID == uc.CustID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                                    FavoriteCustID = sdd.FavoriteCustID,
                                                    IsDeleted = (dbContext.tblDeleteConversations.Where(f => f.CustID == searchParameters.CustID && f.ReceiverID == uc.CustID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                                    CityId = sdd.CityId,
                                                    CreatedBy = sdd.CreatedBy,
                                                    isSentEnquiry = 1,
                                                    //RequirementName = (dbContext.tblProfessionalRequirements.Where(p => p.ProfessionalRequirementID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),
                                                    RequirementName = (dbContext.tblProfessionalRequirements.Where(p => p.ID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),
                                                    ProfessionalRequirementID = sd.ProfessionalRequirementID,
                                                    EnquiryDate = sd.CreatedDate,
                                                    EnquiryType = sd.EnquiryType,
                                                    SenderImage = cd.UserImage,
                                                }).Distinct().ToList();
                            if (custSendList != null)
                                QueryList.AddRange(custSendList);
                        }

                        var custRecLists = (from sdd in receivedEnquiries
                                            join sd in dbContext.tblselectedDealers on sdd.QueryId equals sd.ID
                                            join cd in dbContext.tblCustomerDetails on sd.CustID equals cd.ID
                                            join sc in dbContext.tblStateWithCities on sd.CityId equals sc.StatewithCityID
                                            join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ID
                                            //join cc in dbContext.tblItemCategories on sd.ProductID equals cc.ID
                                            where sdd.CustID == searchParameters.CustID && sdd.IsDeleted == 0
                                            && cd.IsActive == true
                                            select new FavoriteConversations
                                            {
                                                QueryId = sdd.QueryId,
                                                //CustID = cd.CustID,
                                                CustID = cd.ID,
                                                FirmName = cd.FirmName,
                                                EmailID = cd.EmailID,
                                                MobileNumber = cd.MobileNumber,
                                                //BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.BusinessDemandID == sd.BusinessDemandID).FirstOrDefault().BusinessDemand),
                                                BusinessDemand = (dbContext.tblBusinessDemands.Where(b => b.ID == sd.BusinessDemandID).FirstOrDefault().Demand),
                                                BusinessDemandID = sd.BusinessDemandID.Value,
                                                PurposeOfBusiness = sd.PurposeBusiness,
                                                VillageLocalityname = sc.VillageLocalityName,
                                                Requirements = sd.OpenText,
                                                LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                                IsRead = sdd.IsRead.Value,
                                                SenderRead = sdd.SenderRead.Value,
                                                SenderID = sdd.SenderID.Value,
                                                ChildCategoryName = cc.ChildCategoryName,
                                                //IsFavorite = (dbContext.tblFavoriteConversations.Where(f => f.CustID == searchParameters.CustID && f.ReceiverID == cd.CustID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                                IsFavorite = (dbContext.tblFavoriteConversations.Where(f => f.CustID == searchParameters.CustID && f.ReceiverID == cd.ID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                                FavoriteCustID = sdd.FavoriteCustID,
                                                //IsDeleted = (dbContext.tblDeleteConversations.Where(f => f.CustID == searchParameters.CustID && f.ReceiverID == cd.CustID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                                IsDeleted = (dbContext.tblDeleteConversations.Where(f => f.CustID == searchParameters.CustID && f.ReceiverID == cd.ID && f.QueryId == sdd.QueryId).FirstOrDefault() == null ? 0 : 1),
                                                CityId = sdd.CityId,
                                                CreatedBy = sdd.CreatedBy,
                                                isSentEnquiry = 0,
                                                //RequirementName = (dbContext.tblProfessionalRequirements.Where(p => p.ProfessionalRequirementID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),
                                                RequirementName = (dbContext.tblProfessionalRequirements.Where(p => p.ID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName),
                                                ProfessionalRequirementID = sd.ProfessionalRequirementID,
                                                EnquiryDate = sd.CreatedDate,
                                                EnquiryType = sd.EnquiryType,
                                                SenderImage = cd.UserImage,
                                            }).ToList();
                        QueryList.AddRange(custRecLists);
                        List<FavoriteConversations> result = (List<FavoriteConversations>)QueryList.Union(custRecLists, new FavoritecustomerQueriesComparer()).ToList();

                        QueryList = result;
                        QueryList = QueryList.Where(u => u.CustID != searchParameters.CustID).Distinct().ToList();


                        QueryList = QueryList.Where(q => q.IsFavorite == 1).ToList();
                        //remove deleted
                        QueryList = QueryList.Where(u => u.IsDeleted != 1).Distinct().ToList();
                        //Search by City 
                        if (searchParameters.CityIdList != null && searchParameters.CityIdList.Count() > 0)
                            QueryList = QueryList.Where(u => searchParameters.CityIdList.Exists(c => c.StateWithCityID == u.CityId)).Distinct().ToList();

                        //Search by BusinessDemand
                        if (searchParameters.BusinessDemand != null && searchParameters.BusinessDemand.Count() > 0)
                            //QueryList = QueryList.Where(u => searchParameters.BusinessDemand.Exists(b => b.BusinessDemandID == u.BusinessDemandID)).Distinct().ToList();
                            QueryList = QueryList.Where(u => searchParameters.BusinessDemand.Exists(b => b.ID == u.BusinessDemandID)).Distinct().ToList();

                        //Search by Sent Enquiry -- //Search by Received Enquiry
                        if (searchParameters.IsSentEnquiry == 1)
                        {
                            QueryList = QueryList.Where(q => q.isSentEnquiry == 1).ToList();
                        }
                        else if (searchParameters.IsReceivedEnquiry == 1)
                            QueryList = QueryList.Where(q => q.isSentEnquiry == 0).ToList();

                        //Search by BusinessTypes
                        if (searchParameters.BusinessTypeIds.Count() > 0)
                        {
                            QueryList = (from sdd in QueryList
                                         join btc in dbContext.tblBusinessTypewithCusts on sdd.CustID equals btc.CustID
                                         select new FavoriteConversations
                                         {
                                             QueryId = sdd.QueryId,
                                             CustID = sdd.CustID,
                                             FirmName = sdd.FirmName,
                                             EmailID = sdd.EmailID,
                                             MobileNumber = sdd.MobileNumber,
                                             BusinessDemand = sdd.BusinessDemand,
                                             BusinessDemandID = sdd.BusinessDemandID.Value,
                                             PurposeOfBusiness = sdd.PurposeOfBusiness,
                                             VillageLocalityname = sdd.VillageLocalityname,
                                             Requirements = sdd.Requirements,
                                             LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                             IsRead = sdd.IsRead,
                                             SenderRead = sdd.SenderRead,
                                             SenderID = sdd.SenderID,
                                             ChildCategoryName = sdd.ChildCategoryName,
                                             IsFavorite = sdd.IsFavorite,
                                             FavoriteCustID = sdd.FavoriteCustID,
                                             IsDeleted = sdd.IsDeleted,
                                             CityId = sdd.CityId,
                                             CreatedBy = sdd.CreatedBy,
                                             isSentEnquiry = sdd.isSentEnquiry,
                                             BusinessTID = btc.BusinessTypeID.ToString(),
                                             RequirementName = sdd.RequirementName,
                                             ProfessionalRequirementID = sdd.ProfessionalRequirementID,
                                             EnquiryDate = sdd.EnquiryDate,
                                             EnquiryType = sdd.EnquiryType,
                                             SenderImage = sdd.SenderImage,
                                         }).ToList();

                            //searchParameters.BusinessTypeIds.ForEach(d => d.BusinessTypeID == QueryList.Select(dd => dd.BusinessTID));

                            foreach (var item in searchParameters.BusinessTypeIds)
                            {
                                var CustBusinessTList = QueryList.Where(d => d.BusinessTID == item.BusinessTypeID.ToString()).ToList();
                                if (CustBusinessTList != null)
                                {
                                    var isExists = (from getOld in FilteredList
                                                    join newDAta in QueryList on getOld.CustID equals newDAta.CustID
                                                    where getOld.CustID == newDAta.CustID && getOld.QueryId == newDAta.QueryId
                                                    select getOld);

                                    if (isExists == null || isExists.Count() == 0)
                                    {
                                        flag = true;
                                        FilteredList.AddRange(CustBusinessTList);
                                    }
                                }
                            }

                            FilteredList = (from sdd in FilteredList
                                            select new FavoriteConversations
                                            {
                                                QueryId = sdd.QueryId,
                                                CustID = sdd.CustID,
                                                FirmName = sdd.FirmName,
                                                EmailID = sdd.EmailID,
                                                MobileNumber = sdd.MobileNumber,
                                                BusinessDemand = sdd.BusinessDemand,
                                                PurposeOfBusiness = sdd.PurposeOfBusiness,
                                                VillageLocalityname = sdd.VillageLocalityname,
                                                Requirements = sdd.Requirements,
                                                LastUpdatedMsgDate = sdd.LastUpdatedMsgDate,
                                                IsRead = sdd.IsRead,
                                                SenderID = sdd.SenderID,
                                                ChildCategoryName = sdd.ChildCategoryName,
                                                IsFavorite = sdd.IsFavorite,
                                                FavoriteCustID = sdd.FavoriteCustID,
                                                IsDeleted = sdd.IsDeleted,
                                                isSentEnquiry = sdd.isSentEnquiry,
                                                RequirementName = sdd.RequirementName,
                                                ProfessionalRequirementID = sdd.ProfessionalRequirementID,
                                                EnquiryDate = sdd.EnquiryDate,
                                                EnquiryType = sdd.EnquiryType,
                                                SenderImage = sdd.SenderImage,
                                            }).Distinct().ToList();
                        }

                        if (flag == false)
                            FilteredList = QueryList;

                        //Set IsRead
                        foreach (var item in FilteredList)
                        {
                            if (item.CreatedBy == searchParameters.CustID)
                                item.IsRead = item.SenderRead;
                            else
                                item.IsRead = item.IsRead;
                        }

                        FilteredList = FilteredList.Distinct().OrderByDescending(m => m.LastUpdatedMsgDate).ToList();
                        return FilteredList;
                    }
                }
            }
            catch (Exception ex)
            {
                //string strMessage= ex.Message;

                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }
        private class FavoritecustomerQueriesComparer : IEqualityComparer<FavoriteConversations>
        {
            public bool Equals(FavoriteConversations x, FavoriteConversations y)
            {
                return x.CustID == y.CustID && x.QueryId == y.QueryId;
            }

            // If Equals() returns true for a pair of objects 
            // then GetHashCode() must return the same value for these objects.

            public int GetHashCode(FavoriteConversations myModel)
            {
                return myModel.QueryId.GetHashCode();
            }
        }
        public List<City> GetCityOfSentEnquiries(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<CustomerQueries> list = (from sd in dbContext.tblselectedDealers
                                                      join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                                      join city in dbContext.tblStateWithCities on sd.CityId equals city.StatewithCityID
                                                      where sd.CustID == CustID
                                                      select new CustomerQueries
                                                      {
                                                          QueryId = sd.ID,
                                                          CustID = sd.CustID,
                                                          CityId = sd.CityId,
                                                          VillageLocalityname = city.VillageLocalityName,
                                                          SenderID = sdd.CustID.Value,
                                                          EnquiryDate = sd.CreatedDate,
                                                      }).Distinct().ToList<CustomerQueries>();

                        DateTime EnquiryConfiguredDate = DateTime.Now.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch));
                        list = list.Where(q => q.EnquiryDate.Value.Date > EnquiryConfiguredDate.Date).ToList();

                        //get deleted list
                        var deletedList = dbContext.tblDeleteConversations.Where(d => d.CustID == CustID).ToList();
                        var deletedConversations = dbContext.tblDeleteEnquiries.Where(d => d.CustID == CustID).ToList();

                        //List<CustomerQueries> newList = (from lst in list
                        //                                 join d in deletedList on lst.QueryId equals d.QueryId
                        //                                 where lst.QueryId != d.QueryId
                        //                                 select new CustomerQueries
                        //                                 {
                        //                                     QueryId = lst.QueryId,
                        //                                     CityId = lst.CityId,
                        //                                     VillageLocalityname = lst.VillageLocalityname,
                        //                                 }).Distinct().ToList<CustomerQueries>();

                        if (deletedList != null)
                        {
                            list.RemoveAll(l => deletedList.Exists(d => d.QueryId == l.QueryId && (d.CustID == l.CustID && d.ReceiverID == l.SenderID)));
                        }

                        if (deletedConversations != null)
                        {
                            list.RemoveAll(l => deletedConversations.Exists(d => d.QueryId == l.QueryId && (d.CustID == l.CustID)));
                        }

                        List<CustomerQueries> distinctList = (from lst in list
                                                              select new CustomerQueries
                                                              {
                                                                  CityId = lst.CityId,
                                                                  VillageLocalityname = lst.VillageLocalityname
                                                              }).Distinct().ToList<CustomerQueries>();

                        distinctList = distinctList.Distinct().ToList();

                        distinctList = distinctList.GroupBy(p => p.CityId).Select(g => g.First()).ToList();

                        List<City> cityList = (from l in distinctList
                                               select new City
                                               {
                                                   StateWithCityID = l.CityId,
                                                   VillageLocalityName = l.VillageLocalityname
                                               }
                                               ).Distinct().ToList<City>();

                        return cityList.Distinct().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }

        public List<City> GetFavoriteCities(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<CustomerQueries> list = (from sd in dbContext.tblselectedDealers
                                                      join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                                      join city in dbContext.tblStateWithCities on sdd.CityId equals city.StatewithCityID
                                                      join f in dbContext.tblFavoriteConversations on sdd.QueryId equals f.QueryId
                                                      where sdd.CustID == CustID || sd.CustID == CustID
                                                      && f.CustID == CustID
                                                      select new CustomerQueries
                                                      {
                                                          QueryId = sdd.QueryId,
                                                          CustID = sdd.CustID.Value,
                                                          CityId = sdd.CityId,
                                                          VillageLocalityname = city.VillageLocalityName,
                                                          SenderID = CustID,
                                                          EnquiryDate = sd.CreatedDate
                                                      }).Distinct().ToList<CustomerQueries>();

                        DateTime EnquiryConfiguredDate = DateTime.Now.AddDays(-Convert.ToDouble(dbContext.tblAdminSettings.FirstOrDefault().AddDaysForSearch));
                        list = list.Where(q => q.EnquiryDate.Value.Date > EnquiryConfiguredDate.Date).ToList();

                        //get deleted list
                        var deletedList = dbContext.tblDeleteConversations.Where(d => d.CustID == CustID).ToList();
                        var favoriteList = dbContext.tblFavoriteConversations.Where(d => d.CustID == CustID).ToList();

                        favoriteList.RemoveAll(f => deletedList.Exists(d => d.CustID == f.CustID && d.QueryId == f.QueryId && d.ReceiverID == f.ReceiverID));

                        //List<CustomerQueries> newList = (from lst in list
                        //                                 join d in deletedList on lst.QueryId equals d.QueryId
                        //                                 where lst.QueryId != d.QueryId
                        //                                 select new CustomerQueries
                        //                                 {
                        //                                     QueryId = lst.QueryId,
                        //                                     CityId = lst.CityId,
                        //                                     VillageLocalityname = lst.VillageLocalityname,
                        //                                 }).Distinct().ToList<CustomerQueries>();

                        if (deletedList != null)
                        {
                            //foreach (var item in deletedList)
                            //{
                            //    list.RemoveAll(l => list.Exists(ls => ls.SenderID == item.CustID && ls.QueryId == item.QueryId && ls.CustID == item.ReceiverID));
                            //}
                            list.RemoveAll(l => deletedList.Exists(d => d.QueryId == l.QueryId && d.CustID == l.SenderID && d.ReceiverID == l.CustID));
                        }

                        if (favoriteList != null)
                        {
                            list.RemoveAll(l => !favoriteList.Exists(d => d.QueryId == l.QueryId && (d.CustID == l.CustID)));
                        }

                        List<CustomerQueries> distinctList = (from lst in list
                                                              select new CustomerQueries
                                                              {
                                                                  CityId = lst.CityId,
                                                                  VillageLocalityname = lst.VillageLocalityname
                                                              }).Distinct().ToList<CustomerQueries>();

                        distinctList = distinctList.Distinct().ToList();

                        distinctList = distinctList.GroupBy(p => p.CityId).Select(g => g.First()).ToList();

                        List<City> cityList = (from l in distinctList
                                               select new City
                                               {
                                                   StateWithCityID = l.CityId,
                                                   VillageLocalityName = l.VillageLocalityname
                                               }
                                               ).Distinct().ToList<City>();

                        return cityList.Distinct().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }

        public List<SubCategoryTypes> GetAllSubCategoryList()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<SubCategoryTypes> GetAllSubCategories = (from PView in dbContext.tblSubCategories
                                                                      select new SubCategoryTypes
                                                                      {
                                                                          SubCategoryId = PView.SubCategoryId,
                                                                          SubCategoryName = PView.SubCategoryName,
                                                                      }).Distinct().ToList();

                        return GetAllSubCategories;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<ChildCategories> GetChildCatagories(int SubCategoryID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        List<ChildCategories> ChildCategoryList = (from PView in dbContext.tblChildCategories
                                                                   where PView.SubCategoryId == SubCategoryID
                                                                   select new ChildCategories
                                                                   {
                                                                       ChildCategoryName = PView.ChildCategoryName,
                                                                       SubCategoryId = PView.SubCategoryId
                                                                   }).Distinct().ToList();

                        ChildCategoryList = ChildCategoryList.Distinct().ToList();
                        return ChildCategoryList;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public string RequestForItemCategory(SendMailParameters sendMailParameters, List<Attachment> MailAttachments)
        {
            string ToEmailID = ConfigurationManager.AppSettings["ToEmailID"].ToString();
            string FromMailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
            string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
            string MailServerHost = ConfigurationManager.AppSettings["MailServerHost"].ToString();
            string SendingPort = ConfigurationManager.AppSettings["SendingPort"].ToString();
            try
            {
                //Mail Body
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Hello, <br/>");
                stringBuilder.Append("New Request from user to create new product - <br/>");
                stringBuilder.Append("Item Category Name : " + sendMailParameters.ItemName + "<br/>");
                stringBuilder.Append("Child Category Name : " + sendMailParameters.ChildCategoryName + "<br/>");
                stringBuilder.Append("Sub Category Name : " + sendMailParameters.SubCategoryName + "<br/><br/>");
                stringBuilder.Append("From User - <br/>");
                stringBuilder.Append("Firm Name : " + sendMailParameters.FirmName + "<br/>");
                stringBuilder.Append("Mobile Number : " + sendMailParameters.MobileNumber + "<br/>");
                stringBuilder.Append("City : " + sendMailParameters.CityName + "<br/><br/>");
                sendMailParameters.MailBody = stringBuilder.ToString();

                Helper.SendMail(ToEmailID, string.Empty, FromMailID, sendMailParameters.MailBody, sendMailParameters.MailSubject, MailServerHost, SendingPort, MailPassword, MailAttachments);
                return "Mail Sent";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<tblBusinessDemand> GetBusinessDemands()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<tblBusinessDemand> BusinessDemands = (from tbd in dbContext.tblBusinessDemands
                                                               select tbd).Distinct().ToList();
                    return BusinessDemands;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Implemented on 5th October 2020
        public List<tblProfessionalRequirement> GetProfessionalRequirements()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<tblProfessionalRequirement> professionalRequirements = (from tbd in dbContext.tblProfessionalRequirements
                                                                                 select tbd).OrderBy(p => p.RequirementName).Distinct().ToList();
                    return professionalRequirements;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Added on 21/10/2020 to get the customer details using mobile number
        public tblCustomerDetail GetCustomerByMobileNumber(string MobileNumber)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    tblCustomerDetail tblCustomer = new tblCustomerDetail();
                    tblCustomer = dbContext.tblCustomerDetails.Where(c => c.MobileNumber == MobileNumber).FirstOrDefault();

                    return tblCustomer;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public tblCustomerDetail_Questions Getcustomerbymobilenumber_New(string MobileNumber)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    tblCustomerDetail_Questions customerDetails = new tblCustomerDetail_Questions();
                    tblCustomerDetail_Questions Question = new tblCustomerDetail_Questions();
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    customerDetails = (from c in dbContext.tblCustomerDetails
                                       where c.MobileNumber == MobileNumber
                                       select new tblCustomerDetail_Questions
                                       {
                                           //CustID = c.CustID,
                                           CustID = c.ID,
                                           FirmName = c.FirmName,
                                           IsActive = c.IsActive,
                                           IsBlocked = c.IsBlocked,
                                           MobileNumber = c.MobileNumber,
                                           QuestionAnswered = c.QuestionAnswered,
                                           BlockedDate = c.BlockedDate,
                                       }).FirstOrDefault();

                    if (customerDetails != null)
                    {
                        bool IsBlocked = false;
                        DateTime? BlockedDate = null;

                        if (customerDetails.IsBlocked != null)
                            IsBlocked = customerDetails.IsBlocked.Value;

                        if (customerDetails.BlockedDate != null)
                            BlockedDate = customerDetails.BlockedDate.Value;

                        if (IsBlocked == false)
                        {
                            customerDetails.Blockstatus = false;
                        }
                        else if (IsBlocked == true)
                        {
                            customerDetails.Blockstatus = true;
                            if (BlockedDate.Value.AddHours(24) <= DateTimeNow)
                            {
                                customerDetails.Blockstatus = false;
                                customerDetails.tblQuestion = dbContext.tblQuestions.OrderBy(r => Guid.NewGuid()).Take(1).First();
                            }
                        }
                        else
                        {
                            if (BlockedDate.Value.AddHours(24) <= DateTimeNow)
                                customerDetails.Blockstatus = false;
                            customerDetails.tblQuestion = dbContext.tblQuestions.OrderBy(r => Guid.NewGuid()).Take(1).First();
                        }
                    }
                    else
                    {
                        tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();

                        //tblCustomerDetail.CreatedByID = 1;
                        tblCustomerDetail.CreatedBy = 1;
                        tblCustomerDetail.CreatedDate = DateTimeNow;
                        tblCustomerDetail.IsRegistered = 0;
                        tblCustomerDetail.QuestionAnswered = false;
                        tblCustomerDetail.IsOTPVerified = false;
                        tblCustomerDetail.MobileNumber = MobileNumber;
                        tblCustomerDetail.UserType = "1";
                        tblCustomerDetail.IsActive = false;
                        tblCustomerDetail.IsBlocked = false;
                        dbContext.tblCustomerDetails.Add(tblCustomerDetail);
                        dbContext.SaveChanges();
                        customerDetails = new tblCustomerDetail_Questions();
                        //customerDetails.CustID = tblCustomerDetail.CustID;
                        customerDetails.CustID = tblCustomerDetail.ID;
                        customerDetails.IsActive = false;
                        customerDetails.IsBlocked = false;
                        customerDetails.Blockstatus = false;
                        customerDetails.MobileNumber = MobileNumber;
                        customerDetails.QuestionAnswered = false;
                    }


                    if (customerDetails.Blockstatus == false)
                    {
                        customerDetails.tblQuestion = dbContext.tblQuestions.OrderBy(r => Guid.NewGuid()).Take(1).First();
                    }
                    string IsQuestionRequired = ConfigurationManager.AppSettings["IsQuestionRequired"].ToString();
                    customerDetails.IsQuestionRequired = IsQuestionRequired == "true" ? true : false;

                    return customerDetails;

                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Added to get the Message Count in home screen
        public Dashboard GetDashboardData(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE).Date;
                    Dashboard dashboard = new Dashboard();
                    SentEnquirySearchParameters sentEnquirySearchParameters = new SentEnquirySearchParameters();
                    sentEnquirySearchParameters.CustID = CustID;

                    dashboard.NotificationsCount = dbContext.tblPushNotifications.Where(n => n.CustID == CustID && DbFunctions.TruncateTime(n.NotificationDate.Value) == DateTimeNow).Count();
                    dashboard.ReceivedEnquiryMessagesCount = GetCustomerQueries(CustID, 0).Where(c => c.IsRead == 0).Count();
                    dashboard.FavouriteEnquiryMessagesCount = GetCustomerQueries(CustID, 1).Where(c => c.IsRead == 0).Count();
                    dashboard.SentEnquiryMessagesCount = GetEnquiries(sentEnquirySearchParameters).Where(c => c.ReplyCount > 0).Count();

                    dashboard.CustID = CustID;

                    return dashboard;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //For Admin Portal GetAllCustomers
        public List<CustomerDetails> GetCustomerList(CustomerDetails Context)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    List<CustomerDetails> customerList = new List<CustomerDetails>();
                    string WebsiteURL = ConfigurationManager.AppSettings["WebsiteURL"];

                    if (Context.StateList != null || Context.CityList != null)
                    {
                        customerList = (from c in dbContext.tblCustomerDetails
                                        join s in dbContext.tblStates on c.State equals s.ID
                                        join sc in dbContext.tblStateWithCities on c.City equals sc.ID
                                        where c.City != null && c.State != null && c.FirmName != null
                                        select new CustomerDetails
                                        {
                                            CustID = c.ID,
                                            CustName = c.CustName,
                                            FirmName = c.FirmName,
                                            MobileNumber = c.MobileNumber,
                                            EmailID = c.EmailID,
                                            BillingAddress = c.BillingAddress,
                                            Pincode = c.Pincode,
                                            CreatedDate = c.CreatedDate,
                                            city = new City { StateWithCityID = sc.ID, VillageLocalityName = sc.VillageLocalityName },
                                            state = new State { StateID = s.ID, StateName = s.StateName },
                                            IsActive = c.IsActive,
                                            RegisteredDate = c.CreatedDate.ToString(),
                                            UserImage = !string.IsNullOrEmpty(c.UserImage) ? WebsiteURL + c.UserImage : string.Empty,
                                            CategoryTypeWithCust = (from cp in dbContext.tblCategoryProductWithCusts
                                                                    join cc in dbContext.tblCategoryProducts on cp.CategoryProductID equals cc.ID
                                                                    where cp.CustID == c.ID
                                                                    select new CategoryTypeWithCust
                                                                    {
                                                                        MainCategoryName = cc.MainCategoryName
                                                                    }).ToList(),
                                            SubCategoryTypeWithCust = (from sp in dbContext.tblSubCategoryProductWithCusts
                                                                       join scp in dbContext.tblSubCategories on sp.SubCategoryId equals scp.ID
                                                                       where sp.CustID == c.ID
                                                                       select new SubCategoryProducts
                                                                       {
                                                                           SubCategoryName = scp.SubCategoryName
                                                                       }).ToList(),

                                        }).ToList();

                        foreach(var cust in customerList)
                        {
                            cust.CategoriesStr = String.Join(",", cust.CategoryTypeWithCust.Select(cc => cc.MainCategoryName).ToList());
                            cust.SubCategoriesStr = String.Join(",", cust.SubCategoryTypeWithCust.Select(cc => cc.SubCategoryName).ToList());
                        }

                        List<CustomerDetails> filteredList = new List<CustomerDetails>();

                        if (Context.StateList != null && Context.StateList.Count() > 0)
                        {
                            foreach (var str in Context.StateList)
                            {
                                filteredList.AddRange(customerList.Where(c => (c.state.StateID == null ? 0 : c.state.StateID) == str));
                            }
                        }
                        else
                        {
                            filteredList = customerList;
                        }

                        customerList = filteredList;
                        filteredList = new List<CustomerDetails>();

                        if (Context.CityList != null && Context.CityList.Count() > 0)
                        {
                            foreach (var str in Context.CityList)
                            {
                                filteredList.AddRange(customerList.Where(c => (c.city.StateWithCityID == null ? 0 : c.city.StateWithCityID) == str));
                            }
                        }
                        else
                        {
                            filteredList = customerList;
                        }
                        customerList = filteredList;
                        filteredList = new List<CustomerDetails>();
                    }
                    else
                    {
                        customerList = (from c in dbContext.tblCustomerDetails
                                        join s in dbContext.tblStates on c.State equals s.ID into ss
                                        from s in ss.DefaultIfEmpty()
                                        join sc in dbContext.tblStateWithCities on c.City equals sc.ID into scs
                                        from sc in scs.DefaultIfEmpty()
                                        where c.FirmName != null
                                        select new CustomerDetails
                                        {
                                            CustID = c.ID,
                                            CustName = c.CustName,
                                            FirmName = c.FirmName,
                                            MobileNumber = c.MobileNumber,
                                            EmailID = c.EmailID,
                                            BillingAddress = c.BillingAddress,
                                            Pincode = c.Pincode,
                                            CreatedDate = c.CreatedDate,
                                            city = new City { StateWithCityID = sc.ID, VillageLocalityName = sc.VillageLocalityName },
                                            state = new State { StateID = s.ID, StateName = s.StateName },
                                            IsActive = c.IsActive,
                                            RegisteredDate = c.CreatedDate.ToString(),
                                            TotalSubCategories = dbContext.tblSubCategoryProductWithCusts.Select(ss => ss.SubCategoryId).Distinct().Count(),
                                            UserImage = !string.IsNullOrEmpty(c.UserImage) ? WebsiteURL + c.UserImage : string.Empty,
                                            CategoryTypeWithCust = (from cp in dbContext.tblCategoryProductWithCusts
                                                                    join cc in dbContext.tblCategoryProducts on cp.CategoryProductID equals cc.ID
                                                                    where cp.CustID == c.ID
                                                                    select new CategoryTypeWithCust
                                                                    {
                                                                        MainCategoryName = cc.MainCategoryName
                                                                    }).ToList(),
                                            SubCategoryTypeWithCust = (from sp in dbContext.tblSubCategoryProductWithCusts
                                                                       join scp in dbContext.tblSubCategories on sp.SubCategoryId equals scp.ID
                                                                       where sp.CustID == c.ID
                                                                       select new SubCategoryProducts
                                                                       {
                                                                           SubCategoryName = scp.SubCategoryName
                                                                       }).ToList(),
                                        }).ToList();
                        foreach (var cust in customerList)
                        {
                            cust.CategoriesStr = String.Join(",", cust.CategoryTypeWithCust.Select(cc => cc.MainCategoryName).ToList());
                            cust.SubCategoriesStr = String.Join(",", cust.SubCategoryTypeWithCust.Select(cc => cc.SubCategoryName).ToList());
                        }
                    }

                    if (Context.CustID != 0)
                        customerList = customerList.Where(c => c.CustID == Context.CustID).ToList();

                    if (!string.IsNullOrEmpty(Context.FirmName))
                        customerList = customerList.Where(c => c.FirmName.ToLower().Contains(Context.FirmName.ToLower())).ToList();

                    //if (Context.state.StateID != 0)
                    //    customerList = customerList.Where(c => c.state.StateID == Context.state.StateID).ToList();
                    //if (Context.city.StateWithCityID != 0)
                    //    customerList = customerList.Where(c => c.city.StateWithCityID == Context.city.StateWithCityID).ToList();

                    //List<CustomerDetails> filteredList = new List<CustomerDetails>();

                    //if (Context.StateList != null && Context.StateList.Count() > 0)
                    //{
                    //    foreach (var str in Context.StateList)
                    //    {
                    //        filteredList.AddRange(customerList.Where(c => (c.state.StateID == null ? 0 : c.state.StateID) == str));
                    //    }
                    //}
                    //else
                    //{
                    //    filteredList = customerList;
                    //}



                    //if (Context.CityList != null && Context.CityList.Count() > 0)
                    //{
                    //    foreach (var str in Context.CityList)
                    //    {
                    //        filteredList.AddRange(customerList.Where(c => (c.city.StateWithCityID == null ? 0 : c.city.StateWithCityID) == str));
                    //    }
                    //}
                    //else
                    //{
                    //    filteredList = customerList;
                    //}

                    //customerList = filteredList;

                    if (!string.IsNullOrEmpty(Context.MobileNumber))
                        customerList = customerList.Where(c => c.MobileNumber == Context.MobileNumber).ToList();

                    if (!string.IsNullOrEmpty(Context.FromDate) && !string.IsNullOrEmpty(Context.ToDate))
                    {
                        DateTime FromDateTime = Convert.ToDateTime(Context.FromDate);
                        DateTime ToDateTime = Convert.ToDateTime(Context.ToDate);
                        customerList = customerList.Where(c => Convert.ToDateTime(c.CreatedDate).Date >= FromDateTime.Date && Convert.ToDateTime(c.CreatedDate).Date <= ToDateTime.Date).ToList();
                        if (!string.IsNullOrEmpty(Context.FromTime) && !string.IsNullOrEmpty(Context.ToTime))
                        {
                            DateTime fromTime = DateTime.ParseExact(Context.FromTime,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
                            TimeSpan Fromspan = fromTime.TimeOfDay;
                            DateTime toTime = DateTime.ParseExact(Context.ToTime,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
                            TimeSpan Tospan = toTime.TimeOfDay;

                            customerList = customerList.Where(c => Convert.ToDateTime(c.CreatedDate).TimeOfDay >= Fromspan && Convert.ToDateTime(c.CreatedDate).TimeOfDay <= Tospan).ToList();
                        }
                    }

                    //customerList.ForEach(c => c.UserImage = c.UserImage == null ? string.Empty : (WebsiteURL + c.UserImage)));

                    return customerList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<CustomerDetails> CatCityWiseCustList(CatCityWiseCustListParameters catCityWiseCust)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    List<CustomerDetails> customerList = new List<CustomerDetails>();

                    if (catCityWiseCust.ProductType == 1)
                    {
                        customerList = (from c in dbContext.tblCustomerDetails
                                        join cp in dbContext.tblCategoryProductWithCusts on c.ID equals cp.CustID
                                        join s in dbContext.tblStates on c.State equals s.ID
                                        join sc in dbContext.tblStateWithCities on c.City equals sc.ID
                                        where c.City != null && c.State != null
                                        && cp.CategoryProductID == catCityWiseCust.ProductID
                                        && c.IsActive == true && c.City == catCityWiseCust.CityId
                                        select new CustomerDetails
                                        {
                                            CustID = c.ID,
                                            CustName = c.CustName,
                                            FirmName = c.FirmName,
                                            MobileNumber = c.MobileNumber,
                                            EmailID = c.EmailID,
                                            CreatedDate = c.CreatedDate,
                                            city = new City { StateWithCityID = sc.ID, VillageLocalityName = sc.VillageLocalityName },
                                            state = new State { StateID = s.ID, StateName = s.StateName },
                                            IsActive = c.IsActive,
                                            RegisteredDate = c.CreatedDate.ToString(),
                                        }).Distinct().ToList();
                    }
                    else if (catCityWiseCust.ProductType == 2)
                    {
                        customerList = (from c in dbContext.tblCustomerDetails
                                        join cp in dbContext.tblSubCategoryProductWithCusts on c.ID equals cp.CustID
                                        join s in dbContext.tblStates on c.State equals s.ID
                                        join sc in dbContext.tblStateWithCities on c.City equals sc.ID
                                        where c.City != null && c.State != null
                                        && cp.SubCategoryId == catCityWiseCust.ProductID
                                        && c.IsActive == true && c.City == catCityWiseCust.CityId
                                        select new CustomerDetails
                                        {
                                            CustID = c.ID,
                                            CustName = c.CustName,
                                            FirmName = c.FirmName,
                                            MobileNumber = c.MobileNumber,
                                            EmailID = c.EmailID,
                                            CreatedDate = c.CreatedDate,
                                            city = new City { StateWithCityID = sc.ID, VillageLocalityName = sc.VillageLocalityName },
                                            state = new State { StateID = s.ID, StateName = s.StateName },
                                            IsActive = c.IsActive,
                                            RegisteredDate = c.CreatedDate.ToString(),
                                        }).ToList();
                    }
                    else if (catCityWiseCust.ProductType == 3)
                    {
                        customerList = (from c in dbContext.tblCustomerDetails
                                        join cp in dbContext.tblSubCategoryProductWithCusts on c.ID equals cp.CustID
                                        join s in dbContext.tblStates on c.State equals s.ID
                                        join cc in dbContext.tblChildCategories on cp.SubCategoryId equals cc.SubCategoryId
                                        join sc in dbContext.tblStateWithCities on c.City equals sc.ID
                                        where c.City != null && c.State != null
                                        && cc.ID == catCityWiseCust.ProductID
                                        && c.IsActive == true && c.City == catCityWiseCust.CityId
                                        select new CustomerDetails
                                        {
                                            CustID = c.ID,
                                            CustName = c.CustName,
                                            FirmName = c.FirmName,
                                            MobileNumber = c.MobileNumber,
                                            EmailID = c.EmailID,
                                            CreatedDate = c.CreatedDate,
                                            city = new City { StateWithCityID = sc.ID, VillageLocalityName = sc.VillageLocalityName },
                                            state = new State { StateID = s.ID, StateName = s.StateName },
                                            IsActive = c.IsActive,
                                            RegisteredDate = c.CreatedDate.ToString(),
                                        }).Distinct().ToList();
                    }
                    else if (catCityWiseCust.ProductType == 4)
                    {
                        customerList = (from c in dbContext.tblCustomerDetails
                                        join cp in dbContext.tblSubCategoryProductWithCusts on c.ID equals cp.CustID
                                        join s in dbContext.tblStates on c.State equals s.ID
                                        join cc in dbContext.tblChildCategories on cp.SubCategoryId equals cc.SubCategoryId
                                        join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID
                                        join sc in dbContext.tblStateWithCities on c.City equals sc.ID
                                        where c.City != null && c.State != null
                                        && ic.ID == catCityWiseCust.ProductID
                                        && c.IsActive == true && c.City == catCityWiseCust.CityId
                                        select new CustomerDetails
                                        {
                                            CustID = c.ID,
                                            CustName = c.CustName,
                                            FirmName = c.FirmName,
                                            MobileNumber = c.MobileNumber,
                                            EmailID = c.EmailID,
                                            CreatedDate = c.CreatedDate,
                                            city = new City { StateWithCityID = sc.ID, VillageLocalityName = sc.VillageLocalityName },
                                            state = new State { StateID = s.ID, StateName = s.StateName },
                                            IsActive = c.IsActive,
                                            RegisteredDate = c.CreatedDate.ToString(),
                                        }).Distinct().ToList();
                    }

                    return customerList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public bool UpdateAppUser(CustomerDetails customerDetails, int UserID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    var user = (from u in dbContext.tblCustomerDetails.AsNoTracking()
                                    //where u.CustID == customerDetails.CustID
                                where u.ID == customerDetails.CustID
                                select u).FirstOrDefault();

                    if (user == null)
                    {
                        return false;
                    }
                    tblCustomerDetail tblCustomerDetails = new tblCustomerDetail();
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    dbContext.Entry(user).State = EntityState.Detached;

                    tblCustomerDetails.UpdatedByID = UserID;
                    tblCustomerDetails.IsActive = customerDetails.IsActive;
                    tblCustomerDetails.ReasonForDeactivate = customerDetails.ReasonForDeactivate;
                    tblCustomerDetails.UpdatedByDate = DateTimeNow;
                    //tblCustomerDetails.CustID = customerDetails.CustID;
                    tblCustomerDetails.ID = customerDetails.CustID;
                    dbContext.tblCustomerDetails.Attach(tblCustomerDetails);
                    dbContext.Entry(tblCustomerDetails).Property(c => c.IsActive).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.ReasonForDeactivate).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.UpdatedByDate).IsModified = true;
                    dbContext.Entry(tblCustomerDetails).Property(c => c.UpdatedByID).IsModified = true;

                    //add histrory table 

                    tblCustomerStatusHistory tblCustomerStatus = new tblCustomerStatusHistory();
                    tblCustomerStatus.CustID = customerDetails.CustID;
                    tblCustomerStatus.Comments = customerDetails.ReasonForDeactivate;
                    if (customerDetails.IsActive == true)
                        tblCustomerStatus.CustomerStatus = "Actiated";
                    if (customerDetails.IsActive == false)
                        tblCustomerStatus.CustomerStatus = "Deactivated";
                    tblCustomerStatus.CreatedBy = UserID;
                    tblCustomerStatus.CreatedDate = DateTimeNow;
                    dbContext.tblCustomerStatusHistories.Add(tblCustomerStatus);

                    //Insert into History Table
                    tblHistory history = new tblHistory();
                    history.UserID = UserID;
                    history.CustID = customerDetails.CustID;
                    history.ProductCategory = customerDetails.FirmName;
                    //history.CreationDate = DateTimeNow;
                    history.CreatedDate = DateTimeNow;
                    history.ActivityPage = "appusers";
                    history.ActivityType = "update";
                    history.Comments = customerDetails.ReasonForDeactivate;
                    dbContext.tblHistories.Add(history);

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

        //For Admin Portal Share APK
        public string ShareAPK(int? CustID, string serverPath, string serverURL)
        {
            try
            {
                //var CustomerDetails = dbContext.tblCustomerDetails.Where(c => c.CustID == CustID).FirstOrDefault();
                var CustomerDetails = dbContext.tblCustomerDetails.Where(c => c.ID == CustID).FirstOrDefault();

                string SMSUserName = ConfigurationManager.AppSettings["SMSUserName"];
                string SMSPassword = ConfigurationManager.AppSettings["SMSPassword"];

                string ToEmailID = CustomerDetails.EmailID; /*ConfigurationManager.AppSettings["ToEmailID"].ToString();*/
                string FromMailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
                string MailServerHost = ConfigurationManager.AppSettings["MailServerHost"].ToString();
                string SendingPort = ConfigurationManager.AppSettings["SendingPort"].ToString();
                //string APKPath = ConfigurationManager.AppSettings["APKPath"].ToString();
                string MailSubject = "Kalpavriksha App | Application APK Link";
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Dear ");
                stringBuilder.Append(CustomerDetails.FirmName);
                stringBuilder.Append(",");
                stringBuilder.Append("<br/>");
                stringBuilder.Append("Please find and download the APK of Kalpavriksha Application <br/>");
                stringBuilder.Append(serverPath);
                stringBuilder.Append("<br/>");
                stringBuilder.Append("Thank You");
                string MailBody = stringBuilder.ToString();

                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    string Message = "Your URL link " + serverURL + " for resetting " + "Kalpavriksha" + " login password. link expire in " + 10 + " Minutes";

                    Helper.SendMail(ToEmailID, FromMailID, MailBody, MailSubject, MailServerHost, MailPassword, SendingPort);
                    Helper.SendSMS(SMSUserName, SMSPassword, CustomerDetails.MobileNumber, Message, "N");
                    //tblAPKResendHistory tblAPKResendHistory = new tblAPKResendHistory();
                    //tblAPKResendHistory.CustID = CustomerDetails.CustID;
                    //tblAPKResendHistory.ResendDate = DateTime.Now;
                    ////tblAPKResendHistory.ResendReason = shareAPK.Reason;
                    //tblAPKResendHistory.CreatedBy = CustID.ToString();
                    //dbContext.tblAPKResendHistories.Add(tblAPKResendHistory);
                    //dbContext.SaveChanges();
                    return "Mail Sent";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //Send Promotion
        public string Promotion(PromoWithBusinessTypeRptList promo, List<Attachment> MailAttachments, string ImageURL, int UserID)
        {
            try
            {
                string Result = string.Empty;
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                if (promo.IsEmail == true)
                {
                    string Bcc = string.Empty;
                    List<CustomerDetails> bccList = new List<CustomerDetails>();

                    foreach (var item in promo.businessTypeWiseRpts)
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
                    string MobileNumbers = string.Join(",", promo.businessTypeWiseRpts.Where(b => b.IsChecked == true).Select(c => c.Mobile1));

                    string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                    string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                    string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                    Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, promo.SMSBody, SenderID);
                }
                else if (promo.IsNotification == true)
                {
                    string[] Registration_Ids = promo.businessTypeWiseRpts.Where(b => b.IsChecked == true).Select(c => c.DeviceID).ToArray();
                    int[] Cust_Ids = promo.businessTypeWiseRpts.Where(b => b.IsChecked == true).Select(c => c.CustID).ToArray();
                    Notification notification = new Notification { Title = promo.Title, Body = promo.Body, NotificationDate = DateTimeNow, Image = ImageURL };
                    int itemsSent = 0, skipCount = 0;
                    int regCount = Registration_Ids.Count();
                    while(itemsSent < regCount)
                    {
                        string[] only999 = Registration_Ids.Skip(skipCount).Take(999).ToArray();
                        Helper.SendNotificationMultiple(only999, notification);
                        itemsSent = itemsSent + Registration_Ids.Skip(skipCount).Take(999).Count();
                        skipCount = skipCount + itemsSent;
                    }

                    PushNotifications pushNotifications = new PushNotifications()
                    {
                        Title = promo.Title,
                        NotificationDate = DateTimeNow,
                        CategoryName = string.Empty,
                        ImageURL = ImageURL,
                        PushNotification = promo.Body,
                    };
                    SavePushNotificationsList(Cust_Ids, pushNotifications, UserID);
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

        //Mark as Read / Unread Conversations
        public List<DealersDetails> MarkAsReadUnread(object Context)
        {
            List<DealersDetails> inputContext = new List<DealersDetails>();
            inputContext = (List<DealersDetails>)Context;
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    foreach (DealersDetails item in inputContext)
                    {
                        if (item.IsRead == 1)
                        {
                            var sdDe = (from sd in dbContext.tblselectedDealerDetails where sd.CreatedBy == item.ReceiverID && sd.QueryId == item.QueryId && sd.CustID == item.CustID select sd).FirstOrDefault();
                            if (sdDe != null)
                            {
                                //If the sender is Enquirer
                                sdDe.IsRead = 1;
                                dbContext.tblselectedDealerDetails.Attach(sdDe);
                                dbContext.Entry(sdDe).Property(x => x.IsRead).IsModified = true;
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                tblselectedDealerDetail sdDeC = (from sd in dbContext.tblselectedDealerDetails where sd.CreatedBy == item.CustID && sd.QueryId == item.QueryId && sd.CustID == item.ReceiverID select sd).FirstOrDefault();
                                if (sdDeC != null)
                                {
                                    sdDeC.SenderRead = 1;
                                    dbContext.tblselectedDealerDetails.Attach(sdDeC);
                                    dbContext.Entry(sdDeC).Property(x => x.SenderRead).IsModified = true;
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                        else if (item.IsRead == 0)
                        {
                            var sdDe = (from sd in dbContext.tblselectedDealerDetails where sd.CreatedBy == item.ReceiverID && sd.QueryId == item.QueryId && sd.CustID == item.CustID select sd).FirstOrDefault();
                            if (sdDe != null)
                            {
                                sdDe.IsRead = 0;
                                dbContext.tblselectedDealerDetails.Attach(sdDe);
                                dbContext.Entry(sdDe).Property(x => x.IsRead).IsModified = true;
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                tblselectedDealerDetail sdDeC = (from sd in dbContext.tblselectedDealerDetails where sd.CreatedBy == item.CustID && sd.QueryId == item.QueryId && sd.CustID == item.ReceiverID select sd).FirstOrDefault();
                                if (sdDeC != null)
                                {
                                    sdDeC.SenderRead = 0;
                                    dbContext.tblselectedDealerDetails.Attach(sdDeC);
                                    dbContext.Entry(sdDeC).Property(x => x.SenderRead).IsModified = true;
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                    return inputContext;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //01-02-2021
        public List<ChildCategories> GetItemNamesForUser(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<ChildCategories> itemCategories = new List<ChildCategories>();
                    itemCategories = (from c in dbContext.tblChildCategories
                                      join ic in dbContext.tblItemCategories on c.ID equals ic.ChildCategoryID
                                      join sd in dbContext.tblselectedDealers on c.ID equals sd.ProductID
                                      join sc in dbContext.tblSubCategories on c.SubCategoryId equals sc.SubCategoryId
                                      join cp in dbContext.tblCategoryProducts on sc.CategoryProductID equals cp.CategoryProductID
                                      where sd.CustID == CustID && c.IsActive == true
                                      select new ChildCategories
                                      {
                                          //ItemId = c.ItemId,
                                          //ItemName = c.ItemName,
                                          ItemId = ic.ID,
                                          ItemName = ic.ItemName,
                                          IsProfessional = cp.MainCategoryName.ToLower() == "professionals" ? true : false,
                                      }).Distinct().ToList();
                    return itemCategories;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<BusinessTypes> GetBusinessTypesForItem(int ItemID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<BusinessTypes> businessTypes = new List<BusinessTypes>();
                    businessTypes = (from b in dbContext.tblBusinessTypes
                                     select new BusinessTypes
                                     {
                                         BusinessTypeID = b.ID,
                                         BusinessTypeName = b.Type,
                                         Checked = false,
                                     }).ToList();

                    string MainCategoryName = (from cc in dbContext.tblChildCategories
                                               join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID
                                               join sc in dbContext.tblSubCategories on cc.SubCategoryId equals sc.SubCategoryId
                                               join cp in dbContext.tblCategoryProducts on sc.CategoryProductID equals cp.CategoryProductID
                                               where ic.ID == ItemID//cc.ItemId == ItemID
                                               select new ItemCategory
                                               {
                                                   MainCategoryName = cp.MainCategoryName,
                                                   //ItemId = cc.ItemId
                                                   ID = ic.ID
                                               }).FirstOrDefault().MainCategoryName;
                    if (MainCategoryName.ToLower() == "professionals")
                        businessTypes = businessTypes.Where(b => b.BusinessTypeName.ToLower() == "professionals").ToList();
                    else
                        businessTypes = businessTypes.Where(b => b.BusinessTypeName.ToLower() != "professionals").ToList();

                    return businessTypes;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<State> GetStatesListForUser(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<State> stateList = new List<State>();
                    stateList = (from s in dbContext.tblStates
                                 join c in dbContext.tblStateWithCities on s.StateID equals c.StateID
                                 join sd in dbContext.tblselectedDealers on c.StatewithCityID equals sd.CityId
                                 where sd.CustID == CustID
                                 select new State
                                 {
                                     StateID = s.StateID,
                                     StateName = s.StateName
                                 }).Distinct().ToList();

                    return stateList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<City> GetCitiesListForUser(int CustID, int StateID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<City> cityList = new List<City>();
                    cityList = (from c in dbContext.tblStateWithCities
                                join sd in dbContext.tblselectedDealers on c.StatewithCityID equals sd.CityId
                                where sd.CustID == CustID && c.StateID == StateID
                                select new City
                                {
                                    StateWithCityID = c.StatewithCityID,
                                    VillageLocalityName = c.VillageLocalityName
                                }).Distinct().ToList();

                    return cityList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<CustIDS> GetReplierList(CustDetails custDetails)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<CustIDS> custIDs = new List<CustIDS>();

                    if (custDetails.businessTypes != null && custDetails.businessTypes.Count() > 0)
                    {
                        int[] BTarray = custDetails.businessTypes.Select(b => b.BusinessTypeID).ToArray();
                        //custIDs = custIDs.Where(c => c.businessTypes.All(cd => BTarray.Contains(cd.BusinessTypeID))).ToList();
                        //custIDs = custIDs.Where(c => c.businessTypes.SequenceEqual(custDetails.BusinessTypeIDList)).ToList();

                        var customerIDs = (from sd in dbContext.tblselectedDealers
                                           join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                           join uc in dbContext.tblUserConversations on new { sdd.CustID, sdd.QueryId } equals new { uc.CustID, uc.QueryId }
                                           join c in dbContext.tblCustomerDetails on sdd.CustID equals c.ID
                                           join ct in dbContext.tblStateWithCities on c.City equals ct.StatewithCityID
                                           join s in dbContext.tblStates on ct.StateID equals s.StateID
                                           join ic in dbContext.tblItemCategories on sd.ProductID equals ic.ID
                                           join cc in dbContext.tblChildCategories on ic.ChildCategoryID equals cc.ID
                                           join sc in dbContext.tblSubCategoryProductWithCusts on new { cc.SubCategoryId } equals new { sc.SubCategoryId }
                                           join bt in dbContext.tblBusinessTypewithCusts on c.ID equals bt.CustID
                                           join b in dbContext.tblBusinessTypes on bt.BusinessTypeID equals b.ID
                                           where sd.CustID == custDetails.CustID && (c.City == custDetails.CityID || c.State == custDetails.StateID)
                                           && c.ID != sd.CustID && sc.CustID == c.ID
                                           && BTarray.Contains(bt.BusinessTypeID)
                                           select new CustIDS
                                           {
                                               CustID = sdd.CustID.Value,
                                               FirmName = c.FirmName + " (" + ct.VillageLocalityName + ")",
                                               StateName = s.StateName,
                                               businessTypes = (from b in dbContext.tblBusinessTypewithCusts.ToList()
                                                                    //join btc in dbContext.tblBusinessTypes on b.BusinessTypeID equals btc.BusinessTypeID
                                                                join btc in dbContext.tblBusinessTypes on b.ID equals btc.ID
                                                                where b.CustID == sdd.CustID.Value
                                                                select new BusinessTypes
                                                                {
                                                                    BusinessTypeID = b.BusinessTypeID,
                                                                    BusinessTypeName = btc.Type,
                                                                }).ToList(),
                                           }).AsEnumerable();

                        custIDs = customerIDs.ToList();


                        custIDs = (from c in custIDs
                                   select new CustIDS
                                   {
                                       CustID = c.CustID,
                                       FirmName = c.FirmName,
                                       StateName = c.StateName,
                                       businessTypes = c.businessTypes,
                                   }).Distinct().ToList();

                        custIDs = custIDs.GroupBy(i => i.CustID).Select(i => i.FirstOrDefault()).ToList();

                        return custIDs;
                    }
                    else
                    {
                        var customerIDs = (from sd in dbContext.tblselectedDealers
                                           join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                           join uc in dbContext.tblUserConversations on new { sdd.CustID, sdd.QueryId } equals new { uc.CustID, uc.QueryId }
                                           //join c in dbContext.tblCustomerDetails on sdd.CustID equals c.CustID
                                           join c in dbContext.tblCustomerDetails on sdd.CustID equals c.ID
                                           join ct in dbContext.tblStateWithCities on c.City equals ct.StatewithCityID
                                           join s in dbContext.tblStates on ct.StateID equals s.StateID
                                           //join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ItemId
                                           join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ID
                                           join sc in dbContext.tblSubCategoryProductWithCusts on new { cc.SubCategoryId } equals new { sc.SubCategoryId }
                                           where sd.CustID == custDetails.CustID && (c.City == custDetails.CityID || c.State == custDetails.StateID)
                                           // && c.CustID != sd.CustID && sc.CustID == c.CustID
                                           && c.ID != sd.CustID && sc.CustID == c.ID
                                           select new CustIDS
                                           {
                                               CustID = sdd.CustID.Value,
                                               FirmName = c.FirmName + " (" + ct.VillageLocalityName + ")",
                                               StateName = s.StateName,
                                               businessTypes = (from b in dbContext.tblBusinessTypewithCusts.ToList()
                                                                join btc in dbContext.tblBusinessTypes on b.BusinessTypeID equals btc.ID
                                                                where b.CustID == sdd.CustID.Value
                                                                select new BusinessTypes
                                                                {
                                                                    BusinessTypeID = b.BusinessTypeID,
                                                                    BusinessTypeName = btc.Type,
                                                                }).ToList(),
                                           }).AsEnumerable();

                        custIDs = customerIDs.ToList();

                        custIDs = (from c in custIDs
                                   select new CustIDS
                                   {
                                       CustID = c.CustID,
                                       FirmName = c.FirmName,
                                       StateName = c.StateName,
                                       businessTypes = c.businessTypes,
                                   }).Distinct().ToList();

                        custIDs = custIDs.GroupBy(i => i.CustID).Select(i => i.FirstOrDefault()).ToList();

                        return custIDs;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //public List<CustIDS> GetReplierList(CustDetails custDetails)
        //{
        //    try
        //    {
        //        using (dbContext = new mwbtDealerEntities())
        //        {
        //            List<CustIDS> custIDs = new List<CustIDS>();
        //            var customerIDs = (from sd in dbContext.tblselectedDealers
        //                               join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
        //                               join c in dbContext.tblCustomerDetails on sdd.CustID equals c.CustID                                       
        //                               join ct in dbContext.tblStateWithCities on c.City equals ct.StatewithCityID
        //                               join s in dbContext.tblStates on ct.StateID equals s.StateID
        //                               join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ItemId
        //                               join sc in dbContext.tblSubCategoryProductWithCusts on new { cc.SubCategoryId } equals new { sc.SubCategoryId }
        //                               where sd.CustID == custDetails.CustID && (c.City == custDetails.CityID || c.State == custDetails.StateID)
        //                               && c.CustID != sd.CustID && sc.CustID == c.CustID
        //                               select new CustIDS
        //                               {
        //                                   CustID = sdd.CustID.Value,
        //                                   FirmName = c.FirmName + " (" + ct.VillageLocalityName + ")",
        //                                   StateName = s.StateName,
        //                                   businessTypes = (from b in dbContext.tblBusinessTypewithCusts.ToList()
        //                                                    join btc in dbContext.tblBusinessTypes on b.BusinessTypeID equals btc.BusinessTypeID
        //                                                    where b.CustID == sdd.CustID.Value
        //                                                    select new BusinessTypes
        //                                                    {
        //                                                        BusinessTypeID = b.BusinessTypeID,
        //                                                        BusinessTypeName = btc.BusinessType,
        //                                                    }).ToList(),
        //                               }).AsEnumerable();

        //            custIDs = customerIDs.ToList();

        //            if (custDetails.BusinessTypeIDList != null && custDetails.BusinessTypeIDList.Count() > 0)
        //            {
        //                string[] BTarray = custDetails.BusinessTypeIDList.Select(b => b.BusinessTypeID).ToArray();
        //                custIDs = custIDs.Where(c => c.businessTypes.All(cd => BTarray.Contains(cd.BusinessTypeID))).ToList();
        //            }
        //            custIDs = (from c in customerIDs
        //                       select new CustIDS
        //                       {
        //                           CustID = c.CustID,
        //                           FirmName = c.FirmName,
        //                           StateName = c.StateName,
        //                           businessTypes = c.businessTypes,
        //                       }).Distinct().ToList();

        //            custIDs = custIDs.GroupBy(i => i.CustID).Select(i => i.FirstOrDefault()).ToList();

        //            return custIDs;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}

        public List<BusinessTypes> GetBusinessTypelistFor_OnePlus(CustDetails custDetails)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<BusinessTypes> BusinessTypeIDs = new List<BusinessTypes>();
                    BusinessTypeIDs = (from sd in dbContext.tblselectedDealers
                                       join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                       //join c in dbContext.tblCustomerDetails on sdd.CustID equals c.CustID
                                       join c in dbContext.tblCustomerDetails on sdd.CustID equals c.ID
                                       //join cc in dbContext.tblChildCategories on sd.ProductID equals cc.ItemId
                                       join ic in dbContext.tblItemCategories on sd.ProductID equals ic.ID
                                       join cc in dbContext.tblChildCategories on ic.ChildCategoryID equals cc.ID
                                       join sc in dbContext.tblSubCategoryProductWithCusts on new { cc.SubCategoryId } equals new { sc.SubCategoryId }
                                       //join Bt in dbContext.tblBusinessTypewithCusts on c.CustID equals Bt.CustID
                                       join Bt in dbContext.tblBusinessTypewithCusts on c.ID equals Bt.CustID
                                       join Bts in dbContext.tblBusinessTypes on Bt.BusinessTypeID equals Bts.ID
                                       where sd.CustID == custDetails.CustID && (c.City == custDetails.CityID || c.State == custDetails.StateID || c.InterstCountry.Value == true)
                                       && c.ID != sd.CustID && sc.CustID == c.ID
                                       select new BusinessTypes
                                       {
                                           BusinessTypeID = Bts.ID,
                                           BusinessTypeName = Bts.Type

                                       }).Distinct().ToList();

                    //custIDs = from c in custIDs 
                    //          join 

                    return BusinessTypeIDs;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<State> GetLoadEnquiry_StatelistOnePlus(CustDetails custDetails)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<State> StateEnquiryList = new List<State>();
                    StateEnquiryList = (from sd in dbContext.tblselectedDealers
                                        join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                        join c in dbContext.tblCustomerDetails on sdd.CustID equals c.ID
                                        join ic in dbContext.tblItemCategories on sd.ProductID equals ic.ID
                                        join cc in dbContext.tblChildCategories on ic.ChildCategoryID equals cc.ID
                                        join sc in dbContext.tblSubCategoryProductWithCusts on new { cc.SubCategoryId } equals new { sc.SubCategoryId }
                                        join Bt in dbContext.tblBusinessTypewithCusts on c.ID equals Bt.CustID
                                        join Bts in dbContext.tblBusinessTypes on Bt.BusinessTypeID equals Bts.ID
                                        join CT in dbContext.tblStates on c.State equals CT.StateID
                                        where sd.CustID == custDetails.CustID && (c.City == custDetails.CityID || c.State == custDetails.StateID || c.InterstCountry.Value == true)
                                        && c.ID != sd.CustID && sc.CustID == c.ID
                                        select new State
                                        {

                                            StateID = CT.StateID,
                                            StateName = CT.StateName

                                        }).Distinct().ToList();


                    return StateEnquiryList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<City> GetLoadEnquiry_CitylistOnePlus(CustDetails custDetails)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<City> CityEnquiryList = new List<City>();
                    CityEnquiryList = (from sd in dbContext.tblselectedDealers
                                       join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                       join c in dbContext.tblCustomerDetails on sdd.CustID equals c.ID
                                       join ic in dbContext.tblItemCategories on sd.ProductID equals ic.ID
                                       join cc in dbContext.tblChildCategories on ic.ChildCategoryID equals cc.ID
                                       join sc in dbContext.tblSubCategoryProductWithCusts on cc.SubCategoryId equals sc.SubCategoryId
                                       join Bt in dbContext.tblBusinessTypewithCusts on c.ID equals Bt.CustID
                                       join Bts in dbContext.tblBusinessTypes on Bt.BusinessTypeID equals Bts.ID
                                       join CT in dbContext.tblStateWithCities on c.City equals CT.StatewithCityID
                                       where sd.CustID == custDetails.CustID && (c.City == custDetails.CityID || c.State == custDetails.StateID || c.InterstCountry.Value == true)
                                        && c.ID != sd.CustID && sc.CustID == c.ID
                                       select new City
                                       {

                                           StateWithCityID = CT.StatewithCityID,
                                           VillageLocalityName = CT.VillageLocalityName
                                       }).Distinct().ToList();


                    return CityEnquiryList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public List<CustIDS> FilterEnquiryList(CustDetails custDetails)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<CustIDS> custIDs = new List<CustIDS>();
                    custIDs = (from sd in dbContext.tblselectedDealers
                               join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                               join c in dbContext.tblCustomerDetails on sdd.CustID equals c.ID
                               join ic in dbContext.tblItemCategories on sd.ProductID equals ic.ID
                               join cc in dbContext.tblChildCategories on ic.ChildCategoryID equals cc.ID
                               join sc in dbContext.tblSubCategoryProductWithCusts on new { cc.SubCategoryId } equals new { sc.SubCategoryId }
                               join Bt in dbContext.tblBusinessTypewithCusts on c.ID equals Bt.CustID
                               join Bts in dbContext.tblBusinessTypes on Bt.BusinessTypeID equals Bts.ID
                               where sd.CustID == custDetails.CustID
                               //&& (c.City == custDetails.CityID || c.State == custDetails.StateID || c.InterstCountry.Value == true)
                               //&& c.CustID != sd.CustID && sc.CustID == c.CustID && Bts.BusinessTypeID == custDetails.BusinessTypeID
                               select new CustIDS
                               {
                                   CustID = sdd.CustID.Value,
                                   FirmName = c.FirmName,
                                   StateID = c.State.Value,
                                   CityId = c.City,
                                   businessTypes = (from b in dbContext.tblBusinessTypewithCusts
                                                    where b.CustID == custDetails.CustID
                                                    select new BusinessTypes
                                                    {
                                                        BusinessTypeID = b.BusinessTypeID,
                                                    }).ToList(),
                               }).ToList();

                    if (custDetails.States != null && custDetails.States.Count() > 0)
                    {
                        int[] StatesArray = custDetails.States.Select(b => b.StateID.Value).ToArray();
                        custIDs = custIDs.Where(c => StatesArray.Contains(c.StateID)).ToList();
                    }
                    if (custDetails.Cities != null && custDetails.Cities.Count() > 0)
                    {
                        int[] CitiesArray = custDetails.Cities.Select(b => b.StateWithCityID.Value).ToArray();
                        custIDs = custIDs.Where(c => CitiesArray.Contains(c.CityId.Value)).ToList();
                    }
                    if (custDetails.businessTypes != null && custDetails.businessTypes.Count() > 0)
                    {
                        int[] BTarray = custDetails.businessTypes.Select(b => b.BusinessTypeID).ToArray();
                        custIDs = custIDs.Where(c => c.businessTypes.Any(cd => BTarray.Contains(cd.BusinessTypeID))).ToList();
                    }

                    custIDs = (from c in custIDs
                               select new CustIDS
                               {
                                   CustID = c.CustID,
                                   FirmName = c.FirmName,
                               }).Distinct().ToList();

                    custIDs = custIDs.GroupBy(i => i.CustID).Select(i => i.FirstOrDefault()).ToList();
                    return custIDs;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<EnquiryTypes> GetEnquiryTypes()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<EnquiryTypes> enquiryTypes = new List<EnquiryTypes>();
                    enquiryTypes = (from e in dbContext.tblselectedDealers
                                    where e.EnquiryType != null
                                    select new EnquiryTypes
                                    {
                                        EnquiryType = e.EnquiryType,
                                    }).Distinct().ToList();


                    return enquiryTypes;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public bool SavePushNotifications(int CustID, PushNotifications notification, int UserID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    tblPushNotification tblPushNotification = new tblPushNotification();
                    tblPushNotification.CustID = CustID;
                    tblPushNotification.NotificationDate = notification.NotificationDate;
                    tblPushNotification.CreatedBy = UserID;
                    tblPushNotification.CreatedDate = DateTimeNow;
                    tblPushNotification.PushNotification = notification.PushNotification;
                    tblPushNotification.Title = notification.Title;
                    dbContext.tblPushNotifications.Add(tblPushNotification);
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return false;
            }
        }
        public bool SavePushNotificationsList(int[] CustIDs, PushNotifications notification, int UserID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    //List<int> CustIDs = (from c in dbContext.tblCustomerDetails
                    //                     where RegistrationIDs.Equals(c.DeviceID.ToString())
                    //                     select c.CustID).ToList();

                    foreach (int i in CustIDs)
                    {
                        tblPushNotification tblPushNotification = new tblPushNotification();
                        tblPushNotification.CustID = i;
                        tblPushNotification.Title = notification.Title;
                        tblPushNotification.NotificationDate = notification.NotificationDate;
                        tblPushNotification.PushNotification = notification.PushNotification;
                        tblPushNotification.CreatedBy = UserID;
                        tblPushNotification.CreatedDate = DateTimeNow;
                        tblPushNotification.CategoryName = notification.CategoryName;
                        tblPushNotification.ImageURL = notification.ImageURL;
                        dbContext.tblPushNotifications.Add(tblPushNotification);
                        dbContext.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {

                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return false;
            }
        }
        public List<PushNotifications> GetPushNotifications(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE).Date;
                    var pushNotifications = (from p in dbContext.tblPushNotifications
                                             where p.CustID == CustID
                                             && DbFunctions.TruncateTime(p.NotificationDate.Value) == DateTimeNow
                                             select p).AsQueryable();
                    List<PushNotifications> pushNotifications1 = (from p in pushNotifications
                                                                  select new PushNotifications
                                                                  {
                                                                      ID = p.ID,
                                                                      CustID = p.CustID,
                                                                      CreatedBy = p.CreatedBy,
                                                                      CreatedDate = p.CreatedDate,
                                                                      NotificationDate = p.NotificationDate,
                                                                      PushNotification = p.PushNotification,
                                                                      NotificationDateStr = p.NotificationDate.ToString(),
                                                                      Title = p.Title == null ? string.Empty : p.Title,
                                                                      CategoryName = p.CategoryName == null ? string.Empty : p.CategoryName,
                                                                      ImageURL = p.ImageURL,
                                                                  }).ToList();
                    //dbContext.tblPushNotifications.ToList().FindAll(p => p.CustID == CustID && DbFunctions.TruncateTime(p.NotificationDate.Value) == DateTimeNow).ToList();
                    pushNotifications1.ToList().ForEach(p => p.NotificationDateStr = p.NotificationDate.Value.ToString("HH:mm:ss tt"));
                    return pushNotifications1.OrderByDescending(x => x.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public tblTAndC GetTermsAndConditions()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    var termsAndConditions = dbContext.tblTAndCs.Where(t => t.IsCurrentVersion == true).FirstOrDefault();
                    termsAndConditions.TAndC = HttpUtility.HtmlEncode(termsAndConditions.TAndC);
                    return termsAndConditions;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
    }
}
