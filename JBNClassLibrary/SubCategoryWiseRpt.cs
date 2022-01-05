using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class SubCategoryWiseRpt
    {        
        public int? SubCategoryId { get; set; }              
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int? CustId { get; set; }
        public string CustName { get; set; }
        public List<SubCategoryTypes> SubCategoryTypeWithCust { get; set; }
        public string TypeofSubCategory { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
    }
    public class SubCategoryTypes
    {
        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
    }
    public class GetSubCategoryandCustomerDetails
    {
        mwbtDealerEntities dbcontext = new mwbtDealerEntities();
        public SubCategoryWiseRpt CatObj = new SubCategoryWiseRpt();
        public List<SubCategoryWiseRpt> CatList = new List<SubCategoryWiseRpt>();


        public List<SubCategoryTypes> GetSubCategory()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<SubCategoryTypes> CatList = new List<SubCategoryTypes>();
                    CatList = (from cust in dbcontext.tblSubCategories
                               select new SubCategoryTypes()
                               {
                                   SubCategoryId = cust.SubCategoryId,
                                   SubCategoryName = cust.SubCategoryName
                               }).ToList();
                    return CatList;
                }
            }
            finally
            {

            }
        }
        public List<SubCategoryWiseRpt> GetDetails(SubCategoryWiseRpt  subCategoryWiseRpt)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                   if(subCategoryWiseRpt.SubCategoryId==null)
                    {
                        CatList = (from cust in dbcontext.tblCustomerDetails
                                   join d in dbcontext.tblSubCategoryProductWithCusts
                                   on cust.ID equals d.CustID
                                   select new SubCategoryWiseRpt
                                   {
                                       CustId = d.CustID,
                                       CustName = cust.CustName,
                                       SubCategoryId = d.SubCategoryId,
                                       CreatedDate = d.CreatedDate,

                                       SubCategoryTypeWithCust = (from sc in dbcontext.tblSubCategoryProductWithCusts
                                                                  join s in dbcontext.tblSubCategories on sc.SubCategoryId
                                                                  equals s.SubCategoryId
                                                                  where sc.CustID == cust.ID
                                                                  select new SubCategoryTypes
                                                                  {
                                                                      SubCategoryId = sc.SubCategoryId,
                                                                      SubCategoryName = s.SubCategoryName.Trim(),

                                                                  }).ToList(),
                                   }).ToList();
                        foreach (var item in CatList)
                        {
                            foreach (var item1 in item.SubCategoryTypeWithCust)
                            {
                                item.TypeofSubCategory += item1.SubCategoryName + ' ';
                            }
                        }

                        //if (!string.IsNullOrEmpty(subCategoryWiseRpt.FromDate.ToString()) && !string.IsNullOrEmpty(subCategoryWiseRpt.ToDate.ToString()))
                        //{
                        //    CatList = CatList.Where(i => Convert.ToDateTime(i.CreatedDate) >= subCategoryWiseRpt.FromDate && Convert.ToDateTime(i.CreatedDate) <= subCategoryWiseRpt.FromDate).ToList();
                        //}

                        //if (subCategoryWiseRpt.SubCategoryId > 0)
                        //    CatList = CatList.Where(i => i.SubCategoryId == subCategoryWiseRpt.SubCategoryId).ToList();


                        //CatList = CatList.GroupBy(ac => new
                        //{
                        //    ac.SubCategoryId,
                        //}).Select(ac => new SubCategoryWiseRpt
                        //{
                        //    CustId = ac.Select(i => i.CustId).FirstOrDefault(),
                        //    CustName = ac.Select(i => i.CustName).FirstOrDefault(),
                        //    TypeofSubCategory = ac.Select(i => i.TypeofSubCategory).FirstOrDefault(),
                        //    SubCategoryId = ac.Key.SubCategoryId,
                        //    CreatedDate = ac.Select(i => i.CreatedDate).FirstOrDefault(),
                        //}).ToList();

                        CatList = CatList.GroupBy(ac => new
                        {
                            ac.CustId,
                            ac.CustName
                        }).Select(ac => new SubCategoryWiseRpt
                        {
                            CustId = ac.Key.CustId,
                            CustName = ac.Key.CustName,
                            TypeofSubCategory = ac.Select(i => i.TypeofSubCategory).FirstOrDefault(),
                            SubCategoryId = ac.Select(i => i.SubCategoryId).FirstOrDefault(),
                            CreatedDate = ac.Select(i => i.CreatedDate).FirstOrDefault(),
                        }).OrderBy(i => i.CustName).ToList();

                        return CatList;
                    }
                   else
                    {
                        CatList = (from cust in dbcontext.tblCustomerDetails
                                   join scp in dbcontext.tblSubCategoryProductWithCusts
                                   on cust.ID equals scp.CustID
                                   select new SubCategoryWiseRpt
                                   {
                                       CustId = scp.CustID,
                                       CustName = cust.CustName,
                                       SubCategoryId = scp.SubCategoryId,
                                       CreatedDate = scp.CreatedDate,

                                       SubCategoryTypeWithCust = (from b in dbcontext.tblSubCategoryProductWithCusts
                                                                  join bt in dbcontext.tblSubCategories on b.SubCategoryId
                                                                  equals bt.SubCategoryId
                                                                  where b.CustID == cust.ID
                                                                  select new SubCategoryTypes
                                                                  {
                                                                      SubCategoryId = b.SubCategoryId,
                                                                      SubCategoryName = bt.SubCategoryName.Trim(),

                                                                  }).ToList(),
                                   }).ToList();
                       
                        if (!string.IsNullOrEmpty(subCategoryWiseRpt.FromDate.ToString()) && !string.IsNullOrEmpty(subCategoryWiseRpt.ToDate.ToString()))
                        {
                            CatList = CatList.Where(i => Convert.ToDateTime(i.CreatedDate) >= subCategoryWiseRpt.FromDate && Convert.ToDateTime(i.CreatedDate) <= subCategoryWiseRpt.ToDate).ToList();
                        }

                        if (subCategoryWiseRpt.SubCategoryId > 0)
                            CatList = CatList.Where(i => i.SubCategoryId == subCategoryWiseRpt.SubCategoryId).ToList();

                        foreach (var item in CatList)
                        {
                            foreach (var item1 in item.SubCategoryTypeWithCust)
                            {
                                item.TypeofSubCategory += item1.SubCategoryName + ' ';
                            }
                        }

                        //CatList = CatList.GroupBy(ac => new
                        //{
                        //    ac.SubCategoryId,
                        //}).Select(ac => new SubCategoryWiseRpt
                        //{
                        //    CustId = ac.Select(i => i.CustId).FirstOrDefault(),
                        //    CustName = ac.Select(i => i.CustName).FirstOrDefault(),
                        //    TypeofSubCategory = ac.Select(i => i.TypeofSubCategory).FirstOrDefault(),
                        //    SubCategoryId = ac.Key.SubCategoryId,
                        //    CreatedDate = ac.Select(i => i.CreatedDate).FirstOrDefault(),
                        //}).ToList();

                        CatList = CatList.GroupBy(ac => new
                        {
                            ac.CustId,
                            ac.CustName
                        }).Select(ac => new SubCategoryWiseRpt
                        {
                            CustId = ac.Key.CustId,
                            CustName = ac.Key.CustName,
                            TypeofSubCategory = ac.Select(i => i.TypeofSubCategory).FirstOrDefault(),
                            SubCategoryId = ac.Select(i => i.SubCategoryId).FirstOrDefault(),
                            CreatedDate = ac.Select(i => i.CreatedDate).FirstOrDefault(),
                        }).OrderBy(i => i.CustName).ToList();

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
