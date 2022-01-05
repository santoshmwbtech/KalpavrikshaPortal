using System.Net.Mail;

namespace JBNClassLibrary
{
    public class SendMailParameters
    {
        public int CustID { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public string FirmName { get; set; }
        public string EmailID { get; set; }
        public string MobileNumber { get; set; }
        public string CityName { get; set; }
        public string SubCategoryName { get; set; }
        public string ChildCategoryName { get; set; }
        public string ItemName { get; set; }
        public AttachmentCollection MailAttachment { get; set; }
    }
}
