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
    public class MainCategory
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int CategoryProductID { get; set; }
        [Required(ErrorMessage = "Please enter Main Category Name")]
        public string MainCategoryName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedID { get; set; }
        public Nullable<int> BusinessTypeID { get; set; }
        public bool IsChecked { get; set; }
        [Required(ErrorMessage = "Please enter Reffered by or Reason to create this item")]
        public string RefferedByOrReason { get; set; }
        [Required(ErrorMessage = "Please enter the name of the person who approved this product")]
        public string ApprovedBy { get; set; }
        public int MainCategories { get; set; }
        public int SubCategories { get; set; }
        public int ChildCategories { get; set; }
        public int ItemCategories { get; set; }
        public string CreatedByName { get; set; }
        public bool IsActive { get; set; }
        public bool IsMasterProduct { get; set; }
        public bool IsRejected { get; set; }
        public List<CategoryHistory> histories { get; set; }
        public string DisplayMsg { get; set; }
    }
    public partial class CategoryHistory
    {
        public int ID { get; set; }
        public string CreatedUser { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public string ChildCategory { get; set; }
        public string OldProductName { get; set; }
        public string OldMainCategory { get; set; }
        public string OldSubCategory { get; set; }
        public string OldChildCategory { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
    }
    public class CategoryProduct
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");


        public List<MainCategory> GetCategoryProductList()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {

                    List<MainCategory> CategoryProductList = new List<MainCategory>();
                    CategoryProductList = (from u in dbContext.tblCategoryProducts
                                           join uc in dbContext.tblUsers on u.CreatedBy equals uc.ID
                                           //where u.IsActive == true
                                           select new MainCategory
                                           {
                                               ID = u.ID,
                                               MainCategoryName = u.MainCategoryName.Trim(),
                                               CreatedDate = u.CreatedDate,
                                               CreatedID = u.CreatedBy,
                                               BusinessTypeID = u.BusinessTypeID,
                                               RefferedByOrReason = u.RefferedByOrReason,
                                               ApprovedBy = u.ApprovedBy,
                                               IsMasterProduct = u.IsMasterProduct.Value,
                                               CreatedByName = uc.FullName,
                                               IsActive = u.IsActive,
                                               IsRejected = u.IsRejected.Value                                                     
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
        public MainCategory GetCategoryProductDetail(int ID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    MainCategory product = new MainCategory();
                    product = (from u in dbContext.tblCategoryProducts
                               join uc in dbContext.tblUsers on u.CreatedBy equals uc.ID
                               where u.ID == ID
                               select new MainCategory
                               {
                                   ID = u.ID,
                                   CategoryProductID = u.ID,
                                   MainCategoryName = u.MainCategoryName.Trim(),
                                   CreatedDate = u.CreatedDate,
                                   CreatedID = u.CreatedBy,
                                   BusinessTypeID = u.BusinessTypeID,
                                   RefferedByOrReason = u.RefferedByOrReason,
                                   ApprovedBy = u.ApprovedBy,
                                   CreatedByName = uc.FullName,
                               }).FirstOrDefault();

                    //get history
                    List<CategoryHistory> histories = new List<CategoryHistory>();
                    histories = (from h in dbContext.tblCategoryHistories
                                 join u in dbContext.tblUsers on h.UserID equals u.ID
                                 where h.ProductID == ID && h.ProductCategory == "MainCategory"
                                 select new CategoryHistory
                                 {
                                     OldProductName = h.OldProductName,
                                     CreatedUser = u.FullName,
                                     CreationDate = h.CreatedDate
                                 }).OrderByDescending(ch => ch.CreationDate).ToList();

                    //dbContext.tblCategoryHistories.Where(c => c.ProductID == ID && c.ProductCategory == "MainCategory").ToList();
                    product.histories = histories;
                    return product;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public MainCategory SaveCategoryProduct(MainCategory CategoryProductDetail, int UserID)
        {
            MainCategory Result = new MainCategory();
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        var Product = dbContext.tblCategoryProducts.AsNoTracking().Where(p => p.ID == CategoryProductDetail.ID).FirstOrDefault();
                        int RoleID = dbContext.tblUsers.Where(u => u.ID == UserID).FirstOrDefault().RoleID.Value;
                        //int Count = dbContext.tblCategoryProducts.AsNoTracking().Max(x => x.ID);
                        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                        if (Product != null)
                        {
                            var MainCatOld = dbContext.tblCategoryProducts.Where(s => s.ID == CategoryProductDetail.ID).AsNoTracking().FirstOrDefault();

                            tblCategoryProduct tblCategoryProduct = new tblCategoryProduct();
                            tblCategoryProduct.ID = CategoryProductDetail.ID;
                            tblCategoryProduct.MainCategoryName = CategoryProductDetail.MainCategoryName;
                            tblCategoryProduct.CreatedDate = Product.CreatedDate;
                            tblCategoryProduct.CreatedBy = Product.CreatedBy;
                            tblCategoryProduct.RefferedByOrReason = Product.RefferedByOrReason;
                            tblCategoryProduct.ApprovedBy = Product.ApprovedBy;
                            tblCategoryProduct.BusinessTypeID = CategoryProductDetail.BusinessTypeID;
                            if (RoleID == 1)
                                tblCategoryProduct.IsActive = true;
                            else
                                tblCategoryProduct.IsActive = false;
                            tblCategoryProduct.IsMasterProduct = false;
                            tblCategoryProduct.IsRejected = Product.IsRejected;
                            tblCategoryProduct.IsMasterProduct = Product.IsMasterProduct;
                            dbContext.tblCategoryProducts.Add(tblCategoryProduct);
                            dbContext.Entry(tblCategoryProduct).State = EntityState.Modified;

                            //Insert into History Table
                            tblHistory history = new tblHistory();
                            history.UserID = UserID;
                            history.ProductID = CategoryProductDetail.CategoryProductID;
                            history.ProductCategory = CategoryProductDetail.MainCategoryName;
                            history.CreatedBy = UserID;
                            history.CreatedDate = DateTimeNow;
                            history.Comments = "MainCategory Updated";
                            history.ActivityPage = "maincategories";
                            history.ActivityType = "create";
                            dbContext.tblHistories.Add(history);

                            //Insert into Category History Table
                            tblCategoryHistory categoryHistory = new tblCategoryHistory();
                            categoryHistory.UserID = UserID;
                            categoryHistory.ProductID = CategoryProductDetail.ID;
                            categoryHistory.ProductCategory = "MainCategory";
                            categoryHistory.ProductName = CategoryProductDetail.MainCategoryName;
                            categoryHistory.OldProductName = MainCatOld.MainCategoryName;
                            categoryHistory.CreatedBy = UserID;
                            categoryHistory.CreatedDate = DateTimeNow;
                            dbContext.tblCategoryHistories.Add(categoryHistory);
                            dbContext.SaveChanges();
                            dbcxtransaction.Commit();
                            if (RoleID == 1)
                                Result.DisplayMsg = "Product Updated Successfully!";
                            else
                                Result.DisplayMsg = "Product Updated Successfully and Waiting for Approval";
                        }
                        else
                        {
                            tblCategoryProduct tblCategoryProduct = new tblCategoryProduct();
                            tblCategoryProduct.MainCategoryName = CategoryProductDetail.MainCategoryName;
                            tblCategoryProduct.CreatedDate = DateTime.Now;
                            tblCategoryProduct.CreatedBy = CategoryProductDetail.CreatedID.Value;
                            tblCategoryProduct.RefferedByOrReason = CategoryProductDetail.RefferedByOrReason;
                            tblCategoryProduct.ApprovedBy = CategoryProductDetail.ApprovedBy;
                            tblCategoryProduct.IsMasterProduct = false;
                            if (RoleID == 1)
                                tblCategoryProduct.IsActive = true;
                            else
                                tblCategoryProduct.IsActive = false;
                            tblCategoryProduct.IsRejected = false;
                            tblCategoryProduct.IsMasterProduct = false;
                            dbContext.tblCategoryProducts.Add(tblCategoryProduct);

                            //Insert into History Table
                            tblHistory history = new tblHistory();
                            history.UserID = UserID;
                            history.ProductID = CategoryProductDetail.ID;
                            history.ProductCategory = CategoryProductDetail.MainCategoryName;
                            history.CreatedDate = DateTimeNow;
                            history.Comments = "MainCategory Created";
                            history.ActivityPage = "maincategories";
                            history.ActivityType = "create";
                            dbContext.tblHistories.Add(history);
                            dbContext.SaveChanges();
                            dbcxtransaction.Commit();
                            if (RoleID == 1)
                                Result.DisplayMsg = "Product Saved Successfully!";
                            else
                                Result.DisplayMsg = "Product Saved Successfully and Waiting for Approval";
                        }
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.DisplayMsg = "Internal Server Error! Please contact administrator";
                return Result;
            }
        }
        public bool CheckDuplicateName(string MainCategoryName)
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    var IsValueexists = dbcontext.tblCategoryProducts.Where(c => c.MainCategoryName.ToLower() == MainCategoryName.ToLower()).FirstOrDefault();

                    if (IsValueexists != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Old Code
        //public bool DeleteProduct(int ID)
        //{
        //    var product = dbContext.tblCategoryProducts.FirstOrDefault(s => s.ID == ID);
        //    if (product != null)
        //    {
        //        dbContext.tblCategoryProducts.Remove(product);
        //        dbContext.SaveChanges();
        //    }
        //    return true;

        //}
        //private object ofetch;
        //public MainCategory AddProduct(MainCategory ProductDetails, int UserID)
        //{
        //    try
        //    {
        //        tblCategoryProduct productCat = new tblCategoryProduct();
        //        using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
        //        {
        //            var isExists = (from u in dbContext.tblCategoryProducts
        //                        where u.ID == ProductDetails.ID
        //                        select u).FirstOrDefault();

        //            int RoleID = dbContext.tblUsers.Where(u => u.ID == UserID).FirstOrDefault().RoleID.Value;

        //            if (isExists == null)
        //            {
        //                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        //                productCat.MainCategoryName = ProductDetails.MainCategoryName;
        //                productCat.CreatedDate = DateTimeNow;
        //                productCat.CreatedBy = UserID;
        //                productCat.BusinessTypeID = ProductDetails.BusinessTypeID;
        //                productCat.RefferedByOrReason = ProductDetails.RefferedByOrReason;
        //                productCat.ApprovedBy = ProductDetails.ApprovedBy;
        //                productCat.IsActive = isExists.IsActive;
        //                productCat.IsMasterProduct = false;
        //                dbContext.tblCategoryProducts.Add(productCat);
        //                dbContext.SaveChanges();
        //                return ProductDetails;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}
        #endregion
    }
}
