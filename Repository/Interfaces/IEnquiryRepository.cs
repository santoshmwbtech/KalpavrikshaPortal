using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static JBNClassLibrary.DLEnquiries;

namespace Repository.Interfaces
{
    public interface IEnquiryRepository
    {
        EnquiryListWithTotals GetEnquiries(EnquiriesDL enquiries);
        Task<List<ItemCategory>> GetItemCategories();
        List<CustomerEnquiries> GetCustomerEnquiries(CustomerEnquiries customerEnquiries);
        Task<List<City>> GetEnquiryCities();
        CustomerConversations GetConversations(int CustID, int QueryId, int SenderID, string EnquiryType);
        EnquiryListWithTotals GetLaunchpadReport(EnquiriesDL search);
        Task<string> Promotion(EnquiryListWithTotals promo, List<Attachment> MailAttachments, string ImageURL, int UserID);
    }
}
