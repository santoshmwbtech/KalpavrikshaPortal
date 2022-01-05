using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class Dashboard
    {
        public int? CustID { get; set; }
        public int? SentEnquiryMessagesCount { get; set; }
        public int? FavouriteEnquiryMessagesCount { get; set; }
        public int? ReceivedEnquiryMessagesCount { get; set; }
        public int RoleID { get; set; }
        public List<MainCategory> MainCategories { get; set; }
        public List<SubCat> SubCategories { get; set; }
        public List<childcategory> ChildCategories { get; set; }
        public List<ItemCategory> ItemCategories { get; set; }
        public int? NotificationsCount { get; set; }
    }
}
