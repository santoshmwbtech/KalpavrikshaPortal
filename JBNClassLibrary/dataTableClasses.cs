using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JBNClassLibrary
{
    public class Year
    {
        public string AdYear { get; set; }
    }
    public class Month
    {
        public string AdMonth { get; set; }
    }
    public class AdSearchOptions
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FirmName { get; set; }
        public int? CustID { get; set; }
        public int? AdvertisementMainID { get; set; }
        public int? AdvertisementAreaID { get; set; }
        public int? AdvertisementTypeID { get; set; }
        public string AdvertisementType { get; set; }
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public string AdvertisementArea { get; set; }
        public bool IsApproved { get; set; }
        public bool PaymentStatus { get; set; }
        public bool IsRejected { get; set; }
        public string ApprovedStatus { get; set; }
        public string StrPaymentStatus { get; set; }
        public string ExpiryDate { get; set; }
        public string CreatedDate { get; set; }
        public bool IsExpired { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDir { get; set; }
        public string recordsTotal { get; set; }
        public int pageSize { get; set; }
        public int skip { get; set; }
        public bool IsEmail { get; set; }
        public bool IsSMS { get; set; }
        public bool IsWhatsApp { get; set; }
        public bool IsNotification { get; set; }
        public string SMSBody { get; set; }
        [AllowHtml]
        public string MailBody { get; set; }
        public string MailSubject { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int SMSTemplateID { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
    }
    public class AdMainReport
    {
        public string sortColumn { get; set; }
        public string sortColumnDir { get; set; }
        public int recordsTotal { get; set; }
        public List<AdvertisementMain> AdsList { get; set; }        
    }

    public class SearchVM
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int? AdvertisementAreaID { get; set; }
        public int? AdvertisementTypeID { get; set; }
        public int? ProductID { get; set; }
        public int? StateID { get; set; }
        public int? DistrictID { get; set; }
        public int? CityID { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
    }
}
