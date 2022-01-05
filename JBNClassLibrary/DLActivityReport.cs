using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JBNClassLibrary
{
    public class ActivityReport
    {
        public int CustID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Mobile1 { get; set; }
        public string FirmName { get; set; }
        public string OwnerName { get; set; }
        public string EmailID { get; set; }
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool IsChecked { get; set; }
        public bool CheckAll { get; set; }
        public int? IsActive { get; set; }
        public string Status { get; set; }
        public string DeviceID { get; set; }
        public int? NumberofDays { get; set; }
        public int[] StateList { get; set; }
        public int[] CityList { get; set; }
    }
    public class PromoWithActivityReport
    {
        public List<ActivityReport> activityReports { get; set; }
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
        public string Message { get; set; }
        public bool IsNotification { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int ActiveCustomers { get; set; }
        public int InactiveCustomers { get; set; }
        public int TotalStates { get; set; }
        public int TotalCities { get; set; }
        public int SMSTemplateID { get; set; }

    }
    public class DLActivityReport
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public PromoWithActivityReport CustomerActivityReport(ActivityReport activityReport)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    PromoWithActivityReport promo = new PromoWithActivityReport();
                    List<ActivityReport> activityReports = new List<ActivityReport>();
                    activityReports = (from c in dbContext.tblCustomerDetails
                                       join sc in dbContext.tblStateWithCities on c.City equals sc.ID
                                       join s in dbContext.tblStates on c.State equals s.ID
                                       where c.IsActive == true
                                       select new ActivityReport
                                       {
                                           FirmName = c.FirmName,
                                           DeviceID = c.DeviceID,
                                           Mobile1 = c.MobileNumber,
                                           StateID = s.ID,
                                           CityID = sc.ID,
                                           CityName = sc.VillageLocalityName,
                                           StateName = s.StateName,
                                           OwnerName = c.CustName,
                                           EmailID = c.EmailID,
                                           FromDate = c.CreatedDate.ToString(),
                                           LastLoginDate = c.LastLoginDate.Value
                                       }).ToList();

                    if (activityReport.StateList != null && activityReport.StateList.Count() > 0)
                    {
                        activityReports = activityReports.Where(ac => activityReport.StateList.Contains(ac.StateID)).ToList();
                    }

                    if (activityReport.CityList != null && activityReport.CityList.Count() > 0)
                    {
                        activityReports = activityReports.Where(ac => activityReport.CityList.Contains(ac.CityID)).ToList();
                    }

                    DateTime DateNow = new DateTime();

                    if (activityReport.NumberofDays != 0 && activityReport.NumberofDays != null)
                        DateNow = DateTime.Now.AddDays(-activityReport.NumberofDays.Value);
                    else
                        DateNow = DateTime.Now.AddDays(-7);

                    //activityReports = activityReports.Where(a => Convert.ToDateTime(a.LastLoginDate) <= DateNow).Select(s => { s.Status = "Inactive"; return s; }).ToList();
                    //activityReports = activityReports.Where(a => Convert.ToDateTime(a.LastLoginDate) <= DateNow).ToList();

                    if (!string.IsNullOrEmpty(activityReport.FromDate) && !string.IsNullOrEmpty(activityReport.ToDate))
                    {
                        DateTime FromDate = DateTime.ParseExact(activityReport.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime ToDate = DateTime.ParseExact(activityReport.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        if (activityReport.NumberofDays != 0 && activityReport.NumberofDays != null)
                            DateNow = ToDate.AddDays(-activityReport.NumberofDays.Value);
                        else
                            DateNow = ToDate.AddDays(-7);


                        activityReports = activityReports.Where(i => Convert.ToDateTime(i.LastLoginDate).Date >= FromDate.Date && Convert.ToDateTime(i.LastLoginDate).Date <= ToDate.Date).ToList();
                    }

                    activityReports.ForEach(i => i.Status = i.LastLoginDate.Date >= DateNow.Date ? "Active" : "Inactive");

                    if (activityReport.IsActive == 2 && activityReport.IsActive != null)
                        activityReports = activityReports.Where(a => a.LastLoginDate.Date < DateNow.Date).ToList();

                    if (activityReport.IsActive == 1 && activityReport.IsActive != null)
                        activityReports = activityReports.Where(a => a.LastLoginDate.Date >= DateNow.Date).ToList();


                    promo.TotalStates = activityReports.Select(a => a.StateID).Distinct().Count();
                    promo.TotalCities = activityReports.Select(a => a.CityID).Distinct().Count();
                    promo.ActiveCustomers = activityReports.Where(a => a.Status == "Active").Distinct().Count();
                    promo.InactiveCustomers = activityReports.Where(a => a.Status == "Inactive").Distinct().Count();

                    promo.activityReports = activityReports;
                    return promo;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public string Promotion(PromoWithActivityReport promo, List<Attachment> MailAttachments, string ImageURL, int UserID)
        {
            try
            {
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                string Result = string.Empty;
                string AppName = ConfigurationManager.AppSettings["AppName"].ToString();
                if (promo.IsEmail == true)
                {
                    string Bcc = string.Empty;
                    List<CustomerDetails> bccList = new List<CustomerDetails>();

                    foreach (var item in promo.activityReports)
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
                    Result = "Success";
                }
                else if (promo.IsSMS == true)
                {
                    string MobileNumbers = string.Join(",", promo.activityReports.Where(b => b.IsChecked == true).Select(c => c.Mobile1));

                    string BaseURL = ConfigurationManager.AppSettings["PromoBaseURL"];
                    string APIKey = ConfigurationManager.AppSettings["PromoAPIKey"];
                    string SenderID = ConfigurationManager.AppSettings["PromotionalSenderID"];
                    Result = Helper.SendPromoMessage(BaseURL, APIKey, MobileNumbers, promo.SMSBody, SenderID);
                }
                else if (promo.IsNotification == true)
                {
                    string[] Registration_Ids = promo.activityReports.Where(b => b.IsChecked == true).Select(c => c.DeviceID).ToArray();
                    int[] Cust_Ids = promo.activityReports.Where(b => b.IsChecked == true).Select(c => c.CustID).ToArray();
                    Notification notification = new Notification { Title = promo.Title, Body = promo.Body, NotificationDate = DateTimeNow, Image = ImageURL };
                    Helper.SendNotificationMultiple(Registration_Ids, notification);
                    //Insert into dababase
                    JBNDBClass jBNDBClass = new JBNDBClass();
                    PushNotifications pushNotifications = new PushNotifications()
                    {
                        Title = promo.Title,
                        NotificationDate = DateTimeNow,
                        CategoryName = string.Empty,
                        ImageURL = ImageURL,
                        PushNotification = promo.Body,
                    };
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
    }
}
