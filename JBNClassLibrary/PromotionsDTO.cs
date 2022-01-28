using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JBNClassLibrary
{
    public class PromotionsDTO
    {
        public string NotificationType { get; set; }
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
        public int SMSTemplateID { get; set; }
    }
}
