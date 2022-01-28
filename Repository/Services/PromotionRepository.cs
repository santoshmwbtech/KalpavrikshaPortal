using JBNClassLibrary;
using JBNWebAPI.Logger;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Services
{
    public class PromotionRepository : IPromotionRepository
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public async Task<string> Promotion(PromotionsDTO promotionsDTO, List<Attachment> MailAttachments, string ImageURL, int UserID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    var allCustomers = dbContext.tblCustomerDetails.Where(c => !string.IsNullOrEmpty(c.MobileNumber));
                    var customerList = new List<CustomerDetails>();
                    if (!string.IsNullOrEmpty(promotionsDTO.NotificationType))
                    {
                        if (promotionsDTO.NotificationType.ToLower() == "all")
                        {
                            customerList = (from c in allCustomers
                                            select new CustomerDetails
                                            {
                                                CustID = c.ID,
                                                MobileNumber = c.MobileNumber,
                                                EmailID = c.EmailID,
                                                DeviceID = c.DeviceID,
                                            }).ToList();
                        }
                        else if (promotionsDTO.NotificationType.ToLower() == "kyc completed")
                        {
                            customerList = (from c in allCustomers
                                            where c.IsRegistered == 1
                                            select new CustomerDetails
                                            {
                                                CustID = c.ID,
                                                MobileNumber = c.MobileNumber,
                                                EmailID = c.EmailID,
                                                DeviceID = c.DeviceID,
                                            }).ToList();
                        }
                        else if (promotionsDTO.NotificationType.ToLower() == "kyc not completed")
                        {
                            customerList = (from c in allCustomers
                                            where c.IsRegistered == 0
                                            select new CustomerDetails
                                            {
                                                CustID = c.ID,
                                                MobileNumber = c.MobileNumber,
                                                EmailID = c.EmailID,
                                                DeviceID = c.DeviceID,
                                            }).ToList();
                        }
                    }

                    string Result = string.Empty;
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    if (promotionsDTO.IsEmail == true)
                    {
                        string Bcc = string.Empty;
                        List<string> bccList = customerList.Where(c => !string.IsNullOrEmpty(c.EmailID)).Select(c => c.EmailID).ToList();

                        string ToEmailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                        string FromMailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                        string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
                        string MailServerHost = ConfigurationManager.AppSettings["MailServerHost"].ToString();
                        string SendingPort = ConfigurationManager.AppSettings["SendingPort"].ToString();
                        //string APKPath = ConfigurationManager.AppSettings["APKPath"].ToString();
                        string MailSubject = promotionsDTO.MailSubject;

                        await Helper.SendMailAsync(ToEmailID, FromMailID, promotionsDTO.MailBody, MailSubject, MailServerHost, MailPassword, SendingPort, bccList, MailAttachments);
                        Result = "Email Sent Successfully!!";
                    }
                    else if (promotionsDTO.IsSMS == true)
                    {
                        string MobileNumbers = string.Join(",", customerList.Select(c => c.MobileNumber));

                        string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                        string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                        string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                        Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, promotionsDTO.SMSBody, SenderID);
                    }
                    else if (promotionsDTO.IsNotification == true)
                    {
                        string[] Registration_Ids = customerList.Skip(500).Take(500).Where(c => !string.IsNullOrEmpty(c.DeviceID)).Select(c => c.DeviceID).ToArray();
                        int[] Cust_Ids = customerList.Skip(500).Take(500).Select(c => c.CustID).ToArray();
                        Notification notification = new Notification { Title = promotionsDTO.Title, Body = promotionsDTO.Body, NotificationDate = DateTimeNow, Image = ImageURL };

                        int itemsSent = 0, skipCount = 0, takenCount = 0;
                        int regCount = Registration_Ids.Count();
                        while (itemsSent < regCount)
                        {
                            string[] only999 = Registration_Ids.Skip(skipCount).Take(999).ToArray();
                            Helper.SendNotificationMultiple(only999, notification);
                            takenCount = Registration_Ids.Skip(skipCount).Take(999).Count();
                            itemsSent = itemsSent + takenCount;
                            skipCount = skipCount + takenCount;
                        }

                        //Helper.SendNotificationMultiple(Registration_Ids, notification);
                        PushNotifications pushNotifications = new PushNotifications()
                        {
                            Title = promotionsDTO.Title,
                            NotificationDate = DateTimeNow,
                            CategoryName = string.Empty,
                            ImageURL = ImageURL,
                            PushNotification = promotionsDTO.Body,
                        };
                        JBNDBClass jBNDBClass = new JBNDBClass();
                        jBNDBClass.SavePushNotificationsList(Cust_Ids, pushNotifications, UserID);
                        Result = "Success";
                    }
                    return Result;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException == null ? null : ex.InnerException, ex.StackTrace);
                return "Error!! Please contact administrator";
            }
        }
    }
}
