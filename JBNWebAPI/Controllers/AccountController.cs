using JBNClassLibrary;
using JBNWebAPI.Logger;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace JBNWebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Accounts")]
    public class AccountController : ApiController
    {
        JBNDBClass jbndbclass = new JBNDBClass();
        private mwbtDealerEntities db = new mwbtDealerEntities();
        readonly Logger.LoggerBlock loggerBlock = new Logger.LoggerBlock();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        int IsLogWrite = Convert.ToInt32(ConfigurationManager.AppSettings["IsLogWrite"].ToString());
        
        [HttpPost]
        [ResponseType(typeof(CustDetails))]
        [Route("ValidateCredentials/{MobileNo}/{Password}/{DeviceID}")]
        public IHttpActionResult ValidateCredentials(string MobileNo, string Password, string DeviceID)
        {
            try
            {
                if(IsLogWrite == 1)
                    Helper.TransactionLog(MobileNo + "**" + Password, 1);

                tblCustomerDetail customer = jbndbclass.ValidateCredentials(MobileNo, Password, DeviceID);

                if (customer == null)
                {
                    //loggerBlock.LogWriter.Write("Custid Not Found");
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
                }
                else if(customer.IsActive == false)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable));
                }

                CustDetails customerDetails = new CustDetails();
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                DateTime DateNow = DateTimeNow;
                DateTime ExpiryDate = Convert.ToDateTime(ConfigurationManager.AppSettings["ExpiryDate"].ToString());

                if (DateNow > ExpiryDate)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.MethodNotAllowed, customerDetails));
                }

                //Get full screen ad URL
                DLAdvertisements dLAdvertisements = new DLAdvertisements();
                var Advertisement = dLAdvertisements.GetFullScreenAdURL(customer.ID);

                if(Advertisement != null)
                {
                    customerDetails.AdUserID = Advertisement.CustID.Value;
                    customerDetails.AdFirmName = Advertisement.AdFirmName;
                    customerDetails.StateID = Advertisement.StateID;
                    customerDetails.StateName = Advertisement.StateName;
                    customerDetails.CityID = Advertisement.CityID;
                    customerDetails.VillageLocalityName = Advertisement.VillageLocalityName;
                    customerDetails.ChildCategoryId = Advertisement.ChildCategoryId;
                    customerDetails.ChildCategoryName = Advertisement.ChildCategoryName;
                    customerDetails.FullScreenAdURL = Advertisement.FullScreenAdURL;
                    customerDetails.businessTypes = Advertisement.businessTypes;
                }

                string URL = GetBaseUrl();
                Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                if (!string.IsNullOrEmpty(customerDetails.FullScreenAdURL))
                {
                    Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(customerDetails.FullScreenAdURL));
                    customerDetails.FullScreenAdURL = URL + customerDetails.FullScreenAdURL;
                }

                customerDetails.CustID = customer.ID;
                customerDetails.IsRegistered = customer.IsRegistered;
                customerDetails.UserType = customer.UserType;
                customerDetails.Password = customer.Password;
                customerDetails.SMSOTP = customer.SMSOTP;
                customerDetails.IsOTPVerified = customer.IsOTPVerified ?? false;
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customerDetails);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 0);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customerDetails));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }
        public string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        [HttpPost]
        [ResponseType(typeof(tblCustomerDetail))]
        [Route("RegisterUser")]
        public IHttpActionResult RegisterUser([FromBody] CustomerDetails RegistrationDetails)
        {

            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(RegistrationDetails);
            if (IsLogWrite == 1)
                Helper.TransactionLog(json1, 1);
            try
            {
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                RegistrationDetails.CreatedByID = 1;
                RegistrationDetails.CreatedDate = DateTimeNow;
                RegistrationDetails.IsRegistered = 0;
                RegistrationDetails.IsOTPVerified = false;
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 0);
                CustomerDetails user = jbndbclass.RegisterUser(RegistrationDetails);

                if (user == null)
                {
                    //return Conflict();
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict));
                }
                else if(user.DisplayMessage == "conflict")
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, user));
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [ResponseType(typeof(CustomerDetails))]
        [Route("GetCustomerDetails/{CustID}")]
        public IHttpActionResult GetCustomerDetails(int CustID)
        {
            try
            {
                CustomerDetails customerDetails = new CustomerDetails();
                customerDetails = jbndbclass.GetCustomerDetails(CustID);
                //string mesg = jbndbclass.CustomerDetails(CustID);

                if (!string.IsNullOrEmpty(customerDetails.UserImage))
                {
                    string URL = GetBaseUrl();
                    Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                    Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(customerDetails.UserImage));
                    customerDetails.UserImage = URL + customerDetails.UserImage;
                }

                if (customerDetails == null)
                {
                    return NotFound();
                }

                //return Ok(tblCustomerDetails);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customerDetails));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [ResponseType(typeof(void))]
        //[Route("PutCustomerDetails/{id}")]
        public IHttpActionResult PutCustomerDetails(int id, [FromBody] CustomerDetails tblCustomerDetails)
        {
            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(tblCustomerDetails);
            if (IsLogWrite == 1)
                Helper.TransactionLog(json1, 1);

            if (id != tblCustomerDetails.CustID)
            {
                return BadRequest();
            }
            try
            {
                if (jbndbclass.UpdateCustomerDetails(id, tblCustomerDetails))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, tblCustomerDetails));
                else
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "User Not Found"));

            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //Added by Santosh on 21st June 2021 -- Profile Image and Is BGS Member added
        [HttpPost]
        [ResponseType(typeof(tblCustomerDetail))]
        [Route("UserProfileUpdateWithImage")]
        public async Task<IHttpActionResult> UserProfileUpdateWithImage()
        {
            try
            {
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                byte[] fileBytes = null;
                var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
                var json = await filesReadToProvider.Contents[0].ReadAsStringAsync();
                if (filesReadToProvider.Contents.Count > 1)
                {
                    fileBytes = await filesReadToProvider.Contents[1].ReadAsByteArrayAsync();
                }
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json, 1);

                CustomerDetails tblCustomer = JsonConvert.DeserializeObject<CustomerDetails>(json);

                if (fileBytes != null && fileBytes.Length > 0)
                {
                    string dateNow = DateTimeNow.ToString();
                    dateNow = dateNow.Replace(" ", string.Empty);
                    dateNow = dateNow.Replace(":", "_");
                    dateNow = dateNow.Replace("-", "_");
                    if (dateNow.Contains("/"))
                        dateNow = dateNow.Replace("/", "_");

                    Helper.CreateFolder(HttpContext.Current.Server.MapPath("~/ProfileImages/"));
                    string filePath = HttpContext.Current.Server.MapPath("~/ProfileImages/" + string.Format("profileimg_{0}.jpg", dateNow));
                    MemoryStream ms = new MemoryStream(fileBytes, 0, fileBytes.Length);
                    ms.Write(fileBytes, 0, fileBytes.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    image.Save(filePath);
                    string serverPath = ConfigurationManager.AppSettings["ProfileImagePath"] + string.Format("profileimg_{0}.jpg", dateNow);
                    tblCustomer.UserImage = serverPath;
                }

                if (jbndbclass.UpdateCustomerDetails(tblCustomer.CustID, tblCustomer))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, tblCustomer));
                else
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "User Not Found"));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(CustDetails))]
        [Route("SendOTP/{CustID}/{MobileNo}")]
        public IHttpActionResult SendOTP(int? CustID, string MobileNo)
        {
            try
            {
                if (IsLogWrite == 1)
                    Helper.TransactionLog(CustID + "**" + MobileNo, 1);
                CustDetails customer = jbndbclass.SendOTP(CustID, MobileNo);

                if (customer == null)
                {
                    //loggerBlock.LogWriter.Write("Custid Not Found");
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
                }


                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 0);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customer));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }

        [HttpPost]
        [ResponseType(typeof(CustDetails))]
        [Route("ApproveOTP/{CustID}/{MobileNumber}/{OTP}")]
        public IHttpActionResult ApproveOTP(int? CustID, string MobileNumber, string OTP)
        {
            try
            {
                if (IsLogWrite == 1)
                    Helper.TransactionLog(CustID + "**" + MobileNumber + "**" + OTP, 1);
                CustDetails customer = jbndbclass.ApproveOTP(CustID, MobileNumber, OTP);

                if (customer == null)
                {
                    //loggerBlock.LogWriter.Write("Custid Not Found");
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
                }


                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 0);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customer));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }

        [HttpGet]
        [ResponseType(typeof(tblVideo))]
        [Route("GetVideos")]
        public IHttpActionResult GetVideos()
        {
            try
            {
                List<tblVideo> Videos = new List<tblVideo>();
                Videos = jbndbclass.GetVideos();
                if (Videos == null)
                {
                    return NotFound();
                }
                //return Ok(sendMessageParameters);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Videos));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }        

        [HttpGet]
        [ResponseType(typeof(CustDetails))]
        [Route("GetCustomerOfDeviceID/{DeviceID}")]
        public IHttpActionResult GetCustomerOfDeviceID(string DeviceID)
        {
            try
            {
                if (IsLogWrite == 1)
                    Helper.TransactionLog(DeviceID, 1);
                CustDetails customer = jbndbclass.GetCustomerOfDeviceID(DeviceID);

                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 0);

                if (customer == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
                }

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customer));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }

        [HttpPost]
        [ResponseType(typeof(CustDetails))]
        [Route("SendOTPToMobileNumber/{DeviceID}/{MobileNumber}")]
        public IHttpActionResult SendOTPToMobileNumber(string DeviceID, string MobileNumber)
        {
            try
            {
                if (IsLogWrite == 1)
                    Helper.TransactionLog(DeviceID + "**" + MobileNumber, 1);
                CustDetails customer = jbndbclass.SendOTPToMobileNumber(DeviceID, MobileNumber);
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 0);
                if (customer == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
                }
                if(customer.OTPStatus.ToLower() == "mobile number is already exists")
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, customer));
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customer));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }
        }

        [HttpPost]
        [ResponseType(typeof(CustDetails))]
        [Route("ResendSendOTPToMobileNumber/{DeviceID}/{MobileNumber}")]
        public IHttpActionResult ResendSendOTPToMobileNumber(string DeviceID, string MobileNumber)
        {
            try
            {
                Helper.TransactionLog(DeviceID + "**"+ MobileNumber, 1);
                
                CustDetails customer = jbndbclass.ResendSendOTPToMobileNumber(DeviceID, MobileNumber);
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
                Helper.TransactionLog(json1, 0);
                if (customer == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customer));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(tblQuestion))]
        [Route("GetQuestion")]
        public IHttpActionResult GetQuestion()
        {
            try
            {
                tblQuestion tblQuestion = jbndbclass.GetQuestion();

                if (tblQuestion == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
                }

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, tblQuestion));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(CustDetails))]
        [Route("AnswerQuestion/{DeviceID}/{MobileNumber}/{QuestionID}/{AnswerForQuestion}")]
        public IHttpActionResult AnswerQuestion(string DeviceID, string MobileNumber, int QuestionID, string AnswerForQuestion)
        {
            try
            {
                Helper.TransactionLog(DeviceID + "**"+ MobileNumber + "**" + QuestionID + "**" + AnswerForQuestion, 1);
                CustDetails customer = jbndbclass.AnswerQuestion(DeviceID, MobileNumber, QuestionID, AnswerForQuestion);
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
                Helper.TransactionLog(json1, 0);
                if (customer == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
                }

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customer));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(Boolean))]
        [Route("GetCustomerByMobileNumber/{MobileNumber}")]
        public IHttpActionResult GetCustomerByMobileNumber(string MobileNumber)
        {
            try
            {
                tblCustomerDetail tblCustomerDetail = new tblCustomerDetail();
                tblCustomerDetail = jbndbclass.GetCustomerByMobileNumber(MobileNumber);
                if (tblCustomerDetail == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, false));
                }
                //return Ok(sendMessageParameters);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, true));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(Boolean))]
        [Route("Getcustomerbymobilenumber_New/{mobilenumber}")]
        public IHttpActionResult Getcustomerbymobilenumber_New(string mobilenumber)
        {
            try
            {
                tblCustomerDetail_Questions customerdetails = new tblCustomerDetail_Questions();
                customerdetails = jbndbclass.Getcustomerbymobilenumber_New(mobilenumber);
                if (customerdetails == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, customerdetails));
                }
                //return ok(sendmessageparameters);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customerdetails));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //Block number after 3 wrong answers
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(Boolean))]
        [Route("BlockMobileNumber/{MobileNumber}")]
        public IHttpActionResult BlockMobileNumber(string MobileNumber)
        {
            try
            {
                Helper.TransactionLog("Block Mobile Number Request" + MobileNumber, 1);

                bool Result = jbndbclass.BlockMobileNumber(MobileNumber);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }
        }

        //Check Status
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(string))]
        [Route("CheckStatus/{MobileNumber}")]
        public IHttpActionResult CheckStatus(string MobileNumber)
        {
            try
            {
                Helper.TransactionLog("Check Status" + MobileNumber, 1);

                string Result = jbndbclass.CheckStatus(MobileNumber);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }
        }

        
        //[HttpPost]
        //[ResponseType(typeof(CustDetails))]
        //[Route("ChangePassword/{MobileNo}/{Password}/{SMSOTP}")]
        ////ChangePassword
        //public IHttpActionResult ChangePassword(string MobileNo, string Password, string SMSOTP)
        //{
        //    try
        //    {
        //        if (IsLogWrite == 1)
        //            Helper.TransactionLog(MobileNo + "**" + Password, 1);

        //        tblCustomerDetail customer = jbndbclass.ChangePassword(MobileNo, Password, SMSOTP);

        //        if (customer == null)
        //        {

        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
        //        }
        //        else if (customer.IsActive == false)
        //        {
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable));
        //        }

        //        CustDetails customerDetails = new CustDetails();
        //        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        //        DateTime DateNow = DateTimeNow;
        //        DateTime ExpiryDate = Convert.ToDateTime(ConfigurationManager.AppSettings["ExpiryDate"].ToString());

        //        if (DateNow > ExpiryDate)
        //        {
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.MethodNotAllowed, customerDetails));
        //        }

        //        //Get full screen ad URL
        //        DLAdvertisements dLAdvertisements = new DLAdvertisements();
        //        customerDetails.FullScreenAdURL = dLAdvertisements.GetFullScreenAdURL(customer.CustID);

        //        string URL = GetBaseUrl();
        //        Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
        //        if (!string.IsNullOrEmpty(customerDetails.FullScreenAdURL))
        //        {
        //            Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(customerDetails.FullScreenAdURL));
        //            customerDetails.FullScreenAdURL = URL + customerDetails.FullScreenAdURL;
        //        }

        //        customerDetails.CustID = customer.CustID;
        //        customerDetails.IsRegistered = customer.IsRegistered;
        //        customerDetails.UserType = customer.UserType;
        //        customerDetails.Password = customer.Password;
        //        customerDetails.SMSOTP = customer.SMSOTP;
        //        customerDetails.IsOTPVerified = customer.IsOTPVerified ?? false;
        //        string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customerDetails);
        //        if (IsLogWrite == 1)
        //            Helper.TransactionLog(json1, 0);
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customerDetails));
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
        //        return null;
        //    }
        //}

        //[HttpPost]
        //[ResponseType(typeof(CustDetails))]
        //[Route("ValidateUserForgotPwd/{MobileNo}")]
        //public IHttpActionResult ValidateUserForgotPwd(string MobileNo)
        //{
        //    {
        //        try
        //        {
        //            if (IsLogWrite == 1)
        //                Helper.TransactionLog(MobileNo, 1);

        //            CustDetails customer = jbndbclass.ValidateUserForgotPwd(MobileNo);

        //            if (customer == null)
        //            {
        //                //loggerBlock.LogWriter.Write("Custid Not Found");
        //                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
        //            }
        //            else if (customer.IsRegistered == 0)
        //            {
        //                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable));
        //            }

        //            CustDetails customerDetails = new CustDetails();
        //            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        //            DateTime DateNow = DateTimeNow;
        //            DateTime ExpiryDate = Convert.ToDateTime(ConfigurationManager.AppSettings["ExpiryDate"].ToString());

        //            if (DateNow > ExpiryDate)
        //            {
        //                return ResponseMessage(Request.CreateResponse(HttpStatusCode.MethodNotAllowed, customerDetails));
        //            }

        //            customerDetails.CustID = customer.CustID;
        //            customerDetails.IsRegistered = customer.IsRegistered;
        //            customerDetails.UserType = customer.UserType;
        //            customerDetails.Password = customer.Password;
        //            customerDetails.SMSOTP = customer.SMSOTP;
        //            customerDetails.IsOTPVerified = customer.IsOTPVerified ?? false;
        //            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customerDetails);
        //            if (IsLogWrite == 1)
        //                Helper.TransactionLog(json1, 0);
        //            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customerDetails));
        //        }
        //        catch (Exception ex)
        //        {
        //            Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
        //            return null;
        //        }

        //    }

        //}

        [HttpPost]
        [ResponseType(typeof(CustDetails))]
        [Route("ForgotRest_Password/{MobileNo}/{Password}/{SMSOTP}")]
        //ChangePassword
        public IHttpActionResult ForgotRest_Password(string MobileNo, string Password, string SMSOTP)
        {
            try
            {
                if (IsLogWrite == 1)
                    Helper.TransactionLog(MobileNo + "**" + Password, 1);

                tblCustomerDetail customer = jbndbclass.ForgotRest_Password(MobileNo, Password, SMSOTP);

                if (customer == null)
                {

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
                }
                else if (customer.IsActive == false)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable));
                }

                CustDetails customerDetails = new CustDetails();
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                DateTime DateNow = DateTimeNow;
                DateTime ExpiryDate = Convert.ToDateTime(ConfigurationManager.AppSettings["ExpiryDate"].ToString());

                if (DateNow > ExpiryDate)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.MethodNotAllowed, customerDetails));
                }

                //Get full screen ad URL
                //DLAdvertisements dLAdvertisements = new DLAdvertisements();
                //customerDetails.FullScreenAdURL = dLAdvertisements.GetFullScreenAdURL(customer.CustID);

                //string URL = GetBaseUrl();
                //Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                //if (!string.IsNullOrEmpty(customerDetails.FullScreenAdURL))
                //{
                //    Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(customerDetails.FullScreenAdURL));
                //    customerDetails.FullScreenAdURL = URL + customerDetails.FullScreenAdURL;
                //}

                customerDetails.CustID = customer.ID;
                customerDetails.IsRegistered = customer.IsRegistered;
                customerDetails.UserType = customer.UserType;
                customerDetails.Password = customer.Password;
                customerDetails.SMSOTP = customer.SMSOTP;
                customerDetails.IsOTPVerified = customer.IsOTPVerified ?? false;
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customerDetails);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 0);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customerDetails));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return null;
            }
        }


        [HttpPost]
        [ResponseType(typeof(CustDetails))]
        [Route("ChangePassword/{MobileNo}/{OldPassword}/{NewPassowrd}")]
        //ChangePassword
        public IHttpActionResult Change_Password(string MobileNo, string OldPassword, string NewPassowrd)
        {
            try
            {
                if (IsLogWrite == 1)
                    Helper.TransactionLog(MobileNo + "**" + OldPassword, 1);

                tblCustomerDetail customer = jbndbclass.ChangePassword(MobileNo, OldPassword, NewPassowrd);

                if (customer == null)
                {

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
                }
                else if (customer.IsActive == false)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable));
                }

                CustDetails customerDetails = new CustDetails();
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                DateTime DateNow = DateTimeNow;
                DateTime ExpiryDate = Convert.ToDateTime(ConfigurationManager.AppSettings["ExpiryDate"].ToString());

                if (DateNow > ExpiryDate)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.MethodNotAllowed, customerDetails));
                }

                //Get full screen ad URL
                //DLAdvertisements dLAdvertisements = new DLAdvertisements();
                //customerDetails.FullScreenAdURL = dLAdvertisements.GetFullScreenAdURL(customer.CustID);

                //string URL = GetBaseUrl();
                //Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                //if (!string.IsNullOrEmpty(customerDetails.FullScreenAdURL))
                //{
                //    Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(customerDetails.FullScreenAdURL));
                //    customerDetails.FullScreenAdURL = URL + customerDetails.FullScreenAdURL;
                //}

                customerDetails.CustID = customer.ID;
                customerDetails.IsRegistered = customer.IsRegistered;
                customerDetails.UserType = customer.UserType;
                customerDetails.Password = customer.Password;
                customerDetails.SMSOTP = customer.SMSOTP;
                customerDetails.IsOTPVerified = customer.IsOTPVerified ?? false;
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customerDetails);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 0);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customerDetails));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }
        }

        [HttpPost]
        [ResponseType(typeof(CustDetails))]
        [Route("ValidateUserForgotPwd/{MobileNo}")]
        public IHttpActionResult ValidateUserForgotPwd(string MobileNo)
        {
            {
                try
                {
                    if (IsLogWrite == 1)
                        Helper.TransactionLog(MobileNo, 1);

                    CustDetails customer = jbndbclass.ValidateUserForgotPwd(MobileNo);

                    if (customer == null)
                    {
                        //loggerBlock.LogWriter.Write("Custid Not Found");
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
                    }
                    else if (customer.IsActive == false)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable));
                    }

                    CustDetails customerDetails = new CustDetails();
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    DateTime DateNow = DateTimeNow;
                    DateTime ExpiryDate = Convert.ToDateTime(ConfigurationManager.AppSettings["ExpiryDate"].ToString());

                    if (DateNow > ExpiryDate)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.MethodNotAllowed, customerDetails));
                    }

                    customerDetails.CustID = customer.CustID;
                    customerDetails.IsRegistered = customer.IsRegistered;
                    customerDetails.UserType = customer.UserType;
                    customerDetails.Password = customer.Password;
                    customerDetails.SMSOTP = customer.SMSOTP;
                    customerDetails.IsOTPVerified = customer.IsOTPVerified ?? false;
                    string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(customerDetails);
                    if (IsLogWrite == 1)
                        Helper.TransactionLog(json1, 0);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, customerDetails));
                }
                catch (Exception ex)
                {
                    Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError));
                }
            }
        }

        [HttpPost]
        [ResponseType(typeof(Root))]
        [Route("GenerateCheckSum/{OrderID}")]
        public IHttpActionResult GenerateCheckSum(string OrderID)
        {
            /* initialize an array */
            Dictionary<string, string> paytmParams = new Dictionary<string, string>();

            string MerchantID = ConfigurationManager.AppSettings["PayTMTestMID"].ToString();
            string MerchantKey = ConfigurationManager.AppSettings["PayTMTestMKey"].ToString();

            /* add parameters in Array */
            paytmParams.Add("MID", MerchantID);
            paytmParams.Add("ORDER_ID", OrderID);

            /*Generate checksum by parameters we have
            */
            String paytmChecksum = Paytm.Checksum.generateSignature(paytmParams, MerchantKey);
            bool verifySignature = Paytm.Checksum.verifySignature(paytmParams, MerchantKey, paytmChecksum);
            //PayTM payTM = new PayTM { paytmChecksum = paytmChecksum, verifySignature = verifySignature };

            Root root = new Root();
            Head head = new Head();
            Body body = new Body();
            ResultInfo resultInfo = new ResultInfo();
            head.signature = paytmChecksum;
            root.head = head;
            body.resultInfo = resultInfo;
            root.body = body;

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, root));
        }

        [HttpGet]
        [ResponseType(typeof(tblTAndC))]
        [Route("GetTermsAndConditions")]
        public IHttpActionResult GetTermsAndConditions()
        {
            try
            {
                tblTAndC termsAndConditions = new tblTAndC();
                termsAndConditions = jbndbclass.GetTermsAndConditions();
                if (termsAndConditions == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, termsAndConditions));
                }
                //return ok(sendmessageparameters);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, termsAndConditions));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
    }
}
