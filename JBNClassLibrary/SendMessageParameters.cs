using System;
using System.Collections.Generic;

namespace JBNClassLibrary
{

    public class SendMessageParameters
    {
        public int QueryId { get; set; }
        public int? CustID { get; set; }
        public string FirmName { get; set; }
        public string MobileNumber { get; set; }
        public string VillageLocalityname { get; set; }
        public string BusinessDemand { get; set; }
        public string PurposeBusiness { get; set; }
        public string Requirements { get; set; }
        public string Image { get; set; }
        public string Image2 { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public List<MessageList> MessageList { get; set; }
        public int IsSender { get; set; }
    }
    public class MessageList
    {
        public int ID { get; set; }
        public int? QueryId { get; set; }
        public int? CustID { get; set; }
        public string CustomerName { get; set; }
        public string Message { get; set; }
        public int? IsDealer { get; set; }
        public int? IsCustomer { get; set; }
        public string Image { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? IsRead { get; set; }
        public int? IsArchived { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string SenderProfileImg { get; set; }
        public string ReceiverProfileImg { get; set; }
    }
}
