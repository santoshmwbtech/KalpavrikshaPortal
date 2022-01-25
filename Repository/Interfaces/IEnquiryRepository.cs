using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JBNClassLibrary.DLEnquiries;

namespace Repository.Interfaces
{
    public interface IEnquiryRepository
    {
        EnquiryListWithTotals GetEnquiries(EnquiriesDL enquiries);
        Task<List<ItemCategory>> GetItemCategories();
        Task<List<CustomerQueries>> GetCustomerEnquiries(CustomerEnquiries customerEnquiries);
    }
}
