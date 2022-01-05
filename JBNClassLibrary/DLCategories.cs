using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class AllCategories
    {
        public int? CategoryProductID { get; set; }
        public int? SubCategoryID { get; set; }
        public int? ChildCategoryID { get; set; }
        public int? ItemID { get; set; }
        public string MainCategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string ChildCategoryName { get; set; }
        public string ItemName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class DLCategories
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public List<AllCategories> GetAllCategories(AllCategories allCategories)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {

                    List<AllCategories> CategoryProductList = new List<AllCategories>();
                    if (allCategories.CategoryProductID != 0 && allCategories.CategoryProductID != null)
                    {
                        CategoryProductList = (from pView in dbContext.ProductsViews
                                               where pView.CategoryProductID == allCategories.CategoryProductID
                                               select new AllCategories
                                               {
                                                   CategoryProductID = pView.CategoryProductID,
                                                   MainCategoryName = pView.MainCategoryName.Trim(),
                                                   SubCategoryID = pView.SubCategoryID,
                                                   SubCategoryName = pView.SubCategoryName,
                                                   ChildCategoryID = pView.ChildCategoryID,
                                                   ChildCategoryName = pView.ChildCategoryName,
                                                   ItemID = pView.ItemID,
                                                   ItemName = pView.ItemName
                                               }).ToList();
                    }
                    else if (allCategories.CategoryProductID != 0 && allCategories.CategoryProductID != 0)
                    {
                        CategoryProductList = (from pView in dbContext.ProductsViews
                                               where pView.CategoryProductID == allCategories.CategoryProductID
                                               && pView.SubCategoryID == allCategories.SubCategoryID
                                               select new AllCategories
                                               {
                                                   CategoryProductID = pView.CategoryProductID,
                                                   MainCategoryName = pView.MainCategoryName.Trim(),
                                                   SubCategoryID = pView.SubCategoryID,
                                                   SubCategoryName = pView.SubCategoryName,
                                                   ChildCategoryID = pView.ChildCategoryID,
                                                   ChildCategoryName = pView.ChildCategoryName,
                                                   ItemID = pView.ItemID,
                                                   ItemName = pView.ItemName
                                               }).ToList();
                    }
                    else if (allCategories.CategoryProductID != 0 && allCategories.CategoryProductID != 0 && allCategories.ItemID != 0)
                    {
                        CategoryProductList = (from pView in dbContext.ProductsViews
                                               where pView.CategoryProductID == allCategories.CategoryProductID
                                               && pView.SubCategoryID == allCategories.SubCategoryID
                                               && pView.ItemID == allCategories.ItemID
                                               select new AllCategories
                                               {
                                                   CategoryProductID = pView.CategoryProductID,
                                                   MainCategoryName = pView.MainCategoryName.Trim(),
                                                   SubCategoryID = pView.SubCategoryID,
                                                   SubCategoryName = pView.SubCategoryName,
                                                   ChildCategoryID = pView.ChildCategoryID,
                                                   ChildCategoryName = pView.ChildCategoryName,
                                                   ItemID = pView.ItemID,
                                                   ItemName = pView.ItemName
                                               }).ToList();
                    }
                    else if(!string.IsNullOrEmpty(allCategories.ItemName))
                        CategoryProductList = (from pView in dbContext.ProductsViews
                                               where pView.MainCategoryName.Contains(allCategories.ItemName)
                                               || pView.SubCategoryName.Contains(allCategories.ItemName)
                                               || pView.ChildCategoryName.Contains(allCategories.ItemName)
                                               || pView.ItemName.Contains(allCategories.ItemName)
                                               select new AllCategories
                                               {
                                                   CategoryProductID = pView.CategoryProductID,
                                                   MainCategoryName = pView.MainCategoryName.Trim(),
                                                   SubCategoryID = pView.SubCategoryID,
                                                   SubCategoryName = pView.SubCategoryName,
                                                   ChildCategoryID = pView.ChildCategoryID,
                                                   ChildCategoryName = pView.ChildCategoryName,
                                                   ItemID = pView.ItemID,
                                                   ItemName = pView.ItemName
                                               }).ToList();

                    if (allCategories.CategoryProductID != 0)
                        CategoryProductList = CategoryProductList.Where(c => c.CategoryProductID == allCategories.CategoryProductID).ToList();

                    if (allCategories.SubCategoryID != 0)
                        CategoryProductList = CategoryProductList.Where(c => c.SubCategoryID == allCategories.SubCategoryID).ToList();

                    if (allCategories.ItemID != 0)
                        CategoryProductList = CategoryProductList.Where(c => c.ItemID == allCategories.ItemID).ToList();

                    return CategoryProductList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<tblCategoryProduct> GetAllCategories()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {

                    List<tblCategoryProduct> CategoryProductList = new List<tblCategoryProduct>();
                    CategoryProductList = dbContext.tblCategoryProducts.ToList();

                    return CategoryProductList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<SubCat> GetAllSubCategories(int CategoryProductID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<SubCat> CategoryProductList = new List<SubCat>();
                    CategoryProductList = (from s in dbContext.tblSubCategories
                                               //join c in dbContext.tblChildCategories on s.ID equals c.SubCategoryId
                                           join c in dbContext.tblChildCategories on s.ID equals c.SubCategoryId
                                           where c.IsActive == true && s.CategoryProductID == CategoryProductID

                                           select new SubCat
                                           {
                                               //ID = s.SubCategoryId,
                                               ID = s.ID,
                                               SubCategoryName = s.SubCategoryName,
                                               CreatedDate = s.CreatedDate,
                                               CreatedBy = s.CreatedBy
                                           }).Distinct().ToList();

                    return CategoryProductList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<ItemCategory> GetAllChildCategories(int SubCategoryID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {

                    List<ItemCategory> CategoryProductList = new List<ItemCategory>();
                    //CategoryProductList = dbContext.tblChildCategories.Where(c => c.IsActive == true && c.SubCategoryId == SubCategoryID).ToList();

                    CategoryProductList = (from cc in dbContext.tblChildCategories
                                           join ic in dbContext.tblItemCategories on cc.ID equals ic.ChildCategoryID
                                           where ic.IsActive == true && cc.SubCategoryId == SubCategoryID
                                           select new ItemCategory
                                           {
                                               ID = ic.ID,
                                               ItemName = ic.ItemName
                                           }).ToList();
                    return CategoryProductList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
    }
}
