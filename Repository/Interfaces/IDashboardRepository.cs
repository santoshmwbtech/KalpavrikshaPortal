using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IDashboardRepository
    {
        Task<object> GetDashboardData();
        Task<Dashboard> GetCategoriesForApproval(int UserID);
        Task<object> GetAllProductsData();
    }
}
