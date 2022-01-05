using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class CategoryWiseRpt
    {

        public int? CustId { get; set; }
        public string CustName { get; set; }
        public Nullable<int> CategoryProductID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [Required]
        //[Display(Name = "FromDate :")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime FromDate { get; set; }
        [Required]
        //[Display(Name = "ToDate :")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ToDate { get; set; }
        public List<CategoryTypes> CategoryTypeWithCust { get; set; }
        public string TypeofCategory { get; set; }
    }
    public class CategoryTypes
    {
        public int ID { get; set; }
        public Nullable<int> CategoryProductID { get; set; }
        public Nullable<int> CustID { get; set; }
        public string MainCategoryName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }

    public class GetCategoryandCustomerDetails
    {
        mwbtDealerEntities dbcontext = new mwbtDealerEntities();
        public CategoryWiseRpt CatObj = new CategoryWiseRpt();
        public List<CategoryWiseRpt> CatList = new List<CategoryWiseRpt>();
        public List<CategoryTypes> GetCategory()
        {
            try
            {
                List<CategoryTypes> CatList = new List<CategoryTypes>();
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    CatList = (from cust in dbcontext.tblCategoryProducts
                               select new CategoryTypes()
                               {
                                   CategoryProductID = cust.CategoryProductID,
                                   MainCategoryName = cust.MainCategoryName
                               }).ToList();
                    return CatList;
                }
            }
            finally
            {

            }
        }
        public List<CategoryWiseRpt> GetDetails(CategoryWiseRpt categoryWiseRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if(categoryWiseRpt.CategoryProductID==null)
                    {
                        CatList = (from cat in dbcontext.tblCustomerDetails
                                   join d in dbcontext.tblCategoryProductWithCusts
                                   on cat.ID equals d.CustID
                                   select new CategoryWiseRpt
                                   {
                                       CustId = d.CustID,
                                       CustName = cat.CustName,
                                       CategoryProductID = d.CategoryProductID,
                                       CreatedDate = d.CreatedDate,

                                       CategoryTypeWithCust = (from b in dbcontext.tblCategoryProductWithCusts
                                                               join bt in dbcontext.tblCategoryProducts on b.CategoryProductID
                                                               equals bt.CategoryProductID
                                                               where b.CustID == cat.ID
                                                               select new CategoryTypes
                                                               {
                                                                   CategoryProductID = b.CategoryProductID,
                                                                   MainCategoryName = bt.MainCategoryName.Trim(),
                                                                   CreatedDate = b.CreatedDate
                                                               }).ToList(),
                                   }).ToList();

                                              

                        foreach (var item in CatList)
                        {
                            foreach (var item1 in item.CategoryTypeWithCust)
                            {
                                item.TypeofCategory += item1.MainCategoryName + ' ';
                            }
                        }

                        //CatList = CatList.GroupBy(ac => new
                        //{
                        //    ac.CategoryProductID,
                        //}).Select(ac => new CategoryWiseRpt
                        //{
                        //    CustId = ac.Select(i => i.CustId).FirstOrDefault(),
                        //    CustName = ac.Select(i => i.CustName).FirstOrDefault(),
                        //    TypeofCategory = ac.Select(i => i.TypeofCategory).FirstOrDefault(),
                        //    CategoryProductID = ac.Key.CategoryProductID,
                        //    CreatedDate = ac.Select(i => i.CreatedDate).FirstOrDefault(),
                        //}).ToList();
                        CatList = CatList.GroupBy(ac => new
                        {
                            ac.CustId,
                            ac.CustName
                        }).Select(ac => new CategoryWiseRpt
                        {
                            CustId = ac.Key.CustId,
                            CustName = ac.Key.CustName,
                            TypeofCategory = ac.Select(i => i.TypeofCategory).FirstOrDefault(),
                            CategoryProductID = ac.Select(i => i.CategoryProductID).FirstOrDefault(),
                            CreatedDate = ac.Select(i => i.CreatedDate).FirstOrDefault(),
                        }).OrderBy(i => i.CustName).ToList();

                        return CatList;
                    }
                    else
                    {
                        CatList = (from cat in dbcontext.tblCategoryProductWithCusts
                                   join cust in dbcontext.tblCustomerDetails
                                   on cat.CustID equals cust.ID
                                   select new CategoryWiseRpt
                                   {
                                       CustId = cat.CustID,
                                       CustName = cust.CustName,
                                       CategoryProductID = cat.CategoryProductID,
                                       CreatedDate = cat.CreatedDate,

                                       CategoryTypeWithCust = (from b in dbcontext.tblCategoryProductWithCusts
                                                               join bt in dbcontext.tblCategoryProducts on b.CategoryProductID
                                                               equals bt.CategoryProductID
                                                               where b.CustID == cat.CustID
                                                               select new CategoryTypes
                                                               {
                                                                   CategoryProductID = b.CategoryProductID,
                                                                   MainCategoryName = bt.MainCategoryName.Trim(),
                                                                   CreatedDate = b.CreatedDate
                                                               }).ToList(),
                                   }).ToList();


                        if (!string.IsNullOrEmpty(categoryWiseRpt.FromDate.ToString()) && !string.IsNullOrEmpty(categoryWiseRpt.ToDate.ToString()))
                        {
                            CatList = CatList.Where(i => Convert.ToDateTime(i.CreatedDate) >= categoryWiseRpt.FromDate && Convert.ToDateTime(i.CreatedDate) <= categoryWiseRpt.ToDate).ToList();
                        }

                        if (categoryWiseRpt.CategoryProductID > 0)
                            CatList = CatList.Where(i => i.CategoryProductID == categoryWiseRpt.CategoryProductID).ToList();

                        foreach (var item in CatList)
                        {
                            foreach (var item1 in item.CategoryTypeWithCust)
                            {
                                item.TypeofCategory += item1.MainCategoryName + ' ';
                            }
                        }

                        //CatList = CatList.GroupBy(ac => new
                        //{
                        //    ac.CategoryProductID,
                        //    ac.CustId
                        //}).Select(ac => new CategoryWiseRpt
                        //{
                        //    CustId = ac.Key.CustId,
                        //    CustName = ac.Select(i => i.CustName).FirstOrDefault(),
                        //    TypeofCategory = ac.Select(i => i.TypeofCategory).FirstOrDefault(),
                        //    CategoryProductID = ac.Key.CategoryProductID,
                        //    CreatedDate = ac.Select(i => i.CreatedDate).FirstOrDefault(),
                        //}).ToList();

                        CatList = CatList.GroupBy(ac => new
                        {
                            ac.CustId,
                            ac.CustName
                        }).Select(ac => new CategoryWiseRpt
                        {
                            CustId = ac.Key.CustId,
                            CustName = ac.Key.CustName,
                            TypeofCategory = ac.Select(i => i.TypeofCategory).FirstOrDefault(),
                            CategoryProductID = ac.Select(i => i.CategoryProductID).FirstOrDefault(),
                            CreatedDate = ac.Select(i => i.CreatedDate).FirstOrDefault(),
                        }).OrderBy(i=>i.CustName).ToList();

                        return CatList;
                    }
                   
                }
            }
            finally
            {

            }
            return null;
        }       
        public DateTime GetDateinFormat(DateTime? DateTimeVar)
        {
            DateTime FormattedDate = DateTime.ParseExact(DateTimeVar.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return FormattedDate;
        }
    }
}
