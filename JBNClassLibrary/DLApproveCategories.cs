using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class DLApproveCategories
    {
        public string ApproveMainCategory(MainCategory mainCategory)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    tblCategoryProduct tblCategoryProduct = new tblCategoryProduct();
                    tblCategoryProduct.ID = mainCategory.CategoryProductID;
                    tblCategoryProduct.MainCategoryName = mainCategory.MainCategoryName;
                    tblCategoryProduct.IsActive = true;
                    dbContext.tblCategoryProducts.Attach(tblCategoryProduct);
                    dbContext.Entry(tblCategoryProduct).Property(c => c.IsActive).IsModified = true;
                    dbContext.Entry(tblCategoryProduct).Property(c => c.MainCategoryName).IsModified = true;
                    dbContext.SaveChanges();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return ex.Message;
            }
        }
        public string RejectMainCategory(MainCategory mainCategory)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    tblCategoryProduct tblCategoryProduct = new tblCategoryProduct();
                    tblCategoryProduct.ID = mainCategory.CategoryProductID;
                    tblCategoryProduct.IsActive = false;
                    tblCategoryProduct.IsRejected = true;
                    dbContext.tblCategoryProducts.Attach(tblCategoryProduct);
                    dbContext.Entry(tblCategoryProduct).Property(c => c.IsActive).IsModified = true;
                    dbContext.Entry(tblCategoryProduct).Property(c => c.IsRejected).IsModified = true;
                    dbContext.SaveChanges();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return "error";
            }
        }
        public string ApproveSubCategory(SubCat subCat)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    tblSubCategory tblSubCategory1 = new tblSubCategory();
                    tblSubCategory1.ID = subCat.ID;
                    tblSubCategory1.CategoryProductID = subCat.CategoryProductID;
                    tblSubCategory1.SubCategoryName = subCat.SubCategoryName;
                    tblSubCategory1.IsActive = true;
                    dbContext.tblSubCategories.Attach(tblSubCategory1);
                    dbContext.Entry(tblSubCategory1).Property(c => c.IsActive).IsModified = true;
                    dbContext.Entry(tblSubCategory1).Property(c => c.SubCategoryName).IsModified = true;
                    dbContext.Entry(tblSubCategory1).Property(c => c.CategoryProductID).IsModified = true;
                    dbContext.SaveChanges();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return ex.Message;
            }
        }
        public string RejectSubCategory(SubCat subCat)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    tblSubCategory tblSubCategory = new tblSubCategory();
                    tblSubCategory.ID = subCat.ID;
                    tblSubCategory.IsActive = false;
                    tblSubCategory.IsRejected = true;
                    dbContext.tblSubCategories.Attach(tblSubCategory);
                    dbContext.Entry(tblSubCategory).Property(c => c.IsActive).IsModified = true;
                    dbContext.Entry(tblSubCategory).Property(c => c.IsRejected).IsModified = true;
                    dbContext.SaveChanges();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return "error";
            }
        }
        public string ApproveChildCategory(childcategory ChildCat)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    tblChildCategory tblChildCategory = new tblChildCategory();
                    tblChildCategory.ID = ChildCat.ID;
                    tblChildCategory.SubCategoryId = ChildCat.SubCategoryId;
                    tblChildCategory.ChildCategoryName = ChildCat.ChildCategoryName;
                    tblChildCategory.IsActive = true;
                    dbContext.tblChildCategories.Attach(tblChildCategory);
                    dbContext.Entry(tblChildCategory).Property(c => c.IsActive).IsModified = true;
                    dbContext.Entry(tblChildCategory).Property(c => c.ChildCategoryName).IsModified = true;
                    dbContext.Entry(tblChildCategory).Property(c => c.SubCategoryId).IsModified = true;
                    dbContext.SaveChanges();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return ex.Message;
            }
        }
        public string RejectChildCategory(childcategory childcategory)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    tblChildCategory tblChild = new tblChildCategory();
                    tblChild.ID = childcategory.ID;
                    tblChild.IsActive = false;
                    tblChild.IsRejected = true;
                    dbContext.tblChildCategories.Attach(tblChild);
                    dbContext.Entry(tblChild).Property(c => c.IsActive).IsModified = true;
                    dbContext.Entry(tblChild).Property(c => c.IsRejected).IsModified = true;
                    dbContext.SaveChanges();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return "error";
            }
        }
        public string ApproveItemCategory(ItemCategory item)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    tblItemCategory tblItemCategory = new tblItemCategory();
                    tblItemCategory.ID = item.ID;
                    tblItemCategory.ItemName = item.ItemName;
                    tblItemCategory.ChildCategoryID = item.ChildCategoryId;
                    tblItemCategory.ApprovedBy = item.ItemApprovedBy;
                    tblItemCategory.RefferedByOrReason = item.ItemRefferedByOrReason;
                    tblItemCategory.IsActive = true;
                    dbContext.tblItemCategories.Attach(tblItemCategory);
                    dbContext.Entry(tblItemCategory).Property(c => c.IsActive).IsModified = true;
                    dbContext.Entry(tblItemCategory).Property(c => c.ItemName).IsModified = true;
                    dbContext.Entry(tblItemCategory).Property(c => c.ChildCategoryID).IsModified = true;
                    dbContext.Entry(tblItemCategory).Property(c => c.ApprovedBy).IsModified = true;
                    dbContext.Entry(tblItemCategory).Property(c => c.RefferedByOrReason).IsModified = true;
                    dbContext.SaveChanges();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return ex.Message;
            }
        }
        public string RejectItemCategory(ItemCategory item)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    tblItemCategory itemCat = new tblItemCategory();
                    itemCat.ID = item.ID;
                    itemCat.IsActive = false;
                    itemCat.IsRejected = true;
                    dbContext.tblItemCategories.Attach(itemCat);
                    dbContext.Entry(itemCat).Property(c => c.IsActive).IsModified = true;
                    dbContext.Entry(itemCat).Property(c => c.IsRejected).IsModified = true;
                    dbContext.SaveChanges();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return "error";
            }
        }
    }
}
